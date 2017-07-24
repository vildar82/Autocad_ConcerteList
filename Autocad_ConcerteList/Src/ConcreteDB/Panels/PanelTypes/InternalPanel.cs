using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
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
