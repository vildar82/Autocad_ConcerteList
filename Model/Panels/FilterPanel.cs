using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Errors;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.Model.Panels
{
    public class FilterPanel
    {
        public List<Panel> Panels { get; private set; }

        public static List<Panel> Filter(List<ObjectId> sel)
        {
            List<Panel> panels = new List<Panel>();
            foreach (var item in sel)
            {
                DefinePanel(item,ref panels);
            }
            return panels;
        }

        public void Filter()
        {
            var panels = new List<Panel>();
            var ws = new List<Workspace>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (var ms = db.CurrentSpaceId.Open(OpenMode.ForRead) as BlockTableRecord)
            {
                foreach (var idEnt in ms)
                {
                    var p = DefinePanel(idEnt, ref panels);
                    if (p.BlockName == Options.Instance.WorkspaceBlockName)
                    {
                        Workspace w = new Workspace(idEnt);
                        ws.Add(w);
                    }
                }
            }
            // Определение рабочих областей для панелей.
            //definePanelsWS(panels, ws);
            Panels = panels.OrderBy(p=>p.Mark).ToList();
        }

        private static Panel DefinePanel(ObjectId idEnt, ref List<Panel> panels)
        {
            AcadLib.Result res;
            Panel p = new Panel();
            try
            {
                res = p.Define(idEnt);
                if (res.Success)
                {
                    p.Check();
                    panels.Add(p);
                    return p;
                }                
            }
            catch (Exception ex)
            {
                res = AcadLib.Result.Fail(ex.Message);
            }
            if (!string.IsNullOrEmpty(res.Error))
            {
                Inspector.AddError($"Отфильтрован блок {p.BlockName} - {res.Error}.", idEnt, System.Drawing.SystemIcons.Exclamation);
            }
            return p;
        }

        private void definePanelsWS(List<Panel> panels, List<Workspace> ws)
        {
            RTreeLib.RTree<Workspace> rtree = new RTreeLib.RTree<Workspace>();
            foreach (var w in ws)
            {
                RTreeLib.Rectangle r = new RTreeLib.Rectangle(w.Extents.MinPoint.X, w.Extents.MinPoint.Y,
                                        w.Extents.MaxPoint.X, w.Extents.MaxPoint.Y, 0, 0);
                rtree.Add(r, w);
            }

            foreach (var p in panels)
            {
                RTreeLib.Point pt = new RTreeLib.Point(p.Position.X, p.Position.Y, 0);
                var w = rtree.Nearest(pt, 1).FirstOrDefault();
                p.WS = w;
            }
        }
    }
}
