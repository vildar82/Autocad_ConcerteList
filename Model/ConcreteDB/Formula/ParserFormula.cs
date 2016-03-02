using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace Autocad_ConcerteList.ConcreteDB.Formula
{
    public class ParserFormula
    {
        public string Formula { get; private set; }
        public string Result { get; private set; }
        public List<Eval> Evals { get; private set; }
        private ItemEntryData itemEntryData;        

        public ParserFormula(string formula, ItemEntryData itemEntryData)
        {
            Formula = formula;
            this.itemEntryData = itemEntryData;
        }

        public void Parse()
        {
            Evals = new List<Eval>();
            var evalsSplit = Formula.Split(';');
            foreach (var evalItem in evalsSplit)
            {
                Eval eval = new Eval(evalItem, itemEntryData);                
                Evals.Add(eval);
            }

            // Соединение значений по формуле
            Result = string.Join("", Evals).Replace("--", "-").Replace("--", "-").TrimEnd('-');
        }        
    }
}
