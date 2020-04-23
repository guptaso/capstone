using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class GamingUseCase: Window
    {
        public static Boolean finished;
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

        //Store list of hotkeys (in char) used atm (Cannot use the same hotkey in more than one button)
        private List<char> hotkeyCharList = new List<char>();

        //Also store list of hotkeys (in Key) used atm
        private List<Key> hotKeyList = new List<Key>();

        //Also store list of locations (mainly used for the dynamic button portion)
        private List<string> locations = new List<string>();

        //Also store list of images
        private List<string> imageList = new List<string>();

        //And also store a list of buttons
        private List<Button> buttonList = new List<Button>();

        //When right clicking a specific dynamic button, record that button
        private Button current;

        //Either display on the main screen if there is only one or display on the bottom screen if there are 2
        System.Windows.Forms.Screen currentScreen;

        /*
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        static public Key ResolveKey(char charToResolve)
        {
            return KeyInterop.KeyFromVirtualKey(VkKeyScan(charToResolve));
        }*/

        // Retrieve an icon
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

            //Your boy did it, he managed to KEKW the capstone project
            this.Icon = BitmapFrame.Create(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\Images\kekw.jpg", UriKind.RelativeOrAbsolute));

            //Store the initial set of hotkeys (1 initially)
            hotkeyCharList.Add('+');
            hotKeyList.Add(Key.OemPlus);

            // Also add the applications
            // Ask user if they want to load a pre-existing configuration or not
            MessageBoxResult loadFile = MessageBox.Show("Would you like to load previously saved layouts?", "Load Applications", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (loadFile == MessageBoxResult.Yes)
            {
                if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt"))
                {
                    MessageBox.Show("Error, something happened with the file.  Layouts cannot be loaded", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\");
                    using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt")); // only going to create the file
                }
                else
                    LoadApplicationsFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt");
            }
        }

        //Add the locations via a file
        public void LoadApplicationsFromFile(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                //Read until the file is empty 

                //Step 1: read the application itself
                string application;
                while ((application = reader.ReadLine()) != null)
                {
                    //4 lines per application:
                    /*
                     *  1. Application name
                     *  2. Button height, width, and margins 
                     *  3. Image height, width, transformation origins, and location (unless n)
                     *  4. Text margins
                     */

                    //If we encounter an empty string, stop
                    if (application == "")
                        break;

                    // For steps 2-4, we need a tokenizer
                    char[] separator = { ' ' };

                    // Step 2 (requires tokenizing)
                    string[] buttonInfo = reader.ReadLine().Split(separator);
                    int buttonHeight = Int32.Parse(buttonInfo[0]);
                    int buttonWidth = Int32.Parse(buttonInfo[1]);
                    int buttonMarginOne = Int32.Parse(buttonInfo[2]);
                    int buttonMarginTwo = Int32.Parse(buttonInfo[3]);

                    // Step 3 (requires tokenizing)
                    string[] imageInfo = reader.ReadLine().Split(separator);
                    int imageHeight = Int32.Parse(imageInfo[0]);
                    int imageWidth = Int32.Parse(imageInfo[1]);
                    double originOne = Double.Parse(imageInfo[2]);
                    double originTwo = Double.Parse(imageInfo[3]);
                    string imageLocation = "";
                    for(int i = 0; i < imageInfo[4].Length; i++)
                    {
                        if (imageInfo[4][i] == '$')
                            imageLocation += ' ';
                        else
                            imageLocation += imageInfo[4][i];
                    }
                    if (imageLocation == "n")
                        imageLocation = "";

                    // Step 4 (requires parsing)
                    string[] textInfo = reader.ReadLine().Split(separator);
                    int textMarginOne = Int32.Parse(textInfo[0]);
                    int textMarginTwo = Int32.Parse(textInfo[1]);
                    int textMarginThree = Int32.Parse(textInfo[2]);
                    int textMarginFour = Int32.Parse(textInfo[3]);
                    char hotkey = textInfo[4][0];

                    // Then load that button dynamically
                    if (!AddApplication(application,
                                    buttonHeight, buttonWidth, buttonMarginOne, buttonMarginTwo,
                                        imageLocation, imageHeight, imageWidth, originOne, originTwo,
                                            textMarginOne, textMarginTwo, textMarginThree, textMarginFour, hotkey))
                        break;
                }
            }
        }

        // Actually add the application
        private Boolean AddApplication(string appLocation,
                                        int buttonHeight, int buttonWidth, int buttonMarginOne, int buttonMarginTwo,
                                            string imageLocation, int imageHeight, int imageWidth, double originOne, double originTwo,
                                                int textMarginOne, int textMarginTwo, int textMarginThree, int textMarginFour, char hotkey)
        {

            // Before moving on, check two things: 
            // 1. The file location exists (unless discord was provided)
            // 2. The image location exists (if provided)
            if(appLocation != "https://discordapp.com" && appLocation != "https://discord.gg")
            {
                // Outer if statement is not Discord, thus check if the file path exists
                if (!File.Exists(appLocation) || (!File.Exists(imageLocation) && imageLocation != ""))
                {
                    MessageBox.Show("Error, you have an invalid path in the file, we will no longer read other loaded layouts!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            //Before adding, remove the text if it's already removed
            if (EmptyApplications.Visibility == Visibility.Visible)
                EmptyApplications.Visibility = Visibility.Hidden;

            //Adds the margin (Left, Top, Right, Bottom)
            Thickness buttonMargin = new Thickness(buttonMarginOne, 0, buttonMarginTwo, 0);
            Thickness textMargin = new Thickness(textMarginOne, textMarginTwo, textMarginThree, textMarginFour);

            //Adds an image to the button (WIP)
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
            //BitmapSource source = new BitmapImage(new Uri(appImage, UriKind.RelativeOrAbsolute));
            //image.Source = source;
            image.Height = imageHeight;
            image.Width = imageWidth;
            System.Windows.Point point = new System.Windows.Point(originOne, originTwo);
            image.RenderTransformOrigin = point;

            //Add a text block
            TextBlock text = new TextBlock();
            text.VerticalAlignment = VerticalAlignment.Bottom;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.Margin = textMargin;
            text.Text = Char.ToUpper(hotkey) + "";

            //Create a dock panel that holds both the Image and the TextBlock children
            DockPanel dock = new DockPanel();
            dock.Children.Add(image);
            dock.Children.Add(text);

            

            //Create a new button and give it keydown and click events
            Button newButton = new Button()
            {
                Height = buttonHeight,
                Width = buttonWidth,
                Margin = buttonMargin,
                Background = System.Windows.Media.Brushes.LightGray,
            };
            newButton.KeyDown += DynamicButton_KeyDown;
            newButton.Click += (sender, e) => DynamicButton_Click(sender, e, appLocation);
            newButton.PreviewMouseRightButtonDown += (sender, e) => DynamicButton_RightClick(sender, e, newButton);

            /*
            var border = new Border
            {
                Background = System.Windows.Media.Brushes.GhostWhite,
                BorderBrush = System.Windows.Media.Brushes.Silver,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(10),
                
            };


            dock.Children.Add(border);
            */


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
            locations.Add(appLocation);
            imageList.Add(imageLocation);
            buttonList.Add(newButton);

            return true;
        }


        //Keys register on window.
        private void KeyInteractor(object sender, KeyEventArgs e)
        {
            // triggering the +/= key
            if (e.Key == Key.OemPlus)
            {
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyUp(sender, e);
            }
            // any other key
            else
            {
                e.Handled = true;
                DynamicButton_KeyDown(sender, e);
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
                MenuControl.currentInstance = false;
                MessageBoxResult save = MessageBox.Show("Do you want to save these layouts for future use?", "Save Applications?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(save == MessageBoxResult.Yes)
                    SaveApplications(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CS66B_Project\SavedApplications.txt");
            }
        }


        private void SaveApplications(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                // Total # of apps is based on the # of locations
                for (int i = 0; i < locations.Count; i++)
                {
                    // First line: app location
                    writer.WriteLine(locations[i]);

                    // Second line: button details
                    writer.WriteLine("75 90 11 10");

                    // Third line: image detail
                    string imageLocation = "";
                    for(int j = 0; j < imageList[i].Length; j++)
                    {
                        if (imageList[i][j] == ' ')
                            imageLocation += '$';
                        else
                            imageLocation += imageList[i][j];
                    }
                    if (imageLocation == "")
                        imageLocation = "n";
                    writer.WriteLine("72 75 0.455 -0.263 " + imageLocation);

                    // Fourth line: text details (offset by 1 since first hotkey is +)
                    writer.WriteLine("-80 0 -5 -20 " + hotkeyCharList[i + 1]);
                }
            }

            //Clear the lists before quitting
            hotkeyCharList.Clear();
            hotKeyList.Clear();
            locations.Clear();
            imageList.Clear();
            buttonList.Clear();
        }


        // this opens the help window purely for showing how to work this application
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to clear all applications?  This action cannot be undone", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                //Clear the lists
                hotkeyCharList.Clear();
                hotKeyList.Clear();
                locations.Clear();
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
            }
        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // If we already have an instance of the application running, then don't run another instance
            if (finished)
            {
                MessageBox.Show("What are you doing?  You have an instance of this window open already!", "Already opened");
                return;
            }

            // Alternatively, if we used up all alphanumeric keys, then we can't add anymore
            if (hotkeyCharList.Count == 37)
            {
                MessageBox.Show("You cannot add anymore applications, there are no more hotkeys to assign", "Application Capacity Full");
                return;
            }

            // Otherwise, open the form
            finished = true;
            AddApplication form1 = new AddApplication(this);
            form1.Show();
        }

        // Press the + button (shift =) to call via keydown
        private void Add_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                // If we already have an instance of the application running, then don't run another instance
                if (finished)
                {
                    MessageBox.Show("What are you doing?  You have an instance of this window open already!", "Already opened");
                    return;
                }

                // Alternatively, if we used up all alphanumeric keys, then we can't add anymore
                if(hotkeyCharList.Count == 37)
                {
                    MessageBox.Show("You cannot add anymore applications, there are no more hotkeys to assign", "Application Capacity Full");
                    return;
                }

                // Otherwise, open the form
                finished = true;
                AddApplication form1 = new AddApplication(this);
                form1.Show();
            }
        }

        //Takes in the form inputs from Form1.cs and dynamically adds a new button by appending it to the ScrollViewer
        public bool processFormInputs(string appLocation, string appImage, string appHotKey)
        {

            //First go through the current list of hotkeys. 
            //If at least one hotkey matches, then return false
            for (int i = 0; i < hotkeyCharList.Count; i++)
            {
                if (Char.ToUpper(appHotKey[0]) == hotkeyCharList[i])
                {
                    MessageBox.Show("Please use a different hotkey, it needs to be unique", "NonUnique Hotkey");
                    return false;
                }
            }

            //Otherwise form is valid and hotkey is unique, add the form.


            //Just displaying what will be added
            MessageBox.Show("Your application was successfully added!", "Button successfully created!");

            //Dynamically add button details
            AddApplication(appLocation,
                                75, 90, 11, 10,
                                    appImage, 72, 75, 0.455, -0.263,
                                        -80, 0, -5, -20, appHotKey[0]);

            return true;


        }

        //Dynamic button's KeyDown event
        private void DynamicButton_KeyDown(object sender, KeyEventArgs e)
        {
            //Search for which hotkey was pressed (not including + since there's already an event for it)
            for (int i = 1; i < hotKeyList.Count; i++)
            {
                if (hotKeyList[i] == e.Key)
                {
                    //Start the application at the 1st offset index and then stop searching
                    Process.Start(locations[i - 1]);


                    //Move the button so that it's at the front
                    //Two steps: remove the button and then restore it to the front
                    
                    // Find the button
                    int buttonIndex = i - 1;

                    // Remove elements with that buttonIndex
                    // Just make sure to offset the two hotkeyLists by adding 1
                    // Since we're going to eventually add this back to the lists, create variables to store the removed values before restoring
                    Button tempButton = buttonList[buttonIndex];
                    buttonList.RemoveAt(buttonIndex);
                    string tempLocation = locations[buttonIndex];
                    locations.RemoveAt(buttonIndex);
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
                    locations.Insert(0, tempLocation);
                    imageList.Insert(0, tempImage);
                    hotkeyCharList.Insert(1, tempCharKey);
                    hotKeyList.Insert(1, tempKey);

                    // Finally, update the scrollviewer with changes made to the stack
                    ButtonViewholder.Content = MyStack;

                    break;
                }
            }
        }

        //Dynamic button's Click event
        private void DynamicButton_Click(object sender, RoutedEventArgs e, string location)
        {
            Process.Start(location);

            //Move the button so that it's at the front
            //Two steps: remove the button and then restore it to the front

            // Find the button
            int buttonIndex = locations.FindIndex(x => x == location);

                 // Remove elements with that buttonIndex
            // Just make sure to offset the two hotkeyLists by adding 1
            // Since we're going to eventually add this back to the lists, create variables to store the removed values before restoring
            Button tempButton = buttonList[buttonIndex];
            buttonList.RemoveAt(buttonIndex);
            string tempLocation = locations[buttonIndex];
            locations.RemoveAt(buttonIndex);
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
            locations.Insert(0, tempLocation);
            imageList.Insert(0, tempImage);
            hotkeyCharList.Insert(1, tempCharKey);
            hotKeyList.Insert(1, tempKey);

            // Finally, update the scrollviewer with changes made to the stack
            ButtonViewholder.Content = MyStack;
        }

        private void DynamicButton_RightClick(object sender, MouseButtonEventArgs e, Button currentButton)
        {
            // Record the current instance of the button once it's been clicked
            current = currentButton;
            btnMenu.Visibility = Visibility.Visible;
        }

        private void ScrollHorizontally(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollMe = sender as ScrollViewer;

            if (e.Delta > 0)
                scrollMe.PageLeft();
            else
                scrollMe.PageRight();
            e.Handled = true;
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            btnMenu.Visibility = Visibility.Hidden;
        }

        private void removeBtn(object sender, RoutedEventArgs e)
        {
            // Find the button
            int buttonIndex = buttonList.FindIndex(x => x == current);

            // Remove elements with that buttonIndex
            // Just make sure to offset the two hotkeyLists by adding 1
            buttonList.RemoveAt(buttonIndex);
            locations.RemoveAt(buttonIndex);
            imageList.RemoveAt(buttonIndex);
            hotkeyCharList.RemoveAt(buttonIndex + 1);
            hotKeyList.RemoveAt(buttonIndex + 1);

            // Finally, remove from the actual scrollviewer
            MyStack.Children.RemoveAt(buttonIndex);
            ButtonViewholder.Content = MyStack;

            // If the stack is empty, then redisplay that empty text
            if(MyStack.Children.Count == 0)
                EmptyApplications.Visibility = Visibility.Visible;

            // After removing the button completely, hide the menu
            btnMenu.Visibility = Visibility.Hidden;

        }

        // Another way to close the btn menu is to click anywhere else
        private void CloseBtnMenu(object sender, MouseButtonEventArgs e)
        {
            // If we clicked on the remove button, then we'll have to hide it anyways, 
            // but we want to keep it visible so that the application will actually be removed
            if(e.Source != rBtn)
                btnMenu.Visibility = Visibility.Hidden;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // If there is only one screen, place it on the main screen
            // Otherwise, load it on the companion screen
            if (System.Windows.Forms.Screen.AllScreens.Length == 1)
                currentScreen = System.Windows.Forms.Screen.AllScreens[0];
            else
            {
                // If the second screen exists, then set the application to the top.
                currentScreen = System.Windows.Forms.Screen.AllScreens[1];
                if (currentScreen != null)
                    this.Top = currentScreen.WorkingArea.Height;
            }
            
        }
    }
}
