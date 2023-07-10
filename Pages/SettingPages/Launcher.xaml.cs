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

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
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
            Panuon.WPF.UI.Toast.Show("成功应用设置", ToastPosition.Top);
        }
    }
}
