using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Autocad_ConcerteList.Errors.UI
{
    /// <summary>
    /// Логика взаимодействия для WindowErrors.xaml
    /// </summary>
    public partial class ErrorsView : Window
    {
        public ErrorsView(ErrorsViewModel errVM)
        {                        
            InitializeComponent();
            DataContext = errVM;
            KeyDown += ErrorsView_KeyDown;
        }

        public void DragWindow(object sender, MouseButtonEventArgs args)
        {
            DragMove();
        }

        private void ErrorsView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key ==  Key.Delete)
            {
                var model = DataContext as ErrorsViewModel;
                model.DeleteSelectedErrors();
            }
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer && !e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                ((Control)sender).RaiseEvent(eventArg);                
            }
        }        

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }        

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
