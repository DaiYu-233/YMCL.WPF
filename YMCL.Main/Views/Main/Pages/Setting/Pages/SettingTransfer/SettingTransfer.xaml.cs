using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
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
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;

namespace YMCL.Main.Views.Main.Pages.Setting.Pages.SettingTransfer
{
    /// <summary>
    /// SettingTransfer.xaml 的交互逻辑
    /// </summary>
    public partial class SettingTransfer : Page
    {
        public SettingTransfer()
        {
            InitializeComponent();
        }

        private void ExportHexString_Click(object sender, RoutedEventArgs e)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(File.ReadAllText(Const.SettingDataPath));
            StringBuilder hexBuilder = new StringBuilder(utf8Bytes.Length * 2);
            foreach (byte b in utf8Bytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }
            Method.LauncherMessageBoxShow($"\n{hexBuilder}\n");
            System.Windows.Clipboard.SetText(hexBuilder.ToString());
            Toast.Show(message: $"{Public.Lang.MainLang.Copied}", position: ToastPosition.Top, window: Const.Window.main);
        }

        private void ExportImportParameter_Click(object sender, RoutedEventArgs e)
        {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(File.ReadAllText(Const.SettingDataPath));
            StringBuilder hexBuilder = new StringBuilder(utf8Bytes.Length * 2);
            foreach (byte b in utf8Bytes)
            {
                hexBuilder.AppendFormat("{0:x2}", b);
            }
            var str = $"ymcl://--import setting '{hexBuilder}'";
            Method.LauncherMessageBoxShow($"\n{str}\n");
            System.Windows.Clipboard.SetText(str);
            Toast.Show(message: $"{MainLang.Copied}", position: ToastPosition.Top, window: Const.Window.main);
        }

        private void BeginImport_Click(object sender, RoutedEventArgs e)
        {
            HexTextBox.Text = string.Empty;
            ImportDialog.ShowAsync();
        }

        private void CancelImportButton_Click(object sender, RoutedEventArgs e)
        {
            ImportDialog.Hide();
        }

        private void AcceptImportButton_Click(object sender, RoutedEventArgs e)
        {
            var hexString = HexTextBox.Text.Trim();
            byte[] hexBytes = Enumerable.Range(0, hexString.Length / 2)
                                    .Select(i => Convert.ToByte(hexString.Substring(i * 2, 2), 16))
                                    .ToArray();
            string data = Encoding.ASCII.GetString(hexBytes);
            var source = JObject.FromObject(JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath)));
            var import = JObject.Parse(data);
            source.Merge(import, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(source, Formatting.Indented));
            MessageBoxX.Show($"\n{MainLang.ImportFinish}\n\n{data}", "Yu Minecraft Launcher");
            Method.RestartApp();
        }
    }
}
