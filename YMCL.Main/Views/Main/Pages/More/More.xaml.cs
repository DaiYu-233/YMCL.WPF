using System.Windows.Controls;

namespace YMCL.Main.Views.Main.Pages.More
{
    /// <summary>
    /// More.xaml 的交互逻辑
    /// </summary>
    public partial class More : Page
    {
        Pages.TreasureBox.TreasureBox treasureBox = new();
        public More()
        {
            InitializeComponent();
            MainFrame.Content = treasureBox;
        }

        private void Navigation_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (TreasureBox.IsSealed)
            {
                MainFrame.Content = treasureBox;
            }
        }
    }
}
