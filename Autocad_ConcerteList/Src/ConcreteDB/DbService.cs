﻿using System;
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
        static SerieDbo SerPik2;

        public static SAPREntities ConnectEntities()
        {            
            return new SAPREntities(new EntityConnection(Settings.Default.SaprCon));
        }

        public static void Init()
        {
            using (var ents = ConnectEntities())
            {                
                Groups = ents.I_S_ItemGroup.AsNoTracking().Select(s => new ItemGroupDbo
                {
                    ItemGroupId = s.ItemGroupId,
                    ItemGroup = s.ItemGroup,
                    HasFormula = s.HasFormula,
                    Formula = s.I_C_Formula == null ? null : s.I_C_Formula.FormulaValue,
                    LengthFactor = s.I_C_Formula == null ? (short)1 : s.I_C_Formula.LengthFactor,
                    HeightFactor = s.I_C_Formula == null ? (short)1 : s.I_C_Formula.HeightFactor,
                    ThicknessFactor = s.I_C_Formula == null ? (short)1 : s.I_C_Formula.ThicknessFactor
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
                SerPik2 = ents.I_C_Series.AsNoTracking().Where(s => s.Series == "ПИК-2.0").Select(s => new SerieDbo
                {
                    Name = s.Series,
                    SeriesId = s.SeriesId
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Поиск панели в базе по параметрам
        /// </summary>
        public static ItemConstructionDbo FindByParameters(
            string ItemGroup, short? Lenght, short? Height, short? Thickness,
                short? Formwork, string BalconyDoor, string BalconyCut, string Electrics
            )
        {   
            using (var ents = ConnectEntities())
            {
                var query = (from i in ents.I_J_ItemConstruction.AsNoTracking()
                 join g in ents.I_S_ItemGroup.AsNoTracking() on i.ItemGroupId equals g.ItemGroupId

                 join bc in ents.I_S_BalconyCut.AsNoTracking() on i.BalconyCutId equals bc.BalconyCutId into gbc
                 from bcJoined in gbc.DefaultIfEmpty()

                 join bd in ents.I_S_BalconyDoor.AsNoTracking() on i.BalconyDoorId equals bd.BalconyDoorId into gbd
                 from bdJoined in gbd.DefaultIfEmpty()

                 where g.ItemGroup == ItemGroup &&
                       i.Lenght == Lenght && i.Height == Height && i.Thickness == Thickness &&
                       i.Formwork == Formwork &&
                       (bcJoined == null || bcJoined.BalconyCut == BalconyCut) &&
                       (bdJoined == null || bdJoined.BalconyDoor == BalconyDoor) &&
                       i.Electrics == Electrics
                 select new ItemConstructionDbo {
                     HandMarkNoColour = i.HandMarkNoColour,
                     ItemGroupId = i.ItemGroupId,
                     ItemGroup = g.ItemGroup,
                     Lenght = i.Lenght,
                     Height = i.Height,
                     Thickness = i.Thickness,
                     Formwork = i.Formwork,
                     FormworkMirror = i.FormworkMirror,
                     BalconyCut = bcJoined == null ? null : new BalconyCutDbo {
                         BalconyCutId = i.BalconyCutId.Value,
                         BalconyCutName = bcJoined.BalconyCut, BalconyCutSize = bcJoined.BalconyCutSize
                     },
                     BalconyDoor = bdJoined == null ? null : new BalconyDoorDbo {
                         BalconyDoorId = i.BalconyDoorId.Value,
                         BalconyDoorName = bdJoined.BalconyDoor
                     },
                     Electrics = i.Electrics,
                     ItemConstructionId = i.ItemConstructionId,
                     Weight = i.Weight,
                     Volume = i.Volume
                 }).ToList();

                //var query = ents.I_J_ItemConstruction.AsNoTracking().Where(i =>
                //        i.I_S_ItemGroup.ItemGroup == ItemGroup &&
                //        i.Lenght == Lenght && i.Height == Height && i.Thickness == Thickness &&
                //        i.Formwork == Formwork &&
                //        (BalconyDoor == null || i.I_S_BalconyDoor.BalconyDoor == BalconyDoor) &&
                //        (BalconyCut == null || i.I_S_BalconyCut.BalconyCut == BalconyCut) &&
                //        i.Electrics == Electrics
                //    ).Select(s => new ItemConstructionDbo
                //    {
                //        HandMarkNoColour = s.HandMarkNoColour,
                //        ItemGroupId = s.ItemGroupId,
                //        ItemGroup = s.I_S_ItemGroup.ItemGroup,
                //        Lenght = s.Lenght,
                //        Height = s.Height,
                //        Thickness = s.Thickness,
                //        Formwork = s.Formwork,
                //        FormworkMirror = s.FormworkMirror,
                //        BalconyCut = (s.I_S_BalconyCut == null) ? null : (new BalconyCutDbo { BalconyCutId = s.I_S_BalconyCut.BalconyCutId, BalconyCutName = s.I_S_BalconyCut.BalconyCut, BalconyCutSize = s.I_S_BalconyCut.BalconyCutSize }),
                //        BalconyDoor = (s.I_S_BalconyDoor == null)? null : (new BalconyDoorDbo { BalconyDoorId =s.I_S_BalconyDoor.BalconyDoorId, BalconyDoorName = s.I_S_BalconyDoor.BalconyDoor}),
                //        Electrics = s.Electrics,
                //        ItemConstructionId = s.ItemConstructionId,
                //        Weight = s.Weight,
                //        Volume = s.Volume
                //    }).ToList();
                var first = query.FirstOrDefault();

                if (query.Skip(1).Any())
                {
                    // Несколько записей в базе с этими параметрвами
                    string idsItems = string.Join(",", query.Select(i => i.ItemConstructionId));
                    Logger.Log.Error($"Найдено несколько панелей по параметрам для марки {first.HandMarkNoColour}, ItemConstructionId: {idsItems}.");
                }                
                return first;
            }            
        }

        public static ItemConstructionDbo FindByParameters(Panel panel)
        {
            return FindByParameters(panel.ItemGroup, panel.Lenght, panel.Height, panel.Thickness, panel.Formwork,
                panel.BalconyDoor, panel.BalconyCut, panel.Electrics);
        }

        public static ItemConstructionDbo FindByParametersFromAllLoaded(Panel panel)
        {
            if (Items == null)
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
                                 BalconyCut = bcJoined == null ? null : new BalconyCutDbo {
                                     BalconyCutId = i.BalconyCutId.Value,
                                     BalconyCutName = bcJoined.BalconyCut, BalconyCutSize = bcJoined.BalconyCutSize
                                 },
                                 BalconyDoor = bdJoined == null ? null : new BalconyDoorDbo {
                                     BalconyDoorId = i.BalconyDoorId.Value,
                                     BalconyDoorName = bdJoined.BalconyDoor
                                 },
                                 Electrics = i.Electrics,
                                 ItemConstructionId = i.ItemConstructionId,
                                 Weight = i.Weight,
                                 Volume = i.Volume
                             }).ToList();

                    //Items = ents.I_J_ItemConstruction.AsNoTracking().Where(i => i.I_S_ItemGroup.HasFormula != null).
                    //    Select(s=> new ItemConstructionDbo
                    //    {
                    //        HandMarkNoColour = s.HandMarkNoColour,
                    //        ItemGroupId = s.ItemGroupId,
                    //        ItemGroup = s.I_S_ItemGroup.ItemGroup,
                    //        Lenght = s.Lenght,
                    //        Height = s.Height,
                    //        Thickness = s.Thickness,
                    //        Formwork = s.Formwork,
                    //        FormworkMirror = s.FormworkMirror,
                    //        BalconyCut = (s.I_S_BalconyCut == null) ? null : (new BalconyCutDbo { BalconyCutId = s.I_S_BalconyCut.BalconyCutId, BalconyCutName = s.I_S_BalconyCut.BalconyCut, BalconyCutSize = s.I_S_BalconyCut.BalconyCutSize }),
                    //        BalconyDoor = (s.I_S_BalconyDoor == null) ? null : (new BalconyDoorDbo { BalconyDoorId = s.I_S_BalconyDoor.BalconyDoorId, BalconyDoorName = s.I_S_BalconyDoor.BalconyDoor }),
                    //        Electrics = s.Electrics,
                    //        ItemConstructionId = s.ItemConstructionId,
                    //        Weight = s.Weight,
                    //        Volume = s.Volume
                    //    }).ToList();                    
                }
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
            return finds.FirstOrDefault();
            //return Items.FirstOrDefault(i =>
            //            i.ItemGroup == panel.ItemGroup &&
            //            i.Lenght == panel.Lenght && i.Height == panel.Height && i.Thickness == panel.Thickness &&
            //            i.Formwork == panel.Formwork &&
            //            i.BalconyDoor?.BalconyDoorName == panel.BalconyDoor &&
            //            i.BalconyCut?.BalconyCutName == panel.BalconyCut &&
            //            i.Electrics == panel.Electrics
            //);
        }

        /// <summary>
        /// Определение марки панели по формуле из базы
        /// </summary>
        public static string GetDbMark(Panel panel)
        {
            string markDb = null;

            // Получение id группы
            if (panel.DbGroup == null)
            {
                panel.DbGroup = FindGroup(panel.ItemGroup);
                if (panel.DbGroup == null)
                {
                    throw new Exception($"Не найдена группа {panel.ItemGroup}");
                }
            }                        
            
            if (panel.DbGroup.HasFormula == null)
            {
                // Не задано значение HasFormula - считается, что это ошибка определения формулы для группы.
                throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
            }
            bool hasFormula = panel.DbGroup.HasFormula.Value;
            if (hasFormula)
            {
                // Формула для группы панели
                string formula = panel.DbGroup.Formula;
                if (formula == null)
                {
                    // Добавление ошибки в панель.
                    throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
                }
                else
                {
                    // Получение марки панели по формуле
                    ParserFormula parserFormula = new ParserFormula(formula, panel);
                    parserFormula.Parse();
                    markDb = parserFormula.Result;
                }
            }
            else
            {
                markDb = panel.Mark;
            }
            return markDb;
        }

        public static void Register(List<Panel> panels)
        {

            // Регистрация панели в базе.
            //using (var ents = ConnectEntities())
            //{
                foreach (var panel in panels)
                {
                    try
                    {
                        Register(panel);
                    }
                    catch (Exception ex)
                    {
                        // Ошибка при сохранении панели
                        Inspector.AddError($"Ошибка сохранения панели в базу - '{panel.Mark}'");
                        Logger.Log.Error(ex, $"Ошибка сохранения панели в базу - '{panel.ParamsToString()}'");
                    }                    
                }
                //ents.SaveChanges();
            //}
        }

        public static bool Register(Panel panel)
        {
            using (var ents = ConnectEntities())
            {
                var newItem = ents.I_J_ItemConstruction.Add(new I_J_ItemConstruction()
                {
                    HandMarkNoColour = panel.Mark,
                    ItemGroupId = panel.DbGroup.ItemGroupId,
                    Lenght = panel.Lenght,
                    Height = panel.Height,
                    Thickness = panel.Thickness,
                    Formwork = panel.Formwork,
                    BalconyCutId = panel.BalconyCutItem.BalconyCutId,
                    BalconyDoorId  = panel.BalconyDoorItem.BalconyDoorId,
                    Electrics = panel.Electrics
                });
                if (SerPik2 != null)
                {
                    ents.I_J_ItemSeries.Add(new I_J_ItemSeries()
                    {
                        I_J_ItemConstruction = newItem,
                        SeriesId = SerPik2.SeriesId
                    });
                }
#if DB
                ents.SaveChanges();
#endif
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