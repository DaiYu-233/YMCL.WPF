using iNKORE.UI.WPF.Modern;
using Microsoft.Win32;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Pkcs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
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
            DesktopLyricTextAlignmentComboBox.SelectedIndex = (int)setting.DesktopLyricTextAlignment;
            DesktopLyricTextSizeSlider.Value = (int)setting.DesktopLyricTextSize;
            CustomHomePageComboBox.SelectedIndex = (int)setting.CustomHomePage;
            CustomHomePageXamlUrlTextBox.Text = setting.CustomHomePageNetXamlUrl;
            CustomHomePageCSharpUrlTextBox.Text = setting.CustomHomePageNetCSharpUrl;
            CustomHomePageComboBox_SelectionChanged(null, null);
        }
        private void LoadTheme(object? sender, EventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            ThemeManager.Current.AccentColor = setting.ThemeColor;
            ResourceDictionary appResources = System.Windows.Application.Current.Resources;
            appResources["IconBlue"] = new SolidColorBrush((System.Windows.Media.Color)ThemeManager.Current.AccentColor);
            ColorPicker.SelectedColor = setting.ThemeColor;
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
            LoadTheme(null, null);
        }
        private void CustomHomePageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomHomePageUIBorder.Height = CustomHomePageComboBox.SelectedIndex == 2 ? 83.5 : 45;
            EditCustomHomePageXamlBtn.Visibility = CustomHomePageComboBox.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            //EditCustomHomePageCSharpBtn.Visibility = CustomHomePageComboBox.SelectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            EditCustomHomePageCSharpBtn.Visibility = Visibility.Collapsed;

            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (CustomHomePageComboBox.SelectedIndex == (int)setting.CustomHomePage)
            {
                return;
            }
            Toast.Show(message: LangHelper.Current.GetText("NeedRestartApp"), position: ToastPosition.Top, window: Const.Window.main);
            setting.CustomHomePage = (SettingItem.CustomHomePage)CustomHomePageComboBox.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));

            if (setting.CustomHomePage == SettingItem.CustomHomePage.LocalFile && !File.Exists(Const.CustomHomePageXamlPath))
            {
                string resourceName = "YMCL.Main.Public.Text.CustomHomePageDefault.xaml";
                Assembly _assembly = Assembly.GetExecutingAssembly();
                Stream stream = _assembly.GetManifestResourceStream(resourceName);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(Const.CustomHomePageXamlPath, result);
                }
            }

            Function.RestartApp();
        }
        private void CustomHomePageXamlUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.CustomHomePageNetXamlUrl = CustomHomePageXamlUrlTextBox.Text;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void EditCustomHomePageBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", Const.CustomHomePageXamlPath);
        }
        private void EditCustomHomePageCSharpBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", Const.CustomHomePageCSharpPath);
        }
        private void CustomHomePageCSharpUrlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.CustomHomePageNetCSharpUrl = CustomHomePageCSharpUrlTextBox.Text;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void ColorPicker_SelectedColorChanged(object sender, Panuon.WPF.SelectedValueChangedRoutedEventArgs<System.Windows.Media.Color?> e)
        {
            var color = ColorPicker.SelectedColor;
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.ThemeColor == color || color == null)
            {
                return;
            }
            setting.ThemeColor = (System.Windows.Media.Color)color;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadTheme(null, null);
        }

        private void DesktopLyricTextAlignmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (DesktopLyricTextAlignmentComboBox.SelectedIndex == (int)setting.DesktopLyricTextAlignment)
            {
                return;
            }
            setting.DesktopLyricTextAlignment = (TextAlignment)DesktopLyricTextAlignmentComboBox.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            Const.Window.desktopLyric.Lyric.TextAlignment = (TextAlignment)DesktopLyricTextAlignmentComboBox.SelectedIndex;
        }

        private void DesktopLyricTextSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            DesktopLyricTextSizeSlider.Value = Math.Round(DesktopLyricTextSizeSlider.Value);
            DesktopLyricTextSizeSilderInfo.Text = $"{DesktopLyricTextSizeSlider.Value}";
            if (Const.Window.desktopLyric != null)
            {
                Const.Window.desktopLyric.Lyric.FontSize = DesktopLyricTextSizeSlider.Value;
            }
        }

        private void DesktopLyricTextSizeSlider_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.DesktopLyricTextSize = DesktopLyricTextSizeSlider.Value;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
    }
}
