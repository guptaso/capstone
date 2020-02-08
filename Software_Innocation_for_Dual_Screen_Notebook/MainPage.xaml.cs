using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            String[] spearator = { ","};
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
            //newButton.KeyDown = "";
            
            

           if (!String.IsNullOrEmpty(appInput.Text))
            {
                //var success = await Windows.System.Launcher.LaunchUriAsync(new Uri(@appInput.Text));
                //newButton.Click += btnLaunchMap_Click;
                newButton.Click += async (s, en) => {
                    String[] spearator = { "," };
                    String[] strlist = appInput.Text.Split(spearator, StringSplitOptions.RemoveEmptyEntries);

                    foreach (String st in strlist)
                    {
                        Uri uri = new Uri(st);
                        await Launcher.LaunchUriAsync(uri);
                        //System.Diagnostics.Process.Start(st);
                    }
                };
            }
            myGrid.Children.Add(newButton);
            myFlyout.Hide();


            /* Emptys out the textboxes *//*
            this.nameInput.Text = "";
            this.appInput.Text = "";
            this.keyboardInput.Text = "";
            this.pngInput.Text = "";*/

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
