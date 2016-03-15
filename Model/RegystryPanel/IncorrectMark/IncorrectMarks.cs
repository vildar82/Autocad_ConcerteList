using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.RegystryPanel
{
    public static class IncorrectMarks
    {
        /// <summary>
        /// Показ несоответствующих марок панелей
        /// </summary>        
        public static bool Show(List<Panel> incorrectPanels)
        {
            if (incorrectPanels == null || incorrectPanels.Count == 0)
            {
                return true;
            }

            FormPanels formIncorrect = new FormPanels(incorrectPanels);
            formIncorrect.Text = "Несоответствующие марки";
            formIncorrect.toolTip1.SetToolTip(formIncorrect.listBoxIncorrectPanels, "Марка панели из атрбибута блока отличается от марки сформированной по формуле из базы ЖБИ.");
            var dlgRes = formIncorrect.ShowDialog();            
            return dlgRes == System.Windows.Forms.DialogResult.OK ? true : false;
        }

        /// <summary>
        /// Исправленние панелей с некорректными марками на чертеже
        /// </summary>        
        public static void Fix(List<Panel> panelsIncorrectMark)
        {
            // Найти блоки со старой маркой и исправить на марку из базы.
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var ms = SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead) as BlockTableRecord;
                foreach (var idEnt in ms)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead, false, true) as BlockReference;
                    if (blRef == null || blRef.AttributeCollection == null) continue;

                    foreach (ObjectId idAtr in blRef.AttributeCollection)
                    {
                        var atrRef = idAtr.GetObject(OpenMode.ForRead, false, true) as AttributeReference;
                        if (atrRef.Tag.Equals("МАРКА", StringComparison.OrdinalIgnoreCase))
                        {
                            var panel = panelsIncorrectMark.FirstOrDefault(p =>
                                    p.Mark.Equals(atrRef.TextString, StringComparison.OrdinalIgnoreCase));
                            if (panel != null)
                            {
                                atrRef.UpgradeOpen();
                                atrRef.TextString = panel.MarkDb;
                            }
                            break;
                        }
                    }                                       
                }
                t.Commit();
            }
        }
    }
}
