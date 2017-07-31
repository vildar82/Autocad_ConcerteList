using Autocad_ConcerteList.ConcreteDB.Panels.ParsersMark;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.ConcreteDB.Panels.PanelTypes
{
    /// <summary>
    /// Внутренние стеновые панели
    /// </summary>
    public class InternalPanel : Panel
    {
        public InternalPanel(MarkPart markPart, BlockReference blRef, string blName) : base(markPart, blRef, blName)
        {
			
        }

	    public override short GetGab(string nameGabRu, short value, double factor)
        {
            if (value == 2790 && nameGabRu == HeightNameRu)
            {
                return 29;
            }
            return base.GetGab(nameGabRu, value, factor);
        }

        public override string GetHeightMarkPart(short heightFactor)
        {
            if (Height == 2790)
            {
                HeightMark = 29;
                return HeightMark.ToString();
            }
            return base.GetHeightMarkPart(heightFactor);
        }        
    }
}
