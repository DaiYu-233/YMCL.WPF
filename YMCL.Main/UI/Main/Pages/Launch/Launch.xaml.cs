using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Resolver;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using YMCL.Main.Public;
using YMCL.Main.UI.Initialize;
using YMCL.Main.UI.Lang;

namespace YMCL.Main.UI.Main.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        public Launch()
        {
            InitializeComponent();

            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));

            if (setting.UseCustomHomePage)
            {
                LoadCustomPage();
            }
        }

        async void LoadCustomPage()
        {
            try
            {
                FileStream fs = new FileStream(Const.LaunchPageXamlPath, FileMode.Open);
                DependencyObject rootElement = (DependencyObject)XamlReader.Load(fs);
                this.PageRoot.Content = rootElement;
            }
            catch (Exception ex)
            {
                MessageBoxX.Show($"\n{LangHelper.Current.GetText("Launch_Launch_CustomPageSourceError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
            }
        }


        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MinecraftFolderComboBox.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            foreach (var item in minecraftFolder)
            {
                MinecraftFolderComboBox.Items.Add(item);
            }
            if (setting.MinecraftFolder == null || !minecraftFolder.Contains(setting.MinecraftFolder))
            {
                MinecraftFolderComboBox.SelectedIndex = 0;
            }
            else
            {
                MinecraftFolderComboBox.SelectedItem = setting.MinecraftFolder;
            }

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = "https://v1.hitokoto.cn";
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Public.Class.Hitokoto.Root>(data);
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
                        await Dispatcher.BeginInvoke(() => { Toast.Show(message: obj.hitokoto, position: ToastPosition.Top, window: Const.Window.mainWindow); });
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void OpenVersionList_Click(object sender, RoutedEventArgs e)
        {
            VersionListBorder.Visibility = Visibility.Visible;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                To = new Thickness(10, 10, 10, 10),
                From = new Thickness(10, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                To = new Thickness(0, -30, 80, 0),
                From = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanel.BeginAnimation(MarginProperty, animation1);
            LoadMinecraftVersion();
        }

        private void ReturnHomePage_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = new Thickness(10, 10, 10, 10),
                To = new Thickness(10, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.35")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                From = new Thickness(0, -30, 80, 0),
                To = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.35")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanel.BeginAnimation(MarginProperty, animation1);
        }

        void LoadMinecraftVersion()
        {
            List<GameEntry> versions = new();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            GameResolver resolver = new(setting.MinecraftFolder);
            try
            {
                 versions = resolver.GetGameEntitys().ToList();
            }catch(Exception ex)
            {

            }
            var a = versions;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMinecraftVersion();
        }

        private void DelMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            var index = MinecraftFolderComboBox.SelectedIndex;
            minecraftFolder.RemoveAt(index);
            MinecraftFolderComboBox.Items.RemoveAt(index);
            if (minecraftFolder.Count == 0)
            {
                minecraftFolder.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
                MinecraftFolderComboBox.Items.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
            }
            MinecraftFolderComboBox.SelectedIndex = 0;
            File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MinecraftFolder = MinecraftFolderComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
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
                    Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_NeedMinecraftFolder"), ToastPosition.Top);
                    return;
                }
                var isIncludePath = false;
                var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.MinecraftFolderDataPath));
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
                        Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_ExistsMinecraftFolder"), ToastPosition.Top);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                    else
                    {
                        minecraftFolder.Add(path);
                        File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
                        MinecraftFolderComboBox.Items.Add(path);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                }
                else
                {
                    var obj = new List<string>()
                    {
                        System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft")
                    };
                    var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    File.WriteAllText(Const.MinecraftFolderDataPath, data);
                }
            }
        }
    }
}
