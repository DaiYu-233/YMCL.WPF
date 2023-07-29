using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
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
        string NowVersion = "1.0.0.20230727_Alpha";
        string NowVersionType = "内部测试版";


        string id = "97B62D3AD1724EFA9AFC7A8D8971BBB1";
        UpdateD.Update update = new UpdateD.Update();

        bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public About()
        {
            InitializeComponent();

            //if (!File.Exists("./YMCL.LauncherInfo.json"))
            //{
            //    MessageBoxX.Show("文件 YMCL.LauncherInfo.json 丢失\n重新下载YMCL以解决此问题", "Yu Minecraft Launcher");
            //    return;
            //}
            //var str = File.ReadAllText("./YMCL.LauncherInfo.json");
            //var obj = JsonConvert.DeserializeObject<LauncherInfo>(str);
            
            NowVersionText.Text = "YMCL-"+NowVersion;
            NowVersionTypeText.Text = NowVersionType;
        }

        private void AddCustomJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            if(update.GetUpdate(id, NowVersion) == true)
            {
                var V = MessageBoxX.Show(update.GetUpdateRem(id), "发现新版本 - " + update.GetVersionInternet(id), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    if (!IsAdministrator())
                    {
                        MessageBoxX.Show("需要管理员权限\n请使用管理员权限运行", "Yu Minecraft Launcher");
                        return;
                    }
                    if (!File.Exists("./YMCL-Updater.exe"))
                    {
                        var H = MessageBoxX.Show("YMCL更新程序 YMCL-Updater.exe 丢失\n点击确定开始下载", "Yu Minecraft Launcher", MessageBoxButton.OKCancel);
                        if (H == MessageBoxResult.OK)
                        {
                            Panuon.WPF.UI.Toast.Show("开始下载", ToastPosition.Top);
                            var handler = PendingBox.Show("正在下载...", "提示");
                            try
                            {
                                // 变量fileName获取你要下载的链接的文件名，
                                string fileName = System.IO.Path.GetFileName("https://ymcl.daiyu.fun/YMCL-Updater.exe");
                                // fileUrl为你的下载链接
                                string fileUrl = "https://ymcl.daiyu.fun/YMCL-Updater.exe";
                                // savePath是你要保存在本地的文件夹，需要加上你下载文件的文件名，要和文件名一致，他自己会在本地下载字节流转换
                                string savePath = @"./" + fileName;
                                // 加上using 
                                using (WebClient client = new WebClient())
                                {
                                    // 填下载链接，和本地地址，下载完检查文件夹即可
                                    client.DownloadFile(fileUrl, savePath);
                                }
                                Panuon.WPF.UI.Toast.Show("下载完成", ToastPosition.Top);
                                handler.Close();
                            }
                            catch (Exception ex)
                            {
                                handler.Close();
                                MessageBoxX.Show("下载失败\n\n错误信息：\n"+ex.ToString(), "Yu Minecraft Launcher");

                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    Process.Start("./YMCL-Updater.exe");
                    System.Environment.Exit(0);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show("当前已是最新版本", ToastPosition.Top);
                //MessageBoxX.Show("当前已是最新版本", "Yu Minecraft Launcher");
            }
        }
    }
}
