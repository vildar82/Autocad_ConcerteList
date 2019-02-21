using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class CheckPanelsViewModel : PanelsBaseView
    {
        private const string filterAll = "Все панели";
        private const string filterErrors = "Панели с ошибками";

        private string _curFilter;
        private string _search = "";

        public CheckPanelsViewModel (List<KeyValuePair<IIPanel, List<IIPanel>>> panels) : base(panels)
        {
            // Фильтр панелей с ошибками
            CurFilter = filterErrors;
        }

        public string Search {
            get => _search;
            set {
                _search = value;
                Filtering();
            }
        }
        public ObservableCollection<string> Filter { get; set; }
            = new ObservableCollection<string> { filterAll, filterErrors };
        public string CurFilter {
            get => _curFilter;
            set {
                _curFilter = value;
                Filtering();
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
