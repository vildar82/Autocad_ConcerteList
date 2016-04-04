using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autocad_ConcerteList.Model.RegystryPanel.IncorrectMark;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad_ConcerteList.Model.Panels
{
    public class CheckPanels
    {
        public List<Panel> Panels { get; private set; }
        public List<Panel> New { get; private set; }        

        public CheckPanels(List<Panel> panels)
        {
            Panels = panels;
        }

        public void Check()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            New = Panels.Where(p => p.DbItem == null).ToList();
            if (New.Count ==0)
            {                
                ed.WriteMessage($"\nНет новых панелей.");                
            }
            else
            {
                FormPanels panelForm = new FormPanels(New);
                panelForm.Text = "Новые панели";
                panelForm.SetGroupedPanels(true);

                var errPanels = New.Where(p => p.ErrorStatus != EnumErrorItem.None);
                if (errPanels.Any())
                {
                    panelForm.BackColor = System.Drawing.Color.Red;
                }

                panelForm.buttonCancel.Visible = false;
                panelForm.buttonOk.Visible = false;
                Application.ShowModelessDialog(panelForm);
            }            
            ed.WriteMessage($"\nОбработано {Panels.Count} блоков панелей.");
        }        
    }
}
