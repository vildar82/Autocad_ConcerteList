using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override string CorrectLentghParseValue(string valueString)
        {
            if (Length == 7270)
            {
                return "72";
            }
            else if (Length == 7180)
            {
                return "71";
            }
            return valueString;
        }

        public override string CorrectHeightParseValue(string valueString)
        {
            if (Height == 1840)
            {
                return "19";
            }
            return valueString;
        }
    }
}
