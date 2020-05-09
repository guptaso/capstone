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
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            key.SetValue("MyApplication", System.Reflection.Assembly.GetExecutingAssembly().Location);


            // make sure file and directory exists
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folderPath = Path.Combine(docPath, "KeyStrokesApp");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string appPath = Path.Combine(docPath, "KeyStrokesApp\\saveClicks.txt");
            string layoutPath = Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt");

            if (!File.Exists(appPath))
            {
                File.Create(appPath);
            }

            if (!File.Exists(layoutPath))
            {
                File.Create(layoutPath);
            }

            // to get the main window
            main = new MainWindow();
            main.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            main.Loadgrid();
        }
    }
}
