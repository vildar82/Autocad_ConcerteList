using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using AcadLib;
using AcadLib.Errors;
using Autocad_ConcerteList.Properties;
using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using Autocad_ConcerteList.Src.ConcreteDB.Formula;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

namespace Autocad_ConcerteList.Src.ConcreteDB
{
    /// <summary>
    /// Сервисные функции работы с базой ЖБИ
    /// </summary>
    public static class DbService
    {
        static List<BalconyCutDbo> BalconyCuts;
        static List<BalconyDoorDbo> BalconyDoors;
        static List<ItemGroupDbo> Groups;
        static List<ItemConstructionDbo> Items;
        public static List<SerieDbo> Series { get; set; }

        public static SAPREntities ConnectEntities()
        {            
            return new SAPREntities(new EntityConnection(Settings.Default.SaprCon));
        }

        public static void Init()
        {
            using (var ents = ConnectEntities())
            {
                Groups = (from s in ents.I_S_ItemGroup.AsNoTracking()
                          join f in ents.I_C_Formula on s.FormulaId equals f.FormulaId into gf
                          from fJoined in gf.DefaultIfEmpty()
                          select new ItemGroupDbo {
                              ItemGroupId = s.ItemGroupId,
                              ItemGroup = s.ItemGroup,
                              HasFormula = s.HasFormula,
                              Formula = (fJoined == null) ? null : fJoined.FormulaValue,
                              LengthFactor = (fJoined == null) ? (short)1 : fJoined.LengthFactor,
                              HeightFactor = (fJoined == null) ? (short)1 : fJoined.HeightFactor,
                              ThicknessFactor = (fJoined == null) ? (short)1 : fJoined.ThicknessFactor,
                              GabKey = (fJoined == null) ? null : fJoined.GabKey
                          }).ToList();                
                BalconyCuts = ents.I_S_BalconyCut.AsNoTracking().Select(s => new BalconyCutDbo
                {
                    BalconyCutId = s.BalconyCutId,
                    BalconyCutName = s.BalconyCut,
                    BalconyCutSize = s.BalconyCutSize
                }).ToList();
                BalconyDoors = ents.I_S_BalconyDoor.AsNoTracking().Select(s => new BalconyDoorDbo
                {
                    BalconyDoorId = s.BalconyDoorId,
                    BalconyDoorName = s.BalconyDoor
                }).ToList();
                Series = ents.I_C_Series.AsNoTracking().Select(s => new SerieDbo
                {
                    Name = s.Series,
                    SeriesId = s.SeriesId
                }).ToList();
                
                // Загрузка всех панелей
                LoadItems();
            }
        }

        private static ItemConstructionDbo SelectItemPanelFromFinds (List<ItemConstructionDbo> panels)
        {
            if (panels.Skip(1).Any())
            {
                // Несколько записей в базе с этими параметрвами
                //    string idsItems = string.Join(",", panels.Select(i => i.ItemConstructionId));
                //    Logger.Log.Error($"Найдено несколько панелей по параметрам для марки {panels.First().HandMarkNoColour}, ItemConstructionId: {idsItems}.");
                var res = panels.FirstOrDefault(p => p.Weight != null);
                if (res != null)
                {
                    return res;
                }
            }
            else if (panels.Any())
            {
                return panels.First();
            }
            return null;
        }

        public static ItemConstructionDbo FindByMark (string mark)
        {
            if (Items == null)
            {
                LoadItems();
            }
            var finds = Items.FindAll(i=>i.HandMarkNoColour.Equals(mark, StringComparison.OrdinalIgnoreCase));
            if (finds.Count == 0)
            {
                finds = Items.FindAll(i => i.HandMarkNoColour.Equals(mark.Replace(" ", "")));
            }
            if (finds.Count>0)
            {
                return SelectItemPanelFromFinds(finds);                
            }
            else
            {
                return null;
            }
        }        

        public static ItemConstructionDbo FindByParametersFromAllLoaded(Panel panel)
        {
            if (Items == null)
            {
                LoadItems();
            }
            var itemSearch = new ItemConstructionDbo()
            {
                ItemGroup = panel.ItemGroup,
                Lenght = panel.Lenght,
                Height = panel.Height,
                Thickness = panel.Thickness,
                Formwork = panel.Formwork,
                BalconyCut = panel.BalconyCutItem,
                BalconyDoor = panel.BalconyDoorItem,
                Electrics = panel.Electrics
            };
            var finds = Items.FindAll(i => i.Equals(itemSearch));
            return SelectItemPanelFromFinds(finds);
        }

        private static void LoadItems ()
        {
            using (var ents = ConnectEntities())
            {
                Items = (from i in ents.I_J_ItemConstruction.AsNoTracking()
                         join g in ents.I_S_ItemGroup on i.ItemGroupId equals g.ItemGroupId

                         join bc in ents.I_S_BalconyCut on i.BalconyCutId equals bc.BalconyCutId into gbc
                         from bcJoined in gbc.DefaultIfEmpty()

                         join bd in ents.I_S_BalconyDoor on i.BalconyDoorId equals bd.BalconyDoorId into gbd
                         from bdJoined in gbd.DefaultIfEmpty()

                         where g.HasFormula != null
                         select new ItemConstructionDbo {
                             HandMarkNoColour = i.HandMarkNoColour,
                             ItemGroupId = i.ItemGroupId,
                             ItemGroup = g.ItemGroup,
                             Lenght = i.Lenght,
                             Height = i.Height,
                             Thickness = i.Thickness,
                             Formwork = i.Formwork,
                             FormworkMirror = i.FormworkMirror,
                             BalconyCut = (bcJoined == null) ? null : new BalconyCutDbo {
                                 BalconyCutId = i.BalconyCutId.Value,
                                 BalconyCutName = bcJoined.BalconyCut, BalconyCutSize = bcJoined.BalconyCutSize
                             },
                             BalconyDoor = (bdJoined == null) ? null : new BalconyDoorDbo {
                                 BalconyDoorId = i.BalconyDoorId.Value,
                                 BalconyDoorName = bdJoined.BalconyDoor
                             },
                             Electrics = i.Electrics,
                             ItemConstructionId = i.ItemConstructionId,
                             Weight = i.Weight,
                             Volume = i.Volume
                         }).ToList();
            }
        }

