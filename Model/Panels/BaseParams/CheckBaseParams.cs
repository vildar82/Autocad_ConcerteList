using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autocad_ConcerteList.Model.RegystryPanel;
using Autodesk.AutoCAD.ApplicationServices;

namespace Autocad_ConcerteList.Model.Panels.BaseParams
{
    public static class CheckBaseParams
    {
        public static void Check(List<Panel> panels)
        {
            bool showForm = false;
            DataTable dtBDoor = getDataTableBDoor();
            var unknownBDoor = panels.Where(p => (!string.IsNullOrEmpty(p.BalconyDoor) && p.BalconyDoorId == null)).GroupBy(g=>g.BalconyDoor);
            if (unknownBDoor.Any())
            {
                showForm = true;
                foreach (var item in unknownBDoor)
                {
                    var doorRow = new BalconyDoorRow(item);
                    var r = dtBDoor.NewRow();                    
                    r[0] = doorRow;
                    //r[2] = doorRow;
                    dtBDoor.Rows.Add(doorRow);
                }                
            }

            DataTable dtBCut = getDataTableBCut();
            var unknownBCut = panels.Where(p => (!string.IsNullOrEmpty(p.BalconyCut) && p.BalconyCutId == null)).GroupBy(g => g.BalconyCut);
            if (unknownBCut.Any())
            {
                showForm = true;
                foreach (var item in unknownBCut)
                {
                    var cutRow = new BalconyCutRow(item);
                    var r = dtBCut.NewRow();
                    r[0] = cutRow;                    
                    //r[3] = cutRow;
                    dtBCut.Rows.Add(cutRow);
                }
            }

            if (showForm)
            {
                FormBaseParams formParams = new FormBaseParams(dtBDoor, dtBCut);
                if (Application.ShowModalDialog(formParams) != System.Windows.Forms.DialogResult.OK)
                {
                    Application.ShowModelessDialog(formParams);
                    throw new System.Exception(AcadLib.General.CanceledByUser);
                }
            }
        }

        private static DataTable getDataTableBDoor()
        {
            DataTable dtBDoor = new DataTable("Балколны");
            dtBDoor.Columns.Add("Имя").ReadOnly = true;
            dtBDoor.Columns.Add("Сторона");
            //dtBDoor.Columns.Add("Object").ColumnMapping = MappingType.Hidden;
            return dtBDoor;
        }

        private static DataTable getDataTableBCut()
        {
            DataTable dtBCut = new DataTable("Подрезки");
            dtBCut.Columns.Add("Имя").ReadOnly=true;
            dtBCut.Columns.Add("Сторона");
            dtBCut.Columns.Add("Ширина");
            //dtBCut.Columns.Add("Object").ColumnMapping = MappingType.Hidden;
            return dtBCut;
        }
    }
}
