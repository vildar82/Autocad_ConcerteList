using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using AcadLib;
using AcadLib.Errors;
using Autocad_ConcerteList.Properties;
using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using Autocad_ConcerteList.Src.ConcreteDB.FormulaEval;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

namespace Autocad_ConcerteList.Src.ConcreteDB
{
    /// <summary>
    /// Сервисные функции работы с базой ЖБИ
    /// </summary>
    public static class DbService
    {
        public const string MountParamName = "Закладная";
        public const string PrognParamName = "Зубцы";
        public const string BalconyCutParamName = "Подрезка";
        public const string BalconyDoorParamName = "Балконная дверь";
        public const string StepHeightParamName = "Ступень";
        static List<ModificatorDbo> Modificators;        
        static List<ItemGroupDbo> Groups;
        static List<ItemConstructionDbo> Items;        
        static List<string> colorsHandMarkFull;
        public static List<SerieDbo> Series { get; set; }

        public static MDMEntities ConnectEntities()
        {   
            var con = new EntityConnection(Settings.Default.MdmCon);            
            return new MDMEntities(con);
        }

        public static void Init()
        {
            using (var ents = ConnectEntities())
            {
                Groups = (from s in ents.Item_group.AsNoTracking()
                          join f in ents.Formula on s.Formula_id equals f.Formula_id into gf
                          from fJoined in gf.DefaultIfEmpty()
                          select new ItemGroupDbo {
                              ItemGroupId = s.Item_group_id,
                              ItemGroup = s.Item_group1,
                              HasFormula = s.Formula_usage,
                              Formula = (fJoined == null) ? null : fJoined.Formula1,
                              LengthFactor = (fJoined == null) ? (short)1 : fJoined.Formula_Length_Factor,
                              HeightFactor = (fJoined == null) ? (short)1 : fJoined.Formula_Height_Factor,
                              ThicknessFactor = (fJoined == null) ? (short)1 : fJoined.Formula_Thickness_Factor,
                              GabKey = (fJoined == null) ? null : fJoined.Formula_Gab_Key
                          }).ToList();
                Modificators = (from i in ents.Item_modification.AsNoTracking()
                                join m in ents.Item_modification_type on i.Item_modification_type_id equals m.Item_modification_type_id into mf
                                from mJoined in mf.DefaultIfEmpty()
                                join s in ents.Side on i.Side_id equals s.Side_id into sf
                                from sJoined in sf.DefaultIfEmpty()
                                select new ModificatorDbo {
                                    Id = i.Item_modification_id,
                                    Measure = i.Item_modification_measure,
                                    Code = i.Item_modification_code,
                                    TypeModificator = mJoined.Item_modification_type1,
                                    Side = sJoined == null ? null : sJoined.Side1
                                }).ToList();                
                Series = ents.Product.AsNoTracking().Select(s => new SerieDbo {
                    Name = s.Product1,
                    SeriesId = s.Product_id
                }).ToList();

                // Загрузка всех панелей
                LoadItems(ents);

                // Список цветов (item_construction_colors hand_mark)
                colorsHandMarkFull = ents.Item_construction_colour.Select(s => s.Hand_mark).ToList();
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
                using (var ents = ConnectEntities())
                {
                    LoadItems(ents);
                }
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
                using (var ents = ConnectEntities())
                {
                    LoadItems(ents);
                }
            }
            var itemSearch = new ItemConstructionDbo()
            {
                ItemGroup = panel.Item_group,
                Length = panel.Length,
                Height = panel.Height,
                Thickness = panel.Thickness,
                Formwork = panel.Formwork,
                MountElement = panel.Mount_element,
                Prong = panel.Prong,
                BalconyCut = panel.Balcony_cut,
                BalconyDoor = panel.Balcony_door,
                StepHeightIndex = panel.Step_height,
                StepCount = panel.Steps,
                StepFirstHeight = panel.First_step,
                Electrics = panel.Electrics
            };
            var finds = Items.FindAll(i => i.Equals(itemSearch));
            return SelectItemPanelFromFinds(finds);
        }

