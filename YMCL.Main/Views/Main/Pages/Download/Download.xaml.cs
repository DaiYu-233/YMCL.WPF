using System.Windows.Controls;
using YMCL.Main.Views.Main.Pages.Download.Pages.AutoInstall;
using YMCL.Main.Views.Main.Pages.Download.Pages.Mods;

namespace YMCL.Main.Views.Main.Pages.Download
{
    /// <summary>
    /// Download.xaml 的交互逻辑
    /// </summary>
    public partial class Download : Page
    {
        AutoInstall autoInstall = new();
        Mods mods = new();
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
            if (Mods.IsSelected)
            {
                MainFrame.Content = mods;
            }
        }
    }
}
