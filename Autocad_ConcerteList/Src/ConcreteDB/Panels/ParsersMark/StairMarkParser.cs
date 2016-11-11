using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Парсер марки лестничного марша (ЛМ-1.11-24)
    /// </summary>
    public class StairMarkParser : IMarkParser
    {
        ParserMark parserBase;
        /// <summary>
        /// Парсинг марки и запись результатов в основной объект парсингва марки
        /// </summary>        
        public void Parse (ParserMark parserBase)
        {
            this.parserBase = parserBase;
            if (parserBase.MarkInput.StartsWith("ЛМ-"))
            {
                // Предполагаемый состав оставшейся части марки - 1.11-24 (1-индекс высоты ступеней, 11-кол ступеней, 24- высота первой ступени)
                parseNew(parserBase.MarkInput.Substring(3));
            }
            else
            {
                throw new NotImplementedException($"Парсинг {parserBase.MarkInput} - не предусмотрен.");
            }
        }

        /// <summary>
        /// Разбор части марки после ЛМ-, типа 1.11-24 - 
        /// Новая запись марки для ЛМ.
        /// </summary>
        /// <param name="markFromStep"></param>
        private void parseNew (string markFromStep)
        {
            var dots = markFromStep.Split('.');
            parserBase.StepHeightIndex = int.Parse(dots[0]);
            var dashs = dots[1].Split('-');
            parserBase.StepsCount = int.Parse(dashs[0]);
            if (dashs.Length>1)
            {
                parserBase.StepFirstHeight = int.Parse(dashs[1]);
            }
            parserBase.MarkWoGroupClassIndex = parserBase.MarkInput;                        
        }
    }
}
