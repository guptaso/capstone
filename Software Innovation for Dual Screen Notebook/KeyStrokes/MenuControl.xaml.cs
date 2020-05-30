using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace KeyStrokes
{
    public partial class MenuControl : UserControl
    {

        private MainWindow main;
        public static Boolean currentInstance = false;
        public static SolidColorBrush currentBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD4D4E4");
        public static double opacity = 1.0;
        public static GamingUseCase game_window;

        public MenuControl()
        {
            InitializeComponent();
            main = ((MainWindow)App.Current.MainWindow);
            currentBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD4D4E4"));

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (addButton.Visibility == Visibility.Visible)
            {
                addButton.Visibility = Visibility.Hidden;
            }
            else
            {
                menu.Visibility = Visibility.Hidden;
            }
        }

        private void add_button_Click(object sender, RoutedEventArgs e)
        {
            addButton.Visibility = Visibility.Visible;
            addButton.Open();
        }

        private void open_gaming_case(object sender, RoutedEventArgs e)
        {
            // If the application is already open, then don't open another instance...
            if (currentInstance)
            {
                MessageBox.Show("What are you doing?  You have an instance of this window open already!", "Already opened");
                return;
            }
            currentInstance = true;
            game_window = new GamingUseCase();
            game_window.Show();

            // After showing the gaming use case window, the behavior will force both windows' opacities to be the same
            // To prevent this behavior, manually change the main window's background brush and opacity
            background_design_change_nosender();
        }

        private void layout_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)layout_box.SelectedItem;
            string name = layout_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            string[] x = name.ToString().Split('x');
            string a = x[1];
            string b = "";
            if (x.Length > 2)
            {
                b = x[2];
                a = a.ToString().Split(' ')[1];
                main.grid.set_grid(Int16.Parse(a), Int16.Parse(b));
            }
        }

        private void bottom_bar_Click(object sender, RoutedEventArgs e)
        {


            if (main.bottomBar.Visibility == Visibility.Hidden)
            {
                main.bottomBar.Visibility = Visibility.Visible;
            }
            else
            {
                main.bottomBar.Visibility = Visibility.Hidden;
            }

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Another case where right clicking caused an exception
            try
            {
                main.DragMove();
            }
            catch (Exception) {}
        }

        private void background_design_change(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem ComboItem = (ComboBoxItem)background_design_box.SelectedItem;
            string name = background_design_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            string[] selectedVal = name.ToString().Split(' ');
            if (selectedVal.Length > 1)
            {
                Trace.WriteLine("Result: " + selectedVal[1]);
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(selectedVal[1]);
                SolidColorBrush brush = new SolidColorBrush(color);
                if (selectedVal[1] != "Transparent")
                    currentBrush = brush;
                else
                    currentBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD4D4E4");
                main.Background = brush;
            }

            // This function is called before the main window is loaded
            // Prevent an exception from happening by checking if main != null
            // If so, keep the current opacity value while the background is changed
            if (main != null)
                main.Background.Opacity = opacity;

            // For gaming window, also change the background and force opacity to be 1
            if (game_window != null)
            {
                // Use a different brush to set gaming_window's background (same color, different brush)
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(selectedVal[1]);
                SolidColorBrush brush = new SolidColorBrush(color);
                if (selectedVal[1] != "Transparent")
                    game_window.Background = brush;
                else
                    game_window.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD4D4E4");

                // Opacity must be 1 since gaming_window does not allow transparency
                game_window.Background.Opacity = 1;
            }
        }

        // This is a duplicate of background_design_change, except with no arguments.
        // Have to resort to this because GamingUseCase doesn't have transparent background, thus the opacity MUST stay the same
        // However, adjusting the opacity of it (or changing the background) resorts to both windows having the same opacity, which is NOT intended
        // Thus, we have to change the brush assignment so that both windows' opacity levels don't affect each other
        private void background_design_change_nosender()
        {
            ComboBoxItem ComboItem = (ComboBoxItem)background_design_box.SelectedItem;
            string name = background_design_box.SelectedItem.ToString();
            Trace.WriteLine(name.ToString());
            string[] selectedVal = name.ToString().Split(' ');
            if (selectedVal.Length > 1)
            {
                Trace.WriteLine("Result: " + selectedVal[1]);
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(selectedVal[1]);
                SolidColorBrush brush = new SolidColorBrush(color);
                if (selectedVal[1] != "Transparent")
                    currentBrush = brush;
                else
                    currentBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD4D4E4");
                if(main != null)
                    main.Background = brush;
            }

            if(main != null)
                main.Background.Opacity = opacity;
        }

        // Upon moving the opacity slider, change the opacity (brightness level) of the main window
        private void background_opacity_change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            opacity = background_opacity_slider.Value / 100;
            try
            {
                if (game_window != null)
                    game_window.Background.Opacity = 1;                                 // gaming case's window doesn't allow transparency, so keep its opacity at 1 no matter what

                background_design_change_nosender();                                    // keep the same background, but change the brush
                                                                                        // or else it'll affect GamingUseCase's opacity
                //Console.WriteLine("Main window's opacity: " + main.Background.Opacity + "\n");
            }
            catch (InvalidOperationException) { }
            catch (NullReferenceException) { }
        }

        private void toolTipChanged(object sender, RoutedEventArgs e)
        {
            if (EnabledState.IsChecked.Value == true)
            {
                main.media_play_pause.ToolTip = "Play/Pause";
                main.media_forward.ToolTip = "Forward";
                main.media_back.ToolTip = "Previous";
                main.undo.ToolTip = "Undo Last Action";
                main.redo.ToolTip = "Redo Last Action";

                addButton.nameInput.ToolTip = "This name will be displayed on the button";
                addButton.appInput.ToolTip = "Click on Choose File to select Apps to launch with your button";
                addButton.keyEnum.ToolTip = "Select a sequences of keys to bind to your button";
                addButton.fileNames.ToolTip = "Drag a file or application here";
                addButton.m.ToolTip = "Browse for a file through a file explorer";
                addButton.confirm_btn.ToolTip = "Tip: Right Click on your new button to make edits";

            }
            else
            {
                main.media_play_pause.ToolTip = null;
                main.media_forward.ToolTip = null;
                main.media_back.ToolTip = null;
                main.undo.ToolTip = null;
                main.redo.ToolTip = null;

                addButton.nameInput.ToolTip = null;
                addButton.appInput.ToolTip = null;
                addButton.keyEnum.ToolTip = null;
                addButton.fileNames.ToolTip = null;
                addButton.m.ToolTip = null;
                addButton.confirm_btn.ToolTip = null;
            }
        }
        /*
        private void add_existingBtn_Click(object sender, RoutedEventArgs e)
        {
            //MyPopup.IsOpen = true;
        }

        private void hide_popup(object sender, RoutedEventArgs e)
        {
            //MyPopup.IsOpen = false;
        }

        private void addPreBtn(object sender, RoutedEventArgs e)
        {
            String[] spearator = { ":" };
            //String[] selectedList = buttonOptions.SelectedItem.ToString().Split(spearator, 200, StringSplitOptions.RemoveEmptyEntries);
            //string selected = selectedList[1];

            List<VirtualKeyShort.Key> shortcut = new List<VirtualKeyShort.Key>();
            List<VirtualKeyShort.Key> holder = null;
            if (selected != "  ")
            {
                Action<object, RoutedEventArgs> newClick = null;

                if (selected.Contains("Copy"))
                {
                    shortcut.Add(VirtualKeyShort.Key.CONTROL);
                    shortcut.Add(VirtualKeyShort.Key.KEY_C);
                    holder = new List<VirtualKeyShort.Key>(shortcut);
                }

                else if (selected.Contains("Paste"))
                {
                    shortcut.Add(VirtualKeyShort.Key.CONTROL);
                    shortcut.Add(VirtualKeyShort.Key.KEY_V);
                    holder = new List<VirtualKeyShort.Key>(shortcut);
                }

                newClick += (se, ev) =>
                {
                    Shortcut.send(holder.ToArray());
                };
                main.grid.addButton(selected, newClick);
                hide_popup(sender, e);
                main.menu_control.Visibility = Visibility.Collapsed;
            }
        }*/
        private void autoStateChanged(object sender, RoutedEventArgs e)
        {
            string state = "";
            if (AutoStartEnabledState.IsChecked == true)
            {
                state = "yes";

            }
            else
            {
                state = "no";
            }
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            File.Delete(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"));
            File.WriteAllText(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"), string.Empty);
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"), true))
            {
                outputFile.WriteLine(state);
            }
        }

        public void setAutoState()
        {
            string start = "";
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            using (StreamReader sr = new StreamReader(System.IO.Path.Combine(docPath, "KeyStrokesApp\\autoStart.txt"), true))
            {
                start = sr.ReadLine();
            }
            if (start == "" || start == "yes")
            {
                AutoStartEnabledState.IsChecked = true;
            }
            else
            {
                AutoStartEnabledState.IsChecked = false;
            }
        }
    }
}
