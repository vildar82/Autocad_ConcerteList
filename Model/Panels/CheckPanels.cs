using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autocad_ConcerteList.Model.RegystryPanel.IncorrectMark;
using Autocad_ConcerteList.Model.RegystryPanel.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad_ConcerteList.Model.Panels
{
    /// <summary>
    /// Показ формы результата проверки панелей
    /// </summary>
    public class CheckPanels
    {
        /// <summary>
        /// Все найденные панели в чертеже
        /// </summary>
        public List<Panel> Panels { get; private set; }         

        public CheckPanels(List<Panel> panels)
        {
            Panels = panels;
        }

        public void Check()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            // Панели для показа в форме проверки - новые и с ошибками
            var checkPanels = Panels.Where(p => p.IsNew || p.ErrorStatus != EnumErrorItem.None || !p.IsCorrectBlockName).ToList();
            if (checkPanels.Count ==0)
            {                
                ed.WriteMessage($"\nНет новых панелей и нет панелей с ошибками.");                
            }
            else
            {
                //FormPanels panelForm = new FormPanels(chackPanels);
                //panelForm.Text = "Новые панели";
                //panelForm.SetGroupedPanels(true);

                //var errPanels = chackPanels.Where(p => p.ErrorStatus != EnumErrorItem.None || !p.IsCorrectBlockName);
                //if (errPanels.Any())
                //{
                //panelForm.BackColor = System.Drawing.Color.Red;
                //}

                //panelForm.buttonCancel.Visible = false;
                //panelForm.buttonOk.Visible = false;
                //Application.ShowModelessDialog(panelForm);
                WindowCheckPanels winPanels = new WindowCheckPanels(checkPanels);
                Application.ShowModalWindow(winPanels);
            }            
            ed.WriteMessage($"\nОбработано {Panels.Count} блоков панелей.");
        }        
    }
}
