using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class ItemGroupDbo
    {
        public decimal ItemGroupId { get; set; }        
        public string ItemGroup { get; set; }        
        public Nullable<bool> HasFormula { get; set; }
        public string Formula { get; set; }
    }
}
