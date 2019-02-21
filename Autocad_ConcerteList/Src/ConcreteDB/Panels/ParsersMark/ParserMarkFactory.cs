using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NetLib;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public static class ParserMarkFactory
    {
        /// <summary>
        /// Сопоставление группы панели (без цифр!) с типом панели
        /// </summary>
        private static readonly Dictionary<string, PanelTypeEnum> dictPanelTypes = new Dictionary<string, PanelTypeEnum>
        {            
            { "ЭБ", PanelTypeEnum.EB },
            { "ЛП", PanelTypeEnum.LP },
            { "ОЛ", PanelTypeEnum.OL },
            { "ПЛ", PanelTypeEnum.PL },
            { "ПБ", PanelTypeEnum.Slab },
            { "БП", PanelTypeEnum.Slab },
            { "П", PanelTypeEnum.Slab },
	        { "ПК", PanelTypeEnum.Slab },
	        { "ПКД", PanelTypeEnum.Slab },
	        { "ПКТ", PanelTypeEnum.Slab },
			{ "ЛМ", PanelTypeEnum.Stair },
            { "БВ", PanelTypeEnum.VentBlock },
            { "ДУ", PanelTypeEnum.VentBlock },                        
            { "В", PanelTypeEnum.WallInner },
            { "ВЧ", PanelTypeEnum.WallInner }            
        };

	    private static readonly Dictionary<PanelTypeEnum, Type> dictMarkParsers =
		    new Dictionary<PanelTypeEnum, Type>
		    {
			    {PanelTypeEnum.Stair, typeof(StairMarkParser)}
		    };

        public static IParserMark Create(MarkPart markPart)
        {
            IParserMark parserMark = null;
            //  Для указанных групп - индивидуальные парсеры
            if (dictMarkParsers.TryGetValue(markPart.PanelType, out Type parserType))
            {
                parserMark = (IParserMark)Activator.CreateInstance(parserType, markPart);
            }
            else
            {
                // Для всех остальных групп
                parserMark = new ParserMark(markPart);
            }
            return parserMark;
        }  

        public static Result<MarkPart> DefineParts(string mark)
        {
            // "3НСНг-Б1.2-389.294.42-2"
            var markPart = new MarkPart(mark);
            // Разделение группы от габаритов - первая точка. Не подходит для марки "3НСНг-Б1.2-389.294.42-2"!!!
            markPart.IsNewBIndex = IsNewBIndexInMark(mark, out int indexNewB, out string newBValue, out string gab);
            markPart.NewBIndex = newBValue;
            var indexFirstDot = mark.IndexOf('.');
            if (markPart.IsNewBIndex)
            {
                markPart.PartGroup = mark.Substring(0,indexNewB).Trim('-').Trim();
                var gabAndDop = gab.Trim('-').Trim();
                DefineGabAndDop(gabAndDop, ref markPart);
            }
            else if (indexFirstDot != -1)
            {
                // Есть точка. Значит группа соеденена с габаритом длины. "2П72"
                markPart.PartGroup = SeparateGroupFromLen(mark.Substring(0, indexFirstDot), indexFirstDot - 1);
                if (string.IsNullOrEmpty(markPart.PartGroup))
                {
                    return Result.Fail<MarkPart>("Не определена группа панели.");
                }
                var gabAndDop = mark.Substring(markPart.PartGroup.Length);
                DefineGabAndDop(gabAndDop, ref markPart);
            }
            else
            {
                // нет габаритов в марке. Разделить по первому тире
                var indexDash = mark.IndexOf('-');
                if (indexDash == -1)
                {
                    return Result.Fail<MarkPart>("Ошибка определения блока панели - В марке определена только группа панели.");
                }
                markPart.PartGroup = mark.Substring(0, indexDash).Trim();
                if (string.IsNullOrEmpty(markPart.PartGroup))
                {
                    return Result.Fail<MarkPart>("Не определена группа панели.");
                }
                markPart.PartDop = mark.Substring(indexDash + 1);
            }            
            markPart.MarkInputAfterGroup = markPart.Mark.Substring(markPart.PartGroup.Length).Trim();
            // Определение типа панели
            DefineItemGroupWoClassNew(markPart);
            markPart.PanelType = DefinePanelType(markPart.PartGroup);
            // определение серии
            markPart.PanelSeria = DefineSeria(markPart);            
            //markPart.DBGroup = DbService.FindGroup(markPart.ItemGroupWoClassNew);            
            return Result.Ok(markPart);
        }

        /// <summary>
        /// Проверка, что в марке панели есть индекс вида -Б1.2-
        /// </summary>
        private static bool IsNewBIndexInMark(string mark, out int index,
            out string newBValue, out string gab)
        {
            // "3НСНг-Б1.2-389.294.42-2"
            var match = Regex.Match(mark, @"[-|\s]Б\d(\.\d)?[-| ]");
            if (match.Success)
            {
                newBValue = match.Value.Trim('-').Trim();
                index = match.Index;
                gab = mark.Substring(match.Index + match.Length);
                return true;
            }
            newBValue = string.Empty;
            index = 0;
            gab = string.Empty;
            return false;
        }

        private static PanelTypeEnum DefinePanelType(string itemGroup)
        {
            var itemGroupWoDigits = Regex.Replace(itemGroup, "[0-9]", "").ToUpper().Trim();
            if (Regex.IsMatch(itemGroupWoDigits, "НС|НЧ"))
            {
                return PanelTypeEnum.WallOuter;
            }
            if (Regex.IsMatch(itemGroupWoDigits, "НФ|ПСФ"))
            {
                return PanelTypeEnum.WallOuterFreeze;
            }
            if (!dictPanelTypes.TryGetValue(itemGroupWoDigits, out PanelTypeEnum panelType))
            {
                throw new Exception($"Неопределенная группа панели - {itemGroup}");
            }
            return panelType;
        }

        private static PanelSeria DefineSeria(MarkPart markPart)
        {
            if (markPart.PanelType == PanelTypeEnum.WallOuter)
            {
                var indexDot = markPart.PartGab.IndexOf(".");
                if (indexDot != -1)
                {
                    var length = markPart.PartGab.Substring(0, indexDot).Trim();
                    if (length.Length == 3)
                    {
                        return PanelSeria.PIK2;
                    }                    
                }
            }
			else if (markPart.PanelType == PanelTypeEnum.WallInner)
            {
				var indexHeight = markPart.PartGab.Split('.').Skip(1).FirstOrDefault();
	            if (!string.IsNullOrEmpty(indexHeight) && indexHeight.Length == 3)
	            {
		            return PanelSeria.PIK2;
	            }
			}
            return PanelSeria.PIK1;
        }

        private static string SeparateGroupFromLen(string markInput, int indexFirstDot)
        {
            var indexDash = markInput.IndexOf('-');
            if (indexDash != -1)
            {
                indexFirstDot = indexDash;
            }
            // Отделить группу от длины в строке марки. "2П72.29"
            for (var i = indexFirstDot; i >= 0; i--)
            {
                if (!char.IsDigit(markInput[i]))
                {
                    return markInput.Substring(0, i + 1).Trim('-').Trim();
                }
            }
            return string.Empty;
        }

        private static void DefineGabAndDop(string input, ref MarkPart markGroup)
        {
            // На входе - "544.363-1-2э", получить 544.363 и 1-2э
            if (input.StartsWith("-"))
            {
                input = input.Substring(1);
            }
            var splitDash = input.Trim().Split(new[] { '-' }, 2);
            if (splitDash.Length > 1)
            {
                // Есть и габариты и доп параметры
                markGroup.PartGab = splitDash[0];
                markGroup.PartDop = splitDash[1];
            }
            else
            {
                // Нет тире - нет доп параметров. Только габариты.
                markGroup.PartGab = input.Trim();
            }
        }

        private static void DefineItemGroupWoClassNew(MarkPart markPart)
        {
            var splitSpace = markPart.PartGroup.Trim().Split(' ');

            if (int.TryParse(splitSpace[0], out int num))
            {
                markPart.ItemGroupWoClassNew = markPart.PartGroup.Trim();
            }
            else
            {
                if (splitSpace.Length == 2)
                {
                    markPart.GroupIndexClassNew = splitSpace[1].Trim();
                }
                markPart.ItemGroupWoClassNew = splitSpace[0].Trim();
            }
        }                
    }
}