using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class RegPanelsViewModel
    {
        public ObservableCollection<PanelViewModel> Panels { get; set; }
        public Brush Background { get; set; }
        public PanelViewModel SelectedPanel { get; set; }
        public bool CanRegistry { get; set; }
        /// <summary>
        /// Количество строк
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
        public string CountString {
            get { return $"Строк {CountRow}, блоков {CountBlocks}"; }
        }        

        public RegPanelsViewModel (List<KeyValuePair<Panel, List<Panel>>> regPanels)
        {
            Panels = new ObservableCollection<PanelViewModel>();            
            foreach (var item in regPanels)
            {                
                Panels.Add(new PanelViewModel(item.Key, item.Value));
            }

            // Фон - есть панели с ошибками - красная
            if (regPanels.Any(p=>p.Key.HasErrors))
            {
                Background = new SolidColorBrush(Colors.Red);
                CanRegistry = false;
            }
            else
            {
                Background = new SolidColorBrush(Colors.Lime);
                CanRegistry = true;
            }
        }        
    }
}
