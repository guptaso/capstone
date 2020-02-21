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
    public partial class ButtonGridControl : UserControl
    {
        private List<Button> buttonList;
        private int col = 0;
        private int row = 0;

        public ButtonGridControl()
        {
            InitializeComponent();

            buttonList = new List<Button>();

            addButton("+", Add_Click);
        }

        public void addButton(String content, Action<object, RoutedEventArgs> click)
        {
            var template = (ControlTemplate)buttonGrid.FindResource("button");
            Button b = new Button { Template = template };
            
            b.Content = content;
            b.Width = Double.NaN;

            // set the click handler
            b.Click += click.Invoke;
            
            // option to remove the button
            //newButton.RightTapped += async (s, en) =>
            b.MouseDown += async (s, en) =>
            {
                MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        grid.Children.Remove(b);
                        break;
                }
            };


            // change this to actaully set the col and row
            Grid.SetColumn(b, col++);
            Grid.SetRow(b, row);
            buttonList.Add(b);
            grid.Children.Add(b);
        }

        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
        }
    }
}
