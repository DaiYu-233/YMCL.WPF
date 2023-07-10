using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        string NowVersion = "1.0.0";
        string id = "97B62D3AD1724EFA9AFC7A8D8971BBB1";
        UpdateD.Update update = new UpdateD.Update();
        public About()
        {
            InitializeComponent();
        }

        private void AddCustomJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            if(update.GetUpdate(id, NowVersion) == true)
            {
                MessageBoxX.Show(update.GetUpdateRem(id),"发现新版本 - "+update.GetVersionInternet(id));
                var V = MessageBoxX.Show("(下载文件并替换) 下载地址:\n"+update.GetUpdateFile(id), "更新到YMCL - " + update.GetVersionInternet(id), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    Process.Start(new ProcessStartInfo(update.GetUpdateFile(id)) { UseShellExecute = true, Verb = "open" }); 
                }
            }
            else
            {
                MessageBoxX.Show("当前已是最新版本", "暂无更新");
            }
        }
    }
}
