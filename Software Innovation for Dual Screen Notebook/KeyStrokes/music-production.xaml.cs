using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for music_production.xaml
    /// </summary>
    public partial class MusicProduction
    {
        private MainWindow main;

        public MusicProduction()
        {
            main = ((MainWindow)App.Current.MainWindow);

            List<VirtualKeyShort.Key> shortcut = new List<VirtualKeyShort.Key>();

            shortcut.Add(VirtualKeyShort.Key.CONTROL);
            shortcut.Add(VirtualKeyShort.Key.KEY_A);

            AddButtonWindow.add_config_shortcut("Select All", shortcut);

            shortcut.Clear();

            shortcut.Add(VirtualKeyShort.Key.KEY_D);

            AddButtonWindow.add_config_shortcut("Duplicate", shortcut);



            main.menu_control.Visibility = Visibility.Collapsed;
        }
    }
}
