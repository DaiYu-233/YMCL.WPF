using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        string NowVersion = "1.0.0.20230805_Alpha";
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

        private async void AddCustomJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            if(update.GetUpdate(id, NowVersion) == true)
            {
                var V = MessageBoxX.Show(update.GetUpdateRem(id), "发现新版本 - " + update.GetVersionInternet(id), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    //if (!IsAdministrator())
                    //{
                    //    MessageBoxX.Show("需要管理员权限\n请使用管理员权限运行", "Yu Minecraft Launcher");
                    //    return;
                    //}
                    CheckUpdateBtn.IsEnabled = false;
                    if (!File.Exists("./YMCL-Updater.exe"))
                    {
                        string UpUrl = "https://ymcl.daiyu.fun/Up.exe";
                        string UpsSavePath = "./Up.exe";
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage response = await client.GetAsync(UpUrl, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();

                                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                                {
                                    using (var fileStream = new System.IO.FileStream(UpsSavePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                    {
                                        byte[] buffer = new byte[8192];
                                        int bytesRead;
                                        long totalBytesRead = 0;
                                        long totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1;

                                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                        {
                                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                                            totalBytesRead += bytesRead;

                                            if (totalBytes > 0)
                                            {
                                                DownloadInfo.Visibility = Visibility.Visible;
                                                double progress = ((double)totalBytesRead / totalBytes) * 100;
                                                DownloadProText.Text = $"{Math.Round(progress, 1)}%";
                                                DownloadProBar.Value = progress;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    string url = update.GetUpdateFile(id); ;
                    string savePath = "./YMCL-Temp.exe";

                    using (HttpClient client = new HttpClient())
                    {
                        using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (var downloadStream = await response.Content.ReadAsStreamAsync())
                            {
                                using (var fileStream = new System.IO.FileStream(savePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    byte[] buffer = new byte[8192];
                                    int bytesRead;
                                    long totalBytesRead = 0;
                                    long totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1;

                                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        totalBytesRead += bytesRead;

                                        if (totalBytes > 0)
                                        {
                                            DownloadInfo.Visibility = Visibility.Visible;

                                            double progress = ((double)totalBytesRead / totalBytes) * 100;
                                            DownloadProText.Text = $"{Math.Round(progress, 1)}%";
                                            DownloadProBar.Value = progress;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    DownloadInfo.Visibility = Visibility.Hidden;

                    Process.Start("./Up.exe");
                    System.Environment.Exit(0);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main,"当前已是最新版本", ToastPosition.Top);
                //MessageBoxX.Show("当前已是最新版本", "Yu Minecraft Launcher");
            }
        }
    }
}
