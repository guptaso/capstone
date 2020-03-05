using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;

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
                main.menu_control.Visibility = Visibility.Visible;
            }
        }
    }
}
