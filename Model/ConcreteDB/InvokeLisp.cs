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
        public static void CreateBlock(Model.ConcreteDB.DataSet.ConcerteDataSet.I_R_ItemRow itemRow)
        {
            // Формирование списка с парметрами панели для передачи в лисп функцию
            ResultBuffer rb = new ResultBuffer(
               new TypedValue[]
               {
                  new TypedValue ((int)LispDataType.Text, "TestLispFunc"),
                  new TypedValue ((int)LispDataType.ListBegin, -1),
                  new TypedValue ((int)LispDataType.Text, itemRow.HandMark),
                  new TypedValue ((int)LispDataType.Int32, 2000)
               }
            );

            var res = Application.Invoke(rb);
        }
    }
}
