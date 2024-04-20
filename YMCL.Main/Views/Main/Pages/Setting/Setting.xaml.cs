using System.Windows.Controls;

namespace YMCL.Main.Views.Main.Pages.Setting
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {

        Pages.Personalize.Personalize launcher = new();
        Pages.Launch.Launch launch = new();
        Pages.Account.Account account = new();
        Pages.Launcher.Launcher about = new();
        Pages.Download.Download download = new();

        public Setting()
        {
            InitializeComponent();
            MainFrame.Content = launch;
        }

        private void Navigation_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (Launcher.IsSelected)
            {
                MainFrame.Content = launcher;
            }
            if (Launch.IsSelected)
            {
                MainFrame.Content = launch;
            }
            if (Account.IsSelected)
            {
                MainFrame.Content = account;
            }
            if(About.IsSelected)
            {
                MainFrame.Content= about;
            }
            if(Download.IsSelected)
            {
                MainFrame.Content = download;
            }
        }
    }
}
