using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Autodesk.AutoCAD.ApplicationServices;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCheckPanels.xaml
    /// </summary>
    public partial class WindowCheckPanels : Window
    {
        public WindowCheckPanels(CheckPanelsViewModel model)
        {            
            InitializeComponent();            
            var dataModel = model;            
            DataContext = dataModel;
            Title = model.Title;
        }

        private void btnExpandCollapse_Click_1 (object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the Selected Row Button Object
                Button ExpandCollapseObj = (Button)sender;

                // Check the Button Object is null or Not
                if (ExpandCollapseObj != null)
                {
                    // Return the Contains which specified element
                    DataGridRow DgrSelectedRowObj = DataGridRow.GetRowContainingElement(ExpandCollapseObj);

                    // Check the DataGridRow Object is Null or Not
                    if (DgrSelectedRowObj != null)
                    {
                        // if Button Content is "+" then Visible Row Details 
                        if (ExpandCollapseObj != null && ExpandCollapseObj.Content.ToString() == "+")
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Visible;
                            ExpandCollapseObj.Content = "-";
                        }
                        // else Collapsed row Details
                        else
                        {
                            DgrSelectedRowObj.DetailsVisibility = System.Windows.Visibility.Collapsed;
                            ExpandCollapseObj.Content = "+";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
