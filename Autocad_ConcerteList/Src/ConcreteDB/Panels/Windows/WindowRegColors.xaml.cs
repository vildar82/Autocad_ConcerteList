using System.Windows;

namespace Autocad_ConcerteList.Src.ConcreteDB.Panels.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowRegColors.xaml
    /// </summary>
    public partial class WindowRegColors : Window
    {
        public WindowRegColors ()
        {
            InitializeComponent();
        }        

        private void Registry (object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
        }
    }
}
