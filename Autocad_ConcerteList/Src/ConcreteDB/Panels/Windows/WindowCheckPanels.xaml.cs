using System;
using System.Windows;
using System.Windows.Controls;

namespace Autocad_ConcerteList.ConcreteDB.Panels.Windows
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
                var ExpandCollapseObj = (Button)sender;

                // Check the Button Object is null or Not
                if (ExpandCollapseObj != null)
                {
                    // Return the Contains which specified element
                    var DgrSelectedRowObj = DataGridRow.GetRowContainingElement(ExpandCollapseObj);

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
