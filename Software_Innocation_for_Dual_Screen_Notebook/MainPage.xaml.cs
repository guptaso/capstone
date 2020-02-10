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
            this.keyboardInput.Text = "";
            this.pngInput.Text = "";

            //this.Frame.Navigate(typeof(MainPage), "");

        }
        private async void btnLaunchMap_Click(object sender, RoutedEventArgs e)

        {
            String[] spearator = { "," };
            String[] strlist = appInput.Text.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

            foreach (String s in strlist)
            {
                Uri uri = new Uri(s);
                await Launcher.LaunchUriAsync(uri);

            }

            //Uri uri = new Uri("bingmaps:?rtp=adr.Washington,%20DC~adr.New%20York,% 20NY & amp; mode = d & amp; trfc = 1");
            //Uri uri = new Uri("spotify:");
            // Uri uri = new Uri(appInput.Text);
            // await Launcher.LaunchUriAsync(uri);

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
            //newButton.FontSize = btn_add.FontSize;
            newButton.FontSize = 30;
            newButton.Padding = btn_add.Padding;
            newButton.Margin = btn_add.Margin;
            /* adds the input information */
            newButton.Content = this.nameInput.Text;

            newButton.Tag = appInput.Text; //use tag to store the uri location, then can be accessed in the calling function
            newButton.Click += (se, ev) => this.launchApp(se, ev);

            KeyboardAccelerator item = createHotkey();
            item.Invoked += (se, ev) => System.Diagnostics.Trace.WriteLine("cntrl-b");
            newButton.KeyboardAccelerators.Add(item);
            myGrid.Children.Add(newButton);
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
            try
            {
                var uri = new Uri(@uriString);
                DefaultLaunch(uri);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Bad uri, alert user");
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

        void sendUrl(string s)
        {
            //launchURI_Click(s);
        }
        private async void launchURI_Click(object sender, EventArgs e)
        {
            // The URI to launch
            var uriBing = new Uri(@"http://www.bing.com");

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
