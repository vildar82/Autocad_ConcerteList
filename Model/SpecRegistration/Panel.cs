using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.SpecRegistration
{
    /// <summary>
    /// ЖБИ изделие полученное из автокада
    /// </summary>
    public class Panel
    {
        /// <summary>
        /// Марка от конструкторов
        /// </summary>
        public string Mark { get; set; }        
        /// <summary>
        /// 3НСг
        /// </summary>
        public string ItemGroup { get; set; }
        public short Length { get; set; }
        public short Height { get; set; }
        public short Thickness { get; set; }
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
    }
}
