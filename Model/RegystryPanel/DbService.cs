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
        private static I_S_ItemGroupTableAdapter itemGroupAdapter;
        private static myTableFormulaTableAdapter myTableFormula;
        private static Dictionary<string, Tuple<bool, string>> dictFormules;
        private static Dictionary<string, decimal> dictBalconyDoor;
        private static Dictionary<string, decimal> dictBalconyCut;

        public static void Init ()
        {
            itemConstrAdapter = new I_J_ItemConstructionTableAdapter();
            myTableFormula = new myTableFormulaTableAdapter();
            itemGroupAdapter = new I_S_ItemGroupTableAdapter();
            dictFormules = new Dictionary<string, Tuple<bool, string>>();

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

            // Получение id группы
            var itemGroupRow = itemGroupAdapter.GetItemGroup(panel.ItemGroup).FirstOrDefault();
            if (itemGroupRow == null)
            {
                Inspector.AddError($"Не найдена группа {panel.ItemGroup}.", System.Drawing.SystemIcons.Error);
                return null;
            }
            panel.ItemGroupId = itemGroupRow.ItemGroupId;
            panel.ItemGroup = itemGroupRow.ItemGroup;

            // Формула для группы панели
            Tuple<bool, string> formula = getFormula(panel.ItemGroup);
            if (formula == null)
            {
                // Добавление ошибки в панель.
                Inspector.AddError($"Не найдена формула формирования марки для этой группы панелей {panel.ItemGroup}.",
                        System.Drawing.SystemIcons.Error);
            }
            else if (!formula.Item1)
            {
                return panel.Mark;
            }
            else
            {
                // Получение марки панели по формуле
                ParserFormula parserFormula = new ParserFormula(formula.Item2, panel);
                parserFormula.Parse();
                markDb = parserFormula.Result;
            }
            return markDb;
        }

        private static Tuple<bool, string> getFormula(string itemGroup)
        {
            Tuple<bool, string> resVal = null;
            if (!dictFormules.TryGetValue(itemGroup, out resVal))
            {
                // Поиск формулы в базе
                var formulaRow = myTableFormula.GetFormula(itemGroup).FirstOrDefault();
                if (formulaRow != null)
                {
                    resVal = new Tuple<bool, string>(formulaRow.HasFormula, formulaRow.FormulaValue);
                }
                dictFormules.Add(itemGroup, resVal);
            }
            return resVal;
        }

        public static bool Register(Panel item)
        {
            // Регистрация панели в базе.                        

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
            itemConstrAdapter.InsertItem(item.ItemGroupId, item.Lenght, item.Height, item.Thickness, item.Formwork,
                idBalDoor, idBalCut, item.FormworkMirror, item.Electrics, item.Weight, item.Volume, item.MarkDb);
#endif
            return true;
        }
    }
}
