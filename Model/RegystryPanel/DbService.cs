using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.ConcreteDB.DataSet.ConcerteDataSetTableAdapters;
using Autocad_ConcerteList.Model.ConcreteDB.Formula;

namespace Autocad_ConcerteList.Model.RegystryPanel
{
    /// <summary>
    /// Сервисные функции работы с базой ЖБИ
    /// </summary>
    public static class DbService
    {
        private static I_J_ItemConstructionTableAdapter itemConstrAdapter;        
        private static I_S_ItemGroupTableAdapter itemGroupAdapter;
        private static I_C_FormulaTableAdapter formulaAdapter;
        private static I_C_SeriesTableAdapter seriesAdapter;
        private static I_J_ItemSeriesTableAdapter itemSeriesAdapter;
        private static I_R_ItemColourTableAdapter colorAdapter;
        private static I_R_ItemTableAdapter colorConstructAdapter;
        //private static myItemTableAdapter myItemAdapter;
        //private static myFormulaTableAdapter myTableFormula;
        private static Dictionary<decimal,string> dictFormules;
        private static Dictionary<string, decimal> dictBalconyDoor;
        private static Dictionary<string, decimal> dictBalconyCut;

        public static void Init ()
        {
            //  Считывание конфига
            ReadConfigFile();

            itemConstrAdapter = new I_J_ItemConstructionTableAdapter();
            //myTableFormula = new myFormulaTableAdapter();
            //myItemAdapter = new myItemTableAdapter();
            itemGroupAdapter = new I_S_ItemGroupTableAdapter();
            formulaAdapter = new I_C_FormulaTableAdapter();
            seriesAdapter = new I_C_SeriesTableAdapter();
            itemSeriesAdapter = new I_J_ItemSeriesTableAdapter();
            colorAdapter = new I_R_ItemColourTableAdapter();
            colorConstructAdapter = new I_R_ItemTableAdapter();
            dictFormules = new Dictionary<decimal, string>();

            I_S_BalconyDoorTableAdapter balconyDoorAdapter = new I_S_BalconyDoorTableAdapter();
            var balconyDoorTable = balconyDoorAdapter.GetData();
            dictBalconyDoor = balconyDoorTable.ToDictionary(b => b.BalconyDoor, b => b.BalconyDoorId);

            I_S_BalconyCutTableAdapter balconyCutAdapter = new I_S_BalconyCutTableAdapter();
            var balconyCutTable = balconyCutAdapter.GetData();
            dictBalconyCut = balconyCutTable.ToDictionary(b => b.BalconyCut, b => b.BalconyCutId);
        }

        private static void ReadConfigFile()
        {
            // Чтение конфигурационного файла и строки подключения
            try
            {
                string configFile = Assembly.GetExecutingAssembly().Location + ".config";
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = configFile;
                var cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);                
                Properties.Settings.Default.SAPRConnectionString = 
                    cfg.ConnectionStrings.ConnectionStrings["SAPRConnectionString"].ConnectionString;
            }
            catch { }
        }

        /// <summary>
        /// Поиск панели в базе по параметрам
        /// </summary>        
        public static ConcreteDB.DataSet.ConcerteDataSet.I_J_ItemConstructionDataTable FindByParameters(Panel panel)
        {
            return itemConstrAdapter.FinfByParameters(panel.Lenght, panel.Height, panel.Thickness,
                panel.Formwork, panel.FormworkMirror, panel.Electrics, panel.ItemGroupId, panel.BalconyDoorId, panel.BalconyCutId);                   
        }

        public static ConcreteDB.DataSet.ConcerteDataSet.I_J_ItemConstructionDataTable FindByMark(string mark)
        {
            return itemConstrAdapter.FindByMark(mark);
        }

        public static List<Model.ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow> GetSeries()
        {
            var series = seriesAdapter.GetData();
            return series.ToList();
        }

