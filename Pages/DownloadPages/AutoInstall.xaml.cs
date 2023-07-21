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
using MinecraftLaunch;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.FluentCore.Model.Install.Forge;
using Natsurainko.FluentCore.Module.Downloader;
using Natsurainko.FluentCore.Module.Installer;
using Natsurainko.FluentCore.Module.Launcher;
using Natsurainko.FluentCore.Service;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using YMCL.Class;

namespace YMCL.Pages.DownloadPages
{
    /// <summary>
    /// AutoInstall.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstall : Page
    {
        List<Ver> vers = new List<Ver>();
        bool IsForge = false;
        bool IsFabric = false;
        bool IsOptiFine = false;
        bool IsQuilt = false;
        public AutoInstall()
        {
            InitializeComponent();
            ReGetVer();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(VerListView.Items);
            view.Filter = UserFilter;
        }
        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(SearBox.Text))
            {
                return true;
            }
            else
            {
                return ((item as Ver).Id.IndexOf(SearBox.Text, StringComparison.OrdinalIgnoreCase)>=0);
            }
        }
        #region GetGame
        private async void ReGetVer()
        {
            try
            {
                await Task.Run(async () =>
                {

                    Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = false; });
                    Dispatcher.BeginInvoke(() => { vers.Clear(); });
                    var res = GameCoreInstaller.GetGameCoresAsync().Result.Cores.ToList();
                    res.ForEach(x =>
                    {
                        Dispatcher.BeginInvoke(() => { vers.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() }); });
                    });
                    Dispatcher.BeginInvoke(() => { VerListView.ItemsSource = vers; });
                    Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = true; });
                });
            }
            catch
            {

            }


        }
        private async void GetForgeVer()
        {
            VerForgeListView.Items.Clear();
            await Task.Run(async () =>
            {
                var builds = MinecraftForgeInstaller.GetForgeBuildsFromMcVersionAsync(File.ReadAllText("./YMCL/Temp/InsVer.log")).GetAwaiter().GetResult().ToList();
                builds.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Add(x); });
                });
            });
            ForgeProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetFabricVer()
        {
            VerFabricListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = MinecraftFabricInstaller.GetFabricBuildsFromMcVersionAsync(File.ReadAllText("./YMCL/Temp/InsVer.log")).GetAwaiter().GetResult().ToList();
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Add(x); });
                });
            });
            FabricProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetOptiFineVer()
        {
            VerOptiFineListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = MinecraftOptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(File.ReadAllText("./YMCL/Temp/InsVer.log")).GetAwaiter().GetResult().ToList();

                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Add(x); });
                });
            });
            OptiFineProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetQuiltVer()
        {
            VerQuiltListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = MinecraftQuiltInstaller.GetQuiltBuildsFromMcVersionAsync(System.IO.File.ReadAllText("./YMCL/Temp/InsVer.log")).GetAwaiter().GetResult().ToList();
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerQuiltListView.Items.Add(x); });
                });
            });
            QuiltProgressRing.Visibility = Visibility.Hidden;
        }
        #endregion

        #region
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DownloadInfo.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DownloadInfo.Visibility = Visibility.Visible;
        }
        public class Ver
        {
            public string? Id { get; set; }
            public string? ReleaseTime { get; set; }
        }
        private void TestIns()
        {
            if (IsForge || IsFabric || IsOptiFine || IsQuilt)
            {
                fjaz.Text = "有附加安装";
            }
            else
            {
                fjaz.Text = "无附加安装";
            }
        }
        private void CloseInses()
        {
            FabricBr.Visibility = Visibility.Hidden;
            ForgeBr.Visibility = Visibility.Hidden;
            OptiFineBr.Visibility = Visibility.Hidden;
            QuiltBr.Visibility = Visibility.Hidden;
        }
        private void VerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerListView.SelectedIndex >= 0)
            {
                NextStep.IsEnabled = true;
            }

        }
        public class WatermarkService : DependencyObject
        {
            public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
                "Watermark",
                typeof(string),
                typeof(WatermarkService),
                new FrameworkPropertyMetadata(string.Empty));

            public static readonly DependencyProperty IsWatermarkEnabledProperty = DependencyProperty.RegisterAttached(
                "IsWatermarkEnabled",
                typeof(bool),
                typeof(WatermarkService),
                new FrameworkPropertyMetadata(false, IsWatermarkEnabledChanged));

            public static string GetWatermark(DependencyObject obj)
            {
                return (string)obj.GetValue(WatermarkProperty);
            }

            public static void SetWatermark(DependencyObject obj, string value)
            {
                obj.SetValue(WatermarkProperty, value);
            }

            public static bool GetIsWatermarkEnabled(DependencyObject obj)
            {
                return (bool)obj.GetValue(IsWatermarkEnabledProperty);
            }

            public static void SetIsWatermarkEnabled(DependencyObject obj, bool value)
            {
                obj.SetValue(IsWatermarkEnabledProperty, value);
            }

            private static void IsWatermarkEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var textBox = d as TextBox;
                if (textBox == null)
                {
                    return;
                }

                if ((bool)e.NewValue)
                {
                    textBox.GotFocus += TextBox_GotFocus;
                    textBox.LostFocus += TextBox_LostFocus;
                    UpdateWatermark(textBox);
                }
                else
                {
                    textBox.GotFocus -= TextBox_GotFocus;
                    textBox.LostFocus -= TextBox_LostFocus;
                    RemoveWatermark(textBox);
                }
            }

            private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
            {
                var textBox = sender as TextBox;
                if (textBox == null)
                {
                    return;
                }

                RemoveWatermark(textBox);
            }

            private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
            {
                var textBox = sender as TextBox;
                if (textBox == null)
                {
                    return;
                }

                UpdateWatermark(textBox);
            }

            private static void UpdateWatermark(TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Tag = textBox.Background;
                    textBox.Background = Brushes.Transparent;
                    textBox.Text = GetWatermark(textBox);
                }
            }

            private static void RemoveWatermark(TextBox textBox)
            {
                if (textBox.Text == GetWatermark(textBox))
                {
                    textBox.Text = string.Empty;
                    textBox.Background = (Brush)textBox.Tag;
                }
            }
        }

        private void SearBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(VerListView.ItemsSource).Refresh();
        }
        private async void RefreshVerListBtn_Click(object sender, RoutedEventArgs e)
        {
            ReGetVer();


        }
        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            IsForge = false;
            IsFabric = false;
            IsOptiFine = false;
            IsQuilt = false;
            ForgeInfo.Text = "无安装任务";
            FabricInfo.Text = "无安装任务";
            QuiltInfo.Text = "无安装任务";
            OptiFineInfo.Text = "无安装任务";
            TestIns();
            Step1Br.Visibility = Visibility.Hidden;
            Step2Br.Visibility = Visibility.Visible;
            InstallVerText.Text = VerListView.SelectedValue.ToString();

            ForgeProgressRing.Visibility = Visibility.Visible;
            OptiFineProgressRing.Visibility = Visibility.Visible;
            FabricProgressRing.Visibility = Visibility.Visible;
            QuiltProgressRing.Visibility = Visibility.Visible;

            System.IO.File.WriteAllText("./YMCL/Temp/InsVer.log", VerListView.SelectedValue.ToString());

        }

        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            Step1Br.Visibility = Visibility.Visible;
            Step2Br.Visibility = Visibility.Hidden;
        }

        

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseInses();
        }



        private void ToForgeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsQuilt || IsFabric)
            {
                MessageBoxX.Show("Forge与Fabric或Quilt不兼容!");
            }
            else
            {
                ForgeBr.Visibility = Visibility.Visible;
                GetForgeVer();
            }
            TestIns();
        }
        private void VerForgeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerForgeListView.SelectedIndex >= 0)
            {

                Natsurainko.FluentCore.Model.Install.Forge.ForgeInstallBuild forgeInstallBuild
                    = VerForgeListView.SelectedItem as Natsurainko.FluentCore.Model.Install.Forge.ForgeInstallBuild;
                CloseInses();
                IsForge = true;
                FabricInfo.Text = "与Forge不兼容";
                ForgeInfo.Text = forgeInstallBuild.BuildVersion;
                QuiltInfo.Text = "与Forge不兼容";

                TestIns();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsForge = false;
            IsFabric = false;
            IsOptiFine = false;
            IsQuilt = false;
            ForgeInfo.Text = "无安装任务";
            FabricInfo.Text = "无安装任务";
            QuiltInfo.Text = "无安装任务";
            OptiFineInfo.Text = "无安装任务";
            TestIns();
        }

        private void ToOptiFineBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsFabric||IsQuilt)
            {
                MessageBoxX.Show("OptiFine与Fabric或Quilt不兼容!");
                return;
            }
            else
            {
                OptiFineBr.Visibility = Visibility.Visible;
                GetOptiFineVer();
            }
            TestIns();
        }

        private void VerOptiFineListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerOptiFineListView.SelectedIndex >= 0)
            {
                Natsurainko.FluentCore.Model.Install.OptiFine.OptiFineInstallBuild obj
                    = VerOptiFineListView.SelectedItem as Natsurainko.FluentCore.Model.Install.OptiFine.OptiFineInstallBuild;
                CloseInses();
                FabricInfo.Text = "与OptiFine不兼容";
                QuiltInfo.Text = "与OptiFine不兼容";
                IsOptiFine = true;
                OptiFineInfo.Text = obj.BuildVersion;
                TestIns();
            }
        }

        private void ToFabricBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsForge||IsQuilt||IsOptiFine)
            {
                MessageBoxX.Show("Fabric与Forge、OptiFine或Quilt不兼容!"); 
            }
            else
            {
                FabricBr.Visibility = Visibility.Visible;
                GetFabricVer();
            }
            TestIns();
        }

        private void VerFabricListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerFabricListView.SelectedIndex >= 0)
            {
                Natsurainko.FluentCore.Model.Install.Fabric.FabricInstallBuild obj
                    = VerFabricListView.SelectedItem as Natsurainko.FluentCore.Model.Install.Fabric.FabricInstallBuild;
                CloseInses();
                OptiFineInfo.Text = "与Fabric不兼容";
                QuiltInfo.Text = "与Fabric不兼容";
                ForgeInfo.Text = "与Fabric不兼容";
                IsFabric = true;
                FabricInfo.Text = obj.BuildVersion;
                TestIns();
            }
        }

        private void ToQuiltBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsFabric || IsForge||IsOptiFine)
            {
                MessageBoxX.Show("Quilt与Forge、OptiFine或Fabric不兼容!");
            }
            else
            {
                QuiltBr.Visibility = Visibility.Visible;
                GetQuiltVer();
            }
            TestIns();
        }

        private void VerQuiltListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerQuiltListView.SelectedIndex >= 0)
            {
                Natsurainko.FluentCore.Model.Install.Quilt.QuiltInstallBuild obj
                    = VerQuiltListView.SelectedItem as Natsurainko.FluentCore.Model.Install.Quilt.QuiltInstallBuild;
                CloseInses();
                OptiFineInfo.Text = "与Fabric不兼容";
                FabricInfo.Text = "与Fabric不兼容";
                ForgeInfo.Text = "与Fabric不兼容";
                IsQuilt = true;
                QuiltInfo.Text = obj.BuildVersion;
                TestIns();
            }
        }
        #endregion
        private async void StartInsBtn_Click(object sender, RoutedEventArgs e)
        {
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            #region 下载源
            if (obj.DownloadSoure == "Mcbbs")
            {
                DownloadApiManager.Current = DownloadApiManager.Mcbbs;
            }
            else if (obj.DownloadSoure == "BMCLAPI")
            {
                DownloadApiManager.Current = DownloadApiManager.Bmcl;
            }
            else if (obj.DownloadSoure == "Mojang")
            {
                DownloadApiManager.Current = DownloadApiManager.Mojang;
            }
            #endregion
            ResourceDownloader.MaxDownloadThreads = Convert.ToInt32(obj.MaxDownloadThreads); //下载最大线程数

            //Toast.Show($"开始安装{File.ReadAllText("./YMCL/Temp/InsVer.log") }\n下载源：{obj.DownloadSoure}  最大线程：{obj.MaxDownloadThreads}", new ToastOptions { Icon = ToastIcons.Information, Time = 3000, Location = ToastLocation.OwnerTopCenter });
            DownloadText.Text = DownloadText.Text + $"[{ DateTime.Now.ToString()}]    开始安装{File.ReadAllText("./YMCL/Temp/InsVer.log")}  下载源：{obj.DownloadSoure}  最大线程：{obj.MaxDownloadThreads}\n";
            #region UI
            DownloadInfo.Visibility = Visibility.Visible;
            installbz.Text = "Vanllia";
            DownloadText.Text = "";
            Step1Br.Visibility = Visibility.Visible;
            Step2Br.Visibility = Visibility.Hidden;
            #endregion
            await Task.Run(async () => {
                var installer = new MinecraftVanlliaInstaller(new GameCoreLocator(@".minecraft"), File.ReadAllText("./YMCL/Temp/InsVer.log")) ;
            installer.ProgressChanged += (sender, e) =>
            {
                #region UI
                Dispatcher.BeginInvoke(() => {
                    DownloadProText.Text = $"{e.TotleProgress * 100:0.00}%";
                    DownloadProBar.Value = Math.Round(e.TotleProgress * 100, 1);
                    
                });
                #endregion
            };
            var res = await installer.InstallAsync();
            });
            DownloadText.Text = DownloadText.Text + $"游戏核心 {File.ReadAllText("./YMCL/Temp/InsVer.log")} 安装完成";
            MessageBoxX.Show($"游戏核心 {File.ReadAllText("./YMCL/Temp/InsVer.log")} 安装完成");
            DownloadInfo.Visibility = Visibility.Hidden;
        }

        private void DownloadText_TextChanged(object sender, TextChangedEventArgs e)
        {
            DownloadText.Select(DownloadText.Text.Length, 0);
        }
    }
}

//a++;
//if (a == 19)
//{
//    a = 0;
//    DownloadText.Text = "";
//}