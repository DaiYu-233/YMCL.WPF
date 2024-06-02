using iNKORE.UI.WPF.Modern.Controls.Primitives;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

namespace YMCL.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        //[DllImport("User32.dll", EntryPoint = "SendMessage")]
        //private static extern int SendMessage(IntPtr wnd, int msg, IntPtr wP, IntPtr lP);
        //public struct COPYDATASTRUCT
        //{
        //    public IntPtr dwData; // 任意值
        //    public int cbData;    // 指定lpData内存区域的字节数
        //    [MarshalAs(UnmanagedType.LPStr)]
        //    public string lpData; // 发送给目标窗口所在进程的数据
        //}
        public static string[] StartupArgs;
        private static Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "OnlyRun");
            if (!mutex.WaitOne(0, false))
            {
                //Process[] procs = Process.GetProcesses();
                //foreach (Process p in procs)
                //{
                //    if (p.ProcessName.Equals("YMCL.Main"))
                //    {
                //        IntPtr hWnd = p.MainWindowHandle;
                //        SendMessage(hWnd, 9001, 1, (IntPtr)0);
                //    }
                //}
                //MessageBoxX.Show(MainLang.RepeatOpen, "Yu Minecraft Launcher");
                //Shutdown();
            }
            base.OnStartup(e);

            string resourceName = "YMCL.Main.Public.Text.DateTime.txt";
            Assembly _assembly = Assembly.GetExecutingAssembly();
            Stream stream = _assembly.GetManifestResourceStream(resourceName);
            using (StreamReader reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd().Trim();
                Const.Version = result;
            }

            StartupArgs = e.Args;
            var args = e.Args;

            Method.CreateFolder(Const.PublicDataRootPath);
            Method.CreateFolder(Const.DataRootPath);

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            var obj = new Setting();
            if (!File.Exists(Const.SettingDataPath) || JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath)) == null)
            {
                var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(Const.SettingDataPath, data);
            }
            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.Language == null || setting.Language == "zh-CN")
            {
                LangHelper.Current.ChangedCulture("");
            }
            else
            {
                LangHelper.Current.ChangedCulture(setting.Language);
            }
            if (!File.Exists(Const.MinecraftFolderDataPath))
            {
                var minecraftFolder = new List<string>()
                {
                    Path.Combine(System.Windows.Forms.Application.StartupPath , ".minecraft")
                };
                try
                {
                    Method.CreateFolder(Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
                    Method.CreateFolder(Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft", "versions"));
                }
                catch
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
                            Current.Shutdown();
                        }
                        else
                        {
                            MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_FailedToObtainAdministratorPrivileges"), "Yu Minecraft Launcher", MessageBoxIcon.Error);
                            Current.Shutdown();
                        }
                    }
                }
                File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
            }
            if (!File.Exists(Const.JavaDataPath))
            {
                File.WriteAllText(Const.JavaDataPath, "[]");
            }
            if (!File.Exists(Const.PlayListDataPath))
            {
                File.WriteAllText(Const.PlayListDataPath, "[]");
            }
            if (!File.Exists(Const.AccountDataPath))
            {
                DateTime now = DateTime.Now;
                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(new List<AccountInfo>() { new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = null,
                    Name = "Steve"
                }}, Formatting.Indented));
            }
            File.WriteAllText(Const.YMCLPathData, System.Windows.Forms.Application.ExecutablePath);
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                MessageBoxX.Show($"\n{LangHelper.Current.GetText("App_UnhandledException")}：{e.Exception.Message}\n\n{e.Exception.ToString()}", "Yu Minecraft Launcher");
            }
            catch { }
            finally
            {
                e.Handled = true;
            }
        }
        #region NotifyIcon
        private void ResizeWindow_Click(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MainWidth = 1050;
            setting.MainHeight = 600;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            Method.RestartApp();
        }
        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.main.WindowState = WindowState.Normal;
            Const.Window.main.Root.Visibility = Visibility.Visible;
            Const.Window.main.ShowInTaskbar = true;
            Const.Window.main.Show();
            Const.Window.main.Activate();
        }
        private void ShowTasks_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.tasks.WindowState = WindowState.Normal;
            Const.Window.tasks.Show();
            Const.Window.tasks.Activate();
        }
        private void TrueExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.desktopLyric.Change();
        }
        #endregion
    }

}
