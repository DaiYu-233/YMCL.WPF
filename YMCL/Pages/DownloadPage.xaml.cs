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
using YMCL.Pages.SettingPages;

namespace YMCL.Pages
{
    /// <summary>
    /// DownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadPage : Page
    {
        public DownloadPage()
        {
            InitializeComponent();
            DownloadPageFrame.Content = autoInstall;
        }

        //#SubPages
        Pages.DownloadPages.AutoInstallPage autoInstall = new();
        private void Navigation_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var items = Navigation.MenuItems;
            var item = args.SelectedItem;
            if (item == items[0])
            {
                DownloadPageFrame.Content = autoInstall;
            }
        }
    }
}
