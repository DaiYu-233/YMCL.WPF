using ModernWpf.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
using YMCL.Pages.SettingPages;
using Page = System.Windows.Controls.Page;

namespace YMCL.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            SettingPageFrame.Content = launchSettingPage;
        }

        //SunPages
        LaunchSettingPage launchSettingPage = new();
        AccountSettingPage accountSettingPage = new();
        LauncherSettingPage launcherSettingPage = new();
        DownloadSettingPage downloadSettingPage = new();

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var items = Navigation.MenuItems;
            var item = args.SelectedItem;
            if (item == items[0])
            {
                SettingPageFrame.Content = launchSettingPage;
            }
            else if (item == items[1])
            {
                SettingPageFrame.Content = accountSettingPage;
            }
            else if (item == items[2])
            {
                SettingPageFrame.Content = downloadSettingPage;
            }
            else if (item == items[3])
            {
                SettingPageFrame.Content = launcherSettingPage;
            }
        }
    }
}
