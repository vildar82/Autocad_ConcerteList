//Panels = new List<Panel>();

//Panels.Add(new Panel()
//{
//    ItemGroup = "3НСг",
//    Lenght = 7489,
//    Height = 2890,
//    Thickness = 320,
//    Formwork = 12,
//    BalconyCut = "П1",
//    BalconyDoor = "Б1",
//    Electrics = "1э",
//    Weight = 1,
//    Volume = 1,
//    Mark = "3НСг 75.29.32-12Б1П1-1э"
//});

//Panels.Add(new Panel()
//{
//    ItemGroup = "В",
//    Lenght = 2660,
//    Height = 2620,
//    Thickness = 160,                
//    Weight = 10,
//    Volume = 10,
//    Mark = "В266.26.16"
//});

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Autocad_ConcerteList.Model.RegystryPanel
{
    public class ParserRb
    {
        private ResultBuffer rb;

        public List<Panel> Panels { get; private set; }

        public ParserRb(ResultBuffer rb)
        {
            this.rb = rb;            
        }

        /// <summary>
        /// парсинг списка ResultBuffer переданного из лиспа
        /// Получение списка панелей - Panels
        /// При выбрасывании исключения, все прерывается.
        /// </summary>
        public void Parse()
        {
            if (rb == null)
                throw new ArgumentNullException();
            if (rb.AsArray().Length < 4)
            {
                return;
            }

            Panels = new List<Panel>();
            Panel panel = new Panel ();
            int countLB = 0;
            bool startDotPair = true;
            string param = string.Empty;
            object value = null;
            foreach (var item in rb)
            {
                if (item.TypeCode == (short)LispDataType.ListBegin)
                {
                    countLB++;                    
                }
                else if (item.TypeCode == (short)LispDataType.ListEnd)
                {
                    // Закрытие списка
                    countLB--;
                    startDotPair = true;
                    // Если это конец списка описания одной панели
                    if (countLB == 0)
                    {
                        // Начало списка параметров для одной панели
                        if (!string.IsNullOrEmpty(panel.Mark))
                        {
                            panel.DefineDbParams();
                            Panels.Add(panel);
                            panel = new Panel();
                        }
                    }
                }
                else if (item.TypeCode == (short)LispDataType.DottedPair)
                {
                    // Закрытие точечной пары
                    countLB--;
                    startDotPair = true;
                    panel.SetParameter(param, value);
                    param = string.Empty;
                    value = null;
                }
                else
                {
                    if (startDotPair)
                    {
                        param = item.Value.ToString();
                        startDotPair = false;
                    }
                    else
                    {
                        value = item.Value;
                        startDotPair = true;
                    }                                                                                       
                }
            }
        }

        private void printRb()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;
            foreach (var item in rb)
            {
                ed.WriteMessage("\n" + Enum.GetName(typeof(LispDataType),item.TypeCode) + " - " + item.Value);
            }                
        }
    }
}
