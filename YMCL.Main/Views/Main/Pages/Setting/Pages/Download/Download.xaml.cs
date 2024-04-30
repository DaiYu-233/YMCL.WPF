using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using YMCL.Main.Public;

namespace YMCL.Main.Views.Main.Pages.Setting.Pages.Download
{
    /// <summary>
    /// Download.xaml 的交互逻辑
    /// </summary>
    public partial class Download : Page
    {
        public Download()
        {
            InitializeComponent();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            DownloadSourceComboBox.SelectedItem = DownloadSourceComboBox.Items[(int)setting.DownloadSource];
        }

        private void DownloadSourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (DownloadSourceComboBox.SelectedIndex == (int)setting.DownloadSource) { return; }
            setting.DownloadSource = (Public.Class.SettingItem.DownloadSource)DownloadSourceComboBox.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value);
            SilderInfo.Text = $"{SilderBox.Value}";
            if (!_FirstLoad)
            {
                if (SilderBox.Value >= 64)
                {
                    DownloadThreadTooBig.IsOpen = true;
                }
                else
                {
                    DownloadThreadTooBig.IsOpen = false;
                }
            }
        }

        private void SilderBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MaxDownloadThread = Math.Round(SilderBox.Value);
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        bool _FirstLoad = true;
        private void DownloadThreadTooBig_Loaded(object sender, RoutedEventArgs e)
        {
            if(_FirstLoad)
            {
                _FirstLoad = false;
                var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
                SilderBox.Value = setting.MaxDownloadThread;
            }
        }
    }
}
