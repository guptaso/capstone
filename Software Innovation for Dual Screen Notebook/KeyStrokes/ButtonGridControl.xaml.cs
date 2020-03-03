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
        private Button hold;
        public ButtonGridControl()
        {
            InitializeComponent();

            buttonList = new List<Button>();

            //addButton("+", Add_Click);
        }

        public void addButton(String content, Action<object, RoutedEventArgs> click)
        {
            var template = (ControlTemplate)buttonGrid.FindResource("button");
            Button b = new Button { Template = template };

            b.Content = content;
            b.Width = Double.NaN;
            b.MaxHeight = 50;
            b.MaxWidth = 100;

            // set the click handler
            b.Click += click.Invoke;

            // option to remove the button
            //newButton.RightTapped += async (s, en) =>
            b.MouseDown += async (s, en) =>
            {

                /* MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesRemoveNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:

                        // remove button
                        grid.Children.Remove(b);
                        buttonList.Clear();
                        foreach(Button buttonItem in grid.Children)
                        {
                            if (buttonItem is FrameworkElement)
                            {
                                buttonList.Add(buttonItem);
                            }
                        }
                        
                        break;
                }
                */
                btnMenu.Visibility = Visibility.Visible;
                hold = b;

            };

            b.MouseDoubleClick += async (s, en) =>
            {
                btnMenu.Visibility = Visibility.Visible;
                hold = b;
            };


            // change this to actaully set the col and row
            //Grid.SetColumn(b, col++);
            //Grid.SetRow(b, row);

            // add button to the list
            buttonList.Add(b);

            //grid.Children.Add(b);

            // re-adds the buttons from the list 
            grid.Children.Clear();
            foreach (Button buttonItem in buttonList)
            {
                grid.Children.Add(buttonItem);
            }
        }

        public void removeBtn(object sender, RoutedEventArgs e)
        {
            // remove button
            grid.Children.Remove(hold);
            buttonList.Clear();
            foreach (Button buttonItem in grid.Children)
            {
                if (buttonItem is FrameworkElement)
                {
                    buttonList.Add(buttonItem);
                }
            }

            btnMenu.Visibility = Visibility.Hidden;
        }

        public void okBtn(object sender, RoutedEventArgs e)
        {
            rName.Visibility = Visibility.Visible;
            rowIn.Visibility = Visibility.Visible;
            cName.Visibility = Visibility.Visible;
            colIn.Visibility = Visibility.Visible;
            but.Visibility = Visibility.Visible;
        }

        public void changeBtn(object sender, RoutedEventArgs e)
        {
            int newRow = Int32.Parse(rowIn.Text);
            int newCol = Int32.Parse(colIn.Text);

            // get new index 
            // (newRow * number of cols set currently) + newRow = new index 
            int curRows = grid.Rows;
            int curCols = grid.Columns;
            //int a = ((curRows-1)*newRow) + (newCol);
            int a = ((newRow - 1) * curCols + newCol);

            // remove button
            grid.Children.Remove(hold);
            buttonList.Clear();
            foreach (Button buttonItem in grid.Children)
            {
                if (buttonItem is FrameworkElement)
                {
                    buttonList.Add(buttonItem);
                }
            }

            if ((a) <= buttonList.Count + 1)
            {
                // insert button at new position
                Console.WriteLine("count: ");
                Console.WriteLine(buttonList.Count);
                buttonList.Insert(a - 1, hold);
            }
            else
            {
                buttonList.Add(hold);
            }
            // clear grid
            grid.Children.Clear();

            // re-format grid
            foreach (Button buttonItem in buttonList)
            {
                grid.Children.Add(buttonItem);
            }



            // clear and close everything
            btnMenu.Visibility = Visibility.Hidden;
            rowIn.Text = "";
            colIn.Text = "";
            rName.Visibility = Visibility.Hidden;
            rowIn.Visibility = Visibility.Hidden;
            cName.Visibility = Visibility.Hidden;
            colIn.Visibility = Visibility.Hidden;
            but.Visibility = Visibility.Hidden;

        }

        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
        }


        public void set_grid(int row, int col)
        {
            grid.Rows = row;
            grid.Columns = col;
        }


    }
}
