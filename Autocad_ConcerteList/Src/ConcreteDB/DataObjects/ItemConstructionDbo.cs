using System;
using Autocad_ConcerteList.ConcreteDB.DataBase;

namespace Autocad_ConcerteList.ConcreteDB.DataObjects
{
    public class ItemConstructionDbo : IEquatable<ItemConstructionDbo>
    {
        public IIPanel Panel { get; set; }
        public bool IsIgnoreGab { get; set; }
        public int ItemConstructionId { get; set; }
        public string HandMarkNoColour { get; set; }
        public int? ItemGroupId { get; set; }
        public string ItemGroup { get; set; }
        public short? Length { get; set; }
        public short? Height { get; set; }
        public short? Thickness { get; set; }
        public short? Formwork { get; set; }     
        public string MountElement { get; set; }
        public string Prong { get; set; }
        public string BalconyCut { get; set; }
        //public decimal? BalconyCutId { get; set; }
        public string BalconyDoor { get; set; }
        //public decimal? BalconyDoorId { get; set; }
        public int? StepHeightIndex { get; set; }
        public int? StepCount { get; set; }
        public int? StepFirstHeight { get; set; }
        public short? FormworkMirror { get; set; }
        public string Electrics { get; set; }
        public float? Weight { get; set; }
        public float? Volume { get; set; }

        public Item_modification MountElementModif { get => mountElementModif;
	        set { mountElementModif = value; MountElement = value?.Item_modification_code;  } }

	    private Item_modification mountElementModif;

        public Item_modification ProngModif {
            get => prongModif;
	        set { prongModif = value; Prong = value?.Item_modification_code; }
        }

	    private Item_modification prongModif;

        public Item_modification BalconyCutModif {
            get => balconyCutModif;
	        set { balconyCutModif = value; BalconyCut = value?.Item_modification_code; }
        }

	    private Item_modification balconyCutModif;

        public Item_modification BalconyDoorModif {
            get => balconyDoorModif;
	        set { balconyDoorModif = value; BalconyDoor = value?.Item_modification_code; }
        }

	    private Item_modification balconyDoorModif;

        public Item_modification StepHeightModif {
            get => stepHeightModif;
	        set { stepHeightModif = value; StepHeightIndex = value == null? null : (int?)int.Parse(value.Item_modification_code); }
        }

	    private Item_modification stepHeightModif;

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
