using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.RegystryPanel
{
    [Flags]
    public enum EnumErrorItem
    {
        None = 0x0, 
        /// <summary>
        /// Несоответствующая марка
        /// </summary>
        IncorrectMark = 0x1,
        /// <summary>
        /// Различные параматры. у панелй с одной маркой разные параметры (в атрибутах блока - например Длина, Высота, Масса, и т.п.)
        /// </summary>
        DifferentParams = 0x2
    }
}
