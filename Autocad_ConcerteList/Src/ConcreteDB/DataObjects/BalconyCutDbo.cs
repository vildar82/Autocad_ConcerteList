using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class BalconyCutDbo : IEquatable<BalconyCutDbo>
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Side { get; internal set; }
        public Nullable<int> Size { get; set; }

        public bool Equals(BalconyCutDbo other)
        {
            return Id == other.Id &&
                Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
