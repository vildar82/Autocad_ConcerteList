using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    /// <summary>
    /// Наружная стеновая панель
    /// </summary>
    public class ExternalPanel : Panel
    {
        public ExternalPanel(MarkPart markPart, BlockReference blRef, string blName) : base(markPart, blRef, blName)
        {
        }

        public override short GetGab(string nameGabRu, short value, double factor)
        {
            if (value == 3018 && nameGabRu == HeightNameRu)
            {
                return 31;
            }
            return base.GetGab(nameGabRu, value, factor);
        }

        public override string GetHeightMarkPart(short heightFactor)
        {
            if (Height == 3018)
            {
                HeightMark = 31;
                return HeightMark.ToString();
            }
            return base.GetHeightMarkPart(heightFactor);
        }        
    }
}
