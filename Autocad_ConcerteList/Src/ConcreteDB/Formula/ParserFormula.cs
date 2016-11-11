using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;

namespace Autocad_ConcerteList.Src.ConcreteDB.FormulaEval
{
    public class ParserFormula
    {
        public short? Length { get; set; }
        public short? Height { get; set; }
        public short? Thickness { get; set; }
        public string Formula { get; private set; }
        public string Result { get; private set; }
        public List<Eval> Evals { get; private set; }
        private iItem item;        

        public ParserFormula(string formula, iItem item)
        {
            Formula = formula;
            this.item = item;
        }

        public void Parse()
        {
            Evals = new List<Eval>();
            var evalsSplit = Formula.Split(';');
            foreach (var evalItem in evalsSplit)
            {
                Eval eval = new Eval(evalItem, item);
                eval.Evaluate();           
                Evals.Add(eval);
                if (evalItem.Contains("Lenght"))
                {
                    Length =short.Parse(eval.ValueString);
                }
                else if (evalItem.Contains("Height"))
                {
                    Height = short.Parse(eval.ValueString);
                }
                else if (evalItem.Contains("Thickness"))
                {
                    Thickness = short.Parse(eval.ValueString);
                }                
            }

            // Соединение значений по формуле
            Result = string.Join("", Evals).Replace("--", "-").Replace("--", "-").TrimEnd('-');
        }        
    }
}
