using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YMCL.Pages.Windows;

namespace YMCL.Pages.DownloadPages
{
    /// <summary>
    /// AutoInstallPage.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstallPage : Page
    {
        string version = string.Empty;
        public AutoInstallPage()
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            var thread = setting.DownloadThread;
            InitializeComponent();
            switch (setting.DownloadSource)
            {
                case "Mcbbs":
                    APIManager.Current = APIManager.Mcbbs;
                    break;
                case "BMCLApi":
                    APIManager.Current = APIManager.Bmcl;
                    break;
                case "Mojang":
                    APIManager.Current = APIManager.Mojang;
                    break;
            }
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(AllVerList.Items);
            view.Filter = UserFilter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVersions();
        }

        #region Version
        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(SearBox.Text))
            {
                return true;
            }
            else
            {
                return (item as Ver).Id.IndexOf(SearBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
        public class Ver
        {
            public string? Id { get; set; }
            public string? ReleaseTime { get; set; }
        }
        private void SearBox_TextChanged(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            CollectionViewSource.GetDefaultView(AllVerList.ItemsSource).Refresh();
        }
        async void LoadVersions()
        {
            try
            {
                List<Ver> verList = new List<Ver>();
                ReleaseVerList.Items.Clear();
                SnapshotVerList.Items.Clear();
                OldVerList.Items.Clear();
                FoolVerList.Items.Clear();
                await Task.Run(() =>
                {
                    var res = GameCoreInstaller.GetGameCoresAsync().Result.Cores.ToList();
                    res.ForEach(x =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            verList.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() });
                            if (x.Type == "release")
                            {
                                ReleaseVerList.Items.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() });
                            }
                            if (x.Type == "snapshot")
                            {
                                SnapshotVerList.Items.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() });
                            }
                            if (x.Type == "old_alpha" || x.Type == "old_beta")
                            {
                                OldVerList.Items.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() });
                            }
                            if (x.Id == "22w13oneblockatatime" || x.Id == "23w13a_or_b" || x.Id == "20w14∞" ||
                         x.Id == "3D Shareware v1.34" || x.Id == "1.RV-Pre1" || x.Id == "15w14a")
                            {
                                FoolVerList.Items.Add(new Ver() { Id = x.Id, ReleaseTime = x.ReleaseTime.ToString() });
                            }
                        });
                    });
                });
                AllVerList.ItemsSource = verList;
                var release = ReleaseVerList.Items[0] as Ver;
                var snapshot = SnapshotVerList.Items[0] as Ver;
                ReleaseLatestIdTextBlock.Text = release.Id;
                ReleaseLatestTimeTextBlock.Text = release.ReleaseTime;
                SnapshotLatestIdTextBlock.Text = snapshot.Id;
                SnapshotLatestTimeTextBlock.Text = snapshot.ReleaseTime;
            }
            catch
            {
                Toast.Show(Const.Window.main, $"可安装版本列表失败，这可能是网络原因", ToastPosition.Top);
            }
        }
        #endregion
        #region

        #endregion



        string id;
        private void VerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var senderObj = sender as ModernWpf.Controls.ListView;
            var version = senderObj.SelectedItem as Ver;
            if (version == null)
            {
                return;
            }
            ForgeList.SelectedIndex = -1;
            OptifineList.SelectedIndex = -1;
            QuiltList.SelectedIndex = -1;
            FabricList.SelectedIndex = -1;
            ForgeList.IsEnabled = true;
            OptifineList.IsEnabled = true;
            FabricList.IsEnabled = true;
            QuiltList.IsEnabled = true;
            AdditionInstallBlock.Text = "无附加安装";
            id = version.Id;
            CustomIdInput.Text = id;
            ForgeList.Items.Clear();
            OptifineList.Items.Clear();
            FabricList.Items.Clear();
            QuiltList.Items.Clear();
            InstallVerBlock.Text = id;
            VersionView.Visibility = Visibility.Hidden;
            InstallPreview.Visibility = Visibility.Visible;
            ForgeLoading.Visibility = Visibility.Visible;
            ForgeExpander.Header = "Forge";
            OptifineLoading.Visibility = Visibility.Visible;
            OptifineExpander.Header = "Optifine";
            FabricLoading.Visibility = Visibility.Visible;
            FabricExpander.Header = "Fabric";
            QuiltLoading.Visibility = Visibility.Visible;
            QuiltExpander.Header = "Quilt";
            GetAddition();
        }

        async void GetAddition()
        {
            List<ForgeInstallEntity> forges = new List<ForgeInstallEntity>();
            List<OptiFineInstallEntity> optifines = new List<OptiFineInstallEntity>();
            List<FabricInstallBuild> fabrics = new List<FabricInstallBuild>();
            List<QuiltInstallBuild> quilts = new List<QuiltInstallBuild>();

            await Task.Run(async () =>
            {
                forges = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(id)).ToList();
                await Dispatcher.BeginInvoke(() =>
                {
                    forges.ForEach(x =>
                    {
                        ForgeList.Items.Add(x);
                    });
                    ForgeLoading.Visibility = Visibility.Hidden;
                });
                optifines = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(id)).ToList();
                await Dispatcher.BeginInvoke(() =>
                {
                    optifines.ForEach(x =>
                    {
                        OptifineList.Items.Add(x);
                    });
                    OptifineLoading.Visibility = Visibility.Hidden;
                });
                fabrics = (await FabricInstaller.GetFabricBuildsByVersionAsync(id)).ToList();
                await Dispatcher.BeginInvoke(() =>
                {
                    fabrics.ForEach(x =>
                    {
                        FabricList.Items.Add(x);
                    });
                    FabricLoading.Visibility = Visibility.Hidden;
                });
                quilts = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(id)).ToList();
                await Dispatcher.BeginInvoke(() =>
                {
                    quilts.ForEach(x =>
                    {
                        QuiltList.Items.Add(x);
                    });
                    QuiltLoading.Visibility = Visibility.Hidden;
                });
            });







            if (quilts.Count == 0)
            {
                QuiltExpander.Header = "Quilt (无可用版本)";
            }
            if (fabrics.Count == 0)
            {
                FabricExpander.Header = "Fabric (无可用版本)";
            }
            if (optifines.Count == 0)
            {
                OptifineExpander.Header = "Optifine (无可用版本)";
            }
            if (forges.Count == 0)
            {
                ForgeExpander.Header = "Forge (无可用版本)";
            }
        }

        private void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            VersionView.Visibility = Visibility.Visible;
            InstallPreview.Visibility = Visibility.Hidden;
        }

        private void AdditionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdditionInstallBlock.Text = string.Empty;
            var senderObj = sender as ModernWpf.Controls.ListView;
            ForgeList.IsEnabled = true;
            OptifineList.IsEnabled = true;
            FabricList.IsEnabled = true;
            QuiltList.IsEnabled = true;
            var forgeItem = ForgeList.SelectedItem as ForgeInstallEntity;
            var optifineItem = OptifineList.SelectedItem as OptiFineInstallEntity;
            var FabricItem = FabricList.SelectedItem as FabricInstallBuild;
            var QuiltItem = QuiltList.SelectedItem as QuiltInstallBuild;
            switch (senderObj.Name)
            {
                case "ForgeList":

                    FabricList.IsEnabled = false;
                    QuiltList.IsEnabled = false;
                    FabricList.SelectedIndex = -1;
                    QuiltList.SelectedIndex = -1;

                    break;
                case "OptifineList":

                    FabricList.IsEnabled = false;
                    QuiltList.IsEnabled = false;
                    FabricList.SelectedIndex = -1;
                    QuiltList.SelectedIndex = -1;

                    break;
                case "FabricList":

                    ForgeList.IsEnabled = false;
                    OptifineList.IsEnabled = false;
                    QuiltList.IsEnabled = false;
                    ForgeList.SelectedIndex = -1;
                    OptifineList.SelectedIndex = -1;
                    QuiltList.SelectedIndex = -1;

                    break;
                case "QuiltList":

                    ForgeList.IsEnabled = false;
                    OptifineList.IsEnabled = false;
                    FabricList.IsEnabled = false;
                    ForgeList.SelectedIndex = -1;
                    OptifineList.SelectedIndex = -1;
                    FabricList.SelectedIndex = -1;

                    break;
            }
            if (QuiltList.SelectedIndex != -1)
                AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Quilt {QuiltItem.Loader.Version}  ";
            if (ForgeList.SelectedIndex != -1)
                AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Forge {forgeItem.ForgeVersion}  ";
            if (FabricList.SelectedIndex != -1)
                AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Fabric {FabricItem.Loader.Version}  ";
            if (OptifineList.SelectedIndex != -1)
                AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Optifine {optifineItem.FileName}  ";
        }

        private void CancelAddition_Click(object sender, RoutedEventArgs e)
        {
            ForgeList.SelectedIndex = -1;
            OptifineList.SelectedIndex = -1;
            QuiltList.SelectedIndex = -1;
            FabricList.SelectedIndex = -1;
            ForgeList.IsEnabled = true;
            OptifineList.IsEnabled = true;
            FabricList.IsEnabled = true;
            QuiltList.IsEnabled = true;
            AdditionInstallBlock.Text = "无附加安装";
        }

        private async void StartInstallBtn_Click(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            #region 版本名称判断
            if (string.IsNullOrWhiteSpace(CustomIdInput.Text))
            {
                Toast.Show(Const.Window.main, $"版本名称不可为空", ToastPosition.Top);
                return;
            }
            //制定出非法字符串
            string Unlegalstr = "/\\:*?\"<>|";
            //存储提示用户应该修改的文本
            string UserMsg = "";
            //计数结果
            int res = -1;
            //根据用户输入长度循环
            for (int i = 0; i < CustomIdInput.Text.Length; i++)
            {
                res = Unlegalstr.IndexOf(CustomIdInput.Text[i]);//若是没有则会返回-1
                if (res != -1)//如果不等-1，证明有结果
                {
                    UserMsg += CustomIdInput.Text[i].ToString();//存储结果并返回
                }

            }
            if (res > 0)
            {
                Toast.Show(Const.Window.main, $"存在非法字符 {UserMsg} ", ToastPosition.Top); //返回非法字符串
                return;
            }
            if (Directory.Exists(setting.MinecraftPath + "\\versions\\" + CustomIdInput.Text))
            {
                Toast.Show(Const.Window.main, $"不可与现有文件夹重名", ToastPosition.Top);
                return;
            }
            #endregion
            var customId = CustomIdInput.Text;
            DisplayProgressWindow displayProgressWindow = new DisplayProgressWindow();
            displayProgressWindow.Show();
            displayProgressWindow.CurrentStepTextBlock.Text = "Vanllia";
            displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     开始安装 Vanllia {id}   下载源 {setting.DownloadSource}   最大下载线程 {setting.DownloadThread}\n";
            bool forge = false;
            bool optifine = false;
            bool fabric = false;
            bool quilt = false;
            if (QuiltList.SelectedIndex > -1)
                quilt = true;
            else
                quilt = false;
            if (FabricList.SelectedIndex > -1)
                fabric = true;
            else
                fabric = false;
            if (OptifineList.SelectedIndex > -1)
                optifine = true;
            else
                optifine = false;
            if (ForgeList.SelectedIndex > -1)
                forge = true;
            else 
                forge = false; 

            await Task.Run(async () =>
            {
                GameCoreInstaller installer = new(setting.MinecraftPath, id, customId);
                installer.ProgressChanged += (_, x) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProText.Text = $"{x.Progress * 100:0.00}%";
                        displayProgressWindow.TaskProBar.Value = Math.Round(x.Progress * 100, 1);
                        displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";
                    });
                };
                var result = await installer.InstallAsync();

                await Dispatcher.BeginInvoke(() =>
                {
                    Toast.Show(Const.Window.main, $"Vanllia {id} 安装完成", ToastPosition.Top);
                    displayProgressWindow.TaskProgressTextBox.Text = "";
                });


                if (forge)
                {
                    ForgeInstallEntity forgebuild = new();

                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     开始安装 Forge \n";
                        displayProgressWindow.CurrentStepTextBlock.Text = "Forge";
                        forgebuild = ForgeList.SelectedItem as ForgeInstallEntity;
                    });

                    ForgeInstaller forgeinstaller = new(setting.MinecraftPath, forgebuild, setting.Java, customId);
                    installer.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProText.Text = $"{x.Progress * 100:0.00}%";
                            displayProgressWindow.TaskProBar.Value = Math.Round(x.Progress * 100, 1);
                            displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";
                        });
                    };
                    var forgeresult = await forgeinstaller.InstallAsync();

                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, $"Forge 安装完成", ToastPosition.Top);
                    });

                }


                if (optifine)
                {
                    OptiFineInstallEntity opbuild = new();
                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     开始安装 Optifine \n";
                        displayProgressWindow.CurrentStepTextBlock.Text = "Optifine";
                        opbuild = OptifineList.SelectedItem as OptiFineInstallEntity;
                    });



                    OptiFineInstaller opinstaller = new(setting.MinecraftPath, opbuild, setting.Java, customId: customId);
                    installer.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProText.Text = $"{x.Progress * 100:0.00}%";
                            displayProgressWindow.TaskProBar.Value = Math.Round(x.Progress * 100, 1);
                            displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";
                        });
                    };
                    await opinstaller.InstallAsync();

                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, $"Optifine 安装完成", ToastPosition.Top);
                    });

                }


                if (fabric)
                {
                    FabricInstallBuild fabuild = new();
                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     开始安装 Fabric \n";
                        displayProgressWindow.CurrentStepTextBlock.Text = "Fabric";
                        var fabuild = FabricList.SelectedItem as FabricInstallBuild;
                    });


                    FabricInstaller fainstaller = new(setting.MinecraftPath, fabuild, customId: customId);
                    installer.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProText.Text = $"{x.Progress * 100:0.00}%";
                            displayProgressWindow.TaskProBar.Value = Math.Round(x.Progress * 100, 1);
                            displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";
                        });
                    };
                    await fainstaller.InstallAsync();

                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, $"Fabric 安装完成", ToastPosition.Top);
                    });

                }

                if (quilt)
                {
                    QuiltInstallBuild build = new();
                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     开始安装 Quilt \n";
                        displayProgressWindow.CurrentStepTextBlock.Text = "Quilt";
                        build = QuiltList.SelectedItem as QuiltInstallBuild;
                    });

                    QuiltInstaller qinstaller = new(setting.MinecraftPath, build, customId: customId);
                    installer.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProText.Text = $"{x.Progress * 100:0.00}%";
                            displayProgressWindow.TaskProBar.Value = Math.Round(x.Progress * 100, 1);
                            displayProgressWindow.TaskProgressTextBox.Text += $"[{DateTime.Now.ToString()}]     {x.ProgressDescription}\n";
                        });
                    };
                    await qinstaller.InstallAsync();

                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, $"Quilt 安装完成", ToastPosition.Top);
                    });
                }


            });

            displayProgressWindow.Hide();
            VersionView.Visibility = Visibility.Visible;
            InstallPreview.Visibility = Visibility.Hidden;
        }
    }
}
