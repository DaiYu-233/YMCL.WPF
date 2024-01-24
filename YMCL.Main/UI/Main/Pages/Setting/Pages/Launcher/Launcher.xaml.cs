using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using YMCL.Main.Public;
using Application = System.Windows.Forms.Application;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Launcher
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public Launcher()
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

        private void UseCustomHomePageToggle_Toggled(object sender, System.Windows.RoutedEventArgs e)
        {
            //var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            //if (UseCustomHomePageToggle.IsOn == setting.UseCustomHomePage)
            //{
            //    return;
            //}
            //if (UseCustomHomePageToggle.IsOn)
            //{
            //    var message = MessageBoxX.Show(LangHelper.Current.GetText("Launcher_UseCustomHomePageToggle_Toggled_On_Info"), "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
            //    if (message == MessageBoxResult.Cancel)
            //    {
            //        UseCustomHomePageToggle.IsOn = false;
            //        return;
            //    }
            //}
            //setting.UseCustomHomePage = UseCustomHomePageToggle.IsOn;
            //File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting));
            //ProcessStartInfo startInfo = new ProcessStartInfo
            //{
            //    WorkingDirectory = Environment.CurrentDirectory,
            //    FileName = Application.ExecutablePath,
            //};
            //Process.Start(startInfo);
            //System.Windows.Application.Current.Shutdown();
        }

        private void EditCustomHomePageBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start("notepad.exe", Const.LaunchPageXamlPath);
        }
    }
}
