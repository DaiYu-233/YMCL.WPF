using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using Natsurainko.FluentCore.Module.Launcher;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YMCL.Class;
using static PInvoke.Kernel32;

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
            GetVers();
            var obj = JsonConvert.DeserializeObject<SettingInfo>
                (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            if (obj.SelectedGameCoreIndex > VerListView.Items.Count - 1 || obj.SelectedGameCoreIndex == -1)
            {
                VerListView.SelectedIndex = -1;
            }
            else
            {
                VerListView.SelectedItem = VerListView.Items[obj.SelectedGameCoreIndex];
            }

            #region


            if (!Directory.Exists(obj.MinecraftPath))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(obj.MinecraftPath);
                directoryInfo.Create();
            }

            GetHitokoto();




            #endregion
        }

        private async void GetHitokoto()
        {
            await Task.Run(async () =>
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = "https://v1.hitokoto.cn";
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var obj = JsonConvert.DeserializeObject<hitokotoClass>(responseBody);
                        var res = obj.hitokoto + "    ——「" + obj.from + "」";
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



        private void GetVers()
        {
            GameCoreLocator Core = new GameCoreLocator(JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json")).MinecraftPath);
            VerListView.ItemsSource = Core.GetGameCores();
        }

        private void GameVerTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            hitokoto.Visibility = Visibility.Hidden;
            VerListBorder.Visibility = Visibility.Visible;
            GetVers();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
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
            if (VerListView.SelectedIndex == -1)
            {
                GameCoreText.Text = "<游戏核心>";
                hitokoto.Visibility = Visibility.Visible;
                VerListBorder.Visibility = Visibility.Hidden;
                return;
            }
            VerListBorder.Visibility = Visibility.Hidden;
            var id = VerListView.SelectedItem as Natsurainko.FluentCore.Model.Launch.GameCore;
            GameCoreText.Text = id.Id;
            hitokoto.Visibility = Visibility.Visible;
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            obj.SelectedGameCoreIndex = VerListView.SelectedIndex;
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }



        private void hitokoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GetHitokoto();
        }

        private void hitokoto_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(hitokoto.Text);
            Toast.Show(GlobalWindow.form_main, "已复制到剪切板", ToastPosition.Top);
        }



        #endregion

        private async void LaunchGame_Click(ModernWpf.Controls.SplitButton sender, ModernWpf.Controls.SplitButtonClickEventArgs args)
        {
            LaunchGame.IsEnabled = false;
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            LaunchConfig launchConfig = new()
            {
                JvmConfig = new JvmConfig(obj.Java)
                {
                    MaxMemory = obj.MaxMem,
                    MinMemory = 128
                },
                //GameWindowConfig = new GameWindowConfig()
                //{
                //    Width = 999,
                //    Height = 999,
                //    IsFullscreen = false
                //},
                NativesFolder = null, //一般可以无视这个选项
                IsEnableIndependencyCore = obj.AloneCore//是否启用版本隔离
            };

            if (VerListView.SelectedIndex >= 0)
            {
                //LaunchGame.IsEnabled = false;
                if (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log") == "离线登录")
                {
                    launchConfig.Account = new MinecraftOAuth.Authenticator.OfflineAuthenticator
                        (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginName.log")).Auth();
                }
                else if (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log") == "微软登录")
                { 
                    var index = obj.LoginIndex;
                    var list = JsonConvert.DeserializeObject<List<AccountInfo>>
                        (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\YMCL.Account.json"));
                    var data = list[Convert.ToInt32(index)].Data;
                    var auth = JsonConvert.DeserializeObject<MinecraftLaunch.Modules.Models.Auth.MicrosoftAccount>(data);
                    launchConfig.Account = auth;
                }
                else if (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log") == "第三方登录")
                {
                    var index = obj.LoginIndex;
                    var list = JsonConvert.DeserializeObject<List<AccountInfo>>
                        (File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\YMCL.Account.json"));
                    var data = list[Convert.ToInt32(index)].Data;
                    var auth = JsonConvert.DeserializeObject<MinecraftLaunch.Modules.Models.Auth.YggdrasilAccount>(data);
                    launchConfig.Account = auth;
                }
                else
                {
                    Toast.Show(GlobalWindow.form_main, "登录错误", ToastPosition.Top);
                    LaunchGame.IsEnabled = true;
                    return;
                }

                GameCoreUtil gameCoreUtil = new(obj.MinecraftPath);
                JavaMinecraftLauncher javaMinecraftLauncher = new JavaMinecraftLauncher(launchConfig, gameCoreUtil);
                var launchId = GameCoreText.Text;

                await Task.Run(async () =>
                {
                    using var res = await javaMinecraftLauncher.LaunchTaskAsync(launchId , x =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(GlobalWindow.form_main, $"{x.Item1*100}% - {x.Item2}", ToastPosition.Top);
                        });
                    });
                    if (res.State is LaunchState.Succeess)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(GlobalWindow.form_main, $"启动完成，等待游戏窗口出现", ToastPosition.Top);
                        });
                        res.Process.WaitForInputIdle();
                        Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(GlobalWindow.form_main, $"启动成功", ToastPosition.Top);
                        });
                    }
                    else
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show("启动失败：" + res.Exception.Message);
                        });
                        
                    }
                    
                });
                LaunchGame.IsEnabled = true;
            }
            else
            {
                //MessageBoxX.Show("请选择游戏核心");
                Toast.Show(GlobalWindow.form_main, "未选择游戏核心", ToastPosition.Top);
                LaunchGame.IsEnabled = true;
            }
        }


    }
}
