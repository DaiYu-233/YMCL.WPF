using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using ModernWpf;
using Panuon.UI.Silver;
using WpfToast.Controls;
using Toast = WpfToast.Controls.Toast;
using Microsoft.Win32;
using System.Security.Principal;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public string path = System.AppDomain.CurrentDomain.BaseDirectory + @"YMCL\YMCL.bat";
        public string pathroot = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(':'));

        bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        public Launcher()
        {
            InitializeComponent();
            
            #region 初始化
            if (File.Exists("./YMCL/DisplayInformation.txt"))
            {
                if (File.ReadAllText("./YMCL/DisplayInformation.txt") == "false")
                {
                    displayinf.IsOn = false;
                }
                else
                {
                    displayinf.IsOn = true;
                }
            }
            else
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
            if (File.Exists("./YMCL/Theme.txt"))
            {
                if (File.ReadAllText("./YMCL/Theme.txt") == "Dark")
                {
                    DarkThemeSwitch.IsOn = true;
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/Dark.xaml") });
                }
                else
                {
                    DarkThemeSwitch.IsOn = false;
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/Light.xaml") });
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                }
            }
            else
            {
                File.WriteAllText("./YMCL/Theme.txt", "Light");
            }
            #endregion
        }
        private void TestFolder(string Folder)
        {
            if (System.IO.Directory.Exists(Folder)) { }
            else
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Folder);
                directoryInfo.Create();
            }
        }






        private void SaveSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            TestFolder("./YMCL");
            if (DarkThemeSwitch.IsOn == true)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/Dark.xaml") });
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Styles/Light.xaml") });
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }

            if (displayinf.IsOn == true)
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
            else
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "false");
            }
            if (DarkThemeSwitch.IsOn == true)
            {
                File.WriteAllText("./YMCL/Theme.txt", "Dark");
            }
            else
            {
                File.WriteAllText("./YMCL/Theme.txt", "Light");
            }
            //Panuon.WPF.UI.Toast.Show("成功应用设置", ToastPosition.Top);
            Toast.Show("成功应用设置", new ToastOptions { Icon = ToastIcons.Information, ToastMargin = new Thickness(10), Time = 1500, Location = ToastLocation.OwnerTopCenter });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"./YMCL/YMCL.bat", $"{pathroot}\ncd {AppDomain.CurrentDomain.BaseDirectory}:\nstart ./YMCL.exe");
            if (!IsAdministrator())
            {
                MessageBoxX.Show("需要管理员权限以写入注册表\n请使用管理员权限运行", "Yu Minecraft Launcher");
                return;
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
                registryKeyb.SetValue("", path);
            }
            catch (Exception rf)
            {
                MessageBoxX.Show($"写入注册表失败\n\n错误信息: {rf}");
                return;
            }
            finally
            {
                MessageBoxX.Show($"写入成功！","Yu Minecraft Launcher");
            }
        }
    }
}
