using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class MarkPart
    {
        public string Mark { get; set; }
        public string PartGroup { get; set; }
        public string PartGab { get; set; }
        public string PartDop { get; set; }

        public MarkPart (string mark)
        {
            Mark = mark;
            PartGroup = null;
            PartGab = null;
            PartDop = null;
        }
    }
}
