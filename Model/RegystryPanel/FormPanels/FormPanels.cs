﻿using System;
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
        List<Panel> panels;
        public FormPanels(List<Panel> panels)
        {
            InitializeComponent();

            this.panels = panels;
            //panels.Sort((p1, p2) => p1.Mark.CompareTo(p2.Mark));

            FillListView(panels);

            listViewPanels.SelectedIndexChanged += ListView1_SelectedIndexChanged;

            //BindingSource bindSourcePanels = new BindingSource();
            //bindSourcePanels.DataSource = panels;
            
            //listIncorrectPanels.DataSource = bindSourcePanels;
            //listBoxIncorrectPanels.DisplayMember = "Mark";

            //textBoxInfo.DataBindings.Add("Text", bindSourcePanels, "Info");
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewPanels.SelectedItems.Count>0)
            {
                Panel p = listViewPanels.SelectedItems[0].Tag as Panel;
                textBoxInfo.Text = p.Info;
            }
            else
            {
                textBoxInfo.Text = string.Empty;
            }
        }

        private void FillListView(List<Panel> panels)
        {
            listViewPanels.Items.Clear();
            foreach (var p in panels)
            {
                var lwi = new ListViewItem(p.Mark);                
                lwi.Tag = p;
                lwi.SubItems.Add(p.ErrorName);
                listViewPanels.Items.Add(lwi);
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
            if (listViewPanels.SelectedItems.Count > 0)
            {
                Panel p = listViewPanels.SelectedItems[0].Tag as Panel;
                p.Show();                
            }
        }

        private void listViewPanels_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (ListViewItem item in listViewPanels.SelectedItems)
                {
                    listViewPanels.Items.Remove(item);
                    Panel p = item.Tag as Panel;
                    panels.Remove(p);
                }
            }
        }
    }
}
