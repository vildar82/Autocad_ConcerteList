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
    
    public partial class Item_modification_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Item_modification_type()
        {
            this.Item_modification = new HashSet<Item_modification>();
        }
    
        public int Item_modification_type_id { get; set; }
        public Nullable<int> Item_modification_type_id_access { get; set; }
        public string Item_modification_type1 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item_modification> Item_modification { get; set; }
    }
}
