using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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


namespace YMCL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        
        Pages.LaunchPage launch = new Pages.LaunchPage();
        Frame setting = new Frame() { Content = new Pages.SettingPage() };
        Frame download = new Frame() { Content = new Pages.DownloadPage() };
        Frame more = new Frame() { Content = new Pages.MorePage() };
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.Directory.Exists("./YMCL")) { } else
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo("./YMCL");
                directoryInfo.Create();
            }
            ServicePointManager.DefaultConnectionLimit = 512;
            MainFrame.Content = launch;
            

        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (ToLaunch.IsSelected)
            {
                MainFrame.Content=launch;
            }else if(ToSetting.IsSelected)
            {
                MainFrame.Content = setting;
            }
            else if (ToDownload.IsSelected)
            {
                MainFrame.Content = download;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void rdLaunch_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = launch;
            launch.UpdateLogin();
        }

        private void rdSetting_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = setting;
        }

        private void rdDownload_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = download;
        }

        private void rdMore_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = more;
        }
    }
}
