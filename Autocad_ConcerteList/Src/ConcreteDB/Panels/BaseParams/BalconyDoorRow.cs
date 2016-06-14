using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Src.RegystryPanel;

namespace Autocad_ConcerteList.Src.Panels.BaseParams
{
    public class BalconyDoorRow
    {
        public string Name { get; set; }        
        public string Side { get; set; }
        public List<Panel> Panels { get; set; }

        public BalconyDoorRow(IGrouping<string, Panel> item)
        {
            Name = item.Key;
            Panels = item.ToList();
            Side = string.Empty;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
