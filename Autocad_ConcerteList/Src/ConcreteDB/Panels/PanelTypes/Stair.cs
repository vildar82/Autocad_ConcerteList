using Autocad_ConcerteList.ConcreteDB.Panels.ParsersMark;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.ConcreteDB.Panels.PanelTypes
{
    public class Stair : Panel
    {
        private bool isStair1050;

        public Stair(MarkPart markPart, BlockReference blRef, string blName) : base(markPart, blRef, blName)
        {
            IsIgnoreGab = true;
        }

        protected override void BeforeDefineMarkBuFormula()
        {
            if (Height == 1050)
            {
                isStair1050 = true;
                Height = null;
            }
        }

        protected override void AfterDefineMarkByFormula()
        {
            if (isStair1050)
            {
                Height = 1050;
            }
        }
    }
}
