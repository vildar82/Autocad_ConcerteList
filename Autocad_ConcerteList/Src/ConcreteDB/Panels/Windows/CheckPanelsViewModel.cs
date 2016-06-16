using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;

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
        public string CountString {
            get { return $"Строк {CountRow}, блоков {CountBlocks}"; }
        }        

        public CheckPanelsViewModel(List<KeyValuePair<Panel, List<Panel>>> panels)
        {            
            Panels = new ObservableCollection<PanelViewModel>();            
            foreach (var item in panels)
            {                
                Panels.Add(new PanelViewModel(item.Key, item.Value));
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