        /// <summary>
        /// Определение марки панели по формуле из базы
        /// </summary>        
        public static string GetDbMark(Panel panel)
        {
            string markDb = null;

            // Получение id группы     
            var dbGroup = FindGroup(panel.ItemGroup);
            if (dbGroup == null)
            {
                throw new Exception($"Не найдена группа {panel.ItemGroup}");
            }
            
            panel.ItemGroupId = dbGroup.ItemGroupId;
            panel.ItemGroup = dbGroup.ItemGroup;

            bool hasFormula = false;
            try
            {
                hasFormula = dbGroup.HasFormula;
            }
            catch
            {
                // Не задано значение HasFormula - считается, что это ошибка определения формулы для группы.
                throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
            }

            if (hasFormula)
            {
                // Формула для группы панели
                string formula = getFormula(dbGroup.FormulaId);
                if (formula == null)
                {
                    // Добавление ошибки в панель.                     
                    throw new Exception($"Не задана формула формирования марки для этой группы панелей {panel.ItemGroup}");
                }
                else
                {
                    // Попытка получения марки панели по формуле                     
                    ParserFormula parserFormula = new ParserFormula(formula, panel);
                    parserFormula.Parse();
                    markDb = parserFormula.Result;
                }
            }
            else
            {
                // Если для группы панелей задано что нет формулы, то марка панели = марке без вычислений
                markDb = panel.Mark;
            }     
            return markDb;
        }

        /// <summary>
        /// Удаление панели
        /// </summary>
        /// <param name="panel"></param>
        internal static int RemovePanel(Panel panel)
        {
            return itemConstrAdapter.DeleteByHandMark(panel.Mark);
        }

        private static string getFormula(decimal idFormula)
        {
            string resVal = null;
            if (!dictFormules.TryGetValue(idFormula, out resVal))
            {
                // Поиск формулы в базе                
                resVal = formulaAdapter.GetFormula(idFormula);                
                dictFormules.Add(idFormula, resVal);
            }
            return resVal;
        }        

        public static bool Register(Panel item, ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow ser)
        {
            // Регистрация панели в базе.                        

            // idBalconyDoor
            decimal? idBalDoor = null;
            if (!string.IsNullOrEmpty(item.BalconyDoor))
            {
                decimal id;
                if (!dictBalconyDoor.TryGetValue(item.BalconyDoor, out id))
                {
                    throw new Exception($"В базе не определена марка Балкона - {item.BalconyDoor}");
                }
                idBalDoor = id;
            }

            // idBalconyCut
            decimal? idBalCut = null;
            if (!string.IsNullOrEmpty(item.BalconyCut))
            {
                decimal id;
                if (!dictBalconyCut.TryGetValue(item.BalconyCut, out id))
                {
                    throw new Exception($"В базе не определена марка Подрезки - {item.BalconyCut}");
                }                    
                idBalCut = id;
            }

#if !NODB
            decimal itemConstrId = (itemConstrAdapter.InsertItem(item.ItemGroupId, item.Lenght, item.Height, item.Thickness, item.Formwork,
                idBalDoor, idBalCut, item.FormworkMirror, item.Electrics, item.Weight, item.Volume, item.Mark) as decimal?).Value;

            itemSeriesAdapter.InsertItem(itemConstrId, ser.SeriesId);

            RegColor(item, itemConstrId);
#endif
            return true;
        }       

        /// <summary>
        /// Регистрация покраски
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="itemConstructId"></param>
        public static void RegColor(Panel panel, decimal itemConstructId)
        {
            if (!string.IsNullOrEmpty(panel.Color))
            {
                decimal idColor;
                var colorRow = colorAdapter.FindColorId(panel.Color).FirstOrDefault();
                if (colorRow == null)
                {
                    idColor = (colorAdapter.InsertColor(panel.Color) as decimal?).Value;
                }
                else
                {
                    idColor = colorRow.ItemColourId;
                }
                colorConstructAdapter.InsertItem(panel.MarkDb, itemConstructId, idColor);
            }
        }

        public static ConcreteDB.DataSet.ConcerteDataSet.I_S_ItemGroupRow FindGroup(string itemGroup)
        {            
            return itemGroupAdapter.GetItemGroup(itemGroup).FirstOrDefault();            
        }

        public static decimal? GetBalconyCutId(string balconyCut)
        {
            decimal id;
            if (balconyCut!=null && dictBalconyCut.TryGetValue(balconyCut, out id))
            {
                return id;
            }
            else
            {
                return null;
            }
        }

        public static decimal? GetBalconyDoorId(string balconyDoor)
        {
            decimal id;
            if (balconyDoor!=null && dictBalconyDoor.TryGetValue(balconyDoor, out id))
            {
                return id;
            }
            else
            {
                return null;
            }
        }
    }
}
