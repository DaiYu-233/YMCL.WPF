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
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using Natsurainko.FluentCore.Model.Install.Fabric;
using Natsurainko.FluentCore.Model.Install.Forge;
using Natsurainko.FluentCore.Model.Install.OptiFine;
using Natsurainko.FluentCore.Module.Downloader;
using Natsurainko.FluentCore.Module.Installer;
using Natsurainko.FluentCore.Module.Launcher;
using Natsurainko.FluentCore.Service;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using YMCL.Class;

namespace YMCL.Pages.DownloadPages
{
    /// <summary>
    /// AutoInstall.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstall : Page
    {
        string InsVer;
        string InsVerName;
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
                vers.Clear();
                await Task.Run(async () =>
                {

                    Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = false; });
                    var res = GameCoreInstaller.GetGameCoresAsync().Result.Cores.ToList();
                    res.ForEach(x =>
                    {
                        Dispatcher.BeginInvoke(() => { vers.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString(),Type=x.Type }); });
                    });
                    Dispatcher.BeginInvoke(() => { VerListView.ItemsSource = vers; });
                    Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = true; });
                });
            }
            catch
            {
                Panuon.WPF.UI.Toast.Show($"可安装版本列表失败，这可能是网络原因", ToastPosition.Top);
            }


        }
        private async void GetForge()
        {
            QuiltProgressRing.Visibility = Visibility.Visible;
            VerForgeListView.Items.Clear();
            await Task.Run(async () =>
            {
                var builds = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(InsVer)).ToList();
                builds.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Add(x); });
                });
            });
            ForgeProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetFabric()
        {
            QuiltProgressRing.Visibility = Visibility.Visible;
            VerFabricListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await FabricInstaller.GetFabricBuildsByVersionAsync(InsVer)).ToList();
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Add(x); });
                });
            });
            FabricProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetOptiFine()
        {
            QuiltProgressRing.Visibility = Visibility.Visible;
            VerOptiFineListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(InsVer)).ToList();
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Add(x); });
                });
            });
            OptiFineProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetQuilt()
        {
            QuiltProgressRing.Visibility = Visibility.Visible;
            VerQuiltListView.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(InsVer)).ToList();
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
            public string? Type { get; set; }
        }
        private void TestIns()
        {
            if (IsForge || IsFabric || IsOptiFine || IsQuilt)
            {
                fjaz.Text = "";
                if (IsForge)
                {
                    fjaz.Text = $"{fjaz.Text}Forge-{ForgeInfo.Text} ";
                }
                if (IsFabric)
                {
                    fjaz.Text = $"{fjaz.Text}Fabric-{FabricInfo.Text} ";
                }
                if (IsOptiFine)
                {
                    fjaz.Text = $"{fjaz.Text}OptiFine-{OptiFineInfo.Text} ";
                }
                if (IsQuilt)
                {
                    fjaz.Text = $"{fjaz.Text}Quilt-{QuiltInfo.Text} ";
                }
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
        private async void NextStep_Click(object sender, RoutedEventArgs e)
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
            VersionName.Text = InstallVerText.Text;
            VerQuiltListView.Items.Clear();
            VerOptiFineListView.Items.Clear();
            VerFabricListView.Items.Clear();
            VerForgeListView.Items.Clear();
            vers.Clear();
            ForgeProgressRing.Visibility = Visibility.Visible;
            OptiFineProgressRing.Visibility = Visibility.Visible;
            FabricProgressRing.Visibility = Visibility.Visible;
            QuiltProgressRing.Visibility = Visibility.Visible;

            InsVer = InstallVerText.Text;

            
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
                Panuon.WPF.UI.Toast.Show($"不兼容！", ToastPosition.Top);
                //MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
                GetForge();
                ForgeBr.Visibility = Visibility.Visible;
  
      
            }
            TestIns();
        }
        private void VerForgeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerForgeListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-Forge";
                if (IsOptiFine)
                {
                    VersionName.Text = $"{InstallVerText.Text}-Forge-OptiFine";
                }
                ForgeInstallEntity forgeInstallBuild
                    = VerForgeListView.SelectedItem as ForgeInstallEntity;
                CloseInses();
                IsForge = true;
                FabricInfo.Text = "与Forge不兼容";
                ForgeInfo.Text = forgeInstallBuild.ForgeVersion;
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
                Panuon.WPF.UI.Toast.Show($"不兼容！", ToastPosition.Top);
                //MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
                return;
            }
            else
            {
                OptiFineBr.Visibility = Visibility.Visible;
                GetOptiFine();
            }
            TestIns();
        }

        private void VerOptiFineListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerOptiFineListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-OptiFine";
                if (IsForge)
                {
                    VersionName.Text = $"{InstallVerText.Text}-Forge-OptiFine";
                }
                OptiFineInstallEntity obj = VerOptiFineListView.SelectedItem as OptiFineInstallEntity;
                CloseInses();
                FabricInfo.Text = "与OptiFine不兼容";
                QuiltInfo.Text = "与OptiFine不兼容";
                IsOptiFine = true;
                OptiFineInfo.Text = obj.Type + "_" + obj.Patch;
                TestIns();
            }
        }

        private void ToFabricBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsForge||IsQuilt||IsOptiFine)
            {
                Panuon.WPF.UI.Toast.Show($"不兼容！", ToastPosition.Top);
                //MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
                FabricBr.Visibility = Visibility.Visible;

                GetFabric();

            }
            TestIns();
        }

        private void VerFabricListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerFabricListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-Fabric";
                MinecraftLaunch.Modules.Models.Install.FabricInstallBuild obj
                    = VerFabricListView.SelectedItem as MinecraftLaunch.Modules.Models.Install.FabricInstallBuild;
                CloseInses();
                OptiFineInfo.Text = "与Fabric不兼容";
                QuiltInfo.Text = "与Fabric不兼容";
                ForgeInfo.Text = "与Fabric不兼容";
                IsFabric = true;
                FabricInfo.Text = obj.Loader.Version;
                TestIns();
            }
        }

        private void ToQuiltBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (IsFabric || IsForge||IsOptiFine)
            {
                Panuon.WPF.UI.Toast.Show($"不兼容！", ToastPosition.Top);
                //MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
                QuiltBr.Visibility = Visibility.Visible;
                GetQuilt();
                
            }
            TestIns();
        }

        private void VerQuiltListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerQuiltListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-Quilt";
                QuiltInstallBuild obj
                    = VerQuiltListView.SelectedItem as QuiltInstallBuild;
                CloseInses();
                OptiFineInfo.Text = "与Quilt不兼容";
                FabricInfo.Text = "与Quilt不兼容";
                ForgeInfo.Text = "与Quilt不兼容";
                IsQuilt = true;
                QuiltInfo.Text = obj.Loader.Version;
                TestIns();
            }
        }
        #endregion
        private async void StartInsBtn_Click(object sender, RoutedEventArgs e)
        {
            InsVerName = VersionName.Text;
            #region 版本名称判断
            if (VersionName.Text == string.Empty)
            {
                Panuon.WPF.UI.Toast.Show($"版本名称不可为空", ToastPosition.Top);
                return;
            }
            //制定出非法字符串
            string Unlegalstr = "/\\:*?\"<>|";
            //存储提示用户应该修改的文本
            string UserMsg = "";
            //计数结果
            int res = -1;
            //根据用户输入长度循环
            for (int i = 0; i < VersionName.Text.Length; i++)
            {
                res = Unlegalstr.IndexOf(VersionName.Text[i]);//若是没有则会返回-1
                if (res != -1)//如果不等-1，证明有结果
                {
                    UserMsg += VersionName.Text[i].ToString();//存储结果并返回
                }

            }
            if (res > 0)
            {
                Panuon.WPF.UI.Toast.Show($"存在非法字符 {UserMsg} ", ToastPosition.Top); //返回非法字符串
                return;
            }
            if (Directory.Exists("./.minecraft/versions/"+VersionName.Text))
            {
                Panuon.WPF.UI.Toast.Show($"不可与现有文件夹重名", ToastPosition.Top);
                return;
            }
            #endregion
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            #region 下载源
            if (obj.DownloadSoure == "Mcbbs")
            {
                APIManager.Current = APIManager.Mcbbs;
                DownloadApiManager.Current = DownloadApiManager.Mcbbs;
            }
            else if (obj.DownloadSoure == "BMCLAPI")
            {
                APIManager.Current = APIManager.Bmcl;
                DownloadApiManager.Current = DownloadApiManager.Bmcl;
            }
            else if (obj.DownloadSoure == "Mojang")
            {
                APIManager.Current = APIManager.Mojang;
                DownloadApiManager.Current = DownloadApiManager.Mojang;
            }
            #endregion
            ResourceInstaller.MaxDownloadThreads = Convert.ToInt32(obj.MaxDownloadThreads);
            ResourceDownloader.MaxDownloadThreads = Convert.ToInt32(obj.MaxDownloadThreads); //下载最大线程数
            Panuon.WPF.UI.Toast.Show($"开始安装{InsVerName }  下载源：{obj.DownloadSoure}  最大线程：{obj.MaxDownloadThreads}", ToastPosition.Top);
            #region UI
            DownloadInfo.Visibility = Visibility.Visible;
            
            DownloadText.Text = "";
            int a = 0;
            Step1Br.Visibility = Visibility.Visible;
            Step2Br.Visibility = Visibility.Hidden;
            #endregion
            #region Vanllia
            installbz.Text = "Vanllia";
            DownloadText.Text += $"[{DateTime.Now.ToString()}]    开始安装{InsVerName}  下载源：{obj.DownloadSoure}  最大线程：{obj.MaxDownloadThreads}\n";

            await Task.Run(async () => {
                GameCoreInstaller installer = new(obj.MinecraftPath, InsVer, InsVerName);
                installer.ProgressChanged += (_, x) => {
                    Dispatcher.BeginInvoke(() => {
                        DownloadProText.Text = $"{x.Progress * 100:0.00}%";
                        DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
                        a++;
                        if (a == 19)
                        {
                            a = 0;
                            DownloadText.Text = "";
                        }
                        DownloadText.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";

                    });
                };
                var result = await installer.InstallAsync();
            });
            Panuon.WPF.UI.Toast.Show($"Vanllia {InsVer} 安装完成", ToastPosition.Top);



            #endregion

            DownloadText.Text = "";
            if (IsForge)
            {
                DownloadText.Text = "";
                a = 0;
                installbz.Text = "Forge";
                var forgeversion = ForgeInfo.Text;
                var forgebuild = VerForgeListView.SelectedItem as ForgeInstallEntity;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：Forge-{forgeversion}\n";
                await Task.Run(async () => {
                    ForgeInstaller installer = new(obj.MinecraftPath, forgebuild, obj.Java, customId:InsVerName);

                    installer.ProgressChanged += (_, x) => {
                        Dispatcher.BeginInvoke(() => {
                            DownloadProText.Text = $"{x.Progress * 100:0.00}%";
                            DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
                            a++;
                            if (a == 19)
                            {
                                a = 0;
                                DownloadText.Text = "";
                            }
                            DownloadText.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";

                        });
                    };

                    var result = await installer.InstallAsync();
                });
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：Forge-{forgeversion}\n";
                Panuon.WPF.UI.Toast.Show($"Forge {forgeversion} 安装完成", ToastPosition.Top);
            }
            
            if (IsOptiFine)
            {
                DownloadText.Text = "";
                a = 0;
                installbz.Text = "OptiFine";
                var optifineversion = OptiFineInfo.Text;
                var optifinebuild = VerOptiFineListView.SelectedItem as OptiFineInstallEntity;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：OptiFine-{optifineversion}\n";
                    await Task.Run(async () => {
                        OptiFineInstaller installer = new(obj.MinecraftPath, optifinebuild, obj.Java , customId:InsVerName );

                        installer.ProgressChanged += (_, x) => {
                            Dispatcher.BeginInvoke(() => {
                                DownloadProText.Text = $"{x.Progress * 100:0.00}%";
                                DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
                                a++;
                                if (a == 19)
                                {
                                    a = 0;
                                    DownloadText.Text = "";
                                }
                                DownloadText.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";

                            });
                        };

                        var result = await installer.InstallAsync();
                    });
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：OptiFine-{optifineversion}\n";
                Panuon.WPF.UI.Toast.Show($"OptiFine {optifineversion} 安装完成", ToastPosition.Top);
            }

            if (IsFabric)
            {
                DownloadText.Text = "";
                installbz.Text = "Fabric";
                var fabricversion = FabricInfo.Text;
                var fabricbuild = VerFabricListView.SelectedItem;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：Fabric-{fabricversion}\n";
                await Task.Run(() => {
                    var fabricinstaller = new MinecraftFabricInstaller
                    (
                        new GameCoreLocator(obj.MinecraftPath),
                        (Natsurainko.FluentCore.Model.Install.Fabric.FabricInstallBuild)fabricbuild,
                        customId: InsVerName
                    );
                    var fabricres = fabricinstaller.Install();
                });
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：Fabric-{fabricversion}\n";
                Panuon.WPF.UI.Toast.Show($"Fabric {fabricversion} 安装完成", ToastPosition.Top);
            }

            if (IsQuilt)
            {
                DownloadText.Text = "";
                installbz.Text = "Quilt";
                var quiltversion = QuiltInfo.Text;
                var quiltbuild = VerQuiltListView.SelectedItem;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：Quilt-{quiltversion}\n";
                await Task.Run(() => {
                    var quiltinstaller = new MinecraftQuiltInstaller
                    (
                        new GameCoreLocator(obj.MinecraftPath),
                        (Natsurainko.FluentCore.Model.Install.Quilt.QuiltInstallBuild)quiltbuild,
                        customId: InsVerName
                    );
                    var quiltres = quiltinstaller.Install();
                });
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：Quilt-{quiltversion}\n";
                Panuon.WPF.UI.Toast.Show($"Quilt {quiltversion} 安装完成", ToastPosition.Top);
            }

            DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    游戏核心 {InsVerName} 安装完成";
            MessageBoxX.Show($"游戏核心 {InsVerName} 安装完成");
            DownloadInfo.Visibility = Visibility.Hidden;
        }


    }
}
