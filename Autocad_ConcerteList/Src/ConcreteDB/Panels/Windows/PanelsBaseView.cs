namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;
    using NetLib.WPF;

    public class PanelsBaseView : BaseModel
    {
        protected Brush ColorBad = new SolidColorBrush(Colors.Red);
        protected Brush ColorGood = new SolidColorBrush(Colors.Lime);

        protected List<KeyValuePair<IIPanel, List<IIPanel>>> _panels;

        public PanelsBaseView (List<KeyValuePair<IIPanel, List<IIPanel>>> panels)
        {
            _panels = panels;
            UpdateAllPanels();
            CheckState();
        }

        public List<IIPanel> PanelsToReg {
            get {
                return _panels.Select(s => s.Key).ToList();
            }
        }

        public ObservableCollection<PanelViewModel> Panels { get; set; }

        public Brush Background { get; set; }

        public PanelViewModel SelectedPanel { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Количество строй
        /// </summary>
        public int CountRow => Panels.Count;

        /// <summary>
        /// Количество блоков панелей
        /// </summary>
        public int CountBlocks => Panels.Sum(s => s.PanelsInModel.Count);

        public string CountString => $" Строк {CountRow} ";

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