        private static void LoadItems (MDMEntities ents)
        {
            //using (var ents = ConnectEntities())
            //{
                Items = ents.Item_construction.AsNoTracking().Select(s=>
                          new ItemConstructionDbo {
                             HandMarkNoColour = s.Hand_mark_no_colour,
                             ItemGroupId = s.Item_group_id,
                             ItemGroup = s.Item_group.Item_group1,
                             Length = s.Length,
                             Height = s.Height,
                             Thickness = s.Thickness,
                             Formwork = s.Formwork,
                             FormworkMirror = s.FormworkMirror,
                             MountElementModif = s.MountElementModif,
                             ProngModif = s.ProngModif,
                             BalconyCutModif = s.BalconyCutModif,
                             BalconyDoorModif = s.BalconyDoorModif,
                             StepHeightModif = s.StepHeightModif,
                             StepCount = s.Steps,
                             Electrics = s.Electrics,
                             ItemConstructionId = s.Item_construction_id,
                             Weight = s.Weight,
                             Volume = s.Volume
                         }).ToList();
            //}           
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
                return Result.Fail<string>($"Не найдена группа {panel.Item_group}.");
                //throw new Exception($"Не найдена группа {panel.ItemGroup}");
            }
                                                
            if (panel.DbGroup.HasFormula == null)
            {
                // Не задано значение HasFormula - считается, что это ошибка определения формулы для группы.
                return Result.Fail<string>($"Не задана формула формирования марки для этой группы панелей {panel.Item_group}.");
                //throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
            }
            bool hasFormula = panel.DbGroup.HasFormula.Value;
            if (hasFormula)
            {
                // Формула для группы панели
                string formula = panel.DbGroup.Formula;
                if (formula == null)
                {
                    return Result.Fail<string>($"Не задана формула формирования марки для этой группы панелей {panel.Item_group}.");
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
                    var item = new ItemConstructionDbo() {
                        Panel = panel,
                        IsIgnoreGab = panel.IsIgnoreGab,
                        HandMarkNoColour = panel.Mark,
                        ItemGroupId = panel.DbGroup.ItemGroupId,
                        Length = panel.Length,
                        Height = panel.Height,
                        Thickness = panel.Thickness,
                        Formwork = panel.Formwork,
                        MountElement = panel.Mount_element,
                        Prong = panel.Prong,
                        BalconyCut = panel.Balcony_cut,
                        BalconyDoor = panel.Balcony_door,
                        StepHeightIndex = panel.Step_height,
                        StepCount = panel.Steps,
                        StepFirstHeight = panel.First_step,
                        Electrics = panel.Electrics
                    };
                    int id;
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

        /// <summary>
        /// Регистрация колористики
        /// </summary>        
        public static void RegisterColors (List<Panel> panelsColor)
        {
            using (var ents = ConnectEntities())
            {
                var registeredColors = new List<Panel>();
                // Запись колористических индексов в справочник
                Dictionary<string, int> dictColors;
                FillColorsIndexes(panelsColor, ents, out dictColors);
                foreach (var item in panelsColor)
                {
                    try
                    {
                        var colorItemNew = new Item_construction_colour {
                            Item_colour_id = dictColors[item.Color],
                            Hand_mark = item.GetMarkWithColor(),
                            Item_construction_id = item.DbItem.ItemConstructionId                                                                                     
                        };
                        ents.Item_construction_colour.Add(colorItemNew);
                        ents.SaveChanges();

                        registeredColors.Add(item);
                    }
                    catch
                    {
                        // Ошибка если такая колористика уже есть. Стас сказал что это нормально.
                    }                    
                }

                // Лог зарегистрированной колористики
                var sbRegs = new StringBuilder("Зарегистрированная колористика:");
                foreach (var item in registeredColors)
                {
                    sbRegs.AppendLine($"{item.MarkByFormula}, {item.Color} = {item.GetMarkWithColor()}");
                }
                Logger.Log.Error(sbRegs.ToString());
            }
        }

        private static void FillColorsIndexes (List<Panel> panelsColor, MDMEntities ents, out Dictionary<string, int> dictColors)
        {
            dictColors = new Dictionary<string, int>();
            var colorsIndexes = panelsColor.Select(s => s.Color).GroupBy(g => g).Select(s => s.Key);
            var colorsNew = colorsIndexes.Where(w => !ents.Item_colour.Any(a => a.Item_colour1.Equals(w, StringComparison.OrdinalIgnoreCase)));
            if (colorsNew.Any())
            {
                foreach (var item in colorsNew)
                {
                    ents.Item_colour.Add(new Item_colour {
                        Item_colour1 = item
                    });                    
                }
                ents.SaveChanges();
            }
            foreach (var item in colorsIndexes)
            {
                var colorId = ents.Item_colour.First(f => 
                        f.Item_colour1.Equals(item, StringComparison.OrdinalIgnoreCase)).Item_colour_id;
                dictColors.Add(item, colorId);
            }
        }

        private static string getLogRegistryPanels (List<Panel> registerPanelsToLog)
        {
            string res = string.Join("; ", registerPanelsToLog.Select(p=> p.Mark +  " id=" + p.ItemConstructionId));
            return res;
        }

        public static bool Register(ItemConstructionDbo panel, SerieDbo ser, out int id)
        {
            id = -1;
            using (var ents = ConnectEntities())
            {
                var newItem = ents.Item_construction.Add(new Item_construction() {
                    Hand_mark_no_colour = panel.HandMarkNoColour,
                    Item_group_id = panel.ItemGroupId,
                    Length = panel.Length,
                    Height = panel.Height,
                    Thickness = panel.Thickness,
                    Formwork = panel.Formwork,
                    Mount_element_id = GetModificatorId(panel.MountElement, MountParamName)?.Id,
                    Prong_id = GetModificatorId(panel.Prong, PrognParamName)?.Id,
                    Balcony_cut_id = GetModificatorId (panel.BalconyCut, BalconyCutParamName)?.Id,
                    Balcony_door_id = GetModificatorId(panel.BalconyDoor, BalconyDoorParamName)?.Id,
                    Step_height_id = GetModificatorId(panel.StepHeightIndex.ToString(), StepHeightParamName)?.Id,
                    Steps = panel.StepCount,
                    First_step = panel.StepFirstHeight,
                    Electrics = panel.Electrics
                });
                if (ser != null)
                {
                    ents.Item_construction_Product.Add(new Item_construction_Product() {
                        Item_construction = newItem,
                        Product_id = ser.SeriesId
                    });
                }

                // запись панели в таблицу Item_construction_colour, без привязки к Color
                ents.Item_construction_colour.Add(new Item_construction_colour {
                   Item_construction = newItem,
                   Hand_mark = panel.Panel.Mark,
                   Item_colour_id = 1
                });

                ents.SaveChanges();
                id = newItem.Item_construction_id;
            }
            return true;
        }

        public static ModificatorDbo GetModificatorId (string value, string name)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var mod =Modificators.FirstOrDefault(m => m.TypeModificator.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    m.Code.Equals(value, StringComparison.OrdinalIgnoreCase));
            return mod;
        }

        public static ItemGroupDbo FindGroup(string itemGroup)
        {
            return Groups.FirstOrDefault(g => g.ItemGroup.Equals(itemGroup, StringComparison.OrdinalIgnoreCase));
        }

        //public static BalconyCutDbo GetBalconyCutItem(string balconyCut)
        //{
        //    return BalconyCuts.Where(b => b.Name.Equals(balconyCut)).FirstOrDefault();
        //}

        //public static BalconyDoorDbo GetBalconyDoorItem(string balconyDoor)
        //{
        //    return BalconyDoors.Where(b => b.Name.Equals(balconyDoor)).FirstOrDefault();
        //}        

        public static bool HasColorFullHandMark (string colorFullMark)
        {
            var res = colorsHandMarkFull.Contains(colorFullMark);
            return res;
        }
    }
}