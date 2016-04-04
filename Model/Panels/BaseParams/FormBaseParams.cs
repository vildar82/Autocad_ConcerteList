using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autocad_ConcerteList.Model.Panels.BaseParams
{
    public partial class FormBaseParams : Form
    {
        public FormBaseParams(DataTable bDoor, DataTable bCut)
        {
            InitializeComponent();
            dataGridViewBalconyDoor.DataSource = bDoor;
            dataGridViewBalconyCut.DataSource = bCut;
        }

        private void dataGridViewBalconyDoor_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewBalconyDoor.SelectedRows.Count>0)
            {
                var row = dataGridViewBalconyDoor.SelectedRows[0];                
            }
        }

        private void dataGridViewBalconyCut_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewBalconyCut.SelectedRows.Count > 0)
            {
                var row = dataGridViewBalconyCut.SelectedRows[0];
            }
        }

        private void dataGridViewBalconyDoor_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridViewBalconyDoor[e.ColumnIndex, e.RowIndex];
        }

        private void dataGridViewBalconyCut_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dataGridViewBalconyCut[e.ColumnIndex, e.RowIndex];
        }
    }
}
