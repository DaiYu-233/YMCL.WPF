using System.Windows.Controls;
namespace YMCL.Main.UI.Main.Pages.Setting
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Page
    {

        Pages.Launcher.Launcher launcher = new();
        Pages.Launch.Launch launch = new();

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
        }
    }
}
