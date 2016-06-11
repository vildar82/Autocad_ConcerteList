using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AcadLib;
using AcadLib.Blocks;
using Autocad_ConcerteList.Src.ConcreteDB;
using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.Panels;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Autocad_ConcerteList.Src.RegystryPanel
{
    /// <summary>
    /// ЖБИ изделие полученное из автокада
    /// </summary>
    public class Panel : iItem
    {
        private bool _alreadyCalcExtents;
        private Extents3d _extents;
        private string _info;
        private bool _isNullExtents;
        /// <summary>
        /// Соответствие игнорируемых имен блоков.
        /// </summary>
        public static List<string> IgnoredBlockNamesMatch { get; } = new List<string> { "ММС" };

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
        public I_S_BalconyCut BalconyCutItem { get; set; }
        public string BalconyDoor { get; set; }
        public I_S_BalconyDoor BalconyDoorItem { get; set; }
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
        public I_J_ItemConstruction DbItem { get; set; }
        public I_S_ItemGroup DbGroup { get; set; }
        //public short? FormworkMirror { get; set; }
        /// <summary>
        /// Элемтрика - 1э, 2э
        /// </summary>
        public string Electrics { get; set; }
        public string ErrorName
        {
            get
            {
                if (ErrorStatus.HasFlag(EnumErrorItem.IncorrectMark))
                {
                    return "Несоответствующая марка";
                }
                if (ErrorStatus.HasFlag(EnumErrorItem.DifferentParams))
                {
                    return "Разные параметры";
                }
                return "OK";
            }
        }
        /// <summary>
        /// Статус изделия - ок,
        /// </summary>
        public EnumErrorItem ErrorStatus { get; set; }
        public Extents3d Extents
        {
            get
            {
                if (!_alreadyCalcExtents)
                {
                    //FindBlock();
                    using (var blRef = IdBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
                    {
                        try
                        {
                            _extents = blRef.GeometricExtents;
                        }
                        catch
                        {
                        }
                    }
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
        public short? Height { get; set; }
        public ObjectId IdBlRef { get; set; }
        public ObjectId IdBtr { get; set; }
        /// <summary>
        /// Это новая панель - нет в базе (DbItem == null)
        /// </summary>
        public bool IsNew { get { return DbItem == null; } }
        public string Info
        {
            get
            {
                if (_info == null)
                {
                    _info = GetInfo();
                }
                return _info;
            }
            set { _info = value; }
        }
        /// <summary>
        /// Имя блока соответствует атрибуту марки.
        /// Имя блока должно быть равно марке, за исключением индекса класса (2П, 2В, 3В, 3НСНг2)
        /// </summary>
        public bool IsCorrectBlockName { get; set; }
        /// <summary>
        /// Группа изделия - В, П, 3НСг
        /// </summary>
        public string ItemGroup { get; set; }        
        public short? Lenght { get; set; }
        /// <summary>
        /// Марка от конструкторов - из атрибута
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// Марка из базы
        /// </summary>
        public string MarkDb { get; set; }
        /// <summary>
        /// Марка из базы без пробелов
        /// </summary>
        public string MarkDbWoSpace { get; set; }
        /// <summary>
        /// Марка без пробелов
        /// </summary>
        public string MarkWoSpace { get; set; }
        public ParserMark ParseMark { get; set; }
        public Point3d Position { get; set; }
        public short? Thickness { get; set; }
        public float? Volume { get; set; }
        /// <summary>
        /// Пояснение по ошибкам в блоке
        /// </summary>
        public string Warning { get; set; }
        public float? Weight { get; set; }
        /// <summary>
        /// Рабочая область
        /// </summary>
        public Workspace WS { get; set; }        

        /// <summary>
        /// Создание новой панели с отличающимися паркметрами
        /// </summary>
        /// <param name="panelGroupMark">список панелей с одной маркой но различными параметрами</param>
        public static Panel GetErrParams(IGrouping<string, Panel> panelGroupMark)
        {
            var firstP = panelGroupMark.First();
            Panel resPanel = new Panel()
            {
                IdBlRef = firstP.IdBlRef,
                Mark = panelGroupMark.Key,
                Color = firstP.Color,
                BlockName = firstP.BlockName,
                MarkDb = "неопределено",
                ItemGroup = firstP.ItemGroup,
                Lenght = firstP.Lenght,
                Height = firstP.Height,
                Thickness = firstP.Thickness,
                Formwork = firstP.Formwork,
                //FormworkMirror = firstP.FormworkMirror,
                BalconyDoor = firstP.BalconyDoor,
                BalconyCut = firstP.BalconyCut,
                Electrics = firstP.Electrics,
                Volume = firstP.Volume,
                Weight = firstP.Weight,
                ErrorStatus = EnumErrorItem.DifferentParams
            };

            resPanel.Info = "Марка\t\t" + resPanel.Mark + "\r\n" +
                "Марка по базе\t" + resPanel.MarkDb + "\r\n" +
                "Параметры панели из блока:\r\n" +
                "Группа\t\t" + resPanel.ItemGroup + "\r\n" +
                // Длина - * если различная, Lenght если одинаковая, и пусто если не задана
                (panelGroupMark.GroupBy(g => g.Lenght).Skip(1).Any() ? "Длина\t\t*\r\n" :
                    ((resPanel.Lenght == null ? "" : "Длина\t\t" + resPanel.Lenght + "\r\n"))) +
                // Высота
                (panelGroupMark.GroupBy(g => g.Height).Skip(1).Any() ? "Высота\t\t*\r\n" :
                    ((resPanel.Height == null ? "" : "Высота\t\t" + resPanel.Height + "\r\n"))) +
                // Ширина
                (panelGroupMark.GroupBy(g => g.Thickness).Skip(1).Any() ? "Ширина\t\t*\r\n" :
                    ((resPanel.Thickness == null ? "" : "Ширина\t\t" + resPanel.Thickness + "\r\n"))) +
                // Опалубка
                (panelGroupMark.GroupBy(g => g.Formwork).Skip(1).Any() ? "Опалубка\t*\r\n" :
                    ((resPanel.Formwork == null ? "" : "Опалубка\t" + resPanel.Formwork + "\r\n"))) +
                // BalconyDoor
                (panelGroupMark.GroupBy(g => g.BalconyDoor).Skip(1).Any() ? "Балконный проем\t *\r\n" :
                    ((resPanel.BalconyDoor == null ? "" : "Балконный проем\t" + resPanel.BalconyDoor + "\r\n"))) +
                // BalconyCut
                (panelGroupMark.GroupBy(g => g.BalconyCut).Skip(1).Any() ? "Подрезка\t *\r\n" :
                    ((resPanel.BalconyCut == null ? "" : "Подрезка\t" + resPanel.BalconyCut + "\r\n"))) +
                //// FormworkMirror
                //(panelGroupMark.GroupBy(g => g.FormworkMirror).Skip(1).Any() ? "Зеркальность\t*\r\n" :
                //    ((resPanel.FormworkMirror == null ? "" : "Зеркальность\t" + resPanel.FormworkMirror + "\r\n"))) +
                // Electrics
                (panelGroupMark.GroupBy(g => g.Electrics).Skip(1).Any() ? "Электрика\t*\r\n" :
                    ((resPanel.Electrics == null ? "" : "Электрика\t" + resPanel.Electrics + "\r\n"))) +
                // Weight
                (panelGroupMark.GroupBy(g => g.Weight).Skip(1).Any() ? "Вес, кг\t\t*\r\n" :
                    ((resPanel.Weight == null ? "" : "Вес, кг\t\t" + resPanel.Weight + "\r\n"))) +
                // Volume
                (panelGroupMark.GroupBy(g => g.Volume).Skip(1).Any() ? "Объем, м3\t*\r\n" :
                    ((resPanel.Volume == null ? "" : "Объем, м3\t" + resPanel.Volume)));
            return resPanel;
        }

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

        /// <summary>
        /// Проверки панели
        /// </summary>
        public void Check()
        {
            // Проверки.
            // 1. Проверка марки в атрибуте и по формуле
            if (Mark != MarkDb)
            {
                if (MarkWoSpace != MarkDbWoSpace)
                {
                    ErrorStatus |= EnumErrorItem.IncorrectMark;
                    Warning += "Марка в блоке отличается от марки полученной по формуле. ";
                }
                else
                {
                    Warning += "Пропущен пробел в марке '" + Mark + "', правильно '" + MarkDb + "' ";
                }
            }
            // 2. Проверка параметров в базе и в блоке
            if (DbItem != null)
            {                
                if (this.Lenght != DbItem.Lenght ||
                    this.Height != DbItem.Height ||
                    this.Thickness != DbItem.Thickness ||
                    this.Weight != DbItem.Weight)
                {
                    ErrorStatus |= EnumErrorItem.DifferentParams;
                    Warning += "Параметры из атрибутов блока отличаются от параметров из базы. ";
                }
            }

            // 3. Проверка имени блока
            IsCorrectBlockName = BlockName.Equals(ParseMark.MarkWoGroupClassIndex, StringComparison.OrdinalIgnoreCase);
            if (!IsCorrectBlockName)
            {
                Warning += $" Имя блока '{BlockName}' не соответствует марке из атрибута '{Mark}'. ";
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
                    throw new IgnoreBlockException(BlockName);
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

            // Проверка есть ли такая группа ЖБИ в базе
            DbGroup = DbService.FindGroup(ItemGroup);
            if (DbGroup == null)
            {
                return Result.Fail($"Неопределенная группа {ItemGroup}.");
            }

            // Поиск панели в базе по параметрам
            DbItem = DbService.FindByParameters(this);

            // Определение марки по формуле по параметрам
            DefineMarkByFormulaInDb();

            return Result.Ok();
        }        

        public string GetInfo()
        {
            string info = "Марка \t\t" + Mark + "\r\n" +
                          "Марка по формуле \t" + MarkDb + "\r\n\r\n" +
                          "Параметры панели из блока:\r\n" +
                          "Группа \t\t" + ItemGroup + "\r\n" +
                          "Длина \t\t" + Lenght + ((DbItem != null && DbItem.Lenght != Lenght) ? ", в базе " + DbItem.Lenght : "") + "\r\n" +
                          "Высота \t\t" + Height + ((DbItem != null && DbItem.Height != Height) ? ", в базе " + DbItem.Height : "") + "\r\n" +
                          "Ширина \t\t" + Thickness + ((DbItem != null && DbItem.Thickness != Thickness) ? ", в базе " + DbItem.Thickness : "") + "\r\n" +
                          (Formwork == null ? "" : "Опалубка \t" + Formwork + "\r\n") +
                          (string.IsNullOrEmpty(BalconyDoor) ? "" : "Балконный проем\t " + BalconyDoor + "\r\n") +
                          (string.IsNullOrEmpty(BalconyCut) ? "" : "Подрезка\t" + BalconyCut + ", ширина в базе " + DbItem.I_S_BalconyCut?.BalconyCutSize + "\r\n") +                          
                          (string.IsNullOrEmpty(Electrics) ? "" : "Электрика \t" + Electrics + "\r\n") +
                          (string.IsNullOrEmpty(Color) ? "" : "Покраска \t" + Color + "\r\n") +
                          "Вес, кг \t\t" + Weight + ((DbItem != null && DbItem.Weight != Weight) ? ", в базе " + DbItem.Weight : "") + "\r\n" +
                          "Объем, м3 \t" + Volume + ((DbItem != null && DbItem.Volume != Volume) ? ", в базе " + DbItem.Volume : "") + "\r\n" +
                          "Наличие в базе: \t" + (IsNew? "Есть" : "нет") + "\r\n" +
                          (string.IsNullOrEmpty(Warning) ? "" : "Предупреждения: \t" + Warning);
            return info;
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
                IdBlRef.FlickObjectHighlight(2, 100, 100);
            }
        }

        public override string ToString()
        {
            return Mark;
        }        

        /// <summary>
        /// Определение марки по формуле из DB
        /// </summary>
        private void DefineMarkByFormulaInDb()
        {
            try
            {
                MarkDb = DbService.GetDbMark(this);
                MarkDbWoSpace = MarkDb.Replace(" ", "");
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
        }
        
        /// <summary>
        /// Инфо - Строка основных параметров панели
        /// </summary>
        /// <returns></returns>
        public string ParamsToString ()
        {
            return $"Группа={ItemGroup},Lenght={Lenght},Height={Height},Thickness={Thickness},Formwork={Formwork}" +
                $"BalconyDoor={BalconyDoor},BalconyCut={BalconyCut},Electrics={Electrics}.";            
        }    
    }
}