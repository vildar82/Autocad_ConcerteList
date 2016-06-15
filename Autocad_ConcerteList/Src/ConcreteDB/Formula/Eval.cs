using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.Src.ConcreteDB.Formula
{
    public class Eval
    {
        public string EvalString { get; set; }
        public string ValueString { get; set; }
        public bool IsEvaluable { get; set; }
        private iItem item;
        private static DataTable t = new DataTable();
        private static char[] charOperands = new char[] { '/', '*', '-', '+' };

        // Публичные свойства в классе ItemEntryData        
        private static Dictionary<string, PropertyInfo> dictItemProperties = typeof(iItem)
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .ToDictionary(f => f.Name);        

        public Eval (string eval, iItem item)
        {   
            EvalString = eval;
            this.item = item;
            IsEvaluable = eval.StartsWith("'");                  
        }

        public string Evaluate()
        {
            if (IsEvaluable)
            {
                ValueString = EvalString.Replace("'", "");
            }
            else
            {
                // Определение поля в объекте ItemEntryData и получение его значения
                ValueString = getValue(EvalString);
            }
            return ValueString;
        }

        private string getValue(string evalEnter)
        {
            // На входе, например - "dbo.I_R_Item.Lenght / 10". Получить "75"
            var resVal = string.Empty;

            // Значение поумолчанию заданное в строке формулы для этого параметра - в конце ='0'
            string defaultEvalValue = string.Empty;
            var splitEqually = evalEnter.Split('=');
            string evaluate = splitEqually[0];
            if (splitEqually.Length > 1)
            {
                // Дефолтное значение заданное в формуле
                defaultEvalValue = splitEqually[1].Replace("'", "").Trim();
            }

            // Подстановка значений вместо параметров
            var splitByOperators = evaluate.Split(charOperands).Select(i=>i.Trim());
            foreach (var itemOperand in splitByOperators)
            {
                if (itemOperand.Contains("dbo"))
                {
                    string fieldName = itemOperand.Split('.').Last().Trim();
                    string fieldValue = getFieldValue(fieldName);
                    if (fieldValue == null)
                    {
                        evaluate = string.Empty;
                        break;
                    }
                    evaluate = evaluate.Replace(itemOperand, fieldValue);
                }                      
            }                        

            // Вычисление
            if (evaluate.IndexOfAny(charOperands) !=-1)
            {
                var objRes = t.Compute(evaluate, null);
                resVal = GetRoundValue(objRes).ToString();
            }
            else
            {
                resVal = evaluate;
            }      

            if (string.IsNullOrEmpty(resVal))
            {
                // @ - перед выражением, означает, что значение не может быть пустым.                
                if (evalEnter.StartsWith("@"))
                {
                    // Если нет @ вначале, то значение может быть пустым, а если в конце выражения стоит =, то это дефолтное значение, которое нужно подставить
                    resVal = defaultEvalValue;
                }
                else
                {
                    // Ошибка - должно быть значение.
                    throw new Exception($"Пустое значение выражения - {evalEnter}");
                }
            }

            return resVal;
        }

        /// <summary>
        /// Округление вычисленного значения в формуле
        /// </summary>
        /// <param name="objRes"></param>
        /// <returns></returns>
        public static short GetRoundValue(object objRes)
        {
            return Convert.ToInt16(objRes);
        }

        private string getFieldValue(string fieldName)
        {   
            PropertyInfo property;            
            if (dictItemProperties.TryGetValue(fieldName, out property))
            {
                return property.GetValue(item)?.ToString();
            }
            else
            {
                // Ошибка. Параметр из формулы не найден в списке параметров объекта ItemEntryData
                throw new Exception($"Ошибка формирования марки панели по формуле. Не определен параметр {fieldName}");
            }
        }

        public override string ToString()
        {
            return ValueString;
        }
    }
}