        /// <summary>
        /// Определение марки панели по формуле из базы
        /// </summary>
        public static Result<string> GetDbMark (Panel panel, out ParserFormula parseFormula)
        {
            string markDb = null;
            parseFormula = null;

            // Получение id группы  
            if (panel.DbGroup == null)
            {
                panel.DefineItemGroup();
            }            
            if (panel.DbGroup == null)
            {
                return Result.Fail<string>($"Не найдена группа {panel.ItemGroup}.");
                //throw new Exception($"Не найдена группа {panel.ItemGroup}");
            }
                                                
            if (panel.DbGroup.HasFormula == null)
            {
                // Не задано значение HasFormula - считается, что это ошибка определения формулы для группы.
                return Result.Fail<string>($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}.");
                //throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
            }
            bool hasFormula = panel.DbGroup.HasFormula.Value;
            if (hasFormula)
            {
                // Формула для группы панели
                string formula = panel.DbGroup.Formula;
                if (formula == null)
                {
                    return Result.Fail<string>($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}.");
                    // Добавление ошибки в панель.
                    //throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
                }
                else
                {
                    // Получение марки панели по формуле
                    parseFormula = new ParserFormula(formula, panel);
                    try
                    {
                        parseFormula.Parse();
                    }
                    catch
                    {
                        return Result.Fail<string>($" Ошибка определения марки по формуле. ");
                    }                    
                    markDb = parseFormula.Result;
                }
            }
            else
            {
                markDb = panel.Mark;
            }
            return Result.Ok(markDb);
        }

        public static void Register (List<Panel> panels, SerieDbo ser)
        {
            List<Panel> registerPanelsToLog = new List<Panel> ();
            foreach (var panel in panels)
            {
                try
                {
                    ItemConstructionDbo item = new ItemConstructionDbo ()
                        {
                         HandMarkNoColour = panel.Mark,                         
                         ItemGroupId = panel.DbGroup.ItemGroupId,
                         Lenght = panel.Lenght,
                         Height = panel.Height,
                         Thickness = panel.Thickness,
                         Formwork = panel.Formwork,
                         BalconyCutId = panel.BalconyCutItem?.BalconyCutId,
                         BalconyDoorId = panel.BalconyDoorItem?.BalconyDoorId,
                         Electrics = panel.Electrics                         
                    };
                    decimal id;
                    Register(item, ser, out id);
                    panel.ItemConstructionId = id;
                    registerPanelsToLog.Add(panel);
                }
                catch (Exception ex)
                {
                    // Ошибка при сохранении панели
                    Inspector.AddError($"Ошибка сохранения панели в базу - '{panel.Mark}'");
                    Logger.Log.Error(ex, $"Ошибка сохранения панели в базу - '{panel.ParamsToString()}'");
                }
            }
            Logger.Log.Error("Зарегистрированы новые изделия ЖБИ: " + getLogRegistryPanels(registerPanelsToLog));
        }

        private static string getLogRegistryPanels (List<Panel> registerPanelsToLog)
        {
            string res = string.Join("; ", registerPanelsToLog.Select(p=> p.Mark +  " id=" + p.ItemConstructionId));
            return res;
        }

        public static bool Register(ItemConstructionDbo panel, SerieDbo ser, out decimal id)
        {
            id = -1;
            using (var ents = ConnectEntities())
            {
                var newItem = ents.I_J_ItemConstruction.Add(new I_J_ItemConstruction()
                {
                    HandMarkNoColour = panel.HandMarkNoColour,
                    ItemGroupId = panel.ItemGroupId,
                    Lenght = panel.Lenght,
                    Height = panel.Height,
                    Thickness = panel.Thickness,
                    Formwork = panel.Formwork,
                    BalconyCutId = panel.BalconyCutId,
                    BalconyDoorId  = panel.BalconyDoorId,
                    Electrics = panel.Electrics
                });
                if (ser != null)
                {
                    ents.I_J_ItemSeries.Add(new I_J_ItemSeries()
                    {
                        I_J_ItemConstruction = newItem,
                        SeriesId = ser.SeriesId
                    });
                }
#if DB
                ents.SaveChanges();
#endif
                id = newItem.ItemConstructionId;
            }
            return true;
        }

        public static ItemGroupDbo FindGroup(string itemGroup)
        {
            return Groups.FirstOrDefault(g => g.ItemGroup.Equals(itemGroup, StringComparison.OrdinalIgnoreCase));
        }

        public static BalconyCutDbo GetBalconyCutItem(string balconyCut)
        {
            return BalconyCuts.Where(b => b.BalconyCutName.Equals(balconyCut)).FirstOrDefault();
        }

        public static BalconyDoorDbo GetBalconyDoorItem(string balconyDoor)
        {
            return BalconyDoors.Where(b => b.BalconyDoorName.Equals(balconyDoor)).FirstOrDefault();
        }

        
    }
}