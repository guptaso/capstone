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
using System.Linq;
using MaterialDesignThemes.Wpf;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        public static Boolean currentInstance = false;
        private System.Windows.Forms.Screen currentScreen;
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

        public MainWindow()
        {
            InitializeComponent();

        }

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

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // Output all screens that the computer has
            // For the asus zenbook pro duo, there will be 2 screens, where the companion screen is index 1 and main screen is index 0
            // To determine dimensions of the second screen, all the screens on the user laptop will be outputted
            for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                Console.WriteLine("Screen: " + System.Windows.Forms.Screen.AllScreens[i]);

            // If there is only one screen, place it on the main screen
            // Otherwise, load it on the companion screen
            if (System.Windows.Forms.Screen.AllScreens.Length == 1)
                currentScreen = System.Windows.Forms.Screen.AllScreens[0];
            else
            {
                currentScreen = System.Windows.Forms.Screen.AllScreens[1];
                if (currentScreen != null)
                {
                    // Position this to the top of the second screen.  See the output logs for more info
                    this.Top = currentScreen.WorkingArea.Height;
                }
            }

        }

    }
}
