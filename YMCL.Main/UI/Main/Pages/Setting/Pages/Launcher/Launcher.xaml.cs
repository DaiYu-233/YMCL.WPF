using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using UpdateD;
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;
using YMCL.Main.UI.TaskProgress;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Launcher
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public class CPU
        {
            public string? X64 { get; set; }
            public string? X86 { get; set; }
        }
        public Launcher()
        {
            InitializeComponent();

            Version.Text = "YMCL-" + Const.Version;
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            Update updater = new();
            var url = string.Empty;
            var json = string.Empty;
            try
            {
                json = updater.GetUpdateFile(Const.UpdaterId);
            }
            catch
            {
                Toast.Show(message: LangHelper.Current.GetText("CheckUpdateFailed"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                return;
            }

            if (Environment.Is64BitProcess)
            {
                url = JsonConvert.DeserializeObject<CPU>(json).X64;
            }
            else
            {
                url = JsonConvert.DeserializeObject<CPU>(json).X86;
            }

            if (updater.GetUpdate(Const.UpdaterId, Const.Version) == true)
            {
                var V = MessageBoxX.Show(LangHelper.Current.GetText("DownloadUpdateQuestion") + "\n\n" + LangHelper.Current.GetText("UpdateInfo") + "：| \n    " + updater.GetUpdateRem(Const.UpdaterId), LangHelper.Current.GetText("FindNewVersion") + " - " + updater.GetVersionInternet(Const.UpdaterId), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    TaskProgressWindow taskProgress = new TaskProgressWindow(LangHelper.Current.GetText("DownloadUpdate"));
                    taskProgress.Show();

                    DateTime now = DateTime.Now;
                    var savePath = $"{Const.PublicDataRootPath}\\{now.ToString("yyyy-MM-dd-HH-mm-ss")}--YMCL.exe";

                    taskProgress.InsertProgressText(LangHelper.Current.GetText("BeginUpdate"));

                    try
                    {
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
                                                taskProgress.UpdateProgress(progress);
                                            }
                                        }
                                    }
                                }
                            }
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Environment.CurrentDirectory,
                                FileName = System.IO.Path.Combine(Const.PublicDataRootPath, "YMCL-Updater.exe"),
                                Arguments = $"{savePath} {System.Windows.Forms.Application.ExecutablePath}"
                            };
                            Process.Start(startInfo);
                            App.Current.Shutdown();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxX.Show(LangHelper.Current.GetText("DownloadFail") + "：" + ex.Message + "\n\n" + ex.ToString(), "Yu Minecraft Launcher");
                    }



                    taskProgress.InsertProgressText(LangHelper.Current.GetText("FinishUpdate"));
                    taskProgress.Hide();
                }
            }
            else
            {
                Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("LatestVersion"), ToastPosition.Top);
            }
        }
    }
}
