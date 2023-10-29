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

namespace YMCL.Pages.MorePages
{
    /// <summary>
    /// ToolsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ToolsPage : Page
    {
        public ToolsPage()
        {
            InitializeComponent();
        }

        Windows.MusicPlayerWindow musicPlayerWindow = new();

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            musicPlayerWindow.Show();
            musicPlayerWindow.Activate();
            musicPlayerWindow.WindowState = WindowState.Normal;
            musicPlayerWindow.Topmost = true;
            musicPlayerWindow.Topmost = false;
        }
    }
}
