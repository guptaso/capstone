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
using System.Drawing;

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

        //Store list of hotkeys (in char) used atm (Cannot use the same hotkey in more than one button)
        private static List<char> hotkeyCharList = new List<char>();

        //Also store list of hotkeys (in Key) used atm
        private static List<Key> hotKeyList = new List<Key>();

        //Also store list of locations (mainly used for the dynamic button portion)
        private static List<string> locations = new List<string>();

        /*
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        static public Key ResolveKey(char charToResolve)
        {
            return KeyInterop.KeyFromVirtualKey(VkKeyScan(charToResolve));
        }*/

        // Retrieve an icon
        public static ImageSource GetIcon(string fileName)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            return Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0, 0, icon.Width, icon.Height),
                        BitmapSizeOptions.FromEmptyOptions());
        }


        public MainWindow()
        {
            InitializeComponent();

            //Your boy did it, he managed to KEKW the capstone project
            this.Icon = BitmapFrame.Create(new Uri("../../Images/kekw.jpg", UriKind.RelativeOrAbsolute));

            //Store the initial set of hotkeys (4 hotkeys initially)
            hotkeyCharList.Add('+');
            for(int i = 1; i <= 3; i++)
                hotkeyCharList.Add((char)(i + 48));

            hotKeyList.Add(Key.OemPlus);
            hotKeyList.Add(Key.D1);
            hotKeyList.Add(Key.D2);
            hotKeyList.Add(Key.D3);
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
            if(e.Key == Key.D1) {
                //MessageBox.Show("The \'1\' key was pressed, opening Legends of Runeterra", "Button 1 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button1_KeyDown(sender, e);
            }
            else if(e.Key == Key.D2) {
                //MessageBox.Show("The \'2\' key was pressed, opening r/leagueoflegends", "Button 2 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button2_KeyDown(sender, e);
            }
            else if(e.Key == Key.D3) {
                //MessageBox.Show("The \'3\' key was pressed, opening YouTube", "Button 3 Press");
                e.Handled = true; //prevent the action from happening twice.
                //MessageBox.Show(((int)Key.D3).ToString());
                Button3_KeyDown(sender, e);
            }
            else if(e.Key == Key.OemPlus)
            {
                //MessageBox.Show("The \'+\' key was pressed, time to create a new button", "Button + Press");
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyDown(sender, e);
            }
            //For the dynamic
            else
            {
                e.Handled = true;
                //MessageBox.Show(((int)e.Key).ToString());
                DynamicButton_KeyDown(sender, e);
            }
        }


        // this opens the help window purely for showing how to work this application
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            /*
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
            */

            AddApplication form1 = new AddApplication();
            form1.Show();
        }

        // Press the + button (shift =) to call via keydown
        private void Add_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                AddApplication form1 = new AddApplication();
                form1.Show();
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Button was clicked, opening Legends of Runeterra", "Button 1 Click");
            Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }


        private void Button1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.D1)
                Process.Start("D:\\Riot Games\\LoR\\live\\Game\\LoR");
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Button was clicked, opening Reddit's front page", "Button 2 Click");
            Process.Start("https://www.reddit.com/r/all");
        }

        private void Button2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D2)
                Process.Start("https://www.reddit.com/r/all");
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Button was clicked, opening YouTube", "Button 1 Click");
            Process.Start("https://www.youtube.com");
        }


        private void Button3_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.D3)
                Process.Start("https://www.youtube.com");
        }

        //Takes in the form inputs from Form1.cs and dynamically adds a new button by appending it to the ScrollViewer
        public static bool processFormInputs(string appLocation, string appImage, string appHotKey)
        {

            //First go through the current list of hotkeys. 
            //If at least one hotkey matches, then return false
            for (int i = 0; i < hotkeyCharList.Count; i++)
            {
                if (Char.ToUpper(appHotKey[0]) == hotkeyCharList[i]) {
                    MessageBox.Show("Please use a different hotkey, it needs to be unique", "NonUnique Hotkey");
                    return false;
                }
            }

            //Otherwise form is valid and hotkey is unique, add the form.

            //Just displaying what will be added
            MessageBox.Show("Your application was successfully added!", "Button successfully created!");

            //Dynamically add button details

            //Adds the margin (Left, Top, Right, Bottom)
            Thickness buttonMargin = new Thickness(11, 0, 10, 0);
            Thickness textMargin = new Thickness(-80, 0, -5, -20);

            //Adds an image to the button (WIP)
            //If no specified image was provided, then use the exe's icon instead.
            //Otherwise, use that specific image link
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapSource source;
            ImageSource sourceEmpty;
            if (appImage != "")
            {
                source = new BitmapImage(new Uri(appImage, UriKind.RelativeOrAbsolute));
                image.Source = source;
            }
            else
            {
                sourceEmpty = GetIcon(appLocation);
                image.Source = sourceEmpty;
            } 
            //BitmapSource source = new BitmapImage(new Uri(appImage, UriKind.RelativeOrAbsolute));
            //image.Source = source;
            image.Height = 75;
            image.Width = 90;
            System.Windows.Point point = new System.Windows.Point(0.455, -0.263);
            image.RenderTransformOrigin = point;

            //Add a text block
            TextBlock text = new TextBlock();
            text.VerticalAlignment = VerticalAlignment.Bottom;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.Margin = textMargin;
            text.Text = appHotKey.ToUpper();

            //Create a dock panel that holds both the Image and the TextBlock children
            DockPanel dock = new DockPanel();
            dock.Children.Add(image);
            dock.Children.Add(text);

            //Create a new button and give it keydown and click events
            Button newButton = new Button()
            {
                Height = 75,
                Width = 90,
                Margin = buttonMargin,
            };
            newButton.KeyDown += DynamicButton_KeyDown;
            newButton.Click += (sender, e) => DynamicButton_Click(sender, e, appLocation);

            //Contents of the button is simply whatever the dock is
            newButton.Content = dock;


            //Finally, the button is a part of the stack panel, which is a content of the scrollviewer
            ScrollViewer scroll = ((MainWindow)App.Current.MainWindow).myGrid.FindName("ButtonViewholder") as ScrollViewer;
            StackPanel stack = ((MainWindow)App.Current.MainWindow).myGrid.FindName("MyStack") as StackPanel;
            
            stack.Children.Add(newButton);
            scroll.Content = stack;


            //Determine which key was pressed and add it to the Key enum list

            //Is it a number?
            if(Char.IsDigit(appHotKey[0]))
            {
                if (appHotKey[0] == '4')
                    hotKeyList.Add(Key.D4);
                else if(appHotKey[0] == '5')
                    hotKeyList.Add(Key.D5);
                else if (appHotKey[0] == '6')
                    hotKeyList.Add(Key.D6);
                if (appHotKey[0] == '7')
                    hotKeyList.Add(Key.D7);
                else if (appHotKey[0] == '8')
                    hotKeyList.Add(Key.D8);
                else if (appHotKey[0] == '9')
                    hotKeyList.Add(Key.D9);
                else
                    hotKeyList.Add(Key.D0);
            }

            //It must be a letter if it's not a number
            else
            {
                if (Char.ToUpper(appHotKey[0]) == 'A')
                    hotKeyList.Add(Key.A);
                else if (Char.ToUpper(appHotKey[0]) == 'B')
                    hotKeyList.Add(Key.B);
                else if (Char.ToUpper(appHotKey[0]) == 'C')
                    hotKeyList.Add(Key.C);
                else if (Char.ToUpper(appHotKey[0]) == 'D')
                    hotKeyList.Add(Key.D);
                else if (Char.ToUpper(appHotKey[0]) == 'E')
                    hotKeyList.Add(Key.E);
                else if (Char.ToUpper(appHotKey[0]) == 'F')
                    hotKeyList.Add(Key.F);
                else if (Char.ToUpper(appHotKey[0]) == 'G')
                    hotKeyList.Add(Key.G);
                else if (Char.ToUpper(appHotKey[0]) == 'H')
                    hotKeyList.Add(Key.H);
                else if (Char.ToUpper(appHotKey[0]) == 'I')
                    hotKeyList.Add(Key.I);
                else if (Char.ToUpper(appHotKey[0]) == 'J')
                    hotKeyList.Add(Key.J);
                else if (Char.ToUpper(appHotKey[0]) == 'K')
                    hotKeyList.Add(Key.K);
                else if (Char.ToUpper(appHotKey[0]) == 'L')
                    hotKeyList.Add(Key.L);
                else if (Char.ToUpper(appHotKey[0]) == 'M')
                    hotKeyList.Add(Key.M);
                else if (Char.ToUpper(appHotKey[0]) == 'N')
                    hotKeyList.Add(Key.N);
                else if (Char.ToUpper(appHotKey[0]) == 'O')
                    hotKeyList.Add(Key.O);
                else if (Char.ToUpper(appHotKey[0]) == 'P')
                    hotKeyList.Add(Key.P);
                else if (Char.ToUpper(appHotKey[0]) == 'Q')
                    hotKeyList.Add(Key.Q);
                else if (Char.ToUpper(appHotKey[0]) == 'R')
                    hotKeyList.Add(Key.R);
                else if (Char.ToUpper(appHotKey[0]) == 'S')
                    hotKeyList.Add(Key.S);
                else if (Char.ToUpper(appHotKey[0]) == 'T')
                    hotKeyList.Add(Key.T);
                else if (Char.ToUpper(appHotKey[0]) == 'U')
                    hotKeyList.Add(Key.U);
                else if (Char.ToUpper(appHotKey[0]) == 'V')
                    hotKeyList.Add(Key.V);
                else if (Char.ToUpper(appHotKey[0]) == 'W')
                    hotKeyList.Add(Key.W);
                else if (Char.ToUpper(appHotKey[0]) == 'X')
                    hotKeyList.Add(Key.X);
                else if (Char.ToUpper(appHotKey[0]) == 'Y')
                    hotKeyList.Add(Key.Y);
                else
                    hotKeyList.Add(Key.Z);
            }

            //Before finishing up, add the hotkey to the list
            //and the location
            hotkeyCharList.Add(Char.ToUpper(appHotKey[0]));
            locations.Add(appLocation);

            return true;
            

        }

        //Dynamic button's KeyDown event
        private static void DynamicButton_KeyDown(object sender, KeyEventArgs e)
        {
            //Search for which hotkey was pressed
            for(int i = 4; i < hotKeyList.Count; i++)
            {
                if(hotKeyList[i] == e.Key)
                {
                    //Start the application at the 4th offset index and then stop searching
                    Process.Start(locations[i - 4]);
                    break;
                }
            }
        }

        //Dynamic button's Click event
        private static void DynamicButton_Click(object sender, RoutedEventArgs e, string location)
        {
            Process.Start(location);
        }
    }
}
