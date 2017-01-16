using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class ItemGroupDbo
    {
        public int ItemGroupId { get; set; }        
        public string ItemGroup { get; set; }        
        public bool? HasFormula { get; set; }
        public string Formula { get; set; }
        public short LengthFactor { get; set; }
        public short HeightFactor { get; set; }
        public short ThicknessFactor { get; set; }
        /// <summary>
        /// Ключ габаритов в формуле - последовательность параметров Длины, Высоты и Ширины (LHT)
        /// </summary>
        public string GabKey { get; set; }
    }
}
