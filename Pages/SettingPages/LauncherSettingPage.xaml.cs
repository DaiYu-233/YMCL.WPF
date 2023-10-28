using Microsoft.Win32;
using ModernWpf;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// LauncherSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class LauncherSettingPage : Page
    {
        public LauncherSettingPage()
        {
            var color = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath)).ThemeColor;
            InitializeComponent();
            LoadThemeComboBox();
            ColorPicker.SelectedColor = color;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(2);   //设置刷新的间隔时间
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key != null)
            {
                int appsUseLightTheme = (int)key.GetValue("AppsUseLightTheme", -1);
                if (appsUseLightTheme == 0)
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/DarkTheme.xaml") });
                }
                else if (appsUseLightTheme == 1)
                {
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/LightTheme.xaml") });
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                }
                key.Close();
            }

        }

        #region Theme

        DispatcherTimer timer = new DispatcherTimer();

        void LoadThemeComboBox()
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            var index = -1;
            switch (setting.Theme)
            {
                case "System":
                    index = 0; break;
                case "Light":
                    index = 1; break;
                case "Dark":
                    index = 2; break;
            }
            ThemeComBox.SelectedItem = ThemeComBox.Items[index];
        }

        private void ThemeComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTheme();
        }

        void UpdateTheme()
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            var item = ThemeComBox.SelectedIndex;
            switch (item)
            {
                case 0:
                    setting.Theme = "System"; break;
                case 1:
                    setting.Theme = "Light"; break;
                case 2:
                    setting.Theme = "Dark"; break;
            }
            
                
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));

            switch (setting.Theme)
            {
                case "System":
                    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                    if (key != null)
                    {
                        int appsUseLightTheme = (int)key.GetValue("AppsUseLightTheme", -1);
                        if (appsUseLightTheme == 0 || appsUseLightTheme == 1)
                        {
                            timer.Start();
                        }
                        else
                        {
                            Toast.Show(Const.Window.main, "无法确定当前系统主题", ToastPosition.Top);
                        }
                        key.Close();
                    }

                    break;
                case "Light":
                    timer.Stop();
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/LightTheme.xaml") });
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    break;
                case "Dark":
                    timer.Stop();
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/DarkTheme.xaml") });
                    break;
            }

            ThemeManager.Current.AccentColor = setting.ThemeColor;
        }

        private void ColorPicker_SelectedColorChanged(object sender, Panuon.WPF.SelectedValueChangedRoutedEventArgs<Color?> e)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.ThemeColor = ColorPicker.SelectedColor;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            UpdateTheme();
        }


        #endregion
        bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        private void WriteInRegBtn_Click(object sender, RoutedEventArgs e)
        {
            var path = System.Windows.Forms.Application.ExecutablePath;

            //StreamWriter streamWriter = new StreamWriter(Const.YMCLDataRoot + "\\YMCL.bat", false, Encoding.Default);
            //string data = $"       {AppDomain.CurrentDomain.BaseDirectory[0]}:\n       cd {AppDomain.CurrentDomain.BaseDirectory}\n       start ./YMCL.exe";
            //streamWriter.WriteLine(data);
            //streamWriter.Flush();
            //streamWriter.Close();

            if (!IsAdministrator())
            {
                var message = MessageBoxX.Show("需要管理员权限以写入注册表\n点击确定使用管理员权限重启程序", "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Error);
                if (message == MessageBoxResult.OK)
                {
                    //获取当前登陆的windows用户
                    //获取当前Windows用户的WindowsIdentity对象
                    System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                    //创建可以代码访问的windows组成员对象
                    System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
                    //判断是否为管理员

                    //创建启动对象
                    //指定启动进程时的信息
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;//设置当前工作目录的完全路径
                    startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;//获取当前执行文件的路径
                                                                                         //设置启动动作，确保以管理员权限运行
                    startInfo.Verb = "runas";
                    try
                    {
                        System.Diagnostics.Process.Start(startInfo);
                    }
                    catch (Exception)
                    {
                        Toast.Show(Const.Window.main, "以管理员权限启动失败", ToastPosition.Top);
                        return;
                    }
                    //退出当前，使用新打开的程序
                    Application.Current.Shutdown();
                }
                else return;
            }


            try { Registry.ClassesRoot.DeleteSubKey("YMCL"); } catch { }
            try
            {
                RegistryKey keyRoot = Registry.ClassesRoot.CreateSubKey("YMCL", true);
                keyRoot.SetValue("", "Yu Minecraft Launcher");
                keyRoot.SetValue("URL Protocol", path);
                RegistryKey registryKeya = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey("DefaultIcon");
                registryKeya.SetValue("", path);
                RegistryKey registryKeyb = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey(@"shell\open\command");
                registryKeyb.SetValue("", $"\"{path}\" \"%1\"");
            }
            catch (Exception rf)
            {
                MessageBoxX.Show($"写入注册表失败\n\n错误信息: {rf.Message}", "Yu Minecraft Launcher");
            }
            finally
            {
                Toast.Show(Const.Window.main, "写入成功！", ToastPosition.Top);
            }
        }
    }
}
