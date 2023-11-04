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
    /// MorePage.xaml 的交互逻辑
    /// </summary>
    public partial class MorePage : Page
    {
        public MorePage()
        {
            InitializeComponent();
            DownloadPageFrame.Content = toolsPage;
        }

        MorePages.ToolsPage toolsPage = new();
        private void Navigation_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var items = Navigation.MenuItems;
            var item = args.SelectedItem;
            if (item == items[0])
            {
                DownloadPageFrame.Content = toolsPage;
            }
        }
    }
}
