using AcadLib.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public static class ParserMarkFactory
    {
        private static Dictionary<string, Type> dictMarkParsers = new Dictionary<string, Type> {
            { "ЛМ", typeof(StairMarkParser) }
        };

        public static IParserMark Create(string mark)
        {
            IParserMark parserMark = null;
            MarkPart markPart;
            var resDefPart = ParserMark.DefineParts(mark, out markPart);
            if (resDefPart.Success)
            {                
                Type parserType;
                //  Для указанных групп - индивидуальные парсеры
                if (dictMarkParsers.TryGetValue(markPart.PartGroup, out parserType))
                {
                    parserMark = (IParserMark)Activator.CreateInstance(parserType, mark, markPart);
                }
                else
                {
                    // Для всех остальных групп
                    parserMark = new ParserMark(mark, markPart);
                }
            }
            else
            {
                ///???
                Inspector.AddError($"Ошибка определения панели - {resDefPart.Error}");                
            }
            return parserMark;
        }        
    }
}