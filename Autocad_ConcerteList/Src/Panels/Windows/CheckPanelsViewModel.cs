using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Autocad_ConcerteList.Src.RegystryPanel.Windows
{
    public class CheckPanelsViewModel
    {
        public ObservableCollection<PanelViewModel> Panels { get; set; }
        public Brush Background { get; set; }

        public CheckPanelsViewModel(List<Panel> panels)
        {
            Panels = new ObservableCollection<PanelViewModel>();
            foreach (var item in panels)
            {
                Panels.Add(new PanelViewModel(item));
            }

            // Фон - есть панели с ошибками - красная
            if (panels.Any(p=>p.HasErrors))
            {
                Background = new SolidColorBrush(Colors.Red);
            }
            else
            {                
            }
        }
    }
}
