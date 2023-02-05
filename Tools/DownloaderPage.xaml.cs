using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YMCL.Tools
{
    /// <summary>
    /// DownloaderPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloaderPage : Page
    {
        public DownloaderPage()
        {
            InitializeComponent();
        }

        private void FindSaveUri_Click(object sender, RoutedEventArgs e)
        {
            string sPath = "";
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "要把文件藏在哪里呢 awa";  //定义在对话框上显示的文本

            if (folder.ShowDialog() == DialogResult.OK)
            {
                sPath = folder.SelectedPath;
            }
            SavePathTextblock.Text = sPath;
        }
    }
}