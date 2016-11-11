using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    /// <summary>
    /// Модификатор - Подрезка, Балкон, Закладная, 
    /// </summary>
    public class ModificatorDbo
    {
        public string Code { get;  set; }
        public int Id { get; set; }
        public int? Measure { get; set; }
        public string Side { get;  set; }
        public string TypeModificator { get;  set; }
    }
}
