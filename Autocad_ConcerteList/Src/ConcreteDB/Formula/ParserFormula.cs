using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

namespace Autocad_ConcerteList.Src.ConcreteDB.Formula
{
    public class ParserFormula
    {        
        public FormulaItem Formula { get; set; }
        /// <summary>
        /// Результативная марка
        /// </summary>
        public string Result { get; private set; }        
        private IIPanel item;        

        public ParserFormula(FormulaItem formula, IIPanel item)
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
