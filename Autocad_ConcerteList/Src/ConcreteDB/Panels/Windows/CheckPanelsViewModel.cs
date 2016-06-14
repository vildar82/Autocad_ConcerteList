using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class CheckPanelsViewModel
    {
        public ObservableCollection<PanelViewModel> Panels { get; set; }
        public Brush Background { get; set; }
        public PanelViewModel SelectedPanel { get; set; }
        /// <summary>
        /// Количество строй
        /// </summary>
        public int CountRow {
            get { return Panels.Count; }
        }
        /// <summary>
        /// Количество блоков панелей
        /// </summary>
        public int CountBlocks {
            get {
                 return Panels.Sum(s => s.PanelsInModel.Count);
            }
        }

        public CheckPanelsViewModel (Panel panel)
        {
            Panels = new ObservableCollection<PanelViewModel>();            
            Panels.Add(new PanelViewModel(panel));            

            // Фон - есть панели с ошибками - красная
            if (panel.HasErrors)
            {
                Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Background = new SolidColorBrush(Colors.Lime);
            }

        }

        public CheckPanelsViewModel(List<IGrouping<Panel, Panel>> panels)
        {
            Panels = new ObservableCollection<PanelViewModel>();            
            foreach (var item in panels)
            {                
                Panels.Add(new PanelViewModel(item));
            }

            // Фон - есть панели с ошибками - красная
            if (panels.Any(p=>p.Key.HasErrors))
            {
                Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Background = new SolidColorBrush(Colors.Lime);
            }
        }
    }
}
