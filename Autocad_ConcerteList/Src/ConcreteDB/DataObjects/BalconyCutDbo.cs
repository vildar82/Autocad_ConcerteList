using System;

namespace Autocad_ConcerteList.ConcreteDB.DataObjects
{
    public class BalconyCutDbo : IEquatable<BalconyCutDbo>
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string Side { get; internal set; }
        public int? Size { get; set; }

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
