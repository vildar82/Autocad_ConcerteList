using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Autocad_ConcerteList.RegystryPanel.IncorrectMark
{
    public partial class FormPanels : Form
    {
        public FormPanels(List<Panel> panels)
        {
            InitializeComponent();

            //panels.Sort((p1, p2) => p1.Mark.CompareTo(p2.Mark));

            FillListView(panels);

            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;

            //BindingSource bindSourcePanels = new BindingSource();
            //bindSourcePanels.DataSource = panels;
            
            //listIncorrectPanels.DataSource = bindSourcePanels;
            //listBoxIncorrectPanels.DisplayMember = "Mark";

            //textBoxInfo.DataBindings.Add("Text", bindSourcePanels, "Info");
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count>0)
            {
                Panel p = listView1.SelectedItems[0].Tag as Panel;
                textBoxInfo.Text = p.Info;
            }
            else
            {
                textBoxInfo.Text = string.Empty;
            }
        }

        private void FillListView(List<Panel> panels)
        {
            listView1.Items.Clear();
            foreach (var p in panels)
            {
                var lwi = new ListViewItem(p.Mark);                
                lwi.Tag = p;
                lwi.SubItems.Add(p.ErrorName);
                listView1.Items.Add(lwi);
            }
        }

        public void SetSeries(List<Model.ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow> series,
            Model.ConcreteDB.DataSet.ConcerteDataSet.I_C_SeriesRow selected)
        {
            labelSer.Visible = true;
            comboBoxSer.Visible = true;
            comboBoxSer.DataSource = series;
            comboBoxSer.DisplayMember = "Series";
            comboBoxSer.SelectedItem = selected;
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Panel p = listView1.SelectedItems[0].Tag as Panel;
                p.Show();                
            }
        }
    }
}
