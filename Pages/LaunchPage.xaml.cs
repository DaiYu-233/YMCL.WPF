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
using System.Net;
using System.IO;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Models.Auth;
using Newtonsoft.Json;
using YMCL.Pages.SettingPages;
using System.Diagnostics;
using YMCL.Class;
using Panuon.WPF.UI;
using KMCCC.Launcher;
using MinecraftLaunch.Modules.Utils;
using System.Net.Http;
using Natsurainko.FluentCore.Module.Launcher;
using MinecraftLaunch.Modules.Models.Install;

namespace YMCL.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page
    {
        List<GameCore> versions = new List<GameCore>();
        class hitokotoClass
        {
            public string? id { get; set; }
            public string? uuid { get; set; }
            public string? hitokoto { get; set; }
            public string? type { get; set; }
            public string? from { get; set; }
            public string? from_who { get; set; }
            public string? creator { get; set; }
            public string? creator_uid { get; set; }
            public string? reviewer { get; set; }
            public string? commit_from { get; set; }
            public string? created_at { get; set; }
            public string? length { get; set; }
        }

        public LaunchPage()
        {
            InitializeComponent();
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            
            #region





            #endregion


            if (!Directory.Exists(obj.MinecraftPath))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(obj.MinecraftPath);
                directoryInfo.Create();
            }

            GetHitokoto();

        }

        private async void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            if (VerListView.SelectedIndex >= 0)
            {
                LaunchGame.IsEnabled = false;
                if (File.ReadAllText("./YMCL/Temp/LoginType.log") == "离线登录")
                {
                }
                else if (File.ReadAllText("./YMCL/Temp/LoginType.log") == "微软登录")
                {
                }
            }
            else
            {
                //MessageBoxX.Show("请选择游戏核心");
                Toast.Show("请选择游戏核心", ToastPosition.Top);
            }

        }

        private async void GetHitokoto()
        {
            await Task.Run(async() => {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = "https://v1.hitokoto.cn";
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<hitokotoClass>(responseBody);
                        var res = obj.hitokoto + "    ——「" + obj.from+"」";
                        Dispatcher.BeginInvoke(() => { hitokoto.Text = res; });
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(() => { hitokoto.Text = $"请求失败: {ex.Message}"; });
                }
            });
        }




        #region


        public void UpdateLogin()
        {



        }


        private void GetVers()
        {
            GameCoreLocator Core = new GameCoreLocator(JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json")).MinecraftPath);
            VerListView.ItemsSource = Core.GetGameCores();
        }


















        private void  GameVerTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hitokoto.Visibility = Visibility.Hidden;
            VerListBorder.Visibility = Visibility.Visible;
            GetVers();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            hitokoto.Visibility = Visibility.Visible;
            VerListBorder.Visibility = Visibility.Hidden;
        }

        private void VerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
            var id = VerListView.SelectedItem as Natsurainko.FluentCore.Model.Launch.GameCore;
            GameVerTextBlock.Text = id.Id;
            hitokoto.Visibility = Visibility.Visible;
        }

        private void Page_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateLogin();
        }



        #endregion

        private void hitokoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GetHitokoto();
        }

        private void hitokoto_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(hitokoto.Text);
            Toast.Show("已复制到剪切板", ToastPosition.Top);
        }
    }
}
