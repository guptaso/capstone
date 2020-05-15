using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public sealed partial class GamingUseCase : Window
    {
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

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr before, IntPtr after, int X, int Y, int cx, int cy, uint flags);

        [DllImport("user32.dll")]
        public static extern bool LockSetForegroundWindow(uint uLockCode);

        [DllImport("user32.dll")]
        public static extern void DisableProcessWindowsGhosting();

        // Below are the private members of this function to store various pieces of data

        // Flag to determine if the add application form exists yet
        private Boolean isOpenedAddApplicationInstance = false;

        //Store list of hotkeys (in char) used atm (Cannot use the same hotkey in more than one button)
        private List<char> hotkeyCharList = new List<char>();

        //Also store list of hotkeys (in Key) used atm
        private List<Key> hotKeyList = new List<Key>();

        //Also store list of appLocationsList (mainly used for the dynamic button portion)
        private List<string> appLocationsList = new List<string>();

        //Also store list of images
        private List<string> imageList = new List<string>();

        //And also store a list of buttons
        private List<Button> buttonList = new List<Button>();

        //When right clicking a specific dynamic button, record that button
        private Button current;

        //Either display on the main screen if there is only one or display on the bottom screen if there are 2
        private System.Windows.Forms.Screen launchScreen;

        //Detect whether or not we have saved our layout (false for no, true for yes)
        private Boolean saveState = false;

        // Detect whether move button was clicked (if true, don't bring focus to the window)
        private Boolean moveClick = false;

        // Get the window handler of the started application
        private IntPtr hWnd;



        // Retrieve the file's icon
        public static ImageSource GetIcon(string fileName)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            return Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0, 0, icon.Width, icon.Height),
                        BitmapSizeOptions.FromEmptyOptions());
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

        public GamingUseCase()
        {

            InitializeComponent();

            // Your boy did it, he managed to KEKW the capstone project
            // (only if it exists tho :) )
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\Images\kekw.jpg"))
                this.Icon = BitmapFrame.Create(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\Images\kekw.jpg", UriKind.RelativeOrAbsolute));

            // Store the initial set of hotkeys (1 initially)
            hotkeyCharList.Add('+');
            hotKeyList.Add(Key.OemPlus);

            // Also add the applications
            // Ask user if they want to load a pre-existing configuration or not
            MessageBoxResult loadFile = MessageBox.Show("Would you like to load previously saved layouts?", "Load Applications", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (loadFile == MessageBoxResult.Yes)
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt"))
                {
                    MessageBox.Show("Error, something happened with the file.  Layouts cannot be loaded", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\");                                           // creates the folder in Desktop (does nothing if already created)
                    using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt")) ; // only going to create the file
                }
                else
                    LoadApplicationsFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt");
            }

            Background = MenuControl.currentBrush;
            saveState = true;                               // initially, we did not make changes to the window
        }

        //Add the appLocationsList via a file
        public void LoadApplicationsFromFile(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                //Read until the file is empty 

                //Step 1: read the application itself
                string application;
                while ((application = reader.ReadLine()) != null)
                {
                    //3 lines per application:
                    /*
                     *  1. Application Location
                     *  2. Image Location (unless n)
                     *  3. Hotkey
                     */

                    //If we encounter an empty string, stop
                    if (application == "")
                        break;

                    // Step 2
                    string imageLocation = reader.ReadLine();
                    if (imageLocation == "n")
                        imageLocation = "";

                    // Step 3
                    string key = reader.ReadLine();
                    char hotkey = Char.ToUpper(key[0]);

                    // Then load that button dynamically
                    if (!AddApplication(application, imageLocation, hotkey))
                        break;
                }
            }
        }

        // Actually add the application
        private Boolean AddApplication(string appLocation, string imageLocation, char hotkey)
        {

            // Before moving on, check two things: 
            // 1. The file location exists (unless discord or any other preset URL was provided)
            // 2. The image location exists (if provided)
            if (!appLocation.Contains("https://")/*appLocation != "https://discordapp.com" && appLocation != "https://discord.gg"*/)
            {
                // Outer if statement is not Discord, thus check if the file path exists
                if (!File.Exists(appLocation) || (!File.Exists(imageLocation) && imageLocation != ""))
                {
                    MessageBox.Show("Error, you have an invalid path in the file, we will no longer read other loaded layouts!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                if (imageLocation == "" || (!File.Exists(imageLocation) && imageLocation != ""))
                {
                    MessageBox.Show("Error, Pre-loaded URLS must have a local image", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            //Before adding, remove the text if it's already removed
            if (EmptyApplications.Visibility == Visibility.Visible)
                EmptyApplications.Visibility = Visibility.Hidden;

            //Adds the margin (Left, Top, Right, Bottom)
            Thickness buttonMargin = new Thickness(11, -75, 10, 0);
            Thickness textMargin = new Thickness(-80, 0, -5, -20);

            //Adds an image to the button
            //If no specified image was provided, then use the exe's icon instead.
            //Otherwise, use that specific image link
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapSource source;
            ImageSource sourceEmpty;
            if (imageLocation != "")
            {
                source = new BitmapImage(new Uri(imageLocation, UriKind.RelativeOrAbsolute));
                image.Source = source;
            }
            else
            {
                sourceEmpty = GetIcon(appLocation);
                image.Source = sourceEmpty;
            }
            image.Height = 72;
            image.Width = 75;
            System.Windows.Point point = new System.Windows.Point(0.455, -0.263);
            image.RenderTransformOrigin = point;

            //Add a text block
            TextBlock text = new TextBlock();
            text.VerticalAlignment = VerticalAlignment.Bottom;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.Margin = textMargin;
            text.Text = Char.ToUpper(hotkey) + "";
            text.Foreground = new SolidColorBrush(Colors.Navy);
            text.FontWeight = FontWeights.Bold;

            //Create a dock panel that holds both the Image and the TextBlock children
            DockPanel dock = new DockPanel();
            dock.Children.Add(image);
            dock.Children.Add(text);

            //Create a new button and give it keydown and click events
            Button newButton = new Button()
            {
                Height = 75,
                Width = 90,
                Margin = buttonMargin,
                Background = System.Windows.Media.Brushes.LightGray,
                ToolTip = "Right Click me in order to remove me or change my hotkey",
                Name = "DynamicButton"
            };
            newButton.KeyUp += DynamicButton_KeyUp;
            newButton.Click += (sender, e) => DynamicButton_Click(sender, e, appLocation);
            newButton.PreviewMouseRightButtonDown += (sender, e) => DynamicButton_RightClick(sender, e, newButton);

            //Contents of the button is simply whatever the dock is
            newButton.Content = dock;

            //Finally, the button is a part of the stack panel, which is a content of the scrollviewer
            //Located in the ScrollViewer with the name of ButtonViewholder
            MyStack.Children.Add(newButton);
            ButtonViewholder.Content = MyStack;


            //Determine which key was pressed and add it to the Key enum list

            //Is it a number?
            if (Char.IsDigit(hotkey))
            {
                if (hotkey == '1')
                    hotKeyList.Add(Key.D1);
                else if (hotkey == '2')
                    hotKeyList.Add(Key.D2);
                else if (hotkey == '3')
                    hotKeyList.Add(Key.D3);
                else if (hotkey == '4')
                    hotKeyList.Add(Key.D4);
                else if (hotkey == '5')
                    hotKeyList.Add(Key.D5);
                else if (hotkey == '6')
                    hotKeyList.Add(Key.D6);
                else if (hotkey == '7')
                    hotKeyList.Add(Key.D7);
                else if (hotkey == '8')
                    hotKeyList.Add(Key.D8);
                else if (hotkey == '9')
                    hotKeyList.Add(Key.D9);
                else
                    hotKeyList.Add(Key.D0);
            }

            //It must be a letter if it's not a number
            else
            {
                if (Char.ToUpper(hotkey) == 'A')
                    hotKeyList.Add(Key.A);
                else if (Char.ToUpper(hotkey) == 'B')
                    hotKeyList.Add(Key.B);
                else if (Char.ToUpper(hotkey) == 'C')
                    hotKeyList.Add(Key.C);
                else if (Char.ToUpper(hotkey) == 'D')
                    hotKeyList.Add(Key.D);
                else if (Char.ToUpper(hotkey) == 'E')
                    hotKeyList.Add(Key.E);
                else if (Char.ToUpper(hotkey) == 'F')
                    hotKeyList.Add(Key.F);
                else if (Char.ToUpper(hotkey) == 'G')
                    hotKeyList.Add(Key.G);
                else if (Char.ToUpper(hotkey) == 'H')
                    hotKeyList.Add(Key.H);
                else if (Char.ToUpper(hotkey) == 'I')
                    hotKeyList.Add(Key.I);
                else if (Char.ToUpper(hotkey) == 'J')
                    hotKeyList.Add(Key.J);
                else if (Char.ToUpper(hotkey) == 'K')
                    hotKeyList.Add(Key.K);
                else if (Char.ToUpper(hotkey) == 'L')
                    hotKeyList.Add(Key.L);
                else if (Char.ToUpper(hotkey) == 'M')
                    hotKeyList.Add(Key.M);
                else if (Char.ToUpper(hotkey) == 'N')
                    hotKeyList.Add(Key.N);
                else if (Char.ToUpper(hotkey) == 'O')
                    hotKeyList.Add(Key.O);
                else if (Char.ToUpper(hotkey) == 'P')
                    hotKeyList.Add(Key.P);
                else if (Char.ToUpper(hotkey) == 'Q')
                    hotKeyList.Add(Key.Q);
                else if (Char.ToUpper(hotkey) == 'R')
                    hotKeyList.Add(Key.R);
                else if (Char.ToUpper(hotkey) == 'S')
                    hotKeyList.Add(Key.S);
                else if (Char.ToUpper(hotkey) == 'T')
                    hotKeyList.Add(Key.T);
                else if (Char.ToUpper(hotkey) == 'U')
                    hotKeyList.Add(Key.U);
                else if (Char.ToUpper(hotkey) == 'V')
                    hotKeyList.Add(Key.V);
                else if (Char.ToUpper(hotkey) == 'W')
                    hotKeyList.Add(Key.W);
                else if (Char.ToUpper(hotkey) == 'X')
                    hotKeyList.Add(Key.X);
                else if (Char.ToUpper(hotkey) == 'Y')
                    hotKeyList.Add(Key.Y);
                else
                    hotKeyList.Add(Key.Z);

                hotkey = Char.ToUpper(hotkey);
            }

            //Before finishing up, add the hotkey to the list
            //and the location and the image
            hotkeyCharList.Add(hotkey);
            appLocationsList.Add(appLocation);
            imageList.Add(imageLocation);
            buttonList.Add(newButton);

            // Our application was successfully added.  Now change the saveState to false, as we have altered the form
            saveState = false;

            return true;
        }


        //Keys register on window in focus.
        private void KeyInteractor(object sender, KeyEventArgs e)
        {
            // triggering the +/= key
            if (e.Key == Key.OemPlus)
            {
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyUp(sender, e);
            }
            // triggering ctrl + s
            else if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Save_Click(sender, e);
            }
            // triggering alt + f4 or ctrl + qfz
            else if (e.Key == Key.Q && Keyboard.Modifiers == ModifierKeys.Control/* || (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.F4) || Keyboard.IsKeyDown(Key.RightAlt) && Keyboard.IsKeyDown(Key.F4))*/)
            {
                // We can't call CloseWindow function because this function's events does not include Cancel.  
                // To work around this issue, manually close the application ourselves.
                // Because we overloaded an OnClosingEvent called CloseWindow, the function, CloseWindow will then be called upon calling this.Close()
                this.Close();
            }
            // any other key
            else
            {
                e.Handled = true;
                DynamicButton_KeyUp(sender, e);
            }
        }

        // Save all current applications on the Window to the text file
        private void CloseWindow(object sender, CancelEventArgs e)
        {
            // If user changes their mind, then don't quit
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to quit?", "Confirm Closure", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.No)
                e.Cancel = true;

            // Otherwise, ask the user one more time if they want to save all applications onto a text file before leaving
            else
            {
                // If we didn't save before choosing to quit, now is the time to ask if the user wants to save their layouts before quitting
                if (!saveState)
                {
                    MenuControl.currentInstance = false;
                    MessageBoxResult save = MessageBox.Show("Do you want to save these layouts for future use?", "Save Applications?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (save == MessageBoxResult.Yes)
                        SaveApplications(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt");
                }

                //Clear the lists before quitting
                hotkeyCharList.Clear();
                hotKeyList.Clear();
                appLocationsList.Clear();
                imageList.Clear();
                buttonList.Clear();

                //Send back the flag to MenuControl to indicate that we are finished with this window
                MenuControl.currentInstance = false;
            }
        }


        private void SaveApplications(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                // Total # of apps is based on the # of appLocationsList
                for (int i = 0; i < appLocationsList.Count; i++)
                {
                    // First line: app location
                    writer.WriteLine(appLocationsList[i]);

                    // Second line: image location
                    string imageLocation = "";
                    for (int j = 0; j < imageList[i].Length; j++)
                    {
                        if (imageList[i][j] == ' ')
                            imageLocation += '$';
                        else
                            imageLocation += imageList[i][j];
                    }
                    if (imageLocation == "")
                        imageLocation = "n";
                    writer.WriteLine(imageLocation);

                    // Third line: text details (offset by 1 since first hotkey is +)
                    writer.WriteLine(hotkeyCharList[i + 1]);
                }
            }

            saveState = true;   // every time we save, this becomes True
        }


        // this opens a help window
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You will now be navigated to the GitHub repo where this was implemented.", "Navigating to GitHub");
            Process.Start("https://github.com/guptaso/capstone/wiki/Open-Applications-List-Menu-HELP");
        }

        // this clears all applications on the window, but only if they so choose
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (MyStack.Children.Count > 0)
            {
                MessageBoxResult confirm = MessageBox.Show("Are you sure you want to clear all applications?  This action cannot be undone", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    //Clear the lists
                    hotkeyCharList.Clear();
                    hotKeyList.Clear();
                    appLocationsList.Clear();
                    imageList.Clear();
                    buttonList.Clear();

                    //And contents inside the ScrollViewer
                    MyStack.Children.Clear();
                    ButtonViewholder.Content = MyStack;

                    //Finally, add the + button again
                    hotkeyCharList.Add('+');
                    hotKeyList.Add(Key.OemPlus);

                    //Redisplay the empty applications message since contents of the scrollviewer are cleared
                    EmptyApplications.Visibility = Visibility.Visible;

                    //Finally (for real this time), because the form was altered, change the saveState flag to false
                    saveState = false;
                }
            }
        }

        // this saves the layouts currently on the application
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Save the layouts by calling the Save_Applications function
            MessageBox.Show("Applications successfully saved!", "SAVE SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            SaveApplications(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt");
        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {

            // If there already exists an instance, then output an error message box
            if (isOpenedAddApplicationInstance)
            {
                MessageBox.Show("What are you doing?  You already have this form opened", "Add Form Already Opened", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Otherwise, open the form
            isOpenedAddApplicationInstance = true;
            AddApplication form1 = new AddApplication(this);
            form1.Show();
        }


        // Press the + button (shift =) to call via keydown
        private void Add_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                // If there already exists an instance, then output an error message box
                if (isOpenedAddApplicationInstance)
                {
                    MessageBox.Show("What are you doing?  You already have this form opened", "Add Form Already Opened", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Otherwise, open the form
                isOpenedAddApplicationInstance = true;
                AddApplication form1 = new AddApplication(this);
                form1.Show();
            }
        }

        // Once the add form is closed, the add application instance flag will be reset to false
        public void finishedAddApplicationForm()
        {
            isOpenedAddApplicationInstance = false;
        }

        //Takes in the form inputs from Form1.cs and dynamically adds a new button by appending it to the ScrollViewer
        public bool processFormInputs(string appLocation, string appImage, string appHotKey)
        {
            // Altered based on feedback: now allow for non-unique hotkeys
            // However, now we restrict buttons having more than 1 application
            for (int i = 0; i < appLocationsList.Count; i++)
            {
                if (appLocation == appLocationsList[i])
                {
                    MessageBox.Show("Please load a unique application on here", "NonUnique Application");
                    return false;
                }
            }


            //Otherwise form is valid and hotkey is unique, add the form.

            //Just displaying what will be added
            MessageBox.Show("Your application was successfully added!", "Button successfully created!");

            //Dynamically add button details
            return AddApplication(appLocation, appImage, appHotKey[0]);


        }

        private void StartProcess(string appLocation, RoutedEventArgs e)
        {

            string processName = "";
            int processId = 0;

            // After opening the app, reset the scrollviewer to the top
            ButtonViewholder.ScrollToLeftEnd();


            // Now actually create a new process: will require a try block to catch a few exceptions
            try
            {
                //Start the application at the 1st offset index and then stop searching
                Console.WriteLine("\nNumber of screens: " + System.Windows.Forms.Screen.AllScreens.Length);

                // Only PuTTY is causing issues: hard code it so PuTTY will always open at the top (not ideal but can't figure this out rn) 
                if (System.Windows.Forms.Screen.AllScreens.Length > 1 && OpenScreenPad.IsChecked == true/* && !appLocation.Contains("PuTTY")*/)
                {
                    Process openApplication = new Process();
                    openApplication.StartInfo.FileName = appLocation;
                    openApplication.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    openApplication.StartInfo.CreateNoWindow = true;
                    openApplication.Start();
                    Console.WriteLine("The id: " + openApplication.Id);
                    Console.WriteLine("Opened app: " + openApplication.ProcessName);
                    processName = openApplication.ProcessName;
                    processId = openApplication.Id;

                    //Thread.Sleep(2000);
                    openApplication.WaitForInputIdle();                         // If an opened application doesn't have GUI, then InvalidOperationException will be thrown.  Catch this and run it anyways
                    Console.WriteLine("\nViewing all current processes");
                    var processes = Process.GetProcesses().Where(pr => (pr.MainWindowHandle != IntPtr.Zero && pr.ProcessName == openApplication.ProcessName));

                    foreach (var proc in processes)
                    {
                        Console.WriteLine("Process: " + proc.MainWindowTitle + ", Id: " + proc.Id);
                        hWnd = FindWindow(null, proc.MainWindowTitle);
                        if (proc.Id == openApplication.Id)
                            break;
                    }


                    if (SetWindowPos(hWnd, IntPtr.Zero, launchScreen.WorkingArea.Width - (int)this.Width * 2, launchScreen.WorkingArea.Height * 2, 900, 900, 0x0020 | 0x0040)) // 3rd-6th parameters are left, top, width, height
                        Console.WriteLine("succeeded");
                    if (SetForegroundWindow(hWnd))
                        Console.WriteLine("another one succeeded");
                    if (ShowWindow(hWnd, 3))
                        Console.WriteLine("everything succeeded");
                    //DisableProcessWindowsGhosting();
                }

                else
                    Process.Start(appLocation);

                this.Activate();    // bring to foreground
            }
            // Some apps don't have GUI, thus this exception will occur.  However, we still want the app to be displayed
            catch (InvalidOperationException)
            {
                if (System.Windows.Forms.Screen.AllScreens.Length > 1 && OpenScreenPad.IsChecked == true)
                {
                    Thread.Sleep(1500);     // alternative to WaitForInputIdle() is to sleep for 1.5 seconds
                    Console.WriteLine("\nViewing all current processes");
                    var processes = Process.GetProcesses().Where(pr => (pr.MainWindowHandle != IntPtr.Zero && pr.ProcessName == processName));

                    foreach (var proc in processes)
                    {
                        Console.WriteLine("Process: " + proc.MainWindowTitle + ", Id: " + proc.Id);
                        hWnd = FindWindow(null, proc.MainWindowTitle);
                        if (proc.Id == processId)
                        {
                            hWnd = FindWindow(null, proc.MainWindowTitle);
                            break;
                        }
                    }

                    if (SetForegroundWindow(hWnd))
                        Console.WriteLine("another one succeeded");
                    if (SetWindowPos(hWnd, IntPtr.Zero, launchScreen.WorkingArea.Width - (int)this.Width * 2, launchScreen.WorkingArea.Height * 2, 900, 900, 0x0020)) // 3rd-6th parameters are left, top, width, height
                        Console.WriteLine("succeeded");
                    if (ShowWindow(hWnd, 3))
                        Console.WriteLine("everything succeeded");
                    //DisableProcessWindowsGhosting();


                }

                else
                    Process.Start(appLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, something happened with the application.  It cannot be loaded, thus it will be removed", "App location changed or removed", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Exception: " + ex.Message);

                // Before removing the button, update our current button to be the invalid one
                current = buttonList[appLocationsList.FindIndex(x => x == appLocation)];

                // Now remove it
                removeBtn(current, e);

                // Because an app was forcibly removed, make save state be false
                saveState = false;
            }


        }

        //Dynamic button's KeyUp event
        // Requires our window to be in focus!
        private void DynamicButton_KeyUp(object sender, KeyEventArgs e)
        {
            //Search for which hotkey was pressed (not including + since there's already an event for it)
            for (int i = 1; i < hotKeyList.Count; i++)
            {
                if (hotKeyList[i] == e.Key)
                {
                    // If the application cannot be opened for some reason, throw an exception
                    // Otherwise, open as normal

                    // Find the button
                    int buttonIndex = i - 1;

                    //Move the button so that it's at the front
                    //Two steps: remove the button and then restore it to the front

                    // Remove elements with that buttonIndex
                    // Just make sure to offset the two hotkeyLists by adding 1
                    // Since we're going to eventually add this back to the lists, create variables to store the removed values before restoring
                    Button tempButton = buttonList[buttonIndex];
                    buttonList.RemoveAt(buttonIndex);
                    string tempLocation = appLocationsList[buttonIndex];
                    appLocationsList.RemoveAt(buttonIndex);
                    string tempImage = imageList[buttonIndex];
                    imageList.RemoveAt(buttonIndex);
                    char tempCharKey = hotkeyCharList[buttonIndex + 1];
                    hotkeyCharList.RemoveAt(buttonIndex + 1);
                    Key tempKey = hotKeyList[buttonIndex + 1];
                    hotKeyList.RemoveAt(buttonIndex + 1);

                    // Then, remove from the stack
                    MyStack.Children.RemoveAt(buttonIndex);

                    // Add it back to the stack in the very front
                    MyStack.Children.Insert(0, tempButton);

                    // Then, restore the contents
                    buttonList.Insert(0, tempButton);
                    appLocationsList.Insert(0, tempLocation);
                    imageList.Insert(0, tempImage);
                    hotkeyCharList.Insert(1, tempCharKey);
                    hotKeyList.Insert(1, tempKey);

                    // Finally, update the scrollviewer with changes made to the stack
                    ButtonViewholder.Content = MyStack;

                    // And because our window content was changed, change save state to false
                    saveState = false;

                    // Finally start the process
                    StartProcess(appLocationsList[0], e);
                }
            }
        }

        //Dynamic button's Click event
        private void DynamicButton_Click(object sender, RoutedEventArgs e, string location)
        {
            // If the application cannot be opened for some reason, throw an exception
            // Otherwise, open as normal

            //Move the button so that it's at the front
            //Two steps: remove the button and then restore it to the front
            // Find the button
            int buttonIndex = appLocationsList.FindIndex(x => x == location);

            // Remove elements with that buttonIndex
            // Just make sure to offset the two hotkeyLists by adding 1
            // Since we're going to eventually add this back to the lists, create variables to store the removed values before restoring
            Button tempButton = buttonList[buttonIndex];
            buttonList.RemoveAt(buttonIndex);
            string tempLocation = appLocationsList[buttonIndex];
            appLocationsList.RemoveAt(buttonIndex);
            string tempImage = imageList[buttonIndex];
            imageList.RemoveAt(buttonIndex);
            char tempCharKey = hotkeyCharList[buttonIndex + 1];
            hotkeyCharList.RemoveAt(buttonIndex + 1);
            Key tempKey = hotKeyList[buttonIndex + 1];
            hotKeyList.RemoveAt(buttonIndex + 1);

            // Then, remove from the stack
            MyStack.Children.RemoveAt(buttonIndex);

            // Add it back to the stack in the very front
            MyStack.Children.Insert(0, tempButton);

            // Then, restore the contents
            buttonList.Insert(0, tempButton);
            appLocationsList.Insert(0, tempLocation);
            imageList.Insert(0, tempImage);
            hotkeyCharList.Insert(1, tempCharKey);
            hotKeyList.Insert(1, tempKey);

            // Finally, update the scrollviewer with changes made to the stack
            ButtonViewholder.Content = MyStack;

            // And because our window content was changed, change save state to false
            saveState = false;

            // Finally start the process
            StartProcess(location, e);
        }

        // Right click an added application to view the menu of removing or changing hotkeys
        private void DynamicButton_RightClick(object sender, MouseButtonEventArgs e, Button currentButton)
        {
            // Record the current instance of the button once it's been clicked
            current = currentButton;

            // For usability, do work to show the hotkey 

            // Get the dockpanel of the button
            DockPanel getDock = (DockPanel)current.Content;

            // Then, extract the Textblock's text
            TextBlock getText = (TextBlock)getDock.Children[1];
            rBtn.Content = "Remove " + getText.Text;

            // Finally, show the button menu 
            btnMenu.Visibility = Visibility.Visible;
        }

        // Allows us to scroll using the mouse wheel (thanks Stack Overflow)
        private void ScrollHorizontally(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollMe = sender as ScrollViewer;

            if (e.Delta > 0)
                scrollMe.PageLeft();
            else
                scrollMe.PageRight();
            e.Handled = true;
        }

        // Close the menu
        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            btnMenu.Visibility = Visibility.Hidden;
        }

        // Change the hotkey
        private void changeBtn(object sender, RoutedEventArgs e)
        {

            // Open the input box
            string prompt = Microsoft.VisualBasic.Interaction.InputBox("What alphanumeric hotkey would you like to change to?", "Change Hotkey");

            // Cancel button means the prompt is empty
            if (prompt == "")
            {
                btnMenu.Visibility = Visibility.Hidden;
                return;
            }

            // Otherwise, some erroneous input was entered: hotkey input > 1 or hotkey input is NOT alphanumeric
            while (prompt.Length != 1 || !Char.IsLetterOrDigit(prompt[0]))
            {
                // Display which error was detected
                if (prompt.Length != 1)
                    MessageBox.Show("ERROR: Please only have 1 character on the input field", "More than 1 character", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("ERROR: Please use an alphanumeric character", "Non-alphanumeric character", MessageBoxButton.OK, MessageBoxImage.Error);

                // Reprompt the user
                prompt = Microsoft.VisualBasic.Interaction.InputBox("What hotkey would you like to change to?");

                // Handle the cancel button case
                if (prompt == "")
                {
                    btnMenu.Visibility = Visibility.Hidden;
                    return;
                }
            }

            Char upperPrompt = Char.ToUpper(prompt[0]);

            // First, find the button from the Stackpanel 
            int buttonIndex = buttonList.FindIndex(x => x == current);

            // If the new hotkey is the same as the previous, then don't bother changing (but still close the button menu)
            if (upperPrompt == hotkeyCharList[buttonIndex + 1])
            {
                btnMenu.Visibility = Visibility.Hidden;
                return;
            }

            // Change the hotkeys lists (remember to offset by 1 because + is also a recognized command)
            hotkeyCharList[buttonIndex + 1] = upperPrompt;
            if (Char.ToUpper(upperPrompt) == 'A')
                hotKeyList[buttonIndex + 1] = Key.A;
            else if (Char.ToUpper(upperPrompt) == 'B')
                hotKeyList[buttonIndex + 1] = Key.B;
            else if (Char.ToUpper(upperPrompt) == 'C')
                hotKeyList[buttonIndex + 1] = Key.C;
            else if (Char.ToUpper(upperPrompt) == 'D')
                hotKeyList[buttonIndex + 1] = Key.D;
            else if (Char.ToUpper(upperPrompt) == 'E')
                hotKeyList[buttonIndex + 1] = Key.E;
            else if (Char.ToUpper(upperPrompt) == 'F')
                hotKeyList[buttonIndex + 1] = Key.F;
            else if (Char.ToUpper(upperPrompt) == 'G')
                hotKeyList[buttonIndex + 1] = Key.G;
            else if (Char.ToUpper(upperPrompt) == 'H')
                hotKeyList[buttonIndex + 1] = Key.H;
            else if (Char.ToUpper(upperPrompt) == 'I')
                hotKeyList[buttonIndex + 1] = Key.I;
            else if (Char.ToUpper(upperPrompt) == 'J')
                hotKeyList[buttonIndex + 1] = Key.J;
            else if (Char.ToUpper(upperPrompt) == 'K')
                hotKeyList[buttonIndex + 1] = Key.K;
            else if (Char.ToUpper(upperPrompt) == 'L')
                hotKeyList[buttonIndex + 1] = Key.L;
            else if (Char.ToUpper(upperPrompt) == 'M')
                hotKeyList[buttonIndex + 1] = Key.M;
            else if (Char.ToUpper(upperPrompt) == 'N')
                hotKeyList[buttonIndex + 1] = Key.N;
            else if (Char.ToUpper(upperPrompt) == 'O')
                hotKeyList[buttonIndex + 1] = Key.O;
            else if (Char.ToUpper(upperPrompt) == 'P')
                hotKeyList[buttonIndex + 1] = Key.P;
            else if (Char.ToUpper(upperPrompt) == 'Q')
                hotKeyList[buttonIndex + 1] = Key.Q;
            else if (Char.ToUpper(upperPrompt) == 'R')
                hotKeyList[buttonIndex + 1] = Key.R;
            else if (Char.ToUpper(upperPrompt) == 'S')
                hotKeyList[buttonIndex + 1] = Key.S;
            else if (Char.ToUpper(upperPrompt) == 'T')
                hotKeyList[buttonIndex + 1] = Key.T;
            else if (Char.ToUpper(upperPrompt) == 'U')
                hotKeyList[buttonIndex + 1] = Key.U;
            else if (Char.ToUpper(upperPrompt) == 'V')
                hotKeyList[buttonIndex + 1] = Key.V;
            else if (Char.ToUpper(upperPrompt) == 'W')
                hotKeyList[buttonIndex + 1] = Key.W;
            else if (Char.ToUpper(upperPrompt) == 'X')
                hotKeyList[buttonIndex + 1] = Key.X;
            else if (Char.ToUpper(upperPrompt) == 'Y')
                hotKeyList[buttonIndex + 1] = Key.Y;
            else if (Char.ToUpper(upperPrompt) == 'Z')
                hotKeyList[buttonIndex + 1] = Key.Z;
            else if (upperPrompt == '1')
                hotKeyList[buttonIndex + 1] = Key.D1;
            else if (upperPrompt == '2')
                hotKeyList[buttonIndex + 1] = Key.D2;
            else if (upperPrompt == '3')
                hotKeyList[buttonIndex + 1] = Key.D3;
            else if (upperPrompt == '4')
                hotKeyList[buttonIndex + 1] = Key.D4;
            else if (upperPrompt == '5')
                hotKeyList[buttonIndex + 1] = Key.D5;
            else if (upperPrompt == '6')
                hotKeyList[buttonIndex + 1] = Key.D6;
            else if (upperPrompt == '7')
                hotKeyList[buttonIndex + 1] = Key.D7;
            else if (upperPrompt == '8')
                hotKeyList[buttonIndex + 1] = Key.D8;
            else if (upperPrompt == '9')
                hotKeyList[buttonIndex + 1] = Key.D9;
            else
                hotKeyList[buttonIndex + 1] = Key.D0;

            // Change the button display
            // First, the content of the button (called current) is the Dockpanel
            DockPanel changeDock = (DockPanel)current.Content;

            // Second, the 2nd child of the dockpanel is the text (see AddApplication to see the Child order)
            TextBlock changeText = (TextBlock)changeDock.Children[1];
            changeText.Text = upperPrompt + "";
            changeDock.Children[1] = changeText;
            current.Content = changeDock;

            // Third, replace the stack's child and update scrollviewer
            MyStack.Children[buttonIndex] = current;
            ButtonViewholder.Content = MyStack;

            // Finally, close the button menu
            btnMenu.Visibility = Visibility.Hidden;

            // And change the save state to false, since we altered the form
            saveState = false;
        }

        // Remove button from the application window
        private void removeBtn(object sender, RoutedEventArgs e)
        {
            // Find the button
            int buttonIndex = buttonList.FindIndex(x => x == current);

            // Remove elements with that buttonIndex
            // Just make sure to offset the two hotkeyLists by adding 1
            buttonList.RemoveAt(buttonIndex);
            appLocationsList.RemoveAt(buttonIndex);
            imageList.RemoveAt(buttonIndex);
            hotkeyCharList.RemoveAt(buttonIndex + 1);
            hotKeyList.RemoveAt(buttonIndex + 1);

            // Remove from the actual scrollviewer
            MyStack.Children.RemoveAt(buttonIndex);
            ButtonViewholder.Content = MyStack;

            // If the stack is empty, then redisplay that empty text
            if (MyStack.Children.Count == 0)
                EmptyApplications.Visibility = Visibility.Visible;

            // After removing the button completely, hide the menu
            btnMenu.Visibility = Visibility.Hidden;

            // Finally, because our layouts were altered, change the save state to false
            saveState = false;

        }

        // Another way to close the btn menu is to click anywhere else
        private void CloseBtnMenu(object sender, MouseButtonEventArgs e)
        {
            // If we clicked on the remove button, then we'll have to hide it anyways, 
            // but we want to keep it visible so that the application will actually be removed
            // Same thing for the change hotkey button to edit hotkeys before hiding the menu
            if (e.Source != rBtn && e.Source != chBtn)
                btnMenu.Visibility = Visibility.Hidden;
        }


        /*
         * Utility Functions that are obscure and not as obvious when interacting with the application
         * Deals with events that happen upon loading the window, force focus on the window when clicked on
         * Also have the ability to manually move the window w/o using the Title Bar, albeit must be done by holding onto a button
         */

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
                this.Top = (workingArea.Top + (workingArea.Height / 8)) / dpiX;

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

                    //Console.WriteLine(launchScreen.Bounds.Width + ", " + launchScreen.Bounds.Height);
                    var workingArea = launchScreen.WorkingArea;
                    this.Left = workingArea.Left / dpiX;
                    this.Top = workingArea.Top / dpiY;
                }
            }
        }

        // Touch/click anywhere on the Window (but not on any of the buttons) to focus on the window
        private void ForceFocusOnWindow(object sender, MouseEventArgs e)
        {
            // If we tap anywhere outside the move button, restore focus
            if (!moveClick)
                this.Activate();
            moveClick = false;
        }

        // Click and hold anywhere on the MOVE button to move the window
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            moveClick = true;                                                // Don't bring the window to focus when clicking/dragging the move button
                                                                             // set to true to indicate that it was used already
                                                                             // will call ForceFocusOnWindow afterwards

            // If we're holding onto the MOVE button, then we can now move the window wrt the move button's position on the computer
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // Touch and hold anywhere on the MOVE button to move the window
        private void MoveWindowTouch(object sender, TouchEventArgs e)
        {
            moveClick = false;                              // basically, this is set to false because it doesn't call ForceFocusOnWindow implicitly afterwards
            this.CaptureTouch(e.TouchDevice);
        }
    }
}
