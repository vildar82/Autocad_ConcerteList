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
    public class RegPanelsViewModel :PanelsBaseView
    {        
        public bool CanRegistry { get; set; }
        public ObservableCollection<SerieDbo> Series { get; set; }
        public SerieDbo SelectedSerie { get; set; }        

        public RegPanelsViewModel (List<KeyValuePair<Panel, List<Panel>>> regPanels, List<SerieDbo> series): base(regPanels)
        {
            Series = new ObservableCollection<SerieDbo>(series);
            SelectedSerie = Series.FirstOrDefault(s => s.Name.Equals("ПИК-1.0"));
            // Фон - есть панели с ошибками - красная
            if (regPanels.Any(p=>p.Key.HasErrors))
            {                
                CanRegistry = false;
            }
            else
            {                
                CanRegistry = true;
            }
        }        
    }
}
