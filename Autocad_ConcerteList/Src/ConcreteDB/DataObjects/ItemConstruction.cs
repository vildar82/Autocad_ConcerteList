using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class ItemConstruction
    {
        public decimal ItemConstructionId { get; set; }
        public string HandMarkNoColour { get; set; }
        public Nullable<decimal> ItemGroupId { get; set; }
        public string ItemGroup { get; set; }
        public Nullable<short> Lenght { get; set; }
        public Nullable<short> Height { get; set; }
        public Nullable<short> Thickness { get; set; }
        public Nullable<short> Formwork { get; set; }        
        public BalconyCut BalconyCut { get; set; }
        public BalconyDoor BalconyDoor { get; set; }
        public Nullable<short> FormworkMirror { get; set; }
        public string Electrics { get; set; }
        public Nullable<float> Weight { get; set; }
        public Nullable<float> Volume { get; set; }
    }
}
