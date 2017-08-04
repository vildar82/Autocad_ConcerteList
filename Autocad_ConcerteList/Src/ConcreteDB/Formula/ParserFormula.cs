namespace Autocad_ConcerteList.ConcreteDB.Formula
{
    public class ParserFormula
    {        
        public FormulaItem Formula { get; set; }
        /// <summary>
        /// Результативная марка
        /// </summary>
        public string Result { get; private set; }        
        private readonly IPanel item;        

        public ParserFormula(FormulaItem formula, IPanel item)
        {
            Formula = formula;
            this.item = item;
        }

        public void Parse()
        {
            Result = Formula.FormulaFunc(Formula.FormulaParams, item).Replace("--", "-").Replace("--", "-").Replace(".-", "-").TrimEnd('-').TrimEnd('.');            
        }        
    }
}
