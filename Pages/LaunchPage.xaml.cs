using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YMCL.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page
    {
        public LaunchPage()
        {
            InitializeComponent();
            hitokoto();
            LoadMinecraftFolders();

            HitokotoTextBlock.MouseRightButtonDown += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(HitokotoTextBlock.Text);
                Toast.Show("已复制到剪切板", ToastPosition.Top);
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MinecraftVersionBorderCloseButton_Click(sender, e);
        }

        #region MinecraftFolder
        List<string> minecraftFolders = new();
        void LoadMinecraftFolders()
        {
            MinecraftPathComboBox.Items.Clear();
            minecraftFolders = JsonConvert.DeserializeObject<List<string>>
                (File.ReadAllText(Const.YMCLMinecraftFolderDataPath));
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            foreach (var item in minecraftFolders)
            {
                MinecraftPathComboBox.Items.Add(item);
            }
            MinecraftPathComboBox.SelectedItem = setting.MinecraftPath;
        }

        private void AddMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string path = string.Empty;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;//获取选中文件路径
                string[] array = path.Split(@"\");
                if (array[array.Length - 1] != ".minecraft")
                {
                    Toast.Show(Const.Window.main, "需选择 .minecraft 文件夹", ToastPosition.Top);
                    return;
                }
                var isIncludePath = false;
                var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.YMCLMinecraftFolderDataPath));
                if (dataArray != null)
                {
                    foreach (var item in dataArray)
                    {
                        if (item == path)
                        {
                            isIncludePath = true;
                        }
                    }
                    if (isIncludePath)
                    {
                        Toast.Show(Const.Window.main, "已存在此文件夹", ToastPosition.Top);
                        MinecraftPathComboBox.SelectedItem = path;
                    }
                    else
                    {
                        minecraftFolders.Add(path);
                        File.WriteAllText(Const.YMCLMinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolders, Formatting.Indented));
                        MinecraftPathComboBox.Items.Add(path);
                        MinecraftPathComboBox.SelectedItem = path;
                    }
                }
                else
                {
                    var obj = new List<string>()
                    {
                        ".minecraft"
                    };
                    var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    File.WriteAllText(Const.YMCLMinecraftFolderDataPath, data);
                }
            }
        }

        private void DelMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            var index = MinecraftPathComboBox.SelectedIndex;
            minecraftFolders.RemoveAt(index);
            MinecraftPathComboBox.Items.RemoveAt(index);
            if (minecraftFolders.Count == 0)
            {
                minecraftFolders.Add(".minecraft");
                MinecraftPathComboBox.Items.Add(".minecraft");
            }
            MinecraftPathComboBox.SelectedIndex = 0;
            File.WriteAllText(Const.YMCLMinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolders, Formatting.Indented));
        }

        private void MinecraftPathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = MinecraftPathComboBox.SelectedItem as string;
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.MinecraftPath = item;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadMinecraftVersion();
        }
        #endregion
        #region MinecreaftVersion

        void LoadMinecraftVersion()
        {
            VersionListView.Items.Clear();
            List<string> gameCores = new();
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            MinecraftLaunch.Modules.Utils.GameCoreUtil gameCoreUtil = new(setting.MinecraftPath);
            gameCoreUtil.GetGameCores().ToList().ForEach(gameCore =>
            {
                gameCores.Add(gameCore.Id);
            });
            gameCores.ForEach(gameCore =>
            {
                VersionListView.Items.Add(gameCore);
            });
        }

        private void ToVersionList_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
            HitokotoTextBlock.Visibility = Visibility.Hidden;
            LoadMinecraftVersion();
        }

        private void MinecraftVersionBorderCloseButton_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
            HitokotoTextBlock.Visibility = Visibility.Visible;
        }

        private void VersionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MinecraftVersionBorderCloseButton_Click(sender, e);
        }


        #endregion


        //获取一言(https://hitokoto.cn/)
        public async void hitokoto(object? sender = null, MouseButtonEventArgs? e = null)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = "https://v1.hitokoto.cn";
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Class.Hitokoto.Root>(data);
                    if (obj != null)
                    {
                        var res = string.Empty;
                        if (obj.from_who != null)
                        {
                            res = obj.hitokoto + $"    ——「{obj.from} ({obj.from_who})」";
                        }
                        else
                        {
                            res = obj.hitokoto + "    ——「" + obj.from + "」";
                        }
                        await Dispatcher.BeginInvoke(() => { HitokotoTextBlock.Text = res; });
                    }
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.BeginInvoke(() => { HitokotoTextBlock.Text = ex.Message; });
            }
        }


    }
}
