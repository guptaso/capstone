using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            Popup1.IsOpen = true;
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            appInput.Text = "";
            nameInput.Text = "";
            pngInput.Text = "";
            hotkeyControl.Text = "";
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
            /*
            // assigns the keyboard shortcuts to launch
            if (!(String.IsNullOrEmpty(hotkeyControl.Text)))
            {
                KeyboardAccelerator item = createHotkey();
                item.Invoked += (se, ev) => System.Diagnostics.Trace.WriteLine("cntrl-b");
                newButton.KeyboardAccelerators.Add(item);
            }
            */

            // adds the button to the grid
            myGrid.Children.Add(newButton);

            // clears out the flyout fields 
            nameInput.Text = "";
            hotkeyControl.Text = "";
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
