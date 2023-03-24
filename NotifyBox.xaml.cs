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
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Forms;

namespace YMCL
{
    /// <summary>
    /// NotifyBox.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyBox : Window
    {
        public NotifyBox()
        {
            InitializeComponent();
            Left = Screen.PrimaryScreen.WorkingArea.Width - Width - 20;
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 20;
        }

        async private void Storyboard_Completed(object sender, EventArgs e)
        {
            await Task.Delay(3000);
            BeginStoryboard(FindResource("Close")as Storyboard);
        }

        private void Storyboard_Completed_1(object sender, EventArgs e)
        {
            Close();
        }

        public string NotifiMessage { get; set; }

        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Text = NotifiMessage;
        }
    }
}
