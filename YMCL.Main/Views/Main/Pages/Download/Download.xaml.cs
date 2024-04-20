using System.Windows.Controls;
using YMCL.Main.Views.Main.Pages.Download.Pages.AutoInstall;

namespace YMCL.Main.Views.Main.Pages.Download
{
    /// <summary>
    /// Download.xaml 的交互逻辑
    /// </summary>
    public partial class Download : Page
    {
        AutoInstall autoInstall = new();
        public Download()
        {
            InitializeComponent();
            MainFrame.Content = autoInstall;
        }

        private void Navigation_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (AutoInstall.IsSelected)
            {
                MainFrame.Content = autoInstall;
            }
        }
    }
}
