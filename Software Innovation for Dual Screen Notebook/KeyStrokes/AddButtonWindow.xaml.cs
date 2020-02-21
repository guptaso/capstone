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
    public sealed partial class AddButtonWindow : Window
    {


        private List<VirtualKeyShort.Key> shortcut;
        private MainWindow main;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            shortcut = new List<VirtualKeyShort.Key>();

            main = ((MainWindow)App.Current.MainWindow);

        }

        private void Click_Addkey(object sender, RoutedEventArgs e)
        {

            if (shortcut.Count != 0)
            {   
                hotkeyDisplay.Children.Add(new TextBlock { Text = " + " });
            }
            shortcut.Add((VirtualKeyShort.Key)keyEnum.SelectedItem);
            var newKey = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)          
            };

            var newKeyBorder = new Border
            {
                Background = Brushes.GhostWhite,
                BorderBrush = Brushes.Silver,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(3),
                Child = newKey
            };

            var newKeyText = new TextBlock
            {
                Name = keyEnum.SelectedItem.ToString(),
                Text = keyEnum.SelectedItem.ToString(),
            };

            var newKeyClose = new Button
            {
                Width = 15,
                Height = 15,
                Content = "x"
            };

            // this needs a lot of work...
            //newKeyClose.Click += (se, ev) =>
            //{

            //    hotkeyDisplay.Children.Add(new TextBox
            //    {
            //        Text = ev.Source.ToString()
            //    });

            //    //int count;
            //    //Int32.TryParse(this.Uid, out count);
            //    //hotkeyDisplay.Children.
            //    //count += 1;
            //    //if (count != 1)
            //    //{
            //    //    hotkeyDisplay.Children.RemoveAt(count);
            //    //    hotkeyDisplay.Children.RemoveAt(count - 1);
            //    //}
            //    //else
            //    //{
            //    //    hotkeyDisplay.Children.RemoveAt(count);
            //    //    if (shortcut.Count > 1)
            //    //    {
            //    //        hotkeyDisplay.Children.RemoveAt(count);
            //    //    }
            //    //}

            //    //shortcut.RemoveAt((count-1) / 2);
            //};

            newKey.Children.Add(newKeyText);
            newKey.Children.Add(newKeyClose);

            hotkeyDisplay.Children.Add(newKeyBorder);
           
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void Click_Confirm(object sender, RoutedEventArgs e)
        {
            // assigns the app name
            String ButtonText = "";
            if (!String.IsNullOrEmpty(nameInput.Text))
            {
                ButtonText = this.nameInput.Text;    // name of the app 
                //newButton.Name = this.nameInput.Text; add this to addbutton thing
            }

            // will hold the click handler
            Action<object, RoutedEventArgs> click = null;

            string hold = appInput.Text;
            if (!(String.IsNullOrEmpty(hold)))
            {
                click = (se, ev) =>
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


            // launch app does not exist and I don't know why?
            //// assigns the application to launch:
            //String Tag = appInput.Text;
            //if (!(String.IsNullOrEmpty(appInput.Text)))
            //{
            //    Tag = appInput.Text; //use tag to store the uri location, then can be accessed in the calling function
            //    click += (se, ev) => this.launchApp(se, ev);
            //}



            // assigns the keyboard shortcuts to launch
            if (shortcut.Count != 0)
            {
                click = (se, ev) =>
                {
                    Shortcut.send(shortcut.ToArray());
                };
            }

            //main.grid.addButton(ButtonText, click);
            //// adds the button to the grid
            //main.newMyGrid.Children.Add(newButton);

            this.Close();
        }
    }
}
