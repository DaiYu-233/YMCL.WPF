using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using YMCL.Class;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Download.xaml 的交互逻辑
    /// </summary>
    public partial class Download : Page
    {
        public Download()
        {
            InitializeComponent();

            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            if (obj.DownloadSoure=="Mcbbs")
            {
                DownloadSourseCombo.SelectedItem = DownloadSourseCombo.Items[0];
            }else if (obj.DownloadSoure == "BMCLAPI")
            {
                DownloadSourseCombo.SelectedItem = DownloadSourseCombo.Items[1];
            }else if(obj.DownloadSoure == "Mojang")
            {
                DownloadSourseCombo.SelectedItem = DownloadSourseCombo.Items[2];
            }
            SilderBox.Value = Convert.ToInt32(obj.MaxDownloadThreads);
        }

        private void DownloadSourseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            if (DownloadSourseCombo.SelectedIndex == 0)
            {
                obj.DownloadSoure = "Mcbbs";
            }
            else if (DownloadSourseCombo.SelectedIndex == 1)
            {
                obj.DownloadSoure = "BMCLAPI";
            }
            else if (DownloadSourseCombo.SelectedIndex == 2)
            {
                obj.DownloadSoure = "Mojang";
            }
                File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }

        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            SilderBox.Value = Math.Round(SilderBox.Value, 0);
            SilderInfo.Text = SilderBox.Value.ToString();

            
        }

        private void SilderBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SilderBox.Value >= 128)
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "下载线程过大，可能会导致卡顿", ToastPosition.Top);
            }
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            obj.MaxDownloadThreads = SilderInfo.Text;
            File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
