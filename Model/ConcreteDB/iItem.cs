using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Model.ConcreteDB
{
    public interface iItem
    {        
        string Mark { get; }               
        string ItemGroup { get;  }
        short? Lenght { get;  }
        short? Height { get; }
        short? Thickness { get; }        
        short? Formwork { get; }
        string BalconyDoor { get; }
        string BalconyCut { get; }
        short? FormworkMirror { get; }
        string Electrics { get; }
        float? Weight { get; }
        float? Volume { get; }
    }
}
