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
using MinecraftLaunch;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Toolkits;
using Panuon.WPF.UI;

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
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
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
        private async void ReGetVer()
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
                Dispatcher.BeginInvoke(() => { VerListView.ItemsSource = vers; }) ;
                Dispatcher.BeginInvoke(() => { RefreshVerListBtn.IsEnabled = true;  });
            });
            
        }
        private async void GetForgeVer()
        {

            await Task.Run(async () =>
            {
                var res = ForgeInstaller.GetForgeBuildsOfVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")).Result.ToList();
                Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Clear(); });
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Add(x); });
                });
            });

        }
        private async void GetFabricVer()
        {

            await Task.Run(async () =>
            {
                var res = (await FabricInstaller.GetFabricBuildsByVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log"))).ToList();
                Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Clear(); });
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerFabricListView.Items.Add(x); });
                });
            });

        }
        private async void GetOptiFineVer()
        {

            await Task.Run(async () =>
            {
                var res = OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")).Result.ToList();
                Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Clear(); });
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Add(x); });
                });
            });

        }
        private async void GetQuiltVer()
        {

            await Task.Run(async () =>
            {
                var res = QuiltInstaller.GetQuiltBuildsByVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")).Result.ToList();
                Dispatcher.BeginInvoke(() => { VerQuiltListView.Items.Clear(); });
                res.ForEach(x =>
                {
                    Dispatcher.BeginInvoke(() => { VerQuiltListView.Items.Add(x); });
                });
            });

        }


        #region
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
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            System.IO.File.WriteAllText("./YMCL/logs/InsVer.log", VerListView.SelectedValue.ToString());

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
        private void TestFolder(string Folder)
        {
            if (System.IO.Directory.Exists(Folder)) { }
            else
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Folder);
                directoryInfo.Create();
            }
        }
        #endregion
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
                CloseInses();
                TestFolder("./YMCL");
                TestFolder("./YMCL/logs");
                System.IO.File.WriteAllText("./YMCL/logs/InsForge.log", VerForgeListView.SelectedValue.ToString());
                IsForge = true;
                FabricInfo.Text = "与Forge不兼容";
                ForgeInfo.Text = VerForgeListView.SelectedValue.ToString();
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
            if (IsFabric)
            {
                MessageBoxX.Show("已选择安装Fabric,需要安装OptiFabric,否则会导致OptiFine无法使用!");
                OptiFineBr.Visibility = Visibility.Visible;
                GetOptiFineVer();
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
                CloseInses();
                TestFolder("./YMCL");
                TestFolder("./YMCL/logs");
                System.IO.File.WriteAllText("./YMCL/logs/InsOptiFine.log", VerOptiFineListView.SelectedValue.ToString());
                IsOptiFine = true;
                OptiFineInfo.Text = VerOptiFineListView.SelectedValue.ToString();
                TestIns();
            }
        }

        private void ToFabricBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsForge||IsQuilt)
            {
                MessageBoxX.Show("Fabric与Forge或Quilt不兼容!"); 
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
                CloseInses();
                TestFolder("./YMCL");
                TestFolder("./YMCL/logs");
                System.IO.File.WriteAllText("./YMCL/logs/InsFabric.log", VerFabricListView.SelectedValue.ToString());
                IsFabric = true;
                FabricInfo.Text = VerFabricListView.SelectedValue.ToString();
                TestIns();
            }
        }

        private void ToQuiltBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsFabric || IsForge)
            {
                MessageBoxX.Show("Quilt与Forge或Fabric不兼容!"); 
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
            if (VerFabricListView.SelectedIndex >= 0)
            {
                CloseInses();
                TestFolder("./YMCL");
                TestFolder("./YMCL/logs");
                System.IO.File.WriteAllText("./YMCL/logs/InsQuilt.log", VerQuiltListView.SelectedValue.ToString());
                IsQuilt = true;
                QuiltInfo.Text = VerQuiltListView.SelectedValue.ToString();
                TestIns();
            }
        }

        private async void StartInsBtn_Click(object sender, RoutedEventArgs e)
        {
            MinecraftLaunch.Modules.Models.Install.ForgeInstallEntity forgeInstallEntity = null;
            OptiFineInstallEntity optiFineInstallEntity = null;
            installbz.Text = "Vanllia";
            int a = 0;
            qqqqq.Text = "";
            DownloadInfo.Visibility = Visibility.Visible;
            downjdgr.Visibility = Visibility.Visible;
            Step1Br.Visibility = Visibility.Visible;
            Step2Br.Visibility = Visibility.Hidden;
            await Task.Run(async () => {
                GameCoreInstaller installer = new(@".minecraft", System.IO.File.ReadAllText("./YMCL/logs/InsVer.log"));
                
                installer.ProgressChanged += (_, x) => {
                    Dispatcher.BeginInvoke(() => { DownloadProText.Text = Math.Round(x.Progress * 100, 1).ToString()+"%";
                        downjdtext.Text = Math.Round(x.Progress * 100, 0).ToString() + "%";
                        a++;
                        if (a == 21) {
                            a = 0;
                            qqqqq.Text = "";
                        }
                        DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
                        qqqqq.Text = qqqqq.Text + "["+DateTime.Now.ToString()+"]   "+x.ProgressDescription +"\n";
                    });
                    
                };

                var result = await installer.InstallAsync();

            });
            //if (IsForge) {
            //    installbz.Text = "Forge";
            //    await Task.Run(async () =>
            //    {

            //        var res = ForgeInstaller.GetForgeBuildsOfVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")).Result.ToList();
            //        Dispatcher.BeginInvoke(() => { VerForgeListView.Items.Clear(); });
            //        res.ForEach(x =>
            //        {
            //            Dispatcher.BeginInvoke(() => { if (x.ForgeVersion == System.IO.File.ReadAllText("./YMCL/logs/InsForge.log")) { forgeInstallEntity = x; } });

            //        });


            //        ForgeInstaller installer = new(".minecraft", forgeInstallEntity, System.IO.File.ReadAllText("./YMCL/logs/setting/java.log"));
            //        a = 0;
            //        installer.ProgressChanged += (_, x) =>
            //        {
            //            Dispatcher.BeginInvoke(() =>
            //            {
            //                DownloadProText.Text = Math.Round(x.Progress * 100, 1).ToString() + "%";
            //                downjdtext.Text = Math.Round(x.Progress * 100, 0).ToString() + "%";
            //                a++;
            //                if (a == 23)
            //                {
            //                    a = 0;
            //                    qqqqq.Text = "";
            //                }
            //                DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
            //                qqqqq.Text = qqqqq.Text + "[" + DateTime.Now.ToString() + "]   " + x.ProgressDescription + "\n";
            //            });

            //        };
            //    });
            //     }
            //if (IsOptiFine)
            //{
            //    installbz.Text = "OptiFine";
            //    await Task.Run(async () =>
            //    {

            //        var res = OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")).Result.ToList();
            //        Dispatcher.BeginInvoke(() => { VerOptiFineListView.Items.Clear(); });
            //        res.ForEach(x =>
            //        {
            //            Dispatcher.BeginInvoke(() => { if (x.FileName == System.IO.File.ReadAllText("./YMCL/logs/InsOptiFine.log")) { optiFineInstallEntity = x; } });
            //        });

            //        OptiFineInstaller installer = new(".minecraft", optiFineInstallEntity, System.IO.File.ReadAllText("./YMCL/logs/setting/java.log"));

            //        a = 0;
            //        installer.ProgressChanged += (_, x) =>
            //        {
            //            Dispatcher.BeginInvoke(() =>
            //            {
            //                DownloadProText.Text = Math.Round(x.Progress * 100, 1).ToString() + "%";
            //                downjdtext.Text = Math.Round(x.Progress * 100, 0).ToString() + "%";
            //                a++;
            //                if (a == 23)
            //                {
            //                    a = 0;
            //                    qqqqq.Text = "";
            //                }
            //                DownloadProBar.Value = Math.Round(x.Progress * 100, 1);
            //                qqqqq.Text = qqqqq.Text + "[" + DateTime.Now.ToString() + "]   " + x.ProgressDescription + "\n";
            //            });

            //        };
            //    });
            //}


            //NoticeBox.Show($"游戏核心 {System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")} 安装完成", "提示", MessageBoxIcon.Success);
            MessageBoxX.Show($"游戏核心 {System.IO.File.ReadAllText("./YMCL/logs/InsVer.log")} 安装完成");
            
            DownloadInfo.Visibility = Visibility.Hidden;
            downjdgr.Visibility = Visibility.Hidden;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DownloadInfo.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DownloadInfo.Visibility = Visibility.Visible;
        }
    }
}
