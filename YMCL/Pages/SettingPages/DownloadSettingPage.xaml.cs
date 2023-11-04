using MinecraftLaunch.Modules.Utilities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// DownloadSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadSettingPage : Page
    {
        public DownloadSettingPage()
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            var thread = setting.DownloadThread;
            InitializeComponent();
            SilderBox.Value = thread;
            switch (setting.DownloadSource)
            {
                case "Mcbbs":
                    DownloadSourceComboBox.SelectedIndex = 0;
                    break;
                case "BMCLApi":
                    DownloadSourceComboBox.SelectedIndex = 1;
                    break;
                case "Mojang":
                    DownloadSourceComboBox.SelectedIndex = 2;
                    break;
            }
        }

        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value);
            SilderInfo.Text = SilderBox.Value.ToString();
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.DownloadThread = (int)SilderBox.Value;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void DownloadSourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            switch (DownloadSourceComboBox.SelectedIndex)
            {
                case 0:
                    setting.DownloadSource = "Mcbbs";
                    break;
                case 1:
                    setting.DownloadSource = "BMCLApi";
                    break;
                case 2:
                    setting.DownloadSource = "Mojang";
                    break;
            }
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
    }
}
