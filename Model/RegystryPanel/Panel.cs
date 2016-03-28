using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.ConcreteDB;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad_ConcerteList.RegystryPanel
{
    /// <summary>
    /// ЖБИ изделие полученное из автокада
    /// </summary>
    public class Panel  : iItem
    {
        public string BlockName { get; set; }
        /// <summary>
        /// Марка от конструкторов
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// Покраска
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Марка из базы
        /// </summary>
        public string MarkDb { get; set; }
        /// <summary>
        /// 3НСг
        /// </summary>
        public string ItemGroup { get; set; }
        public decimal ItemGroupId { get; set; }
        public short? Lenght { get; set; }
        public short? Height { get; set; }
        public short? Thickness { get; set; }
        /// <summary>
        /// Опалубка
        /// </summary>
        public short? Formwork { get; set; }
        public string BalconyDoor { get; set; }
        public string BalconyCut { get; set; }
        public short? FormworkMirror { get; set; }
        public string Electrics { get; set; }        
        public float? Weight { get; set; }
        public float? Volume { get; set; }      

        public EnumErrorItem ErrorStatus { get; set; }
        
        public ObjectId IdBlRef { get; set; }

        private bool _alreadyCalcExtents;
        private bool _isNullExtents;
        private Extents3d _extents;
        public Extents3d Extents
        {
            get
            {
                if (!_alreadyCalcExtents)
                {
                    //FindBlock();
                    using (var blRef = IdBlRef.Open( OpenMode.ForRead, false, true)as BlockReference)
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

        public string ErrorName
        {
            get
            {
                switch (ErrorStatus)
                {                    
                    case EnumErrorItem.IncorrectMark:
                        return "Несоответствуящая марка";                        
                    case EnumErrorItem.DifferentParams:
                        return "Разные параметры";
                    default:
                        return "";
                }
            }
        }        

        private string _info;

        public void Show()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                Editor ed = doc.Editor;
                ed.Zoom(Extents);
            }
        }

        public string Info
        {
            get
            {
                if (_info == null)
                {
                    _info = getInfo();
                }
                return _info;
            }
            set { _info = value; }
        }        

        public string getInfo()
        {
            string info = "Марка \t\t" + Mark + "\r\n" +
                          "Марка по базе \t" + MarkDb + "\r\n" +
                          "Параметры панели из блока:\r\n" +
                          "Группа \t\t" + ItemGroup + "\r\n" +
                          (Lenght == null ? "" : "Длина \t\t" + Lenght + "\r\n") +
                          (Height == null ? "" : "Высота \t\t" + Height + "\r\n") +
                          (Thickness == null ? "" : "Ширина \t\t" + Thickness + "\r\n") +
                          (Formwork == null ? "" : "Опалубка \t" + Formwork + "\r\n") +
                          (string.IsNullOrEmpty(BalconyDoor) ? "" : "Балконный проем " + BalconyDoor + "\r\n") +
                          (string.IsNullOrEmpty(BalconyCut) ? "" : "Подрезка под балкон " + BalconyCut + "\r\n") +
                          (FormworkMirror == null ? "" : "Зеркальность \t" + FormworkMirror + "\r\n") +
                          (string.IsNullOrEmpty(Electrics) ? "" : "Электрика \t" + Electrics + "\r\n") +
                          (Weight == null ? "" : "Вес, кг \t\t" + Weight + "\r\n") +
                          (Volume == null ? "" : "Объем, м3 \t" + Volume + "\r\n") +
                          (string.IsNullOrEmpty(Color) ? "" : "Покраска \t" + Color);
            return info;
        }

        

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
                FormworkMirror = firstP.FormworkMirror,
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
                (panelGroupMark.GroupBy(g => g.BalconyDoor).Skip(1).Any() ? "Балконный проем *\r\n" :
                    ((resPanel.BalconyDoor == null ? "" : "Балконный проем " + resPanel.BalconyDoor + "\r\n"))) +
                // BalconyCut              
                (panelGroupMark.GroupBy(g => g.BalconyCut).Skip(1).Any() ? "Подрезка под балкон *\r\n" :
                    ((resPanel.BalconyCut == null ? "" : "Подрезка под балкон " + resPanel.BalconyCut + "\r\n"))) +
                // FormworkMirror
                (panelGroupMark.GroupBy(g => g.FormworkMirror).Skip(1).Any() ? "Зеркальность\t*\r\n" :
                    ((resPanel.FormworkMirror == null ? "" : "Зеркальность\t" + resPanel.FormworkMirror + "\r\n"))) +
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

        public void SetParameter(string param, object value)
        {
            switch (param)
            {
                case "block_name":
                    this.BlockName = value?.ToString();
                    break;
                case "Mark":
                    this.Mark = value?.ToString();
                    break;
                case "Color":
                    this.Color = value?.ToString();
                    break;
                case "ItemGroup":
                    this.ItemGroup = value?.ToString();
                    break;
                case "Length":
                    this.Lenght = GetShortNullable(value);
                    break;
                case "Height":
                    this.Height = GetShortNullable(value);
                    break;
                case "Thickness":
                    this.Thickness = GetShortNullable(value);
                    break;
                case "Formwork":
                    this.Formwork = GetShortNullable(value);
                    break;
                case "BalconyDoor":
                    this.BalconyDoor = value?.ToString();
                    break;
                case "BalconyCut":
                    this.BalconyCut = value?.ToString();
                    break;
                case "Electrics":
                    this.Electrics = value?.ToString().ToLower();
                    break;                
                default:
                    Logger.Log.Error($"Неопределенный параметр в панели - {param} = {value}, переданный из лиспа.");
                    break;
            }
        }

        public static short? GetShortNullable(object obj)
        {
            try
            {
                return Convert.ToInt16(obj);
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            return Mark;
        }

        public static void FindBlocks(List<Panel> panels)
        {
            // Найти блоки со старой маркой и исправить на марку из базы.
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var panelsList = panels.ToList();
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
                            var panelsMark = panelsList.FindAll(p => p.Mark.Equals(atrRef.TextString, StringComparison.OrdinalIgnoreCase));                            
                            foreach (var panel in panelsMark)
                            {
                                panel.IdBlRef = blRef.Id;
                            }                            
                            break;
                        }
                    }                    
                }
                t.Commit();
            }
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
    }
}
