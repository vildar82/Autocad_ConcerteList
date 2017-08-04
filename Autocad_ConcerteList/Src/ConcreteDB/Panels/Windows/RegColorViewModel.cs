using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MicroMvvm;

namespace Autocad_ConcerteList.ConcreteDB.Panels.Windows
{
    public class RegColorViewModel : ObservableObject
    {
        public RegColorViewModel (List<IPanel> panels)
        {
            Panels = new ObservableCollection<PanelColorViewModel>(panels.Select(s=>new PanelColorViewModel(s)));            
        }

        public ObservableCollection<PanelColorViewModel> Panels { get; set; }

        public PanelColorViewModel SelectedPanel { get => selectedPanel;
	        set { selectedPanel = value; ShowPanel(); } }
	    private PanelColorViewModel selectedPanel;

        private void ShowPanel ()
        {
            selectedPanel.panel.Show();
        }
    }
}
