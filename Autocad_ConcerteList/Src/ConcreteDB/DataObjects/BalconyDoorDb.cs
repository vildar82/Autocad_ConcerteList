using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class BalconyDoorDb :IEquatable<BalconyDoorDb>
    {
        public decimal BalconyDoorId { get; set; }
        public string BalconyDoorName { get; set; }

        public bool Equals(BalconyDoorDb other)
        {
            return BalconyDoorId == other.BalconyDoorId &&
                BalconyDoorName == other.BalconyDoorName;
        }

        public override int GetHashCode()
        {
            return BalconyDoorId.GetHashCode();
        }
    }
}
