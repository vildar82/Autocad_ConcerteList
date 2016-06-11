using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Model.RegystryPanel.Windows
{
    public class CheckPanelsViewModel
    {
        ObservableCollection<PanelViewModel> Panels = new ObservableCollection<PanelViewModel>();

        public CheckPanelsViewModel(List<Panel> panels)
        {
            foreach (var item in panels)
            {
                Panels.Add(new PanelViewModel(item));
            }
        }
    }
}
