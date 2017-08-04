using System;
using System.Collections.Generic;
using System.Linq;
using Autocad_ConcerteList.Lib;
using Autocad_ConcerteList.Lib.SpatialIndex;
using Autodesk.AutoCAD.DatabaseServices;

namespace Autocad_ConcerteList.ConcreteDB.Panels
{
    public class FilterPanel
    {
        public List<IPanel> Panels { get; private set; }        

        public void Filter()
        {
            var panels = new List<IPanel>();
            var ws = new List<Workspace>();
            var db = HostApplicationServices.WorkingDatabase;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var ms = db.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                foreach (var idEnt in ms)
                {
                    if (!idEnt.IsValidEx()) continue;
                    var panel = PanelFactory.Define(idEnt, out BlockReference blRef, out string blName);
                    if (panel != null)
                    {
                        //panel.Checks();
                        panels.Add(panel);
                    }
                    else if (blRef != null && blName.Equals(Options.Options.Instance.WorkspaceBlockName, StringComparison.OrdinalIgnoreCase))
                    {
                        ws.Add(new Workspace(blRef));
                    }
                }             
                t.Commit();
            }
            definePanelsWS(panels, ws);
            // Панели только в раб.областях., отсортированные по марке            
            Panels = panels.Where(p=> {
                if (p.WS != null)
                {
                    p.Checks();
                    return true;
                }
                else
                    return false;
            }).OrderBy(p=>p.Mark, NetLib.Comparers.AlphanumComparator.New).ToList();            
        }       

        private void definePanelsWS(List<IPanel> panels, List<Workspace> ws)
        {
            var rtree = new RTree<Workspace>();
            foreach (var w in ws)
            {
                var r = new Rectangle(w.Extents.MinPoint.X, w.Extents.MinPoint.Y,
                                        w.Extents.MaxPoint.X, w.Extents.MaxPoint.Y, 0, 0);
                rtree.Add(r, w);
            }

            foreach (var p in panels)
            {
                var pt = new Point(p.Position.X, p.Position.Y, 0);
                var w = rtree.Nearest(pt, 1).FirstOrDefault();
                p.WS = w;
            }
        }
    }
}
