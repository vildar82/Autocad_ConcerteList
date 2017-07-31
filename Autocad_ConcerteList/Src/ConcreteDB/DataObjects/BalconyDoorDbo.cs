using System;

namespace Autocad_ConcerteList.ConcreteDB.DataObjects
{
    public class BalconyDoorDbo :IEquatable<BalconyDoorDbo>
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Side { get; internal set; }

        public bool Equals(BalconyDoorDbo other)
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
