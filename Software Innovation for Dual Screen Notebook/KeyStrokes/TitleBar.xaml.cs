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

        public TitleBar()
        {
            InitializeComponent();          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).Close();
        }
    }
}
