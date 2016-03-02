using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autocad_ConcerteList.ConcreteDB.Formula
{
    public class Eval
    {
        public string EvalString { get; set; }
        public string ValueString { get; set; }
        public bool IsEvaluable { get; set; }
        private ItemEntryData itemEntryData;
        private static DataTable t = new DataTable();
        private static char[] charOperands = new char[] { '/', '*', '-', '+' };

        // Публичные свойства в классе ItemEntryData        
        private static Dictionary<string, PropertyInfo> dictItemProperties = typeof(ItemEntryData)
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .ToDictionary(f => f.Name);

        private static Dictionary<string, string> dictEvalProps = new Dictionary<string, string>()
        {
            { "dbo.I_S_ItemGroup.ItemGroup", "ItemGroup" },
            { "dbo.I_R_Item.Lenght","Length" },
            { "dbo.I_R_Item.Height","Height" },
            { "dbo.I_R_Item.Thickness","Thickness" },
            { "dbo.I_R_Item.Formwork","Formwork" },
            { "dbo.I_S_BalconyDoor.BalconyDoor","BalconyDoor" },
            { "dbo.I_S_BalconyCut.BalconyCut","BalconyCut" },
            { "dbo.I_R_Item.FormworkMirror","FormworkMirror" },
            { "dbo.I_R_Item.Electrics","Electrics" }            
        };        

        public Eval (string eval, ItemEntryData itemEntryData)
        {   
            EvalString = eval;
            this.itemEntryData = itemEntryData;

            IsEvaluable = eval.StartsWith("'");
            if (IsEvaluable)
            {
                ValueString = eval.Replace("'", "");
            }
            else
            {
                // Определение поля в объекте ItemEntryData и получение его значения
                ValueString = getValue(eval);                
            }            
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
                string fieldName;
                if (dictEvalProps.TryGetValue(itemOperand, out fieldName))
                {
                    string fieldValue = getFieldValue(fieldName);
                    evaluate = evaluate.Replace(itemOperand, fieldValue);
                }                
            }                        

            // Вычисление
            if (evaluate.IndexOfAny(charOperands) !=-1)
            {
                resVal = t.Compute(evaluate, null)?.ToString();
            }
            else
            {
                resVal = evaluate;
            }      

            if (string.IsNullOrEmpty(resVal) && evalEnter.StartsWith("*"))
            {
                resVal = defaultEvalValue;
            }

            return resVal;
        }

        private string getFieldValue(string fieldName)
        {   
            PropertyInfo property;            
            if (dictItemProperties.TryGetValue(fieldName, out property))
            {
                return property.GetValue(itemEntryData)?.ToString();
            }
            else
            {
                // Ошибка. Параметр из формулы не найден в списке параметров объекта ItemEntryData
                throw new Exception($"Ошибка формирования марки панели по формуле. Не определен параметр {fieldName}.");
            }
        }

        public override string ToString()
        {
            return ValueString;
        }
    }
}
