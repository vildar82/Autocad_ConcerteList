using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using AcadLib;
using AcadLib.Errors;
using Autocad_ConcerteList.Properties;
using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.ConcreteDB.Formula;

namespace Autocad_ConcerteList.Src.RegystryPanel
{
    /// <summary>
    /// Сервисные функции работы с базой ЖБИ
    /// </summary>
    public static class DbService
    {
        static List<I_S_BalconyCut> BalconyCuts;
        static List<I_S_BalconyDoor> BalconyDoors;
        static I_C_Series SerPik2;

        public static SAPREntities ConnectEntities()
        {            
            return new SAPREntities(new EntityConnection(Settings.Default.SaprCon));
        }

        public static void Init()
        {
            using (var ents = ConnectEntities())
            {
                BalconyCuts = ents.I_S_BalconyCut.ToList();
                BalconyDoors = ents.I_S_BalconyDoor.ToList();
                SerPik2 = ents.I_C_Series.First(s => s.Series == "ПИК-2.0");
            }
        }

        /// <summary>
        /// Поиск панели в базе по параметрам
        /// </summary>
        public static I_J_ItemConstruction FindByParameters(
            string ItemGroup, short? Lenght, short? Height, short? Thickness,
                short? Formwork, string BalconyDoor, string BalconyCut, string Electrics
            )
        {   
            using (var ents = ConnectEntities())
            {
                var query = ents.I_J_ItemConstruction.Where(i =>
                        i.I_S_ItemGroup.ItemGroup == ItemGroup &&
                        i.Lenght == Lenght && i.Height == Height && i.Thickness == Thickness &&
                        i.Formwork == Formwork &&
                        (BalconyDoor == null || i.I_S_BalconyDoor.BalconyDoor == BalconyDoor) &&
                        (BalconyCut == null || i.I_S_BalconyCut.BalconyCut == BalconyCut) &&
                        i.Electrics == Electrics
                    );
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

        public static I_J_ItemConstruction FindByParameters(Panel panel)
        {
            return FindByParameters(panel.ItemGroup, panel.Lenght, panel.Height, panel.Thickness, panel.Formwork,
                panel.BalconyDoor, panel.BalconyCut, panel.Electrics);
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
                string formula = panel.DbGroup.I_C_Formula?.FormulaValue;
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
#if !DB
            // Регистрация панели в базе.
            using (var ents = ConnectEntities())
            {
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
                ents.SaveChanges();
            }
#endif            
        }        

        public static bool Register(Panel panel)
        {
            using (var ents = ConnectEntities())
            {
                var newItem = ents.I_J_ItemConstruction.Add(new I_J_ItemConstruction()
                {
                    HandMarkNoColour = panel.Mark,
                    I_S_ItemGroup = panel.DbGroup,
                    Lenght = panel.Lenght,
                    Height = panel.Height,
                    Thickness = panel.Thickness,
                    Formwork = panel.Formwork,
                    I_S_BalconyCut = panel.BalconyCutItem,
                    I_S_BalconyDoor = panel.BalconyDoorItem,
                    Electrics = panel.Electrics
                });
                ents.I_J_ItemSeries.Add(new I_J_ItemSeries()
                {
                    I_J_ItemConstruction = newItem,
                    I_C_Series = SerPik2
                });

                ents.SaveChanges();
            }
            return true;
        }

        public static I_S_ItemGroup FindGroup(string itemGroup)
        {
            using (var ents = ConnectEntities())
            {
                return ents.I_S_ItemGroup.FirstOrDefault(g => g.ItemGroup.Equals(itemGroup, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static I_S_BalconyCut GetBalconyCutItem(string balconyCut)
        {
            return BalconyCuts.Where(b => b.BalconyCut.Equals(balconyCut)).FirstOrDefault();
        }

        public static I_S_BalconyDoor GetBalconyDoorItem(string balconyDoor)
        {
            return BalconyDoors.Where(b => b.BalconyDoor.Equals(balconyDoor)).FirstOrDefault();
        }
    }
}