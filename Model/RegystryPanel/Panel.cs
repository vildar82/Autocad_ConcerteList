using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autocad_ConcerteList.Model.ConcreteDB;
using Autodesk.AutoCAD.Geometry;
using System.IO;

namespace Autocad_ConcerteList.Model.RegystryPanel
{
    /// <summary>
    /// ЖБИ изделие полученное из автокада
    /// </summary>
    public class Panel : iItem
    {
        public string BlockName { get; set; }
        /// <summary>
        /// Марка от конструкторов
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// Марка от конструкторов без пробелов
        /// </summary>
        public string MarkWoSpace { get; set; }
        /// <summary>
        /// Покраска
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Марка из базы
        /// </summary>
        public string MarkDb { get; set; }
        public string MarkDbWoSpace { get; set; }
        /// <summary>
        /// 3НСг
        /// </summary>        
        public string ItemGroup { get; set; }        
        public decimal? ItemGroupId { get; set; }
        public short? Lenght { get; set; }
        public short? Height { get; set; }
        public short? Thickness { get; set; }
        public short? Formwork { get; set; }
        public string BalconyDoor { get; set; }
        public decimal? BalconyDoorId { get; set; }
        public string BalconyCut { get; set; }
        public decimal? BalconyCutId { get; set; }
        public short? FormworkMirror { get; set; }
        public string Electrics { get; set; }
        public string Aperture { get; set; }
        public string Album { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }        

        /// <summary>
        /// Определение марки по формуле из DB
        /// </summary>
        private void DefineMarkByFormulaInDb()
        {
            MarkDb = DbService.GetDbMark(this);
            MarkDbWoSpace = MarkDb.Replace(" ", "");
        }

        internal void DefineDbParams()
        {
            BalconyCutId = DbService.GetBalconyCutId(BalconyCut);
            BalconyDoorId = DbService.GetBalconyCutId(BalconyDoor);
            DefineMarkByFormulaInDb();
        }

        /// <summary>
        /// Проверка - есть ли параметры в панели, кроме Марки и Группы
        /// </summary>        
        internal bool NoParameters()
        {
            return Lenght == null &&
                   Height == null &&
                   Thickness == null &&
                   Formwork == null &&
                   BalconyDoorId == null &&
                   BalconyCutId == null &&
                   FormworkMirror == null &&
                   Electrics == null;
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
                    break;                    
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
    }
}
