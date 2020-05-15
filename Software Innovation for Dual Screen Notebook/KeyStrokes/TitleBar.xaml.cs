using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeyStrokes
{
    public partial class TitleBar : UserControl
    {
        private MainWindow main;

        public TitleBar()
        {
            InitializeComponent();
            main = ((MainWindow)App.Current.MainWindow);
        }

        private void titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main.Activate();
            main.DragMove();
        }


        private void close_Click(object sender, RoutedEventArgs e)
        {
            main.Close();
        }

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            if (main.WindowState == WindowState.Normal)
            {
                main.WindowState = WindowState.Maximized;
            }
            else
            {
                main.WindowState = WindowState.Normal;
            }
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            main.WindowState = WindowState.Minimized;
        }

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            if (main.menu_control.IsVisible)
            {
                main.menu_control.Visibility = Visibility.Hidden;
            }
            else
            {
                main.Activate();
                main.menu_control.Visibility = Visibility.Visible;
            }
        }
    }
}
