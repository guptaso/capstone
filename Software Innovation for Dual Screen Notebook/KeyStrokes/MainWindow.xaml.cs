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
using System.Linq;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 3;
        private const int WS_EX_NOACTIVE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        // this is used to get the hWnd for imported functions
        // hWnd is a way to identify windows used in win32 framework
        private WindowInteropHelper helper;
        private List<VirtualKeyShort.Key> shortcut;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            shortcut = new List<VirtualKeyShort.Key>();

            // sets the window so that a click does not bring it into focus
            helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVE);

            // this lets WndProc be overriden so that we can get the click massage
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);

            // this activates the window so that when we start it 
            // it does not jump to the back of all windows
            this.Activate();
        }

        // this gets the click message so that 
        // it can still sends the click to the app
        // even though it is out of focus
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            switch (msg)
            {

                case WM_MOUSEACTIVATE:
                    return (IntPtr)MA_NOACTIVATE;
                default:
                    break;
            }

            return IntPtr.Zero;
        }


        //Keys register on window.
        private void KeyInteractor(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                MessageBox.Show("The \'1\' key was pressed, opening Legends of Runeterra", "Button 1 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button1_KeyDown(sender, e);
            }
            else if (e.Key == Key.D2)
            {
                MessageBox.Show("The \'2\' key was pressed, opening r/leagueoflegends", "Button 2 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button2_KeyDown(sender, e);
            }
            else if (e.Key == Key.D3)
            {
                MessageBox.Show("The \'3\' key was pressed, opening YouTube", "Button 3 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button3_KeyDown(sender, e);
            }
            else if (e.Key == Key.OemPlus)
            {
                MessageBox.Show("The \'+\' key was pressed, time to create a new button", "Button + Press");
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyDown(sender, e);
            }
        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
        }


        private void Add_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                Form1 createButton = new Form1();
                createButton.Show();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening Legends of Runeterra", "Button 1 Click");
            Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }


        private void Button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1)
                Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening League of Legends Subreddit", "Button 2 Click");
            Process.Start("https://www.reddit.com/r/leagueoflegends");
        }

        private void Button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D2)
                Process.Start("https://www.youtube.com");
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button was clicked, opening YouTube", "Button 1 Click");
            Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }


        private void Button3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D3)
                Process.Start("https://www.youtube.com");
        }

        private void media_back_Click(object sender, RoutedEventArgs e)
        {
            //VirtualKeyShort.Key[] shortcut = [VirtualKeyShort.Key.MEDIA_PREV_TRACK];
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PREV_TRACK });
        }

        private void media_play_pause_Click(object sender, RoutedEventArgs e)
        {
            // this needs to be changed to accomidate actually checking if the 
            // music is playing to be correct all the time
            if (media_play_pause.Content == "Play")
            {
                media_play_pause.Content = "Pause";

            }
            else
            {
                media_play_pause.Content = "Play";
            }
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PLAY_PAUSE });
        }

        private void media_forward_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_NEXT_TRACK });
        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_C });
        }

        private void redo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_Y });
        }

        // bottom bar   

        private void MenuClick(object sender, RoutedEventArgs e)
        {
            if (layoutBtn.IsVisible)
            {
                layoutBtn.Visibility = Visibility.Hidden;
                newBtn.Visibility = Visibility.Hidden;
                movementStack.Visibility = Visibility.Hidden;
            }
            else
            {
                layoutBtn.Visibility = Visibility.Visible;
                newBtn.Visibility = Visibility.Visible;
                movementStack.Visibility = Visibility.Visible;
            }
        }

        private void addLayout(object sender, RoutedEventArgs e)
        {
            inputBox.Visibility = Visibility.Hidden;
            okInput.Visibility = Visibility.Hidden;
            
            //layoutBtn.Visibility = Visibility.Hidden;
            ComboBoxItem comboBoxItem = new ComboBoxItem();
            string inputText = inputBox.Text;
            comboBoxItem.Content = inputText;
            myComboBox.Items.Add(comboBoxItem);
            inputBox.Text = "";

        }

        private void getInput(object sender, RoutedEventArgs e)
        {
            inputBox.Visibility = Visibility.Visible;
            okInput.Visibility = Visibility.Visible;
            layoutBtn.Visibility = Visibility.Visible;
            

        }

        private void LayoutClick(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)myComboBox.SelectedItem;
            string name = myComboBox.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            //Trace.WriteLine(name.ToString().Substring(35,38));
            string[] x = name.ToString().Split('x');
            string a = x[1];
            string b = "";
            if (x.Length > 2)
            {
                b = x[2];
                a = a.ToString().Split(' ')[1];
                newMyGrid.Rows = Int16.Parse(a);
                newMyGrid.Columns = Int16.Parse(b);
            }
            // a is rows b is cols

        }

        private void mvClick(object sender, RoutedEventArgs e)
        {
            moveBox.Visibility = Visibility.Visible;
        }

        // Ignore this function.... working on basically just copying everything over to move the buttons.... 
        private void mvRCSubmit(object sender, RoutedEventArgs e)
        {
            /*
            string btnName = moveBox.Text;
            int rowNum = Int16.Parse(rowInput.Text);
            int colNum = Int16.Parse(colInput.Text);
            int cols = newMyGrid.Columns;
            
            // goes through each element in the stack pannel 
            foreach (object child in newMyGrid.Children)
            {
                if (child is FrameworkElement)
                {
                    string[] x = child.ToString().Split(' ');
                    string item = x.Last();
                    
                    // element to move found
                    if (String.Equals(item, btnName))
                    {

                        // Remove this button
                        newMyGrid.Children.Remove(child as FrameworkElement);

                        // number of elements to skip over: (# of cols )* (row-1) + cols
                        int skipOverCount = cols * (rowNum - 1) + colNum;
                        int currentCount = 0;

                        // temporary holders 
                        Button hold = new Button();
                        Button tempHold = new Button();
                        foreach (object child2 in newMyGrid.Children)
                        {
                           
                            if (child2 is FrameworkElement)
                            {
                                currentCount = currentCount + 1;

                                // replaces with target button
                                if (currentCount == skipOverCount)
                                {
                                    // copy info about this button into b"
                                    // Button b = new Button();
                                    (child2 as Button).Name = (child as Button).Name;
                                    (child2 as Button).Content = (child as Button).Content;
                                    (child2 as Button).Width = (child as Button).Width;
                                    (child2 as Button).Height = (child as Button).Height;
                                    (child2 as Button).Opacity = (child as Button).Opacity;
                                    (child2 as Button).Background = (child as Button).Background;
                                    (child2 as Button).BorderBrush = (child as Button).BorderBrush;
                                    (child2 as Button).FontSize = (child as Button).FontSize;
                                    (child2 as Button).Padding = (child as Button).Padding;
                                    (child2 as Button).Margin = (child as Button).Margin;
                                    // option to remove the button
                                    //newButton.RightTapped += async (s, en) =>
                                    (child2 as Button).MouseDown += async (s, en) =>
                                    {
                                        MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesNo);
                                        switch (result)
                                        {
                                            case MessageBoxResult.Yes:
                                                myGrid.Children.Remove((child2 as Button));
                                                break;
                                        }
                                    };

                                    // Need to add click event
                                    //b.Click += (child as Button).Click; 
                                    // Also need to copy keyboard shortcut....


                                }
                                // moves the rest of the buttons up 
                                else if (currentCount >= skipOverCount)
                                {

                                }
                            }
                        }
                        break;
                    }
                }
            }*/
        }
    }
}

