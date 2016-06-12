using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class BalconyCutDb : IEquatable<BalconyCutDb>
    {
        public decimal BalconyCutId { get; set; }
        public string BalconyCutName { get; set; }
        public Nullable<int> BalconyCutSize { get; set; }

        public bool Equals(BalconyCutDb other)
        {
            return BalconyCutId == other.BalconyCutId &&
                BalconyCutName == other.BalconyCutName;
        }

        public override int GetHashCode()
        {
            return BalconyCutId.GetHashCode();
        }
    }
}
