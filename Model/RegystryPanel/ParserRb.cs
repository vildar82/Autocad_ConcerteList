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
    }
}
