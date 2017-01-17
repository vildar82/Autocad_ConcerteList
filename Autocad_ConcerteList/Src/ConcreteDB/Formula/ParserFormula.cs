using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Src.ConcreteDB.Panels;

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
        private iPanel item;        

        public ParserFormula(string formula, iPanel item)
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
                    if (evalItem.Contains(Panel.LengthNameEng))
                    {
                        eval.ValueString = item.CorrectLentghParseValue(eval.ValueString);
                        Length = short.Parse(eval.ValueString);
                    }
                    else if (evalItem.Contains(Panel.HeightNameEng))
                    {
                        eval.ValueString = item.CorrectHeightParseValue(eval.ValueString);                        
                        Height = short.Parse(eval.ValueString);                        
                    }
                    else if (evalItem.Contains(Panel.ThicknessNameEng))
                    {
                        eval.ValueString = item.CorrectHeightParseValue(eval.ValueString);
                        Thickness = short.Parse(eval.ValueString);
                    }
                }               
            }
            // Соединение значений по формуле
            Result = string.Join("", Evals).Replace("--", "-").Replace("--", "-").Replace(".-","-").TrimEnd('-').TrimEnd('.');
        }        
    }
}
