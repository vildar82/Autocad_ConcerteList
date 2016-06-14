using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.DataObjects
{
    public class BalconyDoorDbo :IEquatable<BalconyDoorDbo>
    {
        public decimal BalconyDoorId { get; set; }
        public string BalconyDoorName { get; set; }

        public bool Equals(BalconyDoorDbo other)
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
