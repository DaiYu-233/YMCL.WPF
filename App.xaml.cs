using MinecraftLaunch.Modules.Utils;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public App()
        {
            Startup += App_Startup;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        //入口点
        private async void App_Startup(object sender, StartupEventArgs e)
        {
            var obj = new Class.Setting()
            {
                MinecraftPath = AppDomain.CurrentDomain.BaseDirectory + ".minecraft",
                Theme = "Light",
                MamMem = 1024,
                ThemeColor = Color.FromArgb(255, 0, 120, 215)
            };
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
            if (!File.Exists(Const.YMCLFontPath))
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
                                using (var fileStream = new FileStream(Const.YMCLFontPath, FileMode.Create, FileAccess.Write))
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
                    Application.Current.Shutdown();
                }
                catch (Exception)
                {

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
