using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Linq;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class AddButtonWindow : UserControl
    {


        private List<VirtualKeyShort.Key> shortcut;
        private MainWindow main;

        public AddButtonWindow()
        {
            //base.OnSourceInitialized(e);
            InitializeComponent();

            shortcut = new List<VirtualKeyShort.Key>();

            main = ((MainWindow)App.Current.MainWindow);

        }

        public void Open()
        {
            nameInput.Text = "";
            appInput.Text = "";
            //pngInput.Text = "";
            fileNames.Text = "";
            shortcut.Clear();
            redrawHotkeys();
        }

        private void FileDropper(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


                appInput.Text += files[0];
                appInput.Text += ",";
                string x = appInput.Text;
                Console.WriteLine("TEST: ", files[0]);
                String[] spearator = { "\\" };
                String[] toaddList = files[0].Split(spearator, 200, StringSplitOptions.RemoveEmptyEntries);

                ((TextBox)sender).Text += toaddList[toaddList.Length - 1];
                ((TextBox)sender).Text += ",\n";
                mainFileBrowserSP.Visibility = Visibility.Hidden;
            }
        }

        private void FileDropper_PreviewDO(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void redrawHotkeys()
        {
            hotkeyDisplay.Children.Clear();
            for (int i = 0; i < shortcut.Count; i++)
            {
                var newKey = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0)
                };
                var newKeyText = new TextBlock
                {
                    Name = shortcut[i].ToString(),
                    Text = shortcut[i].ToString(),
                };

                var newKeyClose = new Button
                {
                    Width = 15,
                    Height = 20,
                    Content = "x",
                    Background = Brushes.LightGray,
                    Name = shortcut[i].ToString()
                };

                // this needs a lot of work...
                newKeyClose.Click += (se, ev) =>
                {
                    for (int j = 0; j < shortcut.Count; j++)
                    {
                        Button close = (Button)se;
                        if (shortcut[j].ToString() == close.Name)
                        {
                            shortcut.RemoveAt(j);
                        }
                    }
                    redrawHotkeys();
                };

                newKey.Children.Add(newKeyText);
                newKey.Children.Add(newKeyClose);

                var newKeyHolder = new Border
                {
                    Background = Brushes.GhostWhite,
                    BorderBrush = Brushes.Silver,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3),
                    Child = newKey
                };
                hotkeyDisplay.Children.Add(newKeyHolder);
                if (i != shortcut.Count - 1)
                {
                    hotkeyDisplay.Children.Add(new TextBlock { Text = " + " });
                }
            }
        }

        private void Click_Addkey(object sender, RoutedEventArgs e)
        {
            shortcut.Add((VirtualKeyShort.Key)keyEnum.SelectedItem);
            redrawHotkeys();

        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void revealFileUploadOptions(object sender, RoutedEventArgs e)
        {
            mainFileBrowserSP.Visibility = Visibility.Visible;
        }

        private void closeFileBrowser(object sender, RoutedEventArgs e)
        {
            mainFileBrowserSP.Visibility = Visibility.Hidden;
        }

        private void Click_Confirm(object sender, RoutedEventArgs e)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // assigns the app name
            String ButtonText = "ShortCut";
            if (!String.IsNullOrEmpty(nameInput.Text))
            {
                ButtonText = this.nameInput.Text;    // name of the app 
                //newButton.Name = this.nameInput.Text; add this to addbutton thing
            }

            // make it so that the buttons don't have the same name
            List<string> buttonNames = new List<string>();
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveClicks.txt"), true))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    buttonNames.Add(line.Split('|')[0]);
                }
            }
            string result = " ";
            while (result != null) {
                result = buttonNames.FirstOrDefault(x => x == ButtonText);
                if (result != null)
                {
                    ButtonText += "_1";
                }
            }

            string outStr = ButtonText;
            outStr += "|";
            // will hold the click handler
            Action<object, RoutedEventArgs> click = null;

            // assigns the app to launch
            string hold = appInput.Text;
            string str = "";
            if (!(String.IsNullOrEmpty(hold)))
            {
                    String[] spear = { "," };
                    String[] strlist = hold.Split(spear, StringSplitOptions.RemoveEmptyEntries);
                    foreach (String st in strlist)
                    {
                        if (str.Length > 1)
                            str += " & ";
                        string stTrim = st.Trim();
                        // harvest shortcuts from the start menu folder
                        String[] shortcut = null;
                        if (!stTrim.Contains(":\\"))
                        {
                            shortcut = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), stTrim + ".lnk", SearchOption.AllDirectories);
                        }

                        // start command: start "" "<program>"
                        str = "";
                        str += "start \"\" \"";
                        // If we found a shortcut, we can add it to the start command 
                        if (shortcut != null && shortcut.Length != 0) { str += shortcut[0]; }
                        else { str += stTrim; }
                        str += "\"";
                    }
                    if (str.Length > 1)
                    {
                    click = (se, ev) =>
                    {
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
                    };
                }

            }

            // assigns the keyboard shortcuts to launch
            if (shortcut.Count != 0)
            {
                // by making a copy here it makes each button send its own shortcut
                // otherwise all keys send the same shortcut
                List<VirtualKeyShort.Key> holder = new List<VirtualKeyShort.Key>(shortcut);
                click += (se, ev) =>
                {
                    Shortcut.send(holder.ToArray());
                };
            }

            // don't let the user add an empty button...
            if (click != null)
            {
                main.grid.addButton(ButtonText, click);
                Click_Cancel(sender, e);
                main.menu_control.Visibility = Visibility.Collapsed;


                // save it 
                using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveClicks.txt"), true))
                {
                    if (str.Length > 0)
                        outStr += str;
                    else
                        outStr += " ";
                    outStr += "|";

                    if (shortcut.Count != 0)
                    {
                        for (int i = 0; i < shortcut.Count; i++)
                        {
                            outStr += shortcut[i];
                            outStr += " ";
                        }
                    }
                    else
                        outStr += " ";

                    outputFile.WriteLine(outStr);
                }

            }

            keyEnum.SelectedIndex = 0;

        }

        private void keyEnum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void file_open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                appInput.Text += openFileDialog.FileName;
                String[] spearator = { "\\" };
                String[] toaddList = openFileDialog.FileName.Split(spearator, 200, StringSplitOptions.RemoveEmptyEntries);
                ((TextBox)fileNames).Text += toaddList[toaddList.Length - 1];
                ((TextBox)fileNames).Text += ",\n";
                mainFileBrowserSP.Visibility = Visibility.Hidden;
            }
        }

        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

