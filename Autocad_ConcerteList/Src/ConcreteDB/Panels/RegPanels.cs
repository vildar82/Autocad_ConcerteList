using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Src.ConcreteDB;
using Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels
{
    public class RegPanels
    {
        public List<IIPanel> Panels { get; private set; }
        public List<IIPanel> PanelsNewWoErr { get; private set; }

        public RegPanels(List<IIPanel> panels)
        {
            Panels = panels;
        }

        public void Registry()
        {            
            PanelsNewWoErr = Panels.Where(p => p.IsNew!=null && p.IsNew.Value && !p.HasErrors).ToList();

            var panelsNewWithErr = Panels.Where(p=>p.IsNew!=null && p.IsNew.Value && p.HasErrors);
            foreach (var item in panelsNewWithErr)
            {
                Inspector.AddError($"Новая панель с ошибками - {item.Mark}, {item.Warning}, {item.GetErrorStatusDesc()}",
                    item.IdBlRef, System.Drawing.SystemIcons.Error);
            }

            Inspector.ShowDialog();
            Inspector.Clear();

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (PanelsNewWoErr.Count == 0)
            {
                ed.WriteMessage($"\nНет новых панелей без ошибок.");
                return;
            }

            var groupedNewPanels = PanelsNewWoErr.GroupBy(p=>p.MarkWoSpace).OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New);
            var newPanels = new List<KeyValuePair<IIPanel, List<IIPanel>>> ();

            foreach (var item in groupedNewPanels)
            {
                newPanels.Add(new KeyValuePair<IIPanel, List<IIPanel>>(item.First(), item.ToList()));
            }

            RegPanelsViewModel model = new RegPanelsViewModel (newPanels, DbService.Series);
            WindowRegPanels winPanels = new WindowRegPanels(model);
            var dialogRes = Application.ShowModalWindow(winPanels);
            if (dialogRes.HasValue && dialogRes.Value)
            {                
                DbService.Register(model.PanelsToReg, model.SelectedSerie);                
            }
            else
            {
                throw new Exception(AcadLib.General.CanceledByUser);
            }                               
        }
    }
}
