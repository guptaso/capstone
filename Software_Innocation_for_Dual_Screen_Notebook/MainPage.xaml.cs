using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Software_Innocation_for_Dual_Screen_Notebook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public int row = 1;
        public int column = 1;
        private void Click_Cancel(Object sender, RoutedEventArgs e)
        {
            /* Emptys out the textboxes */
            this.nameInput.Text = "";
            this.appInput.Text = "";
            this.hotkeyControl.Text = "";
            this.pngInput.Text = "";


            myFlyout.Hide();
        }

        private async void Click_Confirm(Object sender, RoutedEventArgs e)
        {
            /* adds generic elements to a button */
            Button newButton = new Button();
            newButton.Content = "no shortcut name";
            newButton.Width = btn_add.Width;
            newButton.Height = btn_add.Height;
            newButton.CornerRadius = btn_add.CornerRadius;
            newButton.Opacity = btn_add.Opacity;
            newButton.Background = btn_add.Background;
            newButton.BorderBrush = btn_add.BorderBrush;
            newButton.FontSize = 30;
            newButton.Padding = btn_add.Padding;
            newButton.Margin = btn_add.Margin;

            // option to remove the button
            newButton.RightTapped += async (s, en) =>
            {
                myGrid.Children.Remove(newButton);
            };

            // assigns the app name 
            if (String.IsNullOrEmpty(nameInput.Text))
            {
                newButton.Content = "Unnamed App";
            } else
            {
                newButton.Content = this.nameInput.Text;    // name of the app 
            }

            // assigns the application to launch:
            if (!(String.IsNullOrEmpty(appInput.Text)))
            {
                newButton.Tag = appInput.Text; //use tag to store the uri location, then can be accessed in the calling function
                newButton.Click += (se, ev) => this.launchApp(se, ev);
            }


            // assigns the keyboard shortcuts to launch
            if (!(String.IsNullOrEmpty(hotkeyControl.Text)))
            {
                KeyboardAccelerator item = createHotkey();
                item.Invoked += (se, ev) => System.Diagnostics.Trace.WriteLine("cntrl-b");
                newButton.KeyboardAccelerators.Add(item);
            }

            // adds the button to the grid
            myGrid.Children.Add(newButton);

            // clears out the flyout fields 
            nameInput.Text = "";
            hotkeyControl.Text = "";
            appInput.Text = "";
            pngInput.Text = "";

            // hides the flyout
            myFlyout.Hide();
        }

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

        protected void launchApp(object se, RoutedEventArgs ev)
        {
            Button clicked_button = (Button)se;
            string uriString = (string)clicked_button.Tag;
            String[] spearator = { "," };
            String[] strlist = uriString.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            foreach(String st in strlist) {
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


        private async void test_button(object sender, RoutedEventArgs e)
        {

            /* Access is Denied 
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\sonic\AppData\Roaming\Spotify\Spotify.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
            */

            /* inappropriate permissions */
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = false;   // true hides cmd prompt
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine(@"start C:\Users\sonic\AppData\Roaming\Spotify\Spotify.exe");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
