using System;

namespace Autocad_ConcerteList.ConcreteDB.Formula
{
    public class FormulaItem
    {
        public FormulaParameters FormulaParams { get; set; }
        public Func<FormulaParameters, IPanel, string> FormulaFunc { get; set; }
        public string GabKey { get; set; }

        public FormulaItem(Func<FormulaParameters, IPanel, string> func, FormulaParameters formulaParams, string gabKey)
        {
            FormulaFunc = func;
            FormulaParams = formulaParams;
            GabKey = gabKey;
        }
    }
}
