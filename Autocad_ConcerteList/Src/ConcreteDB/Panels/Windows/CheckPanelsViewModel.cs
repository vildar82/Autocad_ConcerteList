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
    public class CheckPanelsViewModel : PanelsBaseView
    {   
        

        public CheckPanelsViewModel(List<KeyValuePair<Panel, List<Panel>>> panels): base(panels)
        {           
            
        }        
    }
}
