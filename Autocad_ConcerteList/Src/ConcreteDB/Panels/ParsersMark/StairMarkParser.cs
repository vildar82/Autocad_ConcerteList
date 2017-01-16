using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Парсер марки лестничного марша (ЛМ-1.11-24)
    /// Изм. от 15.12.2016 - доб.ширина - ЛМ-1.11.105-24, 105 = Height/10, необязательный параметр!
    /// </summary>
    public class StairMarkParser : ParserMark
    {
        public StairMarkParser(string mark, MarkPart markPart) : base(mark, markPart)
        {

        }

        /// <summary>
        /// Парсинг марки и запись результатов в основной объект парсингва марки
        /// </summary>        
        public override void Parse()
        {
            // Предполагаемый состав оставшейся части марки - 1.11-24 (1-индекс высоты ступеней, 11-кол ступеней, 24- высота первой ступени)
            parseNew(MarkInput.Substring(3));
        }

        /// <summary>
        /// Разбор части марки после ЛМ-, типа 1.11-24. 
        /// Новая запись марки для ЛМ.
        /// Изм. 1.11.105-24, причем 105 необязательный
        /// </summary>
        /// <param name="markFromStep"></param>
        private void parseNew (string markFromStep)
        {
            var dots = markFromStep.Split('.');
            StepHeightIndex = int.Parse(dots[0]);
            string[] dashs;
            if (dots.Length == 3)
            {
                // Задана ширина
                StepsCount = int.Parse(dots[1]);
                dashs = dots[2].Split('-');
                Height = (short)(short.Parse(dashs[0])*10);
            }
            else
            {
                dashs = dots[1].Split('-');
                StepsCount = int.Parse(dashs[0]);
                Height = 1050; // Ширина обычного марша
            }
            if (dashs.Length>1)
            {
                StepFirstHeight = int.Parse(dashs[1]);
            }
            MarkWoGroupClassIndex = MarkInput;                        
        }
    }
}
