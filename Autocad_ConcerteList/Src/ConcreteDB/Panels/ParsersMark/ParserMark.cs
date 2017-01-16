using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class ParserMark : IParserMark
    {
        private MarkPart markPart;

        public ParserMark (string mark, MarkPart markPart)
        {
            MarkInput = mark;
            this.markPart = markPart;
        }

        /// <summary>
        /// Входная марка - из атрибута блока
        /// </summary>
        public string MarkInput { get; private set; }
        /// <summary>
        /// Марка без индекса "Класса бетона" GroupIndexClass
        /// Нужна для проверки имени блока, имя блока должно быть без индекса
        /// Т.к. индекс не влияет на геометрию блока изделия.
        /// </summary>
        public string MarkWoGroupClassIndex { get; set; }
        public Error Error { get;  set; }        
        /// <summary>
        /// Группа изделия. Например "3НСг". 
        /// </summary>
        public string ItemGroup { get; set; }
        /// <summary>
        /// Индекс "Класса Бетона" - Например 2,3 - 2П, 2В, 3В, 3НСг2 (2).
        /// </summary>
        public int GroupIndexClass { get;  set; }
        /// <summary>
        /// Длина - первое число в габаритах марки. Для группы вентблоков это может быть высота.
        /// </summary>        
        public short? Length { get; set; }
        /// <summary>
        /// Высота - второе число в габаритах марки.
        /// </summary>
        public short? Height { get; set; }
        /// <summary>
        /// Толщина - третий параметр в габаритах марки
        /// </summary>
        public short? Thickness { get; set; }        
        /// <summary>
        /// Опалубка. Например 1, 2. 
        /// </summary>
        public short? Formwork { get;  set; }
        //public short? FormworkMirror { get; private set; }   
        /// <summary>
        /// Закладная. Например: Д или пусто
        /// </summary>
        public string MountIndex { get; set; }
        /// <summary>
        /// Зубец. Например: Г или пусто
        /// </summary>
        public string ProngIndex { get; set; }
        /// <summary>
        /// Балкон. Б, Б1. 
        /// </summary>
        public string BalconyDoor { get;  set; }        
        /// <summary>
        /// Подрезка. П, П1. 
        /// </summary>
        public string BalconyCut { get;  set; }
        /// <summary>
        /// Высота ступени - индекс - 1,2,3 (ЛМ-1.11-15)
        /// </summary>
        public int? StepHeightIndex { get; set; }
        /// <summary>
        /// Кол. ступеней. - 11,12
        /// </summary>
        public int? StepsCount { get; set; }
        /// <summary>
        /// Высота первой ступени
        /// </summary>
        public int? StepFirstHeight { get; set; }
        /// <summary>
        /// Электрика. 1э, 2э. 
        /// </summary>
        public string Electrics { get;  set; }        

        public virtual void Parse()
        {
            parsePartGroup();
            parsePartGab();
            parsePartDop();
            // определение индекса класса бетона по группе
            defineIndexClass();                       
        }       

        public static Result DefineParts(string mark, out MarkPart markGroup)
        {
            markGroup = new MarkPart();
            int indexFirstDot = mark.IndexOf('.');
            if (indexFirstDot != -1)
            {
                // Есть точка. Значит группа соеденена с габаритом длины. "2П72"
                markGroup.PartGroup = SeparateGroupFromLen(mark, indexFirstDot - 1);
                if (string.IsNullOrEmpty(markGroup.PartGroup))
                {
                    return Result.Fail("Не определена группа панели.");                    
                }
                else
                {                    
                    string gabAndDop = mark.Substring(markGroup.PartGroup.Length);
                    DefineGabAndDop(gabAndDop, ref markGroup);
                }
            }
            else
            {
                // нет габаритов в марке. Разделить по первому тире
                int indexDash = mark.IndexOf('-');
                if (indexDash == -1)
                {
                    return Result.Fail("В марке определена только группа панели.");
                }
                else
                {
                    markGroup.PartGroup = mark.Substring(0, indexDash);
                    markGroup.PartDop = mark.Substring(indexDash + 1);
                }
            }
            return Result.Ok();
        }

        private static string SeparateGroupFromLen(string markInput, int indexFirstDot)
        {
            // Отделить группу от длины в строке марки. "2П72.29"
            for (int i = indexFirstDot; i >= 0; i--)
            {
                if (!char.IsDigit(markInput[i]))
                {
                    return markInput.Substring(0, i+1);
                }
            }
            return string.Empty;
        }

        private static void DefineGabAndDop(string input, ref MarkPart markGroup)
        {
            // На входе - "544.363-1-2э", получить 544.363 и 1-2э
            var splitDash = input.Split(new[] { '-' }, 2);
            if (splitDash.Length>1)
            {
                // Есть и габариты и доп параметры
                markGroup.PartGab = splitDash[0];
                markGroup.PartDop = splitDash[1];
            }
            else
            {
                // Нет тире - нет доп параметров. Только габариты.
                markGroup.PartGab = input;                
            }
        }

        private void parsePartGroup()
        {
            // Разбор группы. например partGroup = "2П"
            ItemGroup = markPart.PartGroup.Replace(" ", "").Replace("-", "");
        }

        private void parsePartGab()
        {
            // Разбор части строки относящейся к габаритам панели. Они разделены точками. Например partGab = "544.363"
            if (string.IsNullOrEmpty(markPart.PartGab)) return;
            var splitDot = markPart.PartGab.Split('.');
            if (splitDot.Length > 3)
            {
                // Ошибка. максимум, только 3 габарита - Длина, Высота, Толщина
                addErrorMsg("Определено больше трех габаритов разделенных точкой.");
            }
            Length = GetShort(splitDot[0], "Длина");
            if (splitDot.Length > 1)
            {
                Height = GetShort(splitDot[1], "Высота");
                if (splitDot.Length > 2)
                {
                    Thickness = GetShort(splitDot[2], "Толщина");
                }
            }
            if (markPart.PartGab.IndexOf("Д") != -1)
            {
                MountIndex = "Д";
            }
        }

        private short GetShort(string input, string nameParam)
        {
            return (short)GetStartInteger(input);            
        }

        private void parsePartDop()
        {
            // partDop - например "5-1-1э" - теперь без зеркальности "5-1э"
            if (string.IsNullOrEmpty(markPart.PartDop)) return;
            var splitDash = markPart.PartDop.Split('-');
            if (splitDash.Length>2)
            {
                // Ошибка. Может быть только опалубка и электрика. От Зеркальности отказались.
                addErrorMsg("Определено больше двух возможных дополнительных параметра панели - опалубки и электрики.");
            }            
            if (splitDash.Length == 1)
            {
                if (markPart.PartDop.IndexOf("э", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    definePartFormwork(markPart.PartDop);
                }
                else
                {
                    Electrics = markPart.PartDop;
                }
            }
            else if (splitDash.Length>1)
            {
                definePartFormwork(splitDash[0]);
                //Formwork = GetShort(splitDash[1], "Опалубка");                
                Electrics = splitDash[1];                
            }
            //else if (splitDash.Length > 1)
            //{
            //    definePartFormwork(splitDash[0]);
            //    val = splitDash[1];
            //    if (val.IndexOf("э", StringComparison.OrdinalIgnoreCase) == -1)
            //    {
            //        FormworkMirror = GetShort(val, "Зеркальность");
            //    }
            //    else
            //    {
            //        Electrics = val;
            //    }                
            //}
        }

        private void definePartFormwork(string input)
        {
            // Разбор части опалубки. Могут быть Подрезки и Балконы, типа 2П1Б1, где 2 - опалубка, П1-подреза, Б1-балкон

            // Определение индекса опалубки
            var resForm = GetStartInteger(input);
            if (resForm ==0)
            {
                throw new Exception("Не определен индекс опалубки - " + input);
            }
            Formwork = (short)resForm;

            BalconyCut = GetParamIndex("П", input);
            BalconyDoor = GetParamIndex("Б", input);
            if (input.IndexOf("Д") != -1)
            {
                MountIndex = "Д";
            }
            if (input.IndexOf("Г") != -1)
            {
                ProngIndex = "Г";
            }

            //int indexB = input.IndexOf("Б");
            //if (indexP == -1 && indexB == -1)
            //{
            //    Formwork = GetShort(input, "Опалубка");
            //}
            //else if (indexB ==-1)
            //{
            //    Formwork = GetShort(input.Substring(0, indexP), "Опалубка");
            //    BalconyCut = input.Substring(indexP);
            //}
            //else if (indexP == -1)
            //{
            //    Formwork = GetShort(input.Substring(0, indexB), "Опалубка");
            //    BalconyDoor = input.Substring(indexB);
            //}
            //else
            //{
            //    if (indexP<indexB)
            //    {
            //        Formwork = GetShort(input.Substring(0, indexP), "Опалубка");
            //        BalconyCut = input.Substring(indexP, indexB- indexP);
            //        BalconyDoor = input.Substring(indexB);
            //    }
            //    else
            //    {
            //        Formwork = GetShort(input.Substring(0, indexB), "Опалубка");
            //        BalconyDoor = input.Substring(indexB, indexP- indexB);
            //        BalconyCut = input.Substring(indexP);
            //    }
            //}
        }

        public static int GetStartInteger (string input)
        {
            int value = 0;
            var match = Regex.Match(input, @"^\d*");
            if (match.Success)
            {
                if (int.TryParse(match.Value, out value))
                {
                    return value;
                }
            }
            return 0;
        }

        private string GetParamIndex (string parameter, string input)
        {
            string res = null;
            int indexP = input.IndexOf(parameter);
            if (indexP != -1)
            {
                // если след символ цифра - то включение ее в параметр
                if (indexP < input.Length-1 && char.IsDigit(input[indexP + 1]))
                {
                    res = parameter + input[indexP + 1];
                }
                else
                {
                    res = parameter;
                }
            }
            return res;
        }

        /// <summary>
        /// Определение индекса класса бетона по группе 
        /// 2П, 2В - индекс 2
        /// 3В - индекс 2
        /// 3НСНг2 - индекс 2
        /// Других вариантов НЕТ!
        /// </summary>
        private void defineIndexClass()
        {
            switch (ItemGroup.ToUpper())
            {
                case "2П":
                    GroupIndexClass = 2;
                    MarkWoGroupClassIndex = MarkInput.Substring(1);
                    break;
                case "2В":
                    GroupIndexClass = 2;
                    MarkWoGroupClassIndex = MarkInput.Substring(1);
                    break;
                case "3В":
                    GroupIndexClass = 3;
                    MarkWoGroupClassIndex = MarkInput.Substring(1);
                    break;
                case "3НСНГ2":
                    GroupIndexClass = 2;
                    MarkWoGroupClassIndex = MarkInput.Remove(5, 1);
                    break;
                default:
                    GroupIndexClass = 0;
                    MarkWoGroupClassIndex = MarkInput;
                    break;
            }
        }

        private void addErrorMsg(string msg)
        {
            if (Error == null)
            {
                Error = new Error(msg, System.Drawing.SystemIcons.Error);
            }
            else
            {
                Error.AdditionToMessage(msg);
            }
        }

        /// <summary>
        /// Исправление порядка габаритов - в соответствии с ключем GabKey (LHT)
        /// </summary>
        /// <param name="gabKey">Ключ габаритов в формуле - LHT</param>
        public void UpdateGab (string gabKey)
        {
            if (string.IsNullOrEmpty(gabKey))
            {
                return;
            }
            var l = Length; // 0
            var h = Height; // 1
            var t = Thickness; // 2
                        
            Length = getGabByKey("L", gabKey, l, h, t);
            Height = getGabByKey("H", gabKey, l, h, t);
            Thickness = getGabByKey("T", gabKey, l, h, t);
        }

        private short? getGabByKey (string k, string gabKey, short? l, short? h, short? t)
        {
            var index = gabKey.ToUpper().IndexOf(k);
            switch (index)
            {
                case 0:
                    return l;
                case 1:
                    return h;
                case 2:
                    return t;
                default:
                    return null;
            }
        }
    }
}