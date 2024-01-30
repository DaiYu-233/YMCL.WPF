using iNKORE.UI.WPF.Modern;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using Application = System.Windows.Forms.Application;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Personalize
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Personalize : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        public Personalize()
        {
            InitializeComponent();

            var langs = new List<string>() { "zh-CN 简体中文", "zh-Hant 繁體中文", "en-US English", "ja-JP 日本語", "ru-RU Русский язык" };
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            bool added = false;
            langs.ForEach(lang =>
            {
                LanguageComboBox.Items.Add(lang);
            });
            langs.ForEach(lang =>
            {
                var arr = lang.Split(' ');
                if (arr[0] == setting.Language)
                {
                    LanguageComboBox.SelectedItem = lang;
                }
            });
            if (setting.Language == null || setting.Language == string.Empty)
            {
                LanguageComboBox.SelectedItem = "zh-CN 简体中文";
                setting.Language = "zh-CN";
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
            LoadTheme(null, null);
            timer.Tick += LoadTheme;
            timer.Interval = TimeSpan.FromSeconds(0.2);
            timer.Start();
            ThemeComboBox.SelectedIndex = (int)setting.Theme;
        }

        private void LoadTheme(object? sender, EventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.Theme == SettingItem.Theme.Light)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL.Main;component/Public/Style/Light.xaml") });
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
            else if (setting.Theme == SettingItem.Theme.Dark)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL.Main;component/Public/Style/Dark.xaml") });
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key != null)
                {
                    int appsUseLightTheme = (int)key.GetValue("AppsUseLightTheme", -1);
                    if (appsUseLightTheme == 0)
                    {
                        System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL.Main;component/Public/Style/Dark.xaml") });
                        ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    }
                    else if (appsUseLightTheme == 1)
                    {
                        System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL.Main;component/Public/Style/Light.xaml") });
                        ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    }
                    key.Close();
                }
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (LanguageComboBox.SelectedItem.ToString().Split(' ')[0] != setting.Language)
            {
                setting.Language = LanguageComboBox.SelectedItem.ToString().Split(' ')[0];
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting));
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath,
                };
                Process.Start(startInfo);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (ThemeComboBox.SelectedIndex == (int)setting.Theme)
            {
                return;
            }
            setting.Theme = (SettingItem.Theme)ThemeComboBox.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadTheme(null,null);
        }
    }
}
