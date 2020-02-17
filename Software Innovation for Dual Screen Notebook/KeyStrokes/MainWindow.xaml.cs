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
                MessageBox.Show("The \'1\' key was pressed, opening Legends of Runeterra", "Button 1 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button1_KeyDown(sender, e);
            }
            else if(e.Key == Key.D2) {
                MessageBox.Show("The \'2\' key was pressed, opening r/leagueoflegends", "Button 2 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button2_KeyDown(sender, e);
            }
            else if(e.Key == Key.D3) {
                MessageBox.Show("The \'3\' key was pressed, opening YouTube", "Button 3 Press");
                e.Handled = true; //prevent the action from happening twice.
                Button3_KeyDown(sender, e);
            }
            else if(e.Key == Key.OemPlus)
            {
                MessageBox.Show("The \'+\' key was pressed, time to create a new button", "Button + Press");
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyDown(sender, e);
            }
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Button was clicked, time to create a new button", "Button + Click");
            //Form1 createButton = new Form1();
            //createButton.Show();


            ///////////////////////////////////////////////////////////////
            //  SOMETHING NEEDS TO GO HERE BUT I DON"T KNOW WHAT YET...  //
            ///////////////////////////////////////////////////////////////
            // this puts the window into focus when we open the form to  //
            // add new buttons, otherwise we cant type anything because  //  
            // our window wont be the focused window                     //
            ///////////////////////////////////////////////////////////////
            //  SOMETHING NEEDS TO GO HERE BUT I DON"T KNOW WHAT YET...  //
            ///////////////////////////////////////////////////////////////


            Popup1.IsOpen = true;
        }

        private void Click_Addkey(object sender, RoutedEventArgs e)
        {

            if (shortcut.Count != 0)
            {
                hotkeyDisplay.Children.Add(new TextBlock { Text = " + " });
            }
            shortcut.Add((VirtualKeyShort.Key)keyEnum.SelectedItem);
            var newKey = new TextBlock {
                Name = keyEnum.SelectedItem.ToString(),
                Text = keyEnum.SelectedItem.ToString()
            };
            hotkeyDisplay.Children.Add(newKey);
           
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            appInput.Text = "";
            nameInput.Text = "";
            pngInput.Text = "";
            //hotkeyControl.Text = "";
            Popup1.IsOpen = false;
        }

        private void Click_Confirm(object sender, RoutedEventArgs e)
        {
            /* adds generic elements to a button */
            Button newButton = new Button();
            newButton.Content = "no shortcut name";
            newButton.Width = addButton.Width;
            newButton.Height = addButton.Height;
            //newButton.CornerRadius = btn_add.CornerRadius; 
            newButton.Opacity = addButton.Opacity;
            newButton.Background = addButton.Background;
            newButton.BorderBrush = addButton.BorderBrush;
            newButton.FontSize = 30;
            newButton.Padding = addButton.Padding;
            newButton.Margin = addButton.Margin;

            // option to remove the button
            //newButton.RightTapped += async (s, en) =>
            newButton.MouseDown += async(s,en) =>
            {
                MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesNo);
                switch(result)
                {
                    case MessageBoxResult.Yes:
                        myGrid.Children.Remove(newButton);
                        break;
                }
            };
           

            // assigns the app name 
            if (String.IsNullOrEmpty(nameInput.Text))
            {
                newButton.Content = "";
            }
            else
            {
                newButton.Content = this.nameInput.Text;    // name of the app 
            }


            string hold = appInput.Text;
            if (!(String.IsNullOrEmpty(hold)))
            {
                newButton.Click += (se, ev) =>
                {
                    string str = "";

                    String[] spear = { "," };
                    String[] strlist = hold.Split(spear, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String st in strlist)
                    {
                        str = "";
                        str += "start ";
                        str += st;
                        Process cmd = new Process();
                        cmd.StartInfo.FileName = "cmd.exe";
                        cmd.StartInfo.RedirectStandardInput = true;
                        cmd.StartInfo.RedirectStandardOutput = true;
                        cmd.StartInfo.CreateNoWindow = true;   // true hides cmd prompt
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.Start();
                        cmd.StandardInput.WriteLine(str);
                        cmd.StandardInput.Flush();
                        cmd.StandardInput.Close();
                        cmd.WaitForExit();
                        Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                    }                  
                };
            }





            /*
            // assigns the application to launch:
            if (!(String.IsNullOrEmpty(appInput.Text)))
            {
                newButton.Tag = appInput.Text; //use tag to store the uri location, then can be accessed in the calling function
                newButton.Click += (se, ev) => this.launchApp(se, ev);
            }

            */

            // assigns the keyboard shortcuts to launch
            if (shortcut.Count != 0)
            {
                newButton.Click += (se, ev) =>
                {
                    Shortcut.send(shortcut.ToArray());
                };
                // this is needed to clear the form for next shortcut
                // to be added but it causes the shortcut to not work
                //shortcut.Clear();
                //int sizeAtStart = hotkeyDisplay.Children.Count;
                //for (int i = 1; i < sizeAtStart; i++)
                //{
                //    hotkeyDisplay.Children.RemoveAt(1);
                //}
            }

            // adds the button to the grid
            myGrid.Children.Add(newButton);

            // clears out the flyout fields 
            nameInput.Text = "";
            //hotkeyControl.Text = "";
            appInput.Text = "";
            pngInput.Text = "";

            Popup1.IsOpen = false;
        }

        /* KeyboardAccelerator could not be found 
        protected KeyboardAccelerator createHotkey()
        {
            Hashtable ht = new Hashtable();
            ht.Add("Control", 1);
            var keyVals = Enum.GetValues(typeof(VirtualKey));
            string[] hotkeystring = this.hotkeyControl.Text.Split("+");

            VirtualKey key = (VirtualKey)Enum.Parse(typeof(VirtualKey), hotkeystring[1]);
            VirtualKeyModifiers keymod = (VirtualKeyModifiers)Enum.Parse(typeof(VirtualKeyModifiers), hotkeystring[0]);
            var item = new KeyboardAccelerator()
            {
                Modifiers = keymod,
                Key = key
            };
            return item;
        }
        */
        /*
        protected void launchApp(object se, RoutedEventArgs ev)
        {
            Button clicked_button = (Button)se;
            string uriString = (string)clicked_button.Tag;
            String[] spearator = { "," };
            String[] strlist = uriString.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            foreach (String st in strlist)
            {
                try
                {
                    var uri = new Uri(@st);
                    DefaultLaunch(uri);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("Bad uri, alert user");
                }
            }

        }
        async void DefaultLaunch(Uri uri)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            if (success)
            {
                System.Diagnostics.Trace.WriteLine("successful Launch");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("failed launch");
            }
        }
        */

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
            if(e.Key == Key.D1)
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
            if(e.Key == Key.D3)
                Process.Start("https://www.youtube.com");
        }
    }
}
