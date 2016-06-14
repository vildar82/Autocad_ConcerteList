using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class ItemConstructionDbo : IEquatable<ItemConstructionDbo>
    {
        public decimal ItemConstructionId { get; set; }
        public string HandMarkNoColour { get; set; }
        public Nullable<decimal> ItemGroupId { get; set; }
        public string ItemGroup { get; set; }
        public Nullable<short> Lenght { get; set; }
        public Nullable<short> Height { get; set; }
        public Nullable<short> Thickness { get; set; }
        public Nullable<short> Formwork { get; set; }        
        public BalconyCutDbo BalconyCut { get; set; }
        public decimal? BalconyCutId { get; set; }
        public BalconyDoorDbo BalconyDoor { get; set; }
        public decimal? BalconyDoorId { get; set; }
        public Nullable<short> FormworkMirror { get; set; }
        public string Electrics { get; set; }
        public Nullable<float> Weight { get; set; }
        public Nullable<float> Volume { get; set; }

        public bool Equals(ItemConstructionDbo other)
        {
            return ItemGroup == other.ItemGroup &&
                Lenght == other.Lenght &&
                Height == other.Height &&
                Thickness == other.Thickness &&
                Formwork == other.Formwork &&
                BalconyCut == other.BalconyCut &&
                BalconyDoor == other.BalconyDoor &&
                FormworkMirror == other.FormworkMirror &&
                Electrics == other.Electrics;
        }

        public override int GetHashCode()
        {
            return ItemGroup.GetHashCode();
        }
    }
}
