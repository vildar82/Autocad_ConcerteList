using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AcadLib;
using AcadLib.Blocks;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using Autocad_ConcerteList.Src.ConcreteDB.Formula;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// ЖБИ изделие полученное из автокада
    /// </summary>
    public class Panel : iItem, IEquatable<Panel>, IComparable<Panel>
    {
        private static AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;
        private bool _alreadyCalcExtents;
        private Extents3d _extents;
        //private string _info;
        private bool _isNullExtents;
        /// <summary>
        /// Соответствие игнорируемых имен блоков.
        /// </summary>
        public static List<string> IgnoredBlockNamesMatch { get; } = new List<string> { "ММС", "^_", "^оси", "^ось", "^узел", "^узлы", "^формат" };

        public bool CanShow ()
        {
            return !_isNullExtents;
        }

        /// <summary>
        /// Альбом изделия - ?
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// Проес
        /// </summary>
        public string Aperture { get; set; }
        public List<AttributeInfo> AtrsInfo { get; set; }
        public string BalconyCut { get; set; }
        public BalconyCutDbo BalconyCutItem { get; set; }
        public string BalconyDoor { get; set; }
        public BalconyDoorDbo BalconyDoorItem { get; set; }
        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName { get; set; }
        /// <summary>
        /// Покраска
        /// </summary>
        public string Color { get; set; }        
        /// <summary>
        /// Изделие в базе
        /// </summary>
        public ItemConstructionDbo DbItem { get; set; }
        public ItemGroupDbo DbGroup { get; set; }
        //public short? FormworkMirror { get; set; }
        /// <summary>
        /// Элемтрика - 1э, 2э
        /// </summary>
        public string Electrics { get; set; }        
        public bool HasErrors
        {
            get
            {
                return ErrorStatus != ErrorStatusEnum.None ||                    
                    !string.IsNullOrEmpty(Warning);
            }
        }
        /// <summary>
        /// Статус изделия - ок,
        /// </summary>
        public ErrorStatusEnum ErrorStatus { get; set; }
        public Extents3d Extents
        {
            get
            {
                if (!_alreadyCalcExtents)
                {
                    //FindBlock();
#pragma warning disable CS0618
                    using (var blRef = IdBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
                    {
                        try
                        {
                            _extents = blRef.GeometricExtents;
                            _alreadyCalcExtents = true;
                        }
                        catch
                        {
                            _isNullExtents = true;
                        }
                    }
#pragma warning restore CS0618
                }
                if (_isNullExtents)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog("Границы объекта не определены.");
                }
                return _extents;
            }
        }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork { get; set; }
        /// <summary>
        /// Высото (атр)
        /// </summary>
        public short? Height { get; set; }
        public bool IsHeightOk { get; set; }
        public string HeightDesc { get; set; }
        public ObjectId IdBlRef { get; set; }
        public ObjectId IdBtr { get; set; }
        /// <summary>
        /// Это новая панель - нет в базе (DbItem == null)
        /// </summary>
        public bool IsNew { get { return DbItem == null; } }

        public string GetErrorStatusDesc ()
        {
            return ErrorStatusEnameToString(ErrorStatus);
        }

        //public string Info
        //{
        //    get
        //    {
        //        if (_info == null)
        //        {
        //            _info = GetInfo();
        //        }
        //        return _info;
        //    }
        //    set { _info = value; }
        //}
        /// <summary>
        /// Имя блока соответствует атрибуту марки.
        /// Имя блока должно быть равно марке, за исключением индекса класса (2П, 2В, 3В, 3НСНг2)
        /// </summary>
        //public bool IsCorrectBlockName { get; set; }
        /// <summary>
        /// Группа изделия - В, П, 3НСг
        /// </summary>
        public string ItemGroup { get; set; }
        /// <summary>
        /// Длина (атр)
        /// </summary>                   
        public short? Lenght { get; set; }        
        public bool IsLengthOk { get; set; }
        public string LengthDesc { get; set; }
        /// <summary>
        /// Марка от конструкторов - из атрибута
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// Марка из базы
        /// </summary>
        public string MarkByFormula { get; set; }
        /// <summary>
        /// Марка из базы без пробелов
        /// </summary>
        public string MarkByFormulaWoSpace { get; set; }
        /// <summary>
        /// Марка без пробелов
        /// </summary>
        public string MarkWoSpace { get; set; }
        public ParserMark ParseMark { get; set; }
        public Point3d Position { get; set; }
        /// <summary>
        /// Толщина (атр)
        /// </summary>
        public short? Thickness { get; set; }
        public bool IsThicknessOk { get; set; }
        public string ThicknessDesc { get; set; }
        public float? Volume { get; set; }
        /// <summary>
        /// Пояснение по ошибкам в блоке
        /// </summary>
        public string Warning { get; set; }
        /// <summary>
        /// Масса
        /// </summary>
        public float? Weight { get; set; }
        public bool IsWeightOk { get; set; }
        public string WeightDesc { get; set; }
        /// <summary>
        /// Рабочая область
        /// </summary>
        public Workspace WS { get; set; }                

        public static float? GetFloatNullable(object obj)
        {
            try
            {
                var value = obj.ToString();
                if (value == "") return null;
                return float.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        public static short? GetShortNullable(object obj)
        {
            try
            {
                var value = obj.ToString();
                if (value == "") return null;
                return short.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// проверка - это игнорируемое имя блока - точно не изделие ЖБИ, например ММС
        /// </summary>
        public static bool IsIgnoredBlockName(string blockName)
        {
            foreach (var item in IgnoredBlockNamesMatch)
            {
                if (Regex.IsMatch(blockName, item, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckBlockParams()
        {
            // Проверка латинских букв в марке
            var noLatin = Regex.IsMatch(Mark, "^[^a-z|^A-Z]+$");
            if (!noLatin)
            {
                ErrorStatus |= ErrorStatusEnum.MarkHasLatin;
                Warning += $" В марке(атр) есть латинские символы. ";
            }

            // Проверка имени блока
            var isCorBlName = BlockName.Equals(ParseMark.MarkWoGroupClassIndex, StringComparison.OrdinalIgnoreCase);
            if (!isCorBlName)
            {
                ErrorStatus |= ErrorStatusEnum.IncorrectBlockName;
                Warning += $" Имя блока '{BlockName}' не соответствует марке из атрибута '{Mark}'. ";
            }

            // Соответствие параметров и марки       
            if (DbGroup == null)
            {
                DbGroup = DbService.FindGroup(ItemGroup);
            }
            if (DbGroup != null)
            {
                // Определение длин из распарсенной марки
                bool isOk;                
                string desc;
                CheckGab(out isOk, ParseMark.Length, Lenght, DbGroup.LengthFactor, "Длина", out desc);
                IsLengthOk = isOk;                
                LengthDesc = desc;
                
                CheckGab(out isOk, ParseMark.Height, Height, DbGroup.HeightFactor, "Высота", out desc);
                IsHeightOk = isOk;                
                HeightDesc = desc;
                
                CheckGab(out isOk, ParseMark.Thickness, Thickness, DbGroup.ThicknessFactor, "Ширина", out desc);
                IsThicknessOk = isOk;                
                ThicknessDesc = desc;                

                if (!IsLengthOk ||
                    !IsHeightOk ||
                    !IsThicknessOk)
                {
                    ErrorStatus |= ErrorStatusEnum.IncorrectMarkAndParams;
                    Warning += "Габариты из атрибутов блока отличаются от параметров из базы. ";
                }
            }
        }

        /// <summary>
        /// Проверки панели
        /// </summary>
        public void CheckBdParams ()
        {            
            // Проверка марки в атрибуте и по формуле
            if (Mark != MarkByFormula)
            {
                if (MarkWoSpace != MarkByFormulaWoSpace)
                {
                    ErrorStatus |= ErrorStatusEnum.IncorrectMarkAndFormula;
                    Warning += "Марка в блоке отличается от марки полученной по формуле. ";
                }
                else
                {
                    Warning += "Пропущен пробел в марке '" + Mark + "', правильно '" + MarkByFormula + "' ";
                }
            }

            if (DbItem != null)
            {
                // Проверка массы
                IsWeightOk = true;
                if (Weight != DbItem.Weight)
                {
                    IsWeightOk = false;
                    WeightDesc = $"Масса(атр)={Weight}, из базы={DbItem.Weight}";                    
                    Warning += "Масса в атрибуте отличается от массы в базе. ";
                }
            }
        }

        private void CheckGab (out bool isOk, short? parseMarkGab, short? gab, double factor, 
            string nameGab, out string desc)
        {
            isOk = true;
            desc = "пусто";
            if (gab != null && parseMarkGab != null)
            {
                var gabDiv = Eval.GetRoundValue(gab.Value / factor);
                //parseMarkGab = (short)(parseMarkGab * factor);
                if (gabDiv != parseMarkGab.Value)
                {
                    isOk = false;
                }
                desc = $"{nameGab}(атр)={gab}, по марке={parseMarkGab*factor}";
            }
            else if (gab == null)
            {
                isOk = false;
                // Атрибут габарита не задан, а в базе есть значение - пока такого не может быть, т.к. поиск в базе происходит по параметрам из атрибутов
                //desc = $"{nameGab}(атр)=0, {nameGab} по марке={parseMarkGab}";
                desc = $"Параметр {nameGab} не задан.";
            }
            else if (parseMarkGab == null)
            {
                //isOk = false;
                // Атрибут габарита задан, а в базе нет значения
                desc = $"{nameGab}(атр)={gab}, по марке=нет";
            }               
        }

        /// <summary>
        /// Опеределение блока ЖБИ изделия.
        /// </summary>        
        public Result Define(ObjectId idBlRef)
        {
            // определить параметры панели из блока
            var blRef = idBlRef.GetObject(OpenMode.ForRead, false, true) as BlockReference;
            {
                if (blRef == null) return Result.Fail("Это не блок.");
                IdBlRef = idBlRef;
                IdBtr = blRef.BlockTableRecord;
                Position = blRef.Position;
                BlockName = blRef.GetEffectiveName();
                if (IsIgnoredBlockName(BlockName))
                {
                    // Игнорируемое имя блока
                    return Result.Fail("");
                }
                AtrsInfo = AttributeInfo.GetAttrRefs(blRef);
                foreach (var atr in AtrsInfo)
                {
                    if (atr.Tag.Equals("МАРКА", StringComparison.OrdinalIgnoreCase))
                    {
                        Mark = atr.Text.Trim();
                        MarkWoSpace = Mark.Replace(" ", "");
                    }
                    else if (atr.Tag.Equals("ДЛИНА", StringComparison.OrdinalIgnoreCase))
                    {
                        Lenght = GetShortNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals("ВЫСОТА", StringComparison.OrdinalIgnoreCase))
                    {
                        Height = GetShortNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals("ТОЛЩИНА", StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness = GetShortNullable(atr.Text);
                    }                    
                    else if (atr.Tag.Equals("МАССА", StringComparison.OrdinalIgnoreCase))
                    {
                        Weight = GetFloatNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals("ПОКРАСКА", StringComparison.OrdinalIgnoreCase))
                    {
                        Color = atr.Text.Trim();
                    }
                    else if (atr.Tag.Equals("ПРОЕМ", StringComparison.OrdinalIgnoreCase))
                    {
                        Aperture = atr.Text.Trim();
                    }
                    else if (atr.Tag.Equals("ДОК", StringComparison.OrdinalIgnoreCase))
                    {
                        Album = atr.Text.Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(Mark))
            {
                return Result.Fail("Марка не определена.");
            }

            ParseMark = new ParserMark(Mark);
            ParseMark.Parse();            

            // Перенос распарсеных параметров в панель
            FillParseParams();           

            return Result.Ok();
        }        

        public void DefineDbParams (bool checkInAllPanels)
        {
            // Проверка есть ли такая группа ЖБИ в базе
            if (DbGroup == null)
            {
                DbGroup = DbService.FindGroup(ItemGroup);
                if (DbGroup == null)
                {
                    throw new Exception($"Неопределенная группа {ItemGroup}.");
                }
            }

            // Поиск панели в базе по параметрам
            if (checkInAllPanels)
            {
                DbItem = DbService.FindByParametersFromAllLoaded(this);
            }
            else
            {
                DbItem = DbService.FindByParameters(this);
            }

            // Определение марки по формуле по параметрам
            DefineMarkByFormulaInDb();            
        }        

        public void Show()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                if (doc.Database != IdBlRef.Database)
                {
                    Application.ShowAlertDialog($"Переключитесь на чертеж {Path.GetFileNameWithoutExtension(IdBlRef.Database.Filename)}");
                    return;
                }
                Editor ed = doc.Editor;
                ed.Zoom(Extents);
                //IdBlRef.FlickObjectHighlight(2, 100, 100);
            }
        }

        //public override string ToString()
        //{
        //    return Mark;
        //}        

        /// <summary>
        /// Определение марки по формуле из DB
        /// </summary>
        private void DefineMarkByFormulaInDb()
        {
            try
            {
                MarkByFormula = DbService.GetDbMark(this);
                MarkByFormulaWoSpace = MarkByFormula.Replace(" ", "");
            }
            catch (Exception ex)
            {
                Warning += "Ошибка формирования марки панели по параметрам - " + ex.Message + ". ";
            }
        }

        private void FillParseParams()
        {
            ItemGroup = ParseMark.ItemGroup;
            Formwork = ParseMark.Formwork;
            //FormworkMirror = ParseMark.FormworkMirror;
            BalconyCut = ParseMark.BalconyCut;
            BalconyCutItem = DbService.GetBalconyCutItem(BalconyCut);
            BalconyDoor = ParseMark.BalconyDoor;
            BalconyDoorItem = DbService.GetBalconyDoorItem(BalconyDoor);
            Electrics = ParseMark.Electrics;
            // Определение правильности габаритов - определенных по марке и заданных в атрибутах    
                    
        }
        
        /// <summary>
        /// Инфо - Строка основных параметров панели
        /// </summary>
        /// <returns></returns>
        public string ParamsToString ()
        {
            return $"Марка={Mark},Группа={ItemGroup},Lenght={Lenght},Height={Height},Thickness={Thickness},Formwork={Formwork}" +
                $"BalconyDoor={BalconyDoor},BalconyCut={BalconyCut},Electrics={Electrics}.";            
        }

        public string ErrorStatusEnameToString (ErrorStatusEnum status)
        {
            StringBuilder sb = new StringBuilder (status.ToString());

            sb.Replace("None", "Ok");
            sb.Replace("IncorrectMarkAndParams", "Параметры габаритов из атрибутов не соответствуют марке(атр)");
            sb.Replace("IncorrectBlockName", $"Несоответствие имени блока {BlockName} и марки (атр) {Mark}");
            sb.Replace("MarkHasLatin", "В марке есть латинские символы");
            sb.Replace("IncorrectMarkAndFormula", "Марка(атр) отличается от марки по формуле.");
            sb.Replace("DifferentParamInGroup", "Различные параметры в панелях(блоках) одной марки.");            

            var res = sb.ToString();

            if (res.Length>0)
            {
                Warning += res;
            }
            return res;
        }

        public bool Equals (Panel other)
        {
            return Mark.Equals(other.Mark) &&
                   BlockName.Equals(other.BlockName) &&
                   Lenght == other.Lenght &&
                   Height == other.Height &&
                   Thickness == other.Thickness &&
                   Weight == other.Weight;                        
        }

        public int CompareTo (Panel other)
        {
            return alpha.Compare(MarkWoSpace, other.MarkWoSpace);
        }

        public override int GetHashCode ()
        {
            return MarkWoSpace.GetHashCode();
        }
    }
}