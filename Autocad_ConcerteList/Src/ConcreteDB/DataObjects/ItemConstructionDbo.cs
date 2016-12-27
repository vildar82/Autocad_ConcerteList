using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.ConcreteDB.DataBase;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class ItemConstructionDbo : IEquatable<ItemConstructionDbo>
    {
        public Panel Panel { get; set; }
        public bool IsIgnoreGab { get; set; }
        public int ItemConstructionId { get; set; }
        public string HandMarkNoColour { get; set; }
        public Nullable<int> ItemGroupId { get; set; }
        public string ItemGroup { get; set; }
        public Nullable<short> Length { get; set; }
        public Nullable<short> Height { get; set; }
        public Nullable<short> Thickness { get; set; }
        public Nullable<short> Formwork { get; set; }     
        public string MountElement { get; set; }
        public string Prong { get; set; }
        public string BalconyCut { get; set; }
        //public decimal? BalconyCutId { get; set; }
        public string BalconyDoor { get; set; }
        //public decimal? BalconyDoorId { get; set; }
        public int? StepHeightIndex { get; set; }
        public int? StepCount { get; set; }
        public int? StepFirstHeight { get; set; }
        public Nullable<short> FormworkMirror { get; set; }
        public string Electrics { get; set; }
        public Nullable<float> Weight { get; set; }
        public Nullable<float> Volume { get; set; }

        public Item_modification MountElementModif { get { return mountElementModif; }
            set { mountElementModif = value; MountElement = value?.Item_modification_code;  } }
        Item_modification mountElementModif;

        public Item_modification ProngModif {
            get { return prongModif; }
            set { prongModif = value; Prong = value?.Item_modification_code; }
        }
        Item_modification prongModif;

        public Item_modification BalconyCutModif {
            get { return balconyCutModif; }
            set { balconyCutModif = value; BalconyCut = value?.Item_modification_code; }
        }
        Item_modification balconyCutModif;

        public Item_modification BalconyDoorModif {
            get { return balconyDoorModif; }
            set { balconyDoorModif = value; BalconyDoor = value?.Item_modification_code; }
        }
        Item_modification balconyDoorModif;

        public Item_modification StepHeightModif {
            get { return stepHeightModif; }
            set { stepHeightModif = value; StepHeightIndex = value == null? null : (int?)int.Parse(value.Item_modification_code); }
        }
        Item_modification stepHeightModif;

        public bool Equals(ItemConstructionDbo other)
        {
            return ItemGroup == other.ItemGroup &&
                (!IsIgnoreGab && ( 
                Length == other.Length &&
                Height == other.Height &&
                Thickness == other.Thickness)) &&
                Formwork == other.Formwork &&
                string.Equals(MountElement, other.MountElement, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Prong, other.Prong, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(BalconyCut, other.BalconyCut, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(BalconyDoor, other.BalconyDoor, StringComparison.OrdinalIgnoreCase) &&
                FormworkMirror == other.FormworkMirror &&
                StepHeightIndex == other.StepHeightIndex &&
                StepCount == other.StepCount &&
                StepFirstHeight == other.StepFirstHeight &&
                string.Equals(Electrics, other.Electrics, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ItemGroup.GetHashCode();
        }
    }
}
