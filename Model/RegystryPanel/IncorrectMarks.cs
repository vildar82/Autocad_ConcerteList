using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Model.RegystryPanel
{
    public static class IncorrectMarks
    {
        ///// <summary>
        ///// Исправленние панелей с некорректными марками на чертеже
        ///// </summary>        
        //public static void Fix(List<Panel> panelsIncorrectMark)
        //{
        //    // Найти блоки со старой маркой и исправить на марку из базы.
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    using (var t = db.TransactionManager.StartTransaction())
        //    {
        //        var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
        //        foreach (var idEnt in ms)
        //        {
        //            var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
        //            if (blRef == null || blRef.AttributeCollection == null) continue;

        //            foreach (ObjectId idAtr in blRef.AttributeCollection)
        //            {
        //                var atrRef = idAtr.GetObject(OpenMode.ForRead, false, true) as AttributeReference;
        //                if (atrRef.Tag.Equals("МАРКА", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    var panel = panelsIncorrectMark.FirstOrDefault(p =>
        //                            p.Mark.Equals(atrRef.TextString, StringComparison.OrdinalIgnoreCase));
        //                    if (panel != null)
        //                    {
        //                        atrRef.UpgradeOpen();
        //                        atrRef.TextString = panel.MarkDb;
        //                        panel.Mark = panel.MarkDb;
        //                        panel.ErrorStatus = EnumErrorItem.None;
        //                    }
        //                    break;
        //                }
        //            }                                       
        //        }
        //        t.Commit();
        //    }
        //}
    }
}
