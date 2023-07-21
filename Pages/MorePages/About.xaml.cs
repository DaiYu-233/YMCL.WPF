using Newtonsoft.Json;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using YMCL.Class;

namespace YMCL.Pages.MorePages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        string NowVersion;
        string id = "97B62D3AD1724EFA9AFC7A8D8971BBB1";
        UpdateD.Update update = new UpdateD.Update();
        public About()
        {
            InitializeComponent();

            if (!File.Exists("./YMCL.LauncherInfo.json"))
            {
                MessageBoxX.Show("文件 YMCL.LauncherInfo.json 丢失\n重新下载YMCL以解决此问题", "Yu Minecraft Launcher");
                return;
            }
            var str = File.ReadAllText("./YMCL.LauncherInfo.json");
            var obj = JsonConvert.DeserializeObject<LauncherInfo>(str);
            
            NowVersionText.Text = "YMCL-"+obj.Version;
            NowVersionTypeText.Text = obj.VersionType;
            NowVersion = obj.Version;
        }

        private void AddCustomJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            if(update.GetUpdate(id, NowVersion) == true)
            {
                var V = MessageBoxX.Show(update.GetUpdateRem(id)+"\n\n" +update.GetUpdateFile(id), "发现新版本 - " + update.GetVersionInternet(id), MessageBoxButton.OKCancel);
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
