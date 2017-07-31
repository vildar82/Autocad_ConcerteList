namespace Autocad_ConcerteList.ConcreteDB.DataObjects
{
    public class SerieDbo
    {
        public int SeriesId { get; set; }        
        public string Name { get; set; }

        public override string ToString ()
        {
            return Name;
        }
    }
}
