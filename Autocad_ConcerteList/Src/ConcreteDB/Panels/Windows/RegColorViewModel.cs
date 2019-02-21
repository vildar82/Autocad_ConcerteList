using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetLib.WPF;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class RegColorViewModel : BaseModel
    {
        public RegColorViewModel (List<IIPanel> panels)
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
