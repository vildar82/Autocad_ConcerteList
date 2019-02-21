using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// ПЛ
    /// </summary>
    public class PL : Panel
    {
        public PL(MarkPart markPart, BlockReference blRef, string blName) : base(markPart, blRef, blName)
        {
        }

        public override short GetGab(string nameGabRu, short value, double factor)
        {
            if (value == 7270 && nameGabRu == LengthNameRu)
            {
                return 72;
            }
            else if (value == 7180 && nameGabRu == LengthNameRu)
            {
                return 71;
            }
            else if (Height == 1840 && nameGabRu == HeightNameRu)
            {
                return 19;
            }
            return base.GetGab(nameGabRu, value, factor);
        }

        public override string GetLengthMarkPart(short lengthFactor)
        {
            if (Length == 7270)
            {
                LengthMark = 72;
                return LengthMark.ToString();
            }
            else if (Length == 7180)
            {
                LengthMark = 71;
                return LengthMark.ToString();                
            }
            return base.GetLengthMarkPart(lengthFactor);
        }

        public override string GetHeightMarkPart(short heightFactor)
        {
            if (Height == 1840)
            {
                HeightMark = 19;
                return HeightMark.ToString();
            }
            return base.GetHeightMarkPart(heightFactor);
        }        
    }
}
