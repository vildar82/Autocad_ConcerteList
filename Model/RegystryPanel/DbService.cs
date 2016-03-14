using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.ConcreteDB.Formula;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;

namespace Autocad_ConcerteList.RegystryPanel
{
    /// <summary>
    /// Сервисные функции работы с базой ЖБИ
    /// </summary>
    public static class DbService
    {
        private static I_J_ItemConstructionTableAdapter itemConstrAdapter;
        private static I_C_FormulaTableAdapter itemFormulaAdapter;
        private static I_S_ItemGroupTableAdapter itemGroupAdapter;
        private static Dictionary<string, string> dictFormules;
        private static Dictionary<string, decimal> dictBalconyDoor;
        private static Dictionary<string, decimal> dictBalconyCut;

        public static void Init ()
        {
            itemConstrAdapter = new I_J_ItemConstructionTableAdapter();
            itemFormulaAdapter = new I_C_FormulaTableAdapter();
            itemGroupAdapter = new I_S_ItemGroupTableAdapter();
            dictFormules = new Dictionary<string, string>();

            I_S_BalconyDoorTableAdapter balconyDoorAdapter = new I_S_BalconyDoorTableAdapter();
            var balconyDoorTable = balconyDoorAdapter.GetData();
            dictBalconyDoor = balconyDoorTable.ToDictionary(b => b.BalconyDoor, b => b.BalconyDoorId);

            I_S_BalconyCutTableAdapter balconyCutAdapter = new I_S_BalconyCutTableAdapter();
            var balconyCutTable = balconyCutAdapter.GetData();
            dictBalconyCut = balconyCutTable.ToDictionary(b => b.BalconyCut, b => b.BalconyCutId);
        }

        /// <summary>
        /// Поиск панели в базе по параметрам
        /// </summary>        
        public static bool ExistPanel(Panel panel)
        {
            var count = itemConstrAdapter.IsMarkExecute(panel.ItemGroup, panel.Lenght, panel.Height, panel.Thickness, panel.Formwork,
                panel.BalconyDoor, panel.BalconyCut, panel.FormworkMirror, panel.Electrics);
            return count != null && count != 0;            
        }

        /// <summary>
        /// Определение марки панели по формуле из базы
        /// </summary>        
        public static string GetDbMark(Panel panel)
        {
            string markDb = null;
            // Формула для группы панели
            string formula = getFormula(panel.ItemGroup);
            if (string.IsNullOrEmpty(formula))
            {
                // Добавление ошибки в панель.
                Inspector.AddError($"Не найдена формула формирования марки для этой группы панелей {panel.ItemGroup}.",
                        System.Drawing.SystemIcons.Error);
            }
            else
            {
                // Получение марки панели по формуле
                ParserFormula parserFormula = new ParserFormula(formula, panel);
                try
                {
                    parserFormula.Parse();
                    markDb = parserFormula.Result;
                    if (string.IsNullOrEmpty(markDb))
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    Inspector.AddError($"Ошибка при определении марки панели. {ex.Message}",
                        System.Drawing.SystemIcons.Error);
                }                                
            }
            return markDb;
        }

        private static string getFormula(string itemGroup)
        {
            string formula;
            if (!dictFormules.TryGetValue(itemGroup, out formula))
            {
                // Поиск формулы в базе
                formula = itemFormulaAdapter.GetFormula(itemGroup);
                dictFormules.Add(itemGroup, formula);
            }
            return formula;
        }

        public static bool Register(Panel item)
        {
            // Регистрация панели в базе.            
            // Получение id группы
            decimal? idItemGroup = itemGroupAdapter.GetItemGroupId(item.ItemGroup);
            if (idItemGroup == null)
            {
                Inspector.AddError($"Не найдена группа {item.ItemGroup}.", System.Drawing.SystemIcons.Error);
                return false;
            }

            // idBalconyDoor
            decimal? idBalDoor = null;
            if (!string.IsNullOrEmpty(item.BalconyDoor))
            {
                decimal id;
                dictBalconyDoor.TryGetValue(item.BalconyDoor, out id);
                idBalDoor = id;
            }

            // idBalconyCut
            decimal? idBalCut = null;
            if (!string.IsNullOrEmpty(item.BalconyCut))
            {
                decimal id;
                dictBalconyCut.TryGetValue(item.BalconyCut, out id);
                idBalCut = id;
            }

#if !NODB
            itemConstrAdapter.InsertItem(idItemGroup, item.Lenght, item.Height, item.Thickness, item.Formwork,
                idBalDoor, idBalCut, item.FormworkMirror, item.Electrics, item.Weight, item.Volume, item.MarkDb);
#endif
            return true;
        }
    }
}
