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
                await Task.Run(async () =>
                {

                    Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = false; });
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
            
            await Task.Run(async () =>
            {
                var builds = MinecraftForgeInstaller.GetForgeBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                builds.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Add(x); });
                });
            });
            ForgeProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetFabricVer()
        {
            
            await Task.Run(async () =>
            {
                var res = MinecraftFabricInstaller.GetFabricBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Add(x); });
                });
            });
            FabricProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetOptiFineVer()
        {
            
            await Task.Run(async () =>
            {
                var res = MinecraftOptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();

                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Add(x); });
                });
            });
            OptiFineProgressRing.Visibility = Visibility.Hidden;
        }
        private async void GetQuiltVer()
        {
            
            await Task.Run(async () =>
            {
                var res = MinecraftQuiltInstaller.GetQuiltBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
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

            await Task.Run(() =>
            {
                var forge = MinecraftForgeInstaller.GetForgeBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                forge.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Add(x); });
                });
                Dispatcher.BeginInvoke(() => { ForgeProgressRing.Visibility = Visibility.Hidden; });

                var fabric = MinecraftFabricInstaller.GetFabricBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                fabric.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Add(x); });
                });
                Dispatcher.BeginInvoke(() => { FabricProgressRing.Visibility = Visibility.Hidden; });

                var optifine = MinecraftOptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                optifine.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Add(x); });
                });
                Dispatcher.BeginInvoke(() => { OptiFineProgressRing.Visibility = Visibility.Hidden; });

                var quilt = MinecraftQuiltInstaller.GetQuiltBuildsFromMcVersionAsync(InsVer).GetAwaiter().GetResult().ToList();
                quilt.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerQuiltListView.Items.Add(x); });
                });
                Dispatcher.BeginInvoke(() => { QuiltProgressRing.Visibility = Visibility.Hidden; });
            });
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
                MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
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
                MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
                return;
            }
            else
            {
                OptiFineBr.Visibility = Visibility.Visible;

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
                MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
                FabricBr.Visibility = Visibility.Visible;

               

            }
            TestIns();
        }

        private void VerFabricListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerFabricListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-Fabric";
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
                MessageBoxX.Show("不兼容!", "Yu Minecraft Launcher");
            }
            else
            {
                QuiltBr.Visibility = Visibility.Visible;

                
            }
            TestIns();
        }

        private void VerQuiltListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerQuiltListView.SelectedIndex >= 0)
            {
                VersionName.Text = $"{InstallVerText.Text}-Quilt";
                Natsurainko.FluentCore.Model.Install.Quilt.QuiltInstallBuild obj
                    = VerQuiltListView.SelectedItem as Natsurainko.FluentCore.Model.Install.Quilt.QuiltInstallBuild;
                CloseInses();
                OptiFineInfo.Text = "与Quilt不兼容";
                FabricInfo.Text = "与Quilt不兼容";
                ForgeInfo.Text = "与Quilt不兼容";
                IsQuilt = true;
                QuiltInfo.Text = obj.BuildVersion;
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
            DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装{InsVerName}  下载源：{obj.DownloadSoure}  最大线程：{obj.MaxDownloadThreads}\n";

            await Task.Run(async () => {
            var installer = new MinecraftVanlliaInstaller(new GameCoreLocator(obj.MinecraftPath), InsVer, InsVerName);
            installer.ProgressChanged += (sender, e) =>
            {
                #region UI
                Dispatcher.BeginInvoke(() => {
                    DownloadProText.Text = $"{e.TotleProgress * 100:0.00}%";
                    DownloadProBar.Value = Math.Round(e.TotleProgress * 100, 1);
                    a++;
                    if (a == 19)
                    {
                        a = 0;
                        DownloadText.Text = "";

                    }
                    DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    下载资源：{e.TotleProgress * 100:0.00}%\n";

                });
                #endregion
            };
            var res = await installer.InstallAsync();
            });
            Panuon.WPF.UI.Toast.Show($"Vanllia {InsVer} 安装完成", ToastPosition.Top);

            #endregion

            DownloadText.Text = "";
            if (IsForge)
            {
                installbz.Text = "Forge";
                var forgeversion = ForgeInfo.Text;
                var forgebuild = VerForgeListView.SelectedItem;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：Forge-{forgeversion}\n";
                await Task.Run(() => {
                    var forgeinstaller = new MinecraftForgeInstaller
                    (
                        new GameCoreLocator(obj.MinecraftPath),
                        (ForgeInstallBuild)forgebuild,
                        File.ReadAllText("./YMCL/Temp/Java.log"),
                        customId: InsVerName
                    );
                    var forgeres = forgeinstaller.Install();
                });
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：Forge-{forgeversion}\n";
                Panuon.WPF.UI.Toast.Show($"Forge {forgeversion} 安装完成", ToastPosition.Top);
            }
            
            if (IsOptiFine)
            {

                installbz.Text = "OptiFine";
                var optifineversion = OptiFineInfo.Text;
                var optifinebuild = VerOptiFineListView.SelectedItem;
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    开始安装：OptiFine-{optifineversion}\n";
                //if (IsForge)
                //{
                    
                //}
                //else
                //{
                    await Task.Run(() => {
                        var optifineinstaller = new MinecraftOptiFineInstaller
                        (
                            new GameCoreLocator(obj.MinecraftPath),
                            (OptiFineInstallBuild)optifinebuild,
                            File.ReadAllText("./YMCL/Temp/Java.log"),
                            customId: InsVerName
                        );
                        var optifineres = optifineinstaller.Install();
                    });
                //}
                DownloadText.Text = DownloadText.Text + $"[{DateTime.Now.ToString()}]    安装完成：OptiFine-{optifineversion}\n";
                Panuon.WPF.UI.Toast.Show($"OptiFine {optifineversion} 安装完成", ToastPosition.Top);
            }

            if (IsFabric)
            {
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
