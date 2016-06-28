using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using AcadLib;
using AcadLib.Blocks;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using Autocad_ConcerteList.Src.ConcreteDB.Formula;
using Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows;
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
        private const string AtrTagMark = "МАРКА";
        private const string AtrTagLength = "ДЛИНА";
        private const string AtrTagHeight = "ВЫСОТА";
        private const string AtrTagThickness = "ТОЛЩИНА";
        private const string AtrTagWeight = "МАССА";
        private const string AtrTagColor = "ПОКРАСКА";
        private const string AtrTagAperture = "ПРОЕМ";
        private const string AtrTagAlbum = "ДОК";

        private static AcadLib.Comparers.AlphanumComparator alpha = AcadLib.Comparers.AlphanumComparator.New;

        private ParserFormula parserFormula;
        private bool? _isNew;        
        private bool _alreadyCalcExtents;
        private Extents3d _extents;
        //private string _info;
        private bool _isNullExtents;
        /// <summary>
        /// Соответствие игнорируемых имен блоков.
        /// </summary>
        public static List<string> IgnoredBlockNamesMatch { get; } = new List<string> { "ММС", "^_", "^оси", "^ось", "^узел", "^узлы", "^формат", "rab_obl", "^жук", @"\$", @"^\*" };

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
        /// Изменение длины в атрибуте - во всех блоках
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="panelsInModel">Все панели в модели этой марки</param>        
        public short? UpdateLength (short? value, List<Panel> panelsInModel)
        {
            // Проверка длины
            if (value == null)
            {
                return Lenght;
            }
            // Длина должна соответствовать марке
            if (CheckGabInput(value.Value, DbGroup.LengthFactor, ParseMark.Length))
            {
                Lenght = value;                
                SetPanelsAtrValue(panelsInModel, AtrTagLength, Lenght.Value.ToString());
                foreach (var item in panelsInModel)
                {
                    item.Lenght = value;
                }
                // Обновление статуса панели
                Checks();
                return Lenght;
            }
            else
            {
                return null;
            }       
        }        

        /// <summary>
        /// Изменение высоты в атрибуте - во всех блоках
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="panelsInModel">Все панели в модели этой марки</param>        
        public short? UpdateHeight (short? value, List<Panel> panelsInModel)
        {
            // Проверка высоты
            if (value == null)
            {
                return Height;
            }
            // должна соответствовать марке
            if (CheckGabInput(value.Value, DbGroup.HeightFactor, ParseMark.Height))
            {
                Height = value;
                SetPanelsAtrValue(panelsInModel, AtrTagHeight, Height.Value.ToString());
                foreach (var item in panelsInModel)
                {
                    item.Height = value;
                }
                // Обновление статуса панели
                Checks();
                return Height;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Изменение ширины в атрибуте - во всех блоках
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="panelsInModel">Все панели в модели этой марки</param>        
        public short? UpdateThickness (short? value, List<Panel> panelsInModel)
        {
            // Проверка длины
            if (value == null)
            {
                return Thickness;
            }
            // Длина должна соответствовать марке
            if (CheckGabInput(value.Value, DbGroup.ThicknessFactor, ParseMark.Thickness))
            {
                Thickness = value;
                SetPanelsAtrValue(panelsInModel, AtrTagThickness, Thickness.Value.ToString());
                foreach (var item in panelsInModel)
                {
                    item.Thickness = value;
                }
                // Обновление статуса панели
                Checks();
                return Thickness;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Изменение массы в атрибуте - во всех блоках
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="panelsInModel">Все панели в модели этой марки</param>        
        public float? UpdateWeight (float? value, List<Panel> panelsInModel)
        {
            // Проверка длины
            if (value == null)
            {
                return Weight;
            }
            Weight = value;
            SetPanelsAtrValue(panelsInModel, AtrTagWeight, Weight.Value.ToString());
            foreach (var item in panelsInModel)
            {
                item.Weight = value;
            }
            // Обновление статуса панели
            Checks();
            return Weight;
        }

        /// <summary>
        /// Изменение проема в атрибуте - во всех блоках
        /// </summary>
        /// <param name="value">Новое значение</param>
        /// <param name="panelsInModel">Все панели в модели этой марки</param>        
        public string UpdateAperture (string value, List<Panel> panelsInModel)
        {
            // Проверка длины
            if (value == null)
            {
                return Aperture;
            }
            Aperture = value;
            SetPanelsAtrValue(panelsInModel, AtrTagAperture, Aperture);
            foreach (var item in panelsInModel)
            {
                item.Aperture = value;
            }
            // Обновление статуса панели
            Checks();
            return Aperture;
        }

        private bool CheckGabInput(short val, double factor, short? parseGab)
        {
            if (parseGab == null)
            {
                return true;
            }
            if (DbGroup.HasFormula.HasValue && DbGroup.HasFormula.Value)
            {
                var lenDiv = Eval.GetRoundValue(val / factor);
                if (lenDiv != parseGab)
                {
                    MessageBox.Show($"Введенное значение '{val}' не соответствует марке '{Mark}'");
                    return false;
                }
            }
            return true;
        }

        private void SetPanelsAtrValue (List<Panel> panels, string tag, string value)
        {
            try
            {
                using (Commands.Doc.LockDocument())
                {
                    using (var t = panels.First().IdBlRef.Database.TransactionManager.StartTransaction())
                    {
                        foreach (var item in panels)
                        {
                            var atrInfo = item.AtrsInfo.FirstOrDefault(a => a.Tag.Equals(tag));
                            if (atrInfo != null)
                            {
                                var atrRef = atrInfo.IdAtr.GetObject(OpenMode.ForWrite, false, true) as AttributeReference;
                                if (atrRef != null)
                                {
                                    atrRef.TextString = value;
                                }
                            }
                        }
                        t.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления атрибутов - {ex.Message}");
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
        public bool IsHeightOk { get; set; } = true;
        public string HeightDesc { get; set; }
        public ObjectId IdBlRef { get; set; }
        public ObjectId IdBtr { get; set; }
        /// <summary>
        /// Это новая панель - нет в базе (DbItem == null)
        /// </summary>
        public bool? IsNew {
            get {
                return _isNew;
            }
            set {
                _isNew = value;
            }
        }        

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
        public bool IsItemGroupOk { get; set; }
        public string ItemGroupDesc { get; set; }
        /// <summary>
        /// Длина (атр)
        /// </summary>                   
        public short? Lenght { get; set; }
        public bool IsLengthOk { get; set; } = true;
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
        public bool IsThicknessOk { get; set; } = true;
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
        public bool IsWeightOk { get; set; } = true;
        public string WeightDesc { get; set; }
        /// <summary>
        /// Рабочая область
        /// </summary>
        public Workspace WS { get; set; }
        public decimal ItemConstructionId { get; internal set; }

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

        public void Checks ()
        {
            // Сброс статусов
            ResetStatuses();
            try
            {
                ParseMark = new ParserMark(Mark);
                ParseMark.Parse();
                // Перенос распарсеных параметров в панель
                FillParseParams();

                DefineDbParams();

                // Исправление порядка габаритов в распарсенной марке - в соответствии с ключом GabKey в таблице формулы
                if (DbGroup != null)
                {
                    ParseMark.UpdateGab(DbGroup.GabKey);
                }

                // Определение марки по формуле по параметрам
                DefineMarkByFormulaInDb();

                CheckBlockParams();
                CheckBdParams();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, $"Panel.Checks() - марка - {Mark}");
                Warning += "Ошибка при проверка параметров панели - " + ex.Message;
            }
        }

        private void ResetStatuses ()
        {
            ErrorStatus = ErrorStatusEnum.None;
            this.HeightDesc = null;
            this.IsHeightOk = true;
            this.IsItemGroupOk = true;
            this.IsLengthOk = true;
            this.IsThicknessOk = true;
            this.IsWeightOk = true;
            this.ItemGroupDesc = null;
            this.LengthDesc = null;
            this.ThicknessDesc = null;
            this.WeightDesc = null;
            this.Warning = null;                
        }

        private void CheckBlockParams()
        {
            // Проверка латинских букв в марке
            var noLatin = Regex.IsMatch(Mark, "^[^a-z|^A-Z]+$");
            if (!noLatin)
            {
                ErrorStatus |= ErrorStatusEnum.MarkHasLatin;
                Warning += $" В марке(атр) есть латинские символы. ";
            }

            // Проверка имени блока
            var isCorBlName = BlockName.Replace(" ", "").Equals(ParseMark.MarkWoGroupClassIndex.Replace(" ", ""), StringComparison.OrdinalIgnoreCase);
            if (!isCorBlName)
            {
                ErrorStatus |= ErrorStatusEnum.IncorrectBlockName;
                Warning += $" Имя блока '{BlockName}' не соответствует марке из атрибута '{Mark}'. ";
            }

            // Соответствие параметров и марки                   
            if (DbGroup != null && DbGroup.HasFormula!= null && DbGroup.HasFormula.Value)
            {
                // Определение длин из распарсенной марки
                bool isOk;                
                string desc;
                CheckGab(out isOk, parserFormula.Length, Lenght, DbGroup.LengthFactor, "Длина", out desc);
                IsLengthOk = isOk;                
                LengthDesc = desc;
                
                CheckGab(out isOk, parserFormula.Height, Height, DbGroup.HeightFactor, "Высота", out desc);
                IsHeightOk = isOk;                
                HeightDesc = desc;
                
                CheckGab(out isOk, parserFormula.Thickness, Thickness, DbGroup.ThicknessFactor, "Ширина", out desc);
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
        private void CheckBdParams ()
        {
            // Проверка марки в атрибуте и по формуле
            if (Mark != MarkByFormula)
            {
                if (MarkWoSpace != MarkByFormulaWoSpace)
                {
                    ErrorStatus |= ErrorStatusEnum.IncorrectMarkAndFormula;
                    Warning += $" Марка(атр) '{Mark}' отличается от марки по формуле '{MarkByFormula}'. ";
                }
                else
                {
                    // Пробел игнорировать
                    //Warning += " Пропущен пробел в марке '" + Mark + "', правильно '" + MarkByFormula + "'. ";
                }
            }

            if (DbItem != null)
            {
                // Проверка массы 
                if (Weight != DbItem.Weight && DbItem.Weight != null && DbItem.Weight != 0)
                {
                    IsWeightOk = false;
                    WeightDesc = $"Масса(атр)={Weight}, из базы={DbItem.Weight}";
                    Warning += $" Масса в атрибуте '{Weight}' отличается от массы в базе '{DbItem.Weight}'. ";
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
                if (blRef == null) return Result.Fail("");
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
                    if (atr.Tag.Equals(AtrTagMark, StringComparison.OrdinalIgnoreCase))
                    {
                        Mark = atr.Text.Trim();
                        MarkWoSpace = Mark.Replace(" ", "");
                    }
                    else if (atr.Tag.Equals(AtrTagLength, StringComparison.OrdinalIgnoreCase))
                    {
                        Lenght = GetShortNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals(AtrTagHeight, StringComparison.OrdinalIgnoreCase))
                    {
                        Height = GetShortNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals(AtrTagThickness, StringComparison.OrdinalIgnoreCase))
                    {
                        Thickness = GetShortNullable(atr.Text);
                    }                    
                    else if (atr.Tag.Equals(AtrTagWeight, StringComparison.OrdinalIgnoreCase))
                    {
                        Weight = GetFloatNullable(atr.Text);
                    }
                    else if (atr.Tag.Equals(AtrTagColor, StringComparison.OrdinalIgnoreCase))
                    {
                        Color = atr.Text.Trim();
                    }
                    else if (atr.Tag.Equals(AtrTagAperture, StringComparison.OrdinalIgnoreCase))
                    {
                        Aperture = atr.Text.Trim();
                    }
                    else if (atr.Tag.Equals(AtrTagAlbum, StringComparison.OrdinalIgnoreCase))
                    {
                        Album = atr.Text.Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(Mark))
            {

                return Result.Fail("");
            }            

            return Result.Ok();
        }

        private void DefineDbParams ()
        {
            // Проверка есть ли такая группа ЖБИ в базе
            DefineItemGroup();

            if (DbGroup != null)
            {
                if (DbGroup.HasFormula.HasValue && DbGroup.HasFormula.Value)
                {
                    // Поиск панели в базе по параметрам                    
                    DbItem = DbService.FindByParametersFromAllLoaded(this);
                }
                else
                {
                    // Поиск по марке
                    DbItem = DbService.FindByMark(Mark);
                }
                _isNew = DbItem == null;
            }            
        }

        public void DefineItemGroup ()
        {
            DbGroup = DbService.FindGroup(ItemGroup);
            if (DbGroup == null)
            {
                IsItemGroupOk = false;
                ItemGroupDesc = $"Неопределенная группа.";
                Warning += $" Неопределенная группа {ItemGroup}. ";
                return;
                //throw new Exception($"Неопределенная группа {ItemGroup}.");
            }
            IsItemGroupOk = true;
        }

        public void Show()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                if (doc.Database != IdBlRef.Database)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog($"Переключитесь на чертеж {Path.GetFileNameWithoutExtension(IdBlRef.Database.Filename)}");
                    return;
                }
                Editor ed = doc.Editor;
                ed.Zoom(Extents);
                IdBlRef.FlickObjectHighlight(1, 100, 0);
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
                if (DbGroup.HasFormula.HasValue && DbGroup.HasFormula.Value)
                {
                    var res = DbService.GetDbMark(this, out parserFormula);
                    if (res.Success)
                    {
                        MarkByFormula = res.Value;
                        MarkByFormulaWoSpace = MarkByFormula.Replace(" ", "");
                    }
                    else
                    {
                        Warning += " " + res.Error;
                    }
                }
                else
                {
                    MarkByFormula = Mark;
                    MarkByFormulaWoSpace = MarkWoSpace;
                }
            }
            catch (Exception ex)
            {
                Warning += " Ошибка формирования марки панели по параметрам - " + ex.Message + ". ";
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
            sb.Replace("IncorrectBlockName", $"Несоответствие имени блока '{BlockName}' и марки (атр) '{Mark}'");
            sb.Replace("MarkHasLatin", "В марке есть латинские символы");
            sb.Replace("IncorrectMarkAndFormula", $"Марка(атр) '{Mark}' отличается от марки по формуле '{MarkByFormula}'.");
            sb.Replace("DifferentParamInGroup", "Различные параметры в панелях(блоках) одной марки.");            

            var res = sb.ToString();

            if (res.Length>0 && status!= ErrorStatusEnum.None)
            {
                //Warning += " " + res + ". ";
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
                   Weight == other.Weight &&
                   Aperture == other.Aperture;                    
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