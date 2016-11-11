using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class CheckPanelsViewModel : PanelsBaseView
    {
        private const string filterAll = "Все панели";
        private const string filterErrors = "Панели с ошибками";

        private string _curFilter;
        private string _search = "";

        public CheckPanelsViewModel (List<KeyValuePair<Panel, List<Panel>>> panels) : base(panels)
        {
            // Фильтр панелей с ошибками
            CurFilter = filterErrors;
        }

        public string Search {
            get { return _search; }
            set {
                _search = value;
                Filtering();
                RaisePropertyChanged();
            }
        }       
        public ObservableCollection<string> Filter { get; set; } 
            = new ObservableCollection<string>() { filterAll, filterErrors };
        public string CurFilter {
            get { return _curFilter; }
            set {
                _curFilter = value;
                Filtering();
                RaisePropertyChanged();
            }
        }        

        private void Searching ()
        {
            var filterPanels = Panels.Where(p=> Regex.IsMatch(p.MarkAtr, _search, RegexOptions.IgnoreCase)).ToList();
            Panels.Clear();
            foreach (var item in filterPanels)
            {
                Panels.Add(item);
            }
        }

        private void Filtering ()
        {
            UpdateAllPanels();
            switch (_curFilter)
            {                
                case filterErrors:
                    FilterErrorPanels();
                    break;
                default:                    
                    break;
            }
            Searching();
        }

        private void FilterErrorPanels ()
        {
            var errPanels = Panels.Where(p=>p.panel.HasErrors || !p.panel.IsWeightOk).ToList();
            Panels.Clear();
            foreach (var item in errPanels)
            {
                Panels.Add(item);
            }
        }        
    }
}
