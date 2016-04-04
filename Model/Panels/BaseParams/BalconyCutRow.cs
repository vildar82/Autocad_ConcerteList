using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.RegystryPanel;

namespace Autocad_ConcerteList.Model.Panels.BaseParams
{
    public class BalconyCutRow
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public string Side { get; set; }
        public List<Panel> Panels { get; set; }

        public BalconyCutRow(IGrouping<string, Panel> item)
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
