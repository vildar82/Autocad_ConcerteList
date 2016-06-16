using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelsBaseView
    {
        private List<KeyValuePair<Panel, List<Panel>>> _panels;
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
        public string CountString {
            get { return $"Строк {CountRow}, блоков {CountBlocks}"; }
        }

        public PanelsBaseView (List<KeyValuePair<Panel, List<Panel>>> panels)
        {
            _panels = panels;
            UpdateObservableColl();            
        }

        public void UpdateObservableColl()
        {
            var selMarkAtr = SelectedPanel?.MarkAtr;
            if (Panels == null)
            {
                Panels = new ObservableCollection<PanelViewModel>();             
            }
            else
            {                
                Panels.Clear();
            }
            foreach (var item in _panels)
            {
                Panels.Add(new PanelViewModel(item.Key, item.Value, this));
            }
            // Фон - есть панели с ошибками - красная
            if (_panels.Any(p => p.Key.HasErrors))
            {
                Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Background = new SolidColorBrush(Colors.Lime);
            }

            if (!string.IsNullOrEmpty(selMarkAtr))
            {
                SelectedPanel = null;
                SelectedPanel = Panels.FirstOrDefault(p => p.MarkAtr == selMarkAtr);
            }
        }
    }
}
