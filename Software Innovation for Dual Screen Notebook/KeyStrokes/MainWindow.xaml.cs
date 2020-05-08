using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.Xml;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Diagnostics;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private DateTime click_started;
        public static Boolean currentInstance = false;
        private System.Windows.Forms.Screen launchScreen;
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            // Output all processes currently running
            // Some processes' titles are "" for w/e reason, so those processes are excluded

            /*
            Console.WriteLine("\nViewing all current processes");
            var processes = Process.GetProcesses().Where(pr => (pr.MainWindowHandle != IntPtr.Zero && pr.MainWindowTitle != ""));
            IntPtr hWnd;
            foreach (var proc in processes)
            {
                Console.WriteLine(proc.MainWindowTitle);
                hWnd = FindWindow(null, proc.MainWindowTitle);
            }
            Console.WriteLine();
            */
        }

        // Initializes the window to not steal focus by default
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

            // this activates the window so that when we start it 
            // it does not jump to the back of all windows
            this.Activate();

            // Starts the previous layout
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string gridXaml = " ";

            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt"), true))
            {
                gridXaml = sr.ReadLine();
            }


            if (gridXaml != null)
            {
                StringReader stringReader = new StringReader(gridXaml);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                UniformGrid uniformGrid = (UniformGrid)XamlReader.Load(xmlReader);
                List<Button> buttonList = new List<Button>();
                foreach (Button btn in uniformGrid.Children)
                {
                    Console.WriteLine(btn.Content);
                    buttonList.Add(btn);
                }
                uniformGrid.Children.Clear();

                // Goes through and adds the app items
                List<string> buttonApps = new List<string>();
                List<string> buttonSC = new List<string>();
                List<string> buttonNames = new List<string>();
                using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveClicks.txt"), true))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        buttonNames.Add(line.Split('|')[0]);
                        buttonApps.Add(line.Split('|')[1]);
                        buttonSC.Add(line.Split('|')[2]);
                    }
                }

                foreach (Button btn in buttonList)
                {
                    int i = buttonNames.IndexOf(btn.Content.ToString());
                    if (i != -1)
                    {
                        List<VirtualKeyShort.Key> shortcut = new List<VirtualKeyShort.Key>();

                        Action<object, RoutedEventArgs> click = null;
                        click = (se, ev) =>
                        {
                            // Add Application

                            if (buttonApps[i].Length > 1)
                            {
                                Process cmd = new Process();
                                cmd.StartInfo.FileName = "cmd.exe";
                                cmd.StartInfo.RedirectStandardInput = true;
                                cmd.StartInfo.RedirectStandardOutput = true;
                                cmd.StartInfo.CreateNoWindow = true;   // true hides cmd prompt
                                cmd.StartInfo.UseShellExecute = false;
                                cmd.Start();
                                cmd.StandardInput.WriteLine(buttonApps[i]);
                                cmd.StandardInput.Flush();
                                cmd.StandardInput.Close();
                                cmd.WaitForExit();
                            }
                        };

                        List<string> shortcutStr = new List<string>();

                        for (int j = 0; j < buttonSC[i].Split(' ').Length - 1; j++)
                        {
                            shortcutStr.Add(buttonSC[i].Split(' ')[j]);
                        }
                        foreach (string sh in shortcutStr)
                        {
                            if (sh.Length > 1)
                            {
                                VirtualKeyShort.Key x = (VirtualKeyShort.Key)System.Enum.Parse(typeof(VirtualKeyShort.Key), sh);
                                shortcut.Add((VirtualKeyShort.Key)x);
                            }
                        }

                        if (shortcut.Count != 0)
                        {
                            List<VirtualKeyShort.Key> holder = new List<VirtualKeyShort.Key>(shortcut);
                            click += (se, ev) =>
                            {
                                Shortcut.send(holder.ToArray());
                            };
                        }
                        grid.addButton(btn.Content.ToString(), click);
                    }
                }
            }
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


        // Bottom bar, all the words should be changed to icons and made to look much much better
        private void media_back_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PREV_TRACK });
        }

        private void media_play_pause_Click(object sender, RoutedEventArgs e)
        {
            // this needs to be changed to accomidate actually checking if the 
            // music is playing to be correct all the time
            if (media_play_pause.Content.Equals("Play"))
            {
                media_play_pause.Content = new PackIcon { Kind = PackIconKind.PlayPause };
            }
            else
            {
                media_play_pause.Content = new PackIcon { Kind = PackIconKind.PlayPause };
            }
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_PLAY_PAUSE });
        }

        private void media_forward_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.MEDIA_NEXT_TRACK });
        }

        private void undo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_Z });
        }

        private void redo_Click(object sender, RoutedEventArgs e)
        {
            Shortcut.send(new VirtualKeyShort.Key[] { VirtualKeyShort.Key.CONTROL, VirtualKeyShort.Key.KEY_Y });
        }

        private void grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public void Loadgrid()
        {
            string gridXaml = XamlWriter.Save(grid.grid);
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //File.WriteAllText(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt"), string.Empty);
            File.Delete(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt"));
            File.WriteAllText(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt"), string.Empty);
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "KeyStrokesApp\\saveLayout.txt"), true))
            {
                outputFile.WriteLine(gridXaml);
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // Output all screens that the computer has
            // For the asus zenbook pro duo, there will be 2 screens, where the companion screen is index 1 and main screen is index 0
            // To determine dimensions of the second screen, all the screens on the user laptop will be outputted

            /*
             *  DPI for primary screen: sqrt(3840^2+2160^2) / 15.6" = 282.423996 pixels/inch
             *  Scale by the standard 96 pixels/inch 
             * 
             */

            /*
            for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                Console.WriteLine("Screen: " + System.Windows.Forms.Screen.AllScreens[i]);
            */

            // Get the system's DPI.  Determined based on scaling
            /*
             * 100% = 96
             * 125% = 120
             * 150% = 144
             * 175% = 168
             * 200% = 192
             * 225% = 216
             * 250% = 240
             * 300% = 288
             * 350% = 336
             */

            PresentationSource source = PresentationSource.FromVisual(this);
            double dpiX = 0, dpiY = 0;
            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }

            // Console.WriteLine("(" + dpiX + ", " + dpiY + ")");

            // If there is only one screen, place it on the main screen
            // Otherwise, load it on the companion screen
            if (System.Windows.Forms.Screen.AllScreens.Length == 1)
            {
                launchScreen = System.Windows.Forms.Screen.AllScreens[0];
                var workingArea = launchScreen.WorkingArea;
                //this.Top = 40 * (192.0f / dpiX);
                this.Top = (workingArea.Top + (workingArea.Height / 8))/dpiX;

            }
            else
            {
                // gets a list of all screens that are not primary and then
                // choses the first in that list to launch the app
                launchScreen = System.Windows.Forms.Screen.AllScreens.Where(screen => screen.Primary == false).FirstOrDefault();

                if (launchScreen != null)
                {

                    // Positon top of window near the top of the ScreenPad Plus
                    //// The scaling is basically the working height multiplied by the base scaling divided by the given DPI (base 192 with 200%)

                    // Position left so that it's near the center of the ScreenPad Plus
                    // The scaling is similar to Top's but we also have to subtract 240 because 1/4 of the width is aligned too far to the right

                    // THIS WAS ADDED BACK
                    //Console.WriteLine(launchScreen.Bounds.Width + ", " + launchScreen.Bounds.Height);
                    var workingArea = launchScreen.WorkingArea;
                    this.Left = workingArea.Left/dpiX;
                    this.Top = workingArea.Top/dpiY;
                }
            }
        }
    }
}
