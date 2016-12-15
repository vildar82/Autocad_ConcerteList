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
        /// <summary>
        /// Результативная марка
        /// </summary>
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
                if (!string.IsNullOrEmpty(eval.ValueString))
                {
                    if (evalItem.Contains("Length"))
                    {
                        Length = short.Parse(eval.ValueString);
                    }
                    else if (evalItem.Contains("Height"))
                    {
                        Height = short.Parse(eval.ValueString);
                        if (item.Height == 3018 && item.IsExteriorWall)
                        {
                            eval.ValueString = "31";
                            Height = 31;
                        }
                        else if (item.Height == 2790 && item.IsInnerWall)
                        {
                            eval.ValueString = "29";
                            Height = 29;
                        }
                    }
                    else if (evalItem.Contains("Thickness"))
                    {
                        Thickness = short.Parse(eval.ValueString);
                    }
                }               
            }
            // Соединение значений по формуле
            Result = string.Join("", Evals).Replace("--", "-").Replace("--", "-").Replace(".-","-").TrimEnd('-').TrimEnd('.');
        }        
    }
}
