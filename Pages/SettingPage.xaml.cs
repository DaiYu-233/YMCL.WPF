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

namespace YMCL.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        Frame main = new Frame() { Content = new Pages.SettingPages.Main() };
        Frame account = new Frame() { Content = new Pages.SettingPages.Account() };
        public SettingPage()
        {
            InitializeComponent();
            MainFrame.Content = main;
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (ty.IsSelected)
            {
                MainFrame.Content = main;
            }
            else if (zh.IsSelected)
            {
                MainFrame.Content = account;
            }
        }
    }
}
