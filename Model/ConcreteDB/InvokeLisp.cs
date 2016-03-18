using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace Autocad_ConcerteList.ConcreteDB
{
    public static class InvokeLisp
    {
        public static void CreateBlock(ItemEntryData itemEntryData)
        {
            // Формирование списка с парметрами панели для передачи в лисп функцию
            ResultBuffer rb = new ResultBuffer(
               new TypedValue[]
               {
                   new TypedValue((int)LispDataType.Text, "TestLispFunc"),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Mark\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, "\"" + itemEntryData.Mark +"\""),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text,"\"ItemGroup\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, "\"" + itemEntryData.ItemGroup +"\""),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text,"\"Length\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Int16, itemEntryData.Lenght),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text,"\"Height\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Int16, itemEntryData.Height),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Thickness\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Int16, itemEntryData.Thickness),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Formwork\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, itemEntryData.Formwork),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"BalconyDoor\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, "\""+itemEntryData.BalconyDoor+"\""),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"BalconyCut\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, "\""+itemEntryData.BalconyCut+"\""),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"FormworkMirror\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, itemEntryData.FormworkMirror),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Electrics\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, "\""+itemEntryData.Electrics+"\""),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Weight\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Text, itemEntryData.Weight),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListBegin),
                   new TypedValue((int)LispDataType.Text, "\"Volume\""),
                   new TypedValue((int)LispDataType.DottedPair),
                   new TypedValue((int)LispDataType.Double, itemEntryData.Volume),
                   new TypedValue((int)LispDataType.ListEnd),
                   new TypedValue((int)LispDataType.ListEnd)
               }
            );

            // Выполнение лисп-функции            
            var res = Application.Invoke(rb);

            // Проверить результат выполнения функции res = 1 - Ok, !=1 Ошибка.                 

            int intRes = int.TryParse(res.AsArray().FirstOrDefault().Value?.ToString(), out intRes) ? intRes : 0;
            if (intRes != 1)
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                doc.Editor.WriteMessage($"\nОшибочный результат вызова лисп функции - {intRes}.");
            }
        }

        /// <summary>
        /// Вызов лисп функции сбора блоков.
        /// </summary>
        /// <returns>Список блоков и их параметров</returns>
        public static ResultBuffer GetRbPanels()
        {
            ResultBuffer rb = new ResultBuffer(
               new TypedValue[]
               {
                   new TypedValue((int)LispDataType.Text, "test_check_blocks")
               });
            try
            {
                return Application.Invoke(rb);
            }
            catch (System.Exception ex)
            {
                string msg = $"Ошибка вызова функции (test_check_blocks) - {ex.Message}";                
                throw new System.Exception(msg);                
            }            
        }
    }
}
