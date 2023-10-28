using MinecraftLaunch.Modules.Utils;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YMCL.Class;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);

        public App()
        {
            Startup += App_Startup;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        //入口点
        private async void App_Startup(object sender, StartupEventArgs e)
        {
            var obj = new Setting();
            var initalize = PendingBox.Show("正在初始化...", "Yu Minecraft Launcher");
            if (!Directory.Exists(Const.YMCLDataRoot))
            {
                DirectoryInfo directoryInfo = new(Const.YMCLDataRoot);
                directoryInfo.Create();
            }
            if (!Directory.Exists(Const.YMCLDataRoot + "\\Temp"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Const.YMCLDataRoot + "\\Temp");
                directoryInfo.Create();
            }
            if (!Directory.Exists(Const.YMCLPublicDataRoot))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Const.YMCLPublicDataRoot);
                directoryInfo.Create();
            }
            if (!File.Exists(Const.YMCLMinecraftFolderDataPath))
            {
                var folderList = new List<string>()
                    {
                        AppDomain.CurrentDomain.BaseDirectory + ".minecraft"
            };
                var folderData = JsonConvert.SerializeObject(folderList, Formatting.Indented);
                File.WriteAllText(Const.YMCLMinecraftFolderDataPath, folderData);
            }
            if (!File.Exists(Const.YMCLSettingDataPath))
            {
                var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(Const.YMCLSettingDataPath, data);
            }
            if (!File.Exists(Const.YMCLAccountDataPath))
            {
                List<AccountInfo> account = new List<AccountInfo> { new AccountInfo()
                {
                    AccountType = "离线模式",
                    AddTime = DateTime.Now.ToString(),
                    Name = "Steve",
                    Data = null
                }};
                var data = JsonConvert.SerializeObject(account, Formatting.Indented);
                File.WriteAllText(Const.YMCLAccountDataPath, data);
            }
            if (!File.Exists(Const.YMCLJavaDataPath))
            {
                File.WriteAllText(Const.YMCLJavaDataPath, "[]");
            }
            if (!File.Exists(Const.YMCLSongPlayListDataPath))
            {
                File.WriteAllText(Const.YMCLSongPlayListDataPath, "[]");
            }
            if (!File.Exists(Const.YMCLPublicDataRoot + "\\YMCLMiSans.ttf"))
            {
                try
                {
                    await Task.Run(async () =>
                    {
                        using HttpClient client = new HttpClient();
                        using (HttpResponseMessage response = await client.GetAsync("https://ymcl.daiyu.fun/MiSans.ttf", HttpCompletionOption.ResponseHeadersRead))
                        {
                            response.EnsureSuccessStatusCode();

                            using (var downloadStream = await response.Content.ReadAsStreamAsync())
                            {
                                using (var fileStream = new FileStream(Const.YMCLPublicDataRoot + "\\YMCLMiSans.ttf", FileMode.Create, FileAccess.Write))
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
                                            initalize.UpdateMessage($"初始化字体 - {Math.Round(progress, 1)}%");
                                        }
                                    }
                                }
                            }
                        }
                    });
                    System.Windows.Forms.Application.Restart();
                    initalize.Close();
                    Application.Current.Shutdown();
                }
                catch (Exception)
                {

                }
            }
            if (!File.Exists(Const.YMCLPublicDataRoot + "\\InstalledFont"))
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var message = MessageBoxX.Show("需要管理员权限以安装字体\n点击确定使用管理员权限重启程序", "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Error);
                    if (message == MessageBoxResult.OK)
                    {
                        //获取当前登陆的windows用户
                        //获取当前Windows用户的WindowsIdentity对象
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        //创建可以代码访问的windows组成员对象
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        //判断是否为管理员

                        //创建启动对象
                        //指定启动进程时的信息
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.UseShellExecute = true;
                        startInfo.WorkingDirectory = Environment.CurrentDirectory;//设置当前工作目录的完全路径
                        startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;//获取当前执行文件的路径
                        startInfo.Verb = "runas";
                        System.Diagnostics.Process.Start(startInfo);
                        //退出当前，使用新打开的程序
                        initalize.Close();
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    var fontFilePath = Const.YMCLPublicDataRoot + "\\YMCLMiSans.ttf";
                    string fontPath = Path.Combine(System.Environment.GetEnvironmentVariable("WINDIR"), "fonts", Path.GetFileName(fontFilePath));
                    if (!File.Exists(fontPath))
                    {
                        File.Copy(fontFilePath, fontPath);
                        AddFontResource(fontPath);

                        WriteProfileString("fonts", Path.GetFileNameWithoutExtension(fontFilePath) + "(TrueType)", Path.GetFileName(fontFilePath));

                        File.WriteAllText(Const.YMCLPublicDataRoot + "\\InstalledFont", "");

                        System.Windows.Forms.Application.Restart();
                        initalize.Close();
                        Application.Current.Shutdown();
                    }

                }
            }

            initalize.Close();

            if (e.Args.Length > 0)
            {
                var arg = e.Args[0];
                if (e.Args[0].StartsWith("ymcl://"))
                {
                    arg = arg.Substring(7, arg.Length - 8);
                }
                arg = WebUtility.UrlDecode(arg);
                File.WriteAllText(Const.YMCLTempSetupArgsDataPath, arg);
            }
            else
            {
                File.WriteAllText(Const.YMCLTempSetupArgsDataPath, string.Empty);
            }


        }


        //全局异常处理
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxX.Show($"\n{e.Exception.Message}\n\n详细信息：\n{e.Exception.ToString()}", "Yu Minecraft Launcher - 异常");
        }
    }
}
