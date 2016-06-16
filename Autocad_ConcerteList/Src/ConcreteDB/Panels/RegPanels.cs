﻿using System;
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
        public List<Panel> Panels { get; private set; }
        public List<Panel> PanelsNewWoErr { get; private set; }

        public RegPanels(List<Panel> panels)
        {
            Panels = panels;
        }

        public int Registry()
        {
            int regCount = 0;
            PanelsNewWoErr = Panels.Where(p => p.IsNew!=null && p.IsNew.Value && !p.HasErrors).ToList();

            var panelsNewWithErr = Panels.Where(p=>p.IsNew!=null && p.IsNew.Value && p.HasErrors);
            foreach (var item in panelsNewWithErr)
            {
                Inspector.AddError($"Новая панель с ошибками - {item.Mark}, {item.Warning}, {item.GetErrorStatusDesc()}",
                    item.IdBlRef, System.Drawing.SystemIcons.Error);
            }

            var groupedNewPanels = PanelsNewWoErr.GroupBy(p=>p.MarkWoSpace).OrderBy(o=>o.Key, AcadLib.Comparers.AlphanumComparator.New);
            List<KeyValuePair<Panel, List<Panel>>> newPanels = new List<KeyValuePair<Panel, List<Panel>>> ();

            RegPanelsViewModel model = new RegPanelsViewModel (newPanels, DbService.Series);
            WindowRegPanels winPanels = new WindowRegPanels(model, "Новые панели без ошибок");
            var dialogRes = Application.ShowModalWindow(winPanels);
            if (dialogRes.HasValue && dialogRes.Value)
            {
                var panels = groupedNewPanels.Select(s=>s.First()).ToList();
                DbService.Register(panels, model.SelectedSerie);                
            }
            else
            {
                throw new Exception(AcadLib.General.CanceledByUser);
            }

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (PanelsNewWoErr.Count == 0)
            {
                ed.WriteMessage($"\nНет новых панелей.");
            }            
            return regCount;
        }
    }
}
