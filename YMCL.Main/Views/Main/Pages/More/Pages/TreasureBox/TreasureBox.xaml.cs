using System.Windows;
using iNKORE.UI.WPF.Modern.Controls;
using YMCL.Main.Public;
using Page = System.Windows.Controls.Page;

namespace YMCL.Main.Views.Main.Pages.More.Pages.TreasureBox
{
    /// <summary>
    /// TreasureBox.xaml 的交互逻辑
    /// </summary>
    public partial class TreasureBox : Page
    {
        public TreasureBox()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var tag = (sender as HyperlinkButton).Tag.ToString();
            if (tag == "Player")
            {
                Const.Window.musicPlayer.Show();
                Const.Window.musicPlayer.WindowState = WindowState.Normal;
                Const.Window.musicPlayer.Activate();
            }
        }
    }
}
