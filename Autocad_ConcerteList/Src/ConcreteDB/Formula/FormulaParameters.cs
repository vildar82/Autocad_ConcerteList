namespace Autocad_ConcerteList.Src.ConcreteDB.Formula
{
    public class FormulaParameters
    {   
        public FormulaParameters(short lengthFactor, short heightFactor, short thicknessFactor)
        {
            LengthFactor = lengthFactor;
            HeightFactor = heightFactor;
            ThicknessFactor = thicknessFactor;
        }

        public short LengthFactor { get; set; }
        public short HeightFactor { get; set; }
        public short ThicknessFactor { get; set; }
    }
}
