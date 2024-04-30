using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.WPF.UI;
using UpdateD;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
using Application = System.Windows.Application;
using File = System.IO.File;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace YMCL.Main.Views.Main.Pages.Setting.Pages.Launcher
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public Launcher()
        {
            InitializeComponent();

            Version.Text = "YMCL-" + Const.Version;
            UserDataSize.Text = $"{Math.Round(Method.GetDirectoryLength(Const.DataRootPath) / 1024 / 1024, 2)} MiB";
            AppDataSize.Text = $"{Math.Round(Method.GetDirectoryLength(Const.PublicDataRootPath) / 1024 / 1024, 2)} MiB";
        }

        private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            Update updater = new();
            var url = string.Empty;
            _2018k obj = null;
            CheckUpdate.IsEnabled = false;
            try
            {
                obj = JsonConvert.DeserializeObject<_2018k>(updater.GetUpdateFile(Const.UpdaterId));
            }
            catch
            {
                Toast.Show(message: LangHelper.Current.GetText("CheckUpdateFailed"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }

            bool Is64BitProcess = Environment.Is64BitProcess;
            if (obj.EnabledGithubApi)
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36 Edg/91.0.864.54");
                    var githubApi = JArray.Parse(await httpClient.GetStringAsync(obj.GithubApi));
                    foreach (JObject root in githubApi)
                    {
                        JArray assets = (JArray)root["assets"];
                        foreach (JObject asset in assets)
                        {
                            string name = (string)asset["name"];
                            string browser_download_url = (string)asset["browser_download_url"];
                            if (Is64BitProcess && name == "YMCL.Main.x64.exe")
                            {
                                url = browser_download_url;
                                break;
                            }
                            if (!Is64BitProcess && name == "YMCL.Main.x86.exe")
                            {
                                url = browser_download_url;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    Toast.Show(message: LangHelper.Current.GetText("CheckUpdateFailed"), position: ToastPosition.Top, window: Const.Window.main);
                    return;
                }
            }
            else
            {
                if (Is64BitProcess)
                {
                    url = obj.X64;
                }
                else
                {
                    url = obj.X86;
                }
            }
            Debug.WriteLine(url);
            if (updater.GetUpdate(Const.UpdaterId, Const.Version) == true)
            {
                var V = MessageBoxX.Show(LangHelper.Current.GetText("DownloadUpdateQuestion") + "\n\n" + LangHelper.Current.GetText("UpdateInfo") + "：| \n    " + updater.GetUpdateRem(Const.UpdaterId), LangHelper.Current.GetText("FindNewVersion") + " - " + updater.GetVersionInternet(Const.UpdaterId), MessageBoxButton.OKCancel);
                if (V == MessageBoxResult.OK)
                {
                    var task = Const.Window.tasks.CreateTask(LangHelper.Current.GetText("DownloadUpdate"), true);

                    DateTime now = DateTime.Now;
                    var savePath = $"{Const.PublicDataRootPath}\\{now.ToString("yyyy-MM-dd-HH-mm-ss")}--YMCL.exe";
                    task.AppendText(LangHelper.Current.GetText("BeginUpdate"));
                    try
                    {
                        var handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12;
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36 Edg/121.0.0.0");
                            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();

                                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                                {
                                    using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
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
                                                task.UpdateProgress(progress);
                                            }
                                        }
                                    }
                                }
                            }
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Environment.CurrentDirectory,
                                FileName = System.IO.Path.Combine(Const.PublicDataRootPath, "YMCL.Updater.exe"),
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
                    task.AppendText(LangHelper.Current.GetText("FinishUpdate"));
                    task.Destory();
                    CheckUpdate.IsEnabled = true;
                }
                else
                {
                    CheckUpdate.IsEnabled = true;
                }
            }
            else
            {
                Toast.Show(Const.Window.main, LangHelper.Current.GetText("LatestVersion"), ToastPosition.Top);
                CheckUpdate.IsEnabled = true;
            }
            CheckUpdate.IsEnabled = true;
        }

        private void OpenUserDataFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Const.DataRootPath);
        }

        private void OpenAppDataFolder(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Const.PublicDataRootPath);
        }

        private void CreateShortCutButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDataSend = new SaveFileDialog();
            saveDataSend.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            saveDataSend.Filter = "*.url | Short Cut";
            saveDataSend.DefaultExt = ".url";
            saveDataSend.FileName = "YMCL.url";
            saveDataSend.ShowDialog();
            if (saveDataSend.FileName != "YMCL.url")
            {
                string fName = saveDataSend.FileName;
                using (StreamWriter writer = new StreamWriter(fName))
                {
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=ymcl://");
                    writer.WriteLine("IconFile=C:\\ProgramData\\DaiYu.YMCL\\Icon.ico");
                    writer.WriteLine("IconIndex=0");
                }
            }
        }

        private void WriteRegButton_Click(object sender, RoutedEventArgs e)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var message = MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_Download_AdministratorPermissionRequired"), "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                if (message == MessageBoxResult.OK)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = System.Windows.Forms.Application.ExecutablePath,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBoxX.Show(YMCL.Main.Public.Lang.MainLang.FailToWriteReg, "Yu Minecraft Launcher", MessageBoxIcon.Error);
                }
            }

            var bat = "set /p ymcl=<%USERPROFILE%\\AppData\\Roaming\\DaiYu.YMCL\\YMCL.ExePath.DaiYu\r\necho %ymcl%\r\necho %1\r\nstart %ymcl% %1";
            File.WriteAllText(Const.YMCLBat, bat);

            try { Registry.ClassesRoot.DeleteSubKey("YMCL"); } catch { }
            try
            {
                RegistryKey keyRoot = Registry.ClassesRoot.CreateSubKey("YMCL", true);
                keyRoot.SetValue("", "Yu Minecraft Launcher");
                keyRoot.SetValue("URL Protocol", Const.YMCLBat);
                RegistryKey registryKeya = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey("DefaultIcon");
                registryKeya.SetValue("", Const.YMCLBat);
                RegistryKey registryKeyb = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey(@"shell\open\command");
                registryKeyb.SetValue("", $"\"{Const.YMCLBat}\" \"%1\"");
            }
            catch { }

            Toast.Show(Const.Window.main, YMCL.Main.Public.Lang.MainLang.WriteFinish, ToastPosition.Top);
        }
    }
}
