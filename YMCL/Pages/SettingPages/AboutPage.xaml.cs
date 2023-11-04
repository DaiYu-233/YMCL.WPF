using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using UpdateD;
using YMCL.Pages.Windows;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public class CPU
        {
            public string? X64 { get; set; }
            public string? X86 { get; set; }
        }
        public AboutPage()
        {
            InitializeComponent();
            Version.Text = Const.YMCLVersion;
        }

        string id = "97B62D3AD1724EFA9AFC7A8D8971BBB1";
        Update update = new();
        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            var url = string.Empty;
            var json = update.GetUpdateFile(id);
            if (Environment.Is64BitProcess)
            {
                url = JsonConvert.DeserializeObject<CPU>(json).X64;
            }
            else
            {
                url = JsonConvert.DeserializeObject<CPU>(json).X86;
            }

            if (update.GetUpdate(id, Const.YMCLVersion) == true)
            {
                var V = MessageBoxX.Show(update.GetUpdateRem(id), "发现新版本 - " + update.GetVersionInternet(id), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    DisplayProgressWindow displayProgressWindow = new DisplayProgressWindow();
                    displayProgressWindow.Show();
                    CheckUpdate.IsEnabled = false;
                    if (!File.Exists(Const.YMCLDataRoot + "\\Updater.exe"))
                    {
                        displayProgressWindow.CurrentStepTextBlock.Text = "下载更新组件";
                        string UpsSavePath = Const.YMCLDataRoot + "\\Updater.exe";
                        displayProgressWindow.TaskProgressTextBox.Text += "开始下载更新组件\n";
                        using (HttpClient client = new HttpClient())
                        {
                            using (HttpResponseMessage response = await client.GetAsync("https://ymcl.daiyu.fun/YMCLUpdater.exe", HttpCompletionOption.ResponseHeadersRead))
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
                                                double progress = ((double)totalBytesRead / totalBytes) * 100;
                                                displayProgressWindow.TaskProBar.Value = progress;
                                                displayProgressWindow.TaskProText.Text = $"{Math.Round(progress, 1)}%";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    displayProgressWindow.TaskProgressTextBox.Text += "更新组件下载完成\n";
                    displayProgressWindow.TaskProgressTextBox.Text += "开始下载新版本YMCL\n";
                    displayProgressWindow.CurrentStepTextBlock.Text = "下载新版本YMCL";

                    DateTime now = DateTime.Now;
                    var savePath = Const.YMCLTempDataRoot + "\\" + now.ToString("yyyy-MM-dd-HH-mm-ss") + "--YMCL.exe";

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
                                            double progress = ((double)totalBytesRead / totalBytes) * 100;
                                            displayProgressWindow.TaskProBar.Value = progress;
                                            displayProgressWindow.TaskProText.Text = $"{Math.Round(progress, 1)}%";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    displayProgressWindow.Hide();
                    var path = System.Windows.Forms.Application.ExecutablePath;


                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    ProcessStartInfo startInfo = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Const.YMCLDataRoot,//设置当前工作目录的完全路径
                        FileName = Const.YMCLDataRoot + "\\Updater.exe",//获取当前执行文件的路径
                        Verb = "runas",
                        Arguments = $"{savePath} {path}"
                    };
                    Process.Start(startInfo);
                    Environment.Exit(0);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.main, "当前已是最新版本", ToastPosition.Top);
                //MessageBoxX.Show("当前已是最新版本", "Yu Minecraft Launcher");
            }
        }
    }
}
