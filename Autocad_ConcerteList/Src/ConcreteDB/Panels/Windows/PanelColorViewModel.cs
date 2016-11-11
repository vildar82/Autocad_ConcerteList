using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelColorViewModel : ObservableObject
    {
        private Panel panel;
        public PanelColorViewModel(Panel panel)
        {
            this.panel = panel;
            Mark = panel.Mark;
            Color = panel.Color;
            MarkWithColor = panel.GetMarkWithColor();                
        }

        public string Mark { get; set; }
        public string Color { get; set; }
        public string MarkWithColor { get; set; }
    }
}
