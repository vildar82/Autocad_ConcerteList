using System;
using System.Linq;
using System.Text.RegularExpressions;
using AcadLib.Errors;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class ParserMark : IParserMark
    {
        private readonly MarkPart markPart;

        public ParserMark (MarkPart markPart)
        {
            MarkInput = markPart.Mark;
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
        public string ItemGroupForSearchInBD { get; set; }
        public string ItemGroupWoClass { get; set; }
        /// <summary>
        /// Индекс "Класса Бетона" - Например 2,3 - 2П, 2В, 3В, 3НСг2 (2).
        /// </summary>
        public string GroupIndexConcreteClass { get;  set; }
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
        public string GroupIndexClassNew { get; set; }

        public virtual void Parse()
        {
            DefinePartGroup();
            ParsePartGab();
            ParsePartDop();
            // определение индекса класса бетона по группе
            DefineIndexClass();
        }

        protected void DefinePartGroup()
        {
            // Разбор группы. например partGroup = "2П"
            //ItemGroup = markPart.PartGroup.Replace(" ", "").Replace("-", "").Trim();
            if (markPart.IsNewBIndex)
            {
                ItemGroupForSearchInBD = markPart.PartGroup.Replace("-", "").Trim();
                ItemGroup = ItemGroupForSearchInBD + "-" + markPart.NewBIndex;
            }
            else
            {
                ItemGroup = markPart.PartGroup.Replace("-", "").Trim();
                ItemGroupForSearchInBD = ItemGroup;
            }
        }

        private void ParsePartGab()
        {
            // Разбор части строки относящейся к габаритам панели. Они разделены точками. Например partGab = "544.363"
            if (string.IsNullOrEmpty(markPart.PartGab)) return;
            var splitDot = markPart.PartGab.Split('.');
            if (splitDot.Length > 3)
            {
                // Ошибка. максимум, только 3 габарита - Длина, Высота, Толщина
                AddErrorMsg("Определено больше трех габаритов разделенных точкой.");
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

        private void ParsePartDop()
        {
            // partDop - например "5-1-1э" - теперь без зеркальности "5-1э"
            if (string.IsNullOrEmpty(markPart.PartDop)) return;
            var splitDash = markPart.PartDop.Split('-');
            if (splitDash.Length>2)
            {
                // Ошибка. Может быть только опалубка и электрика. От Зеркальности отказались.
                AddErrorMsg("Определено больше двух возможных дополнительных параметра панели - опалубки и электрики.");
            }
            if (splitDash.Length == 1)
            {
                if (markPart.PartDop.IndexOf("э", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    DefinePartFormwork(markPart.PartDop);
                }
                else
                {
                    Electrics = markPart.PartDop;
                }
            }
            else if (splitDash.Length>1)
            {
                DefinePartFormwork(splitDash[0]);
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

        private void DefinePartFormwork(string input)
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
            var value = 0;
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
            var indexP = input.IndexOf(parameter);
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
        protected void DefineIndexClass()
        {
            var groupWoClass = markPart.ItemGroupWoClassNew;
            if (markPart.PanelType != PanelTypeEnum.WallOuter && char.IsDigit(groupWoClass[0]))
            {
                GroupIndexConcreteClass = groupWoClass.Substring(0,1);
                groupWoClass = groupWoClass.Substring(1).Trim();
            }
            var lastChar = groupWoClass.Last();
            if (char.IsDigit(lastChar))
            {
                GroupIndexConcreteClass += lastChar;
                groupWoClass = groupWoClass.Substring(0, groupWoClass.Length - 1);
            }

            ItemGroupWoClass = groupWoClass;
            markPart.ItemGroupWoClassNew = groupWoClass;
            GroupIndexClassNew = markPart.GroupIndexClassNew;
            MarkWoGroupClassIndex = $"{groupWoClass}{markPart.MarkInputAfterGroup}";
        }

        private void AddErrorMsg(string msg)
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

            Length = GetGabByKey("L", gabKey, l, h, t);
            Height = GetGabByKey("H", gabKey, l, h, t);
            Thickness = GetGabByKey("T", gabKey, l, h, t);
        }

        private short? GetGabByKey (string k, string gabKey, short? l, short? h, short? t)
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
