using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autocad_ConcerteList.Src.ConcreteDB.DataObjects;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    public class RegPanelsViewModel :PanelsBaseView
    {
        public bool CanRegistry { get; set; }
        public ObservableCollection<SerieDbo> Series { get; set; }
        public SerieDbo SelectedSerie { get; set; }

        public RegPanelsViewModel (List<KeyValuePair<IIPanel, List<IIPanel>>> regPanels, List<SerieDbo> series): base(regPanels)
        {
            Series = new ObservableCollection<SerieDbo>(series);
            SelectedSerie = Series.FirstOrDefault(s => s.Name.Equals("ПИК-1.0"));            
            CheckState();
        }

        public override void CheckState ()
        {
            // Фон - есть панели с ошибками - красная
            if (_panels.Any(p => p.Key.HasErrors))
            {
                CanRegistry = false;
                Background = ColorBad;
                Title = "Регистрация панелей. Есть ошибки в панелях";
            }
            else
            {
                CanRegistry = true;
                Background = ColorGood;
                Title = "Регистрация панелей";
            }
        }        
    }
}
