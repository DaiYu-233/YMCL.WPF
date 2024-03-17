using iNKORE.UI.WPF.Modern.Controls;
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
using Page = System.Windows.Controls.Page;

namespace YMCL.Main.UI.Main.Pages.More.Pages.TreasureBox
{
    /// <summary>
    /// TreasureBox.xaml 的交互逻辑
    /// </summary>
    public partial class TreasureBox : Page
    {
        MusicPlayer.Main.MusicPlayer musicPlayer = new();
        public TreasureBox()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as HyperlinkButton).Tag.ToString();
            if (tag == "Player")
            {
                musicPlayer.Show();
                musicPlayer.WindowState = WindowState.Normal;
                musicPlayer.Activate();
            }
        }
    }
}
