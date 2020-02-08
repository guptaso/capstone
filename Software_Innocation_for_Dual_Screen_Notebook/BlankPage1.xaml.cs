using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Software_Innocation_for_Dual_Screen_Notebook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();

            this.confirm_btn.Click += ConfirmClick;
            this.cancel_btn.Click += CancelClick;

        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage),"");
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.nameInput.Text.ToString()))
            {
                this.Frame.Navigate(typeof(MainPage), "error");
            }
            else
            {
                Dictionary<string, string> newDictionary = new Dictionary<string, string>();
                newDictionary.Add("name", this.nameInput.Text);
                newDictionary.Add("image", this.pngInput.Text);
                
                if (string.IsNullOrEmpty(this.appInput.Text.ToString()))
                {
                    newDictionary.Add("keyboard", this.keyboardInput.Text);
                    newDictionary.Add("app", "");
                    this.Frame.Navigate(typeof(MainPage), newDictionary);
                }
                else
                {
                    newDictionary.Add("app", this.appInput.Text);
                    newDictionary.Add("keyboard", "");
                    this.Frame.Navigate(typeof(MainPage), newDictionary);
                }
                
            }
        }
    
    }
}
