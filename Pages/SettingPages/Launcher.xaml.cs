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
using Panuon.WPF.UI;
using Microsoft.Win32;
using System.Security.Principal;
using Newtonsoft.Json;
using YMCL.Class;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public string path = System.AppDomain.CurrentDomain.BaseDirectory + @"YMCL\YMCL.bat";


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
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            #region 初始化

                if (obj.DisplayInformation == "False")
                {
                    displayinf.IsOn = false;
                }
                else
                {
                    displayinf.IsOn = true;
                }

            

            if (obj.Theme == "Dark")
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
            
            #endregion
        }







        private void SaveSettingBtn_Click(object sender, RoutedEventArgs e)
        {
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
                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
                obj.DisplayInformation = "True";
                File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
                obj.DisplayInformation = "False";
                File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
            }
            if (DarkThemeSwitch.IsOn == true)
            {
                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
                obj.Theme = "Dark";
                File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
                obj.Theme = "Light";
                File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
            }
            Panuon.WPF.UI.Toast.Show("已保存", ToastPosition.Top);
            //Toast.Show("成功应用设置", new ToastOptions { Icon = ToastIcons.Information, Time = 1500, Location = ToastLocation.OwnerTopCenter });

        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"./YMCL/YMCL.bat", $"{path[0]}:\ncd {AppDomain.CurrentDomain.BaseDirectory}\nstart ./YMCL.exe");
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
                MessageBoxX.Show($"写入注册表失败\n\n错误信息: {rf}","Yu Minecraft Launcher");
                return;
            }
            finally
            {
                Panuon.WPF.UI.Toast.Show("写入成功！", ToastPosition.Top);
                //MessageBoxX.Show($"写入成功！","Yu Minecraft Launcher");
            }
        }
    }
}
