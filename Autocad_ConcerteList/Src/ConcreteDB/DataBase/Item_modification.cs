//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Autocad_ConcerteList.Src.ConcreteDB.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item_modification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item_modification()
        {
            this.BalconyCutModif = new HashSet<Item_construction>();
            this.BalconyDoorModif = new HashSet<Item_construction>();
            this.MountModif = new HashSet<Item_construction>();
            this.ProngModif = new HashSet<Item_construction>();
            this.StepHeightModif = new HashSet<Item_construction>();
        }
    
        public int Item_modification_id { get; set; }
        public Nullable<int> Item_modification_id_access { get; set; }
        public string Item_modification_code { get; set; }
        public Nullable<int> Item_modification_type_id { get; set; }
        public Nullable<int> Side_id { get; set; }
        public Nullable<int> Item_modification_point { get; set; }
        public Nullable<int> Item_modification_measure { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_construction> BalconyCutModif { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_construction> BalconyDoorModif { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_construction> MountModif { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_construction> ProngModif { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_construction> StepHeightModif { get; set; }
        public virtual Item_modification_type Item_modification_type { get; set; }
        public virtual Side Side { get; set; }
    }
}