//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Autocad_ConcerteList.Src.ConcreteDB.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item_group_long
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item_group_long()
        {
            this.Item_group = new HashSet<Item_group>();
        }
    
        public int Item_group_long_id { get; set; }
        public Nullable<int> Item_group_long_id_access { get; set; }
        public string Item_group_long1 { get; set; }
        public int Item_group_short_id { get; set; }
        public System.Guid guid { get; set; }
        public System.DateTime updated_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_group> Item_group { get; set; }
        public virtual Item_group_short Item_group_short { get; set; }
    }
}
