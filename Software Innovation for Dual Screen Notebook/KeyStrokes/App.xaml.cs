using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow main;

        private void App_Startup(object sender, StartupEventArgs e)
        {
                   
            // make sure file and directory exists
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(docPath, "KeyStrokesApp");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string appPath = Path.Combine(docPath, "KeyStrokesApp\\saveClicks.txt");
            string layoutPath = Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt");
            string autoStartPath = Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt");
            string bottomBar = Path.Combine(docPath, "KeyStrokesApp\\bottmBar.txt");

            if (!File.Exists(appPath))
            {
                File.Create(appPath);
            }

            if (!File.Exists(layoutPath))
            {
                File.Create(layoutPath);
            }

            if (!File.Exists(autoStartPath))
            {
                File.Create(autoStartPath);
            }

            if (!File.Exists(bottomBar))
            {
                File.Create(bottomBar);
            }

            string start = "";
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"), true))
            {
                start = sr.ReadLine();
            }
            if (start == "" || start == "yes")
            {
                var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.SetValue("MyApplication", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                string path = @"Software\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("MyApplication", false);
                    }
                }
            }



                // to get the main window
                main = new MainWindow();
            main.menu_control.setAutoState();

            main.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            main.Loadgrid();
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string start;
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"), true))
            {
                start = sr.ReadLine();
            }
            if (start == "" || start == "yes")
            {
                var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                key.SetValue("MyApplication", System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                string path = @"Software\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue("MyApplication", false);
                    }
                }
            }

            File.Delete(System.IO.Path.Combine(docPath, "KeyStrokesApp\\bottmBar.txt"));
            File.WriteAllText(System.IO.Path.Combine(docPath, "KeyStrokesApp\\bottmBar.txt"), string.Empty);
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "KeyStrokesApp\\bottmBar.txt"), true))
            {
                string outStr = "";
                if (main.bottomBar.Visibility == Visibility.Hidden) 
                {
                    outStr = "no";
                }
                else
                {
                    outStr = "yes";
                }
                outputFile.WriteLine(outStr);
            }
        }
    }
}
