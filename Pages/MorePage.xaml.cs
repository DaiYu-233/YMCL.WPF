using Panuon.WPF.UI;
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
using YMCL.Pages.Forms;
using YMCL.Pages.SettingPages;


namespace YMCL.Pages
{
    /// <summary>
    /// MorePage.xaml 的交互逻辑
    /// </summary>
    public partial class MorePage : Page
    {
        Frame aboutp = new Frame() { Content = new Pages.MorePages.About() };

        Forms.MusicPlayer musicPlayer = new Forms.MusicPlayer();
        public MorePage()
        {
            InitializeComponent();
            MainFrame.Content = aboutp;
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (about.IsSelected)
            {
                MainFrame.Content = aboutp;
            }
            else if (Music.IsSelected)
            {
                var _musicplayer = Application.Current.Windows
            .Cast<WindowX>()
            .FirstOrDefault(window => window is MusicPlayer) as MusicPlayer;

                Music.IsSelected = false;
                about.IsSelected = true;
                if (musicPlayer == null || musicPlayer.IsVisible == false)
                {
                    musicPlayer.Show();
                    musicPlayer.Activate();
                    MainWindow.GetWindow(this).WindowState = WindowState.Minimized;
                    // 用于踢掉其他的在上层的窗口
                    musicPlayer.Topmost = true;
                    musicPlayer.Topmost = false;
                    Toast.Show(window: _musicplayer, $"主窗口已最小化", ToastPosition.Top);
                }
                else
                {
                    MainWindow.GetWindow(this).WindowState = WindowState.Minimized;
                    musicPlayer.Activate();
                    musicPlayer.WindowState = System.Windows.WindowState.Normal;
                    musicPlayer.Activate();

                    // 用于踢掉其他的在上层的窗口
                    musicPlayer.Topmost = true;
                    musicPlayer.Topmost = false;
                    Toast.Show(window: _musicplayer, $"主窗口已最小化", ToastPosition.Top);
                }

            }
        }
    }
}
