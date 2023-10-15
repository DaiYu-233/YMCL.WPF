using Panuon.WPF.UI;
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
using static System.Windows.Forms.LinkLabel;

namespace YMCL.Pages
{
    /// <summary>
    /// DisplayProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DisplayProgressWindow : WindowX
    {
        public DisplayProgressWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void TaskProgressText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TaskProgressTextBox.LineCount >= 23)
            {
                TaskProgressTextBox.Text = string.Empty;
            }
        }
    }
}
