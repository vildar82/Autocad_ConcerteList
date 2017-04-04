using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class RegColorViewModel : ObservableObject
    {
        public RegColorViewModel (List<IIPanel> panels)
        {
            Panels = new ObservableCollection<PanelColorViewModel>(panels.Select(s=>new PanelColorViewModel(s)));            
        }

        public ObservableCollection<PanelColorViewModel> Panels { get; set; }

        public PanelColorViewModel SelectedPanel { get { return selectedPanel; } set { selectedPanel = value; ShowPanel(); } }
        PanelColorViewModel selectedPanel;

        private void ShowPanel ()
        {
            selectedPanel.panel.Show();
        }
    }
}
