using Panuon.WPF.UI;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using YMCL.Public.Class;
using YMCL.UI.Lang;
using Application = System.Windows.Application;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);
        [DllImport("gdi32")]
        static extern int AddFontResource(string lpFileName);
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var args = e.Args;

            LangHelper.Current.ChangedCulture("zh-CN");

            if (args.Contains("InstallFont"))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var message = MessageBoxX.Show("需要管理员权限以初始化YMCL\n点击确定使用管理员权限重启程序", "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                    if (message == MessageBoxResult.OK)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory,
                            FileName = System.Windows.Forms.Application.ExecutablePath,
                            Verb = "runas",
                            Arguments = "InstallFont"
                        };
                        Process.Start(startInfo);
                        Current.Shutdown();
                    }
                    else
                    {
                        MessageBoxX.Show("获取管理员权限失败\nYMCL初始化失败", "Yu Minecraft Launcher", MessageBoxIcon.Error);
                        Current.Shutdown();
                    }
                }
                else
                {
                    var fontFilePath = "C:\\ProgramData\\DaiYu.YMCL\\MiSans.ttf";
                    string fontPath = Path.Combine(System.Environment.GetEnvironmentVariable("WINDIR"), "fonts", "YMCL_" + Path.GetFileName(fontFilePath));
                    File.Copy(fontFilePath, fontPath, true); //font是程序目录下放字体的文件夹
                    AddFontResource(fontPath);
                    WriteProfileString("fonts", Path.GetFileNameWithoutExtension(fontFilePath) + "(TrueType)", "YMCL_" + Path.GetFileName(fontFilePath));

                    MessageBoxX.Show("YMCL初始化完成", "Yu Minecraft Launcher", MessageBoxIcon.Success);
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = System.Windows.Forms.Application.ExecutablePath,
                        Verb = "runas",
                        Arguments = ""
                    };
                    Process.Start(startInfo);
                    Current.Shutdown();
                }
            }
        }
    }

}
