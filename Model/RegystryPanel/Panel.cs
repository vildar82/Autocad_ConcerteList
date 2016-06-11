using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AcadLib;
using AcadLib.Blocks;
using Autocad_ConcerteList.Model.ConcreteDB;
using Autocad_ConcerteList.Model.Panels;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace Autocad_ConcerteList.Model.RegystryPanel
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
        public decimal? BalconyCutId { get; set; }
        public string BalconyDoor { get; set; }
        public decimal? BalconyDoorId { get; set; }
        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName { get; set; }
        /// <summary>
        /// Покраска
        /// </summary>
        public string Color { get; set; }
        public ConcreteDB.DataSet.ConcerteDataSet.I_S_ItemGroupRow DbGroup { get; set; }
        /// <summary>
        /// Изделие в базе
        /// </summary>
        public ConcreteDB.DataSet.ConcerteDataSet.myItemRow DbItem { get; set; }
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
        /// <summary>
        /// Наличие в базе - DbItem != null
        /// </summary>
        public bool InBase { get { return DbItem != null; } }
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
        public decimal ItemGroupId { get; set; }
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

        //public static void FindBlocks(List<Panel> panels)
        //{
        //    // Найти блоки со старой маркой и исправить на марку из базы.
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    var panelsList = panels.ToList();
        //    using (var t = db.TransactionManager.StartTransaction())
        //    {
        //        var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
        //        foreach (var idEnt in ms)
        //        {
        //            var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
        //            if (blRef == null || blRef.AttributeCollection == null) continue;

        //            foreach (ObjectId idAtr in blRef.AttributeCollection)
        //            {
        //                var atrRef = idAtr.GetObject(OpenMode.ForRead, false, true) as AttributeReference;
        //                if (atrRef.Tag.Equals("МАРКА", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    var panelsMark = panelsList.FindAll(p => p.Mark.Equals(atrRef.TextString, StringComparison.OrdinalIgnoreCase));
        //                    foreach (var panel in panelsMark)
        //                    {
        //                        panel.IdBlRef = blRef.Id;
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //        t.Commit();
        //    }
        //}

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

        public void Check()
        {
            // Проверки.
            // 1. Проверка марки в атрибуте и в панели
            if (Mark != MarkDb)
            {
                if (MarkWoSpace != MarkDbWoSpace)
                {
                    ErrorStatus |= EnumErrorItem.IncorrectMark;
                    Warning += "Марка в блоке отличается от марки полученной по формуле. ";
                }
                else
                {
                    Warning += "Пропущен пробел в марке '" + Mark + "', правильно '" + MarkDb + "'";
                }
            }
            if (DbItem != null)
            {
                // проверка параметров в базе и в блоке
                if (this.Lenght != DbItem.Lenght ||
                    this.Height != DbItem.Height ||
                    this.Thickness != DbItem.Thickness ||
                    this.Weight != DbItem.Weight)
                {
                    ErrorStatus |= EnumErrorItem.DifferentParams;
                    Warning += "Параметры из атрибутов блока отличаются от параметров из базы. ";
                }
            }
        }

        public Result Define(ObjectId idBlRef)
        {
            // определить параметры панели из блока
            using (var blRef = idBlRef.Open(OpenMode.ForRead, false, true) as BlockReference)
            {
                if (blRef == null) return Result.Fail("Это не блок.");
                IdBlRef = idBlRef;
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

            DbItem = DbService.FindPanelByMark(Mark);
            if (DbItem == null)
            {
                DbItem = DbService.FindPanelByMark(MarkWoSpace);
            }

            DefineMarkByFormulaInDb();

            return Result.Ok();
        }

        public void FindBlock()
        {
            _alreadyCalcExtents = true;
            bool isFind = false;
            // Найти блоки со старой маркой и исправить на марку из базы.
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
                foreach (var idEnt in ms)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                    if (blRef == null || blRef.AttributeCollection == null) continue;

                    foreach (ObjectId idAtr in blRef.AttributeCollection)
                    {
                        var atrRef = idAtr.GetObject(OpenMode.ForRead, false, true) as AttributeReference;
                        if (atrRef.Tag.Equals("МАРКА", StringComparison.OrdinalIgnoreCase))
                        {
                            if (Mark.Equals(atrRef.TextString, StringComparison.OrdinalIgnoreCase))
                            {
                                isFind = true;
                                IdBlRef = blRef.Id;
                                try
                                {
                                    _extents = blRef.GeometricExtents;
                                }
                                catch
                                {
                                    _isNullExtents = true;
                                }
                            }
                            break;
                        }
                    }
                    if (isFind)
                    {
                        break;
                    }
                }
                t.Commit();
            }
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
                          (string.IsNullOrEmpty(BalconyCut) ? "" : "Подрезка\t" + BalconyCut + (DbItem == null ? "" : ", ширина в базе " + DbItem.BalconyCutSize) + "\r\n") +
                          //(FormworkMirror == null ? "" : "Зеркальность \t" + FormworkMirror + "\r\n") +
                          (string.IsNullOrEmpty(Electrics) ? "" : "Электрика \t" + Electrics + "\r\n") +
                          (string.IsNullOrEmpty(Color) ? "" : "Покраска \t" + Color + "\r\n") +
                          "Вес, кг \t\t" + Weight + ((DbItem != null && DbItem.Weight != Weight) ? ", в базе " + DbItem.Weight : "") + "\r\n" +
                          "Объем, м3 \t" + Volume + ((DbItem != null && DbItem.Volume != Volume) ? ", в базе " + DbItem.Volume : "") + "\r\n" +
                          "Наличие в базе: \t" + (DbItem == null ? "Нет" : "Есть") + "\r\n" +
                          (string.IsNullOrEmpty(Warning) ? "" : "Предупреждения: \t" + Warning);
            return info;
        }

        //public void SetParameter(string param, object value)
        //{
        //    switch (param)
        //    {
        //        case "block_name":
        //            this.BlockName = value?.ToString();
        //            break;

        //        case "Mark":
        //            this.Mark = value?.ToString();
        //            break;

        //        case "Color":
        //            this.Color = value?.ToString();
        //            break;

        //        case "ItemGroup":
        //            this.ItemGroup = value?.ToString();
        //            break;

        //        case "Length":
        //            this.Lenght = GetShortNullable(value);
        //            break;

        //        case "Height":
        //            this.Height = GetShortNullable(value);
        //            break;

        //        case "Thickness":
        //            this.Thickness = GetShortNullable(value);
        //            break;

        //        case "Formwork":
        //            this.Formwork = GetShortNullable(value);
        //            break;

        //        case "BalconyDoor":
        //            this.BalconyDoor = value?.ToString();
        //            break;

        //        case "BalconyCut":
        //            this.BalconyCut = value?.ToString();
        //            break;

        //        case "Electrics":
        //            this.Electrics = value?.ToString().ToLower();
        //            break;

        //        default:
        //            {
        //                string errMsg = $"Неопределенный параметр в панели - {param} = {value}";
        //                Logger.Log.Error(errMsg);
        //                throw new ArgumentException(errMsg);
        //            }
        //    }
        //}

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

        internal void DefineDbParams()
        {
            BalconyCutId = DbService.GetBalconyCutId(BalconyCut);
            BalconyDoorId = DbService.GetBalconyCutId(BalconyDoor);
            DefineMarkByFormulaInDb();
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
                Warning += "Ошибка формирования марки панели по параметрам - " + ex.Message;
            }
        }

        private void FillParseParams()
        {
            ItemGroup = ParseMark.ItemGroup;
            Formwork = ParseMark.Formwork;
            //FormworkMirror = ParseMark.FormworkMirror;
            BalconyCut = ParseMark.BalconyCut;
            BalconyCutId = DbService.GetBalconyCutId(BalconyCut);
            BalconyDoor = ParseMark.BalconyDoor;
            BalconyDoorId = DbService.GetBalconyCutId(BalconyDoor);
            Electrics = ParseMark.Electrics;
        }
    }
}