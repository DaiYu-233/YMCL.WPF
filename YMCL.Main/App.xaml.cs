using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
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
                Shutdown();
            }
            base.OnStartup(e);
            StartupArgs = e.Args;
            var args = e.Args;

            Function.CreateFolder(Const.PublicDataRootPath);
            Function.CreateFolder(Const.DataRootPath);

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
                    Function.CreateFolder(Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
                    Function.CreateFolder(Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft", "versions"));
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
        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.mainWindow.WindowState = WindowState.Normal;
            Const.Window.mainWindow.Root.Visibility = Visibility.Visible;
            Const.Window.mainWindow.ShowInTaskbar = true;
            Const.Window.mainWindow.Show();
            Const.Window.mainWindow.Activate();
        }
        private void ShowTasks_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.tasksWindow.WindowState = WindowState.Normal;
            Const.Window.tasksWindow.Show();
            Const.Window.tasksWindow.Activate();
        }
        private void TrueExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }

}
