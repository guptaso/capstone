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
    public partial class MenuControl : UserControl
    {

        private MainWindow main;

        public MenuControl()
        {
            InitializeComponent();    
            main = ((MainWindow)App.Current.MainWindow);

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            titleBar.Visibility = Visibility.Hidden;
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
            titleBar.Visibility = Visibility.Hidden;
        }

        private void layout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)layout_box.SelectedItem;
            string name = layout_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            //Trace.WriteLine(name.ToString().Substring(35,38));
            string[] x = name.ToString().Split('x');
            string a = x[1];
            string b = "";
            if (x.Length > 2)
            {
                b = x[2];
                a = a.ToString().Split(' ')[1];
                main.grid.set_grid(Int16.Parse(a), Int16.Parse(b));
            }
        }
        
    }
}
