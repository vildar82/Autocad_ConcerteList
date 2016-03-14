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
    public partial class FormIncorrectMarks : Form
    {
        public FormIncorrectMarks(List<Autocad_ConcerteList.RegystryPanel.Panel> incorrectPanels)
        {
            InitializeComponent();

            BindingSource binPanels = new BindingSource();
            binPanels.DataSource = incorrectPanels;

            listBoxIncorrectPanels.DataSource = binPanels;
            listBoxIncorrectPanels.DisplayMember = "IncorrectMarkTitle";

            textBoxInfo.DataBindings.Add("Text", binPanels, "Info");
        }
    }
}
