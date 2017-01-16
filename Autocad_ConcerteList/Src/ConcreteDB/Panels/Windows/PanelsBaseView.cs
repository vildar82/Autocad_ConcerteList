using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using MicroMvvm;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class PanelsBaseView : ObservableObject
    {
        protected Brush ColorBad = new SolidColorBrush(Colors.Red);
        protected Brush ColorGood = new SolidColorBrush(Colors.Lime);
        private string _title;
        private PanelViewModel _selectedPanel;
        private Brush _background;
        private ObservableCollection<PanelViewModel> _panelsViewModel;

        protected List<KeyValuePair<iPanel, List<iPanel>>> _panels;

        public PanelsBaseView (List<KeyValuePair<iPanel, List<iPanel>>> panels)
        {
            _panels = panels;
            UpdateAllPanels();
            CheckState();
        }

        public List<iPanel> PanelsToReg {
            get {
                return _panels.Select(s => s.Key).ToList();
            }
        }
        public ObservableCollection<PanelViewModel> Panels {
            get { return _panelsViewModel; }
            set {
                _panelsViewModel = value;
                RaisePropertyChanged();
            }
        }
        public Brush Background {
            get { return _background; }
            set {
                _background = value;
                RaisePropertyChanged();
            }
        }
        public PanelViewModel SelectedPanel {
            get { return _selectedPanel; }
            set {
                _selectedPanel = value;
                RaisePropertyChanged();
            }
        }
        public string Title {
            get { return _title; }
            set {
                _title = value;
                RaisePropertyChanged();
            }
        }

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
            get { return $" Строк {CountRow} "; }
        }        

        protected void UpdateAllPanels ()
        {
            if (Panels != null)
            {
                if (_panels.Count == Panels.Count) return;
                Panels.Clear();
            }
            else
            {
                Panels = new ObservableCollection<PanelViewModel>();
            }            
            foreach (var item in _panels)
            {
                Panels.Add(new PanelViewModel(item.Key, item.Value, this));
            }
        }

        public virtual void CheckState ()
        {
            // Фон - есть панели с ошибками - красная
            if (_panels.Any(p => p.Key.HasErrors))
            {
                Background = ColorBad;
                Title = "Панели с ошибками";
            }
            else
            {
                Background = ColorGood;
                Title = "Новые панели. Панелей с ошибками нет.";
            }
        }

        public void DeleteRow (PanelViewModel panelView)
        {
            var index = _panels.FindIndex(p => p.Key == panelView.panel);
            _panels.RemoveAt(index);
            Panels.Remove(panelView);
        }
    }
}