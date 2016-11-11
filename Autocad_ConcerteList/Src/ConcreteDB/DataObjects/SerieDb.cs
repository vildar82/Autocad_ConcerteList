using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class SerieDbo
    {
        public int SeriesId { get; set; }        
        public string Name { get; set; }

        public override string ToString ()
        {
            return Name;
        }
    }
}
