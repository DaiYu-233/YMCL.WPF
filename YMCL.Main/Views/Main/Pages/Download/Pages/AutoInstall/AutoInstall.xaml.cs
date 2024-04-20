using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MinecraftLaunch;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;

namespace YMCL.Main.Views.Main.Pages.Download.Pages.AutoInstall
{
    /// <summary>
    /// AutoInstall.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstall : Page
    {
        bool _firstLoad = true;
        bool _loaded = false;
        #region UI
        private void LatestSnapshotBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_loaded)
                ReadyInstallGame(LatestSnapshotVersionId.Content.ToString());
        }
        private void LatestReleaseBorder_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_loaded)
                ReadyInstallGame(LatestReleaseVersionId.Content.ToString());
        }
        private void CustomGameIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HandleCustomId(VersionId.Text, CustomGameIdTextBox.Text);
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.DownloadSource == Public.Class.SettingItem.DownloadSource.BmclApi)
            {
                MirrorDownloadManager.IsUseMirrorDownloadSource = true;
            }
            else
            {
                MirrorDownloadManager.IsUseMirrorDownloadSource = false;
            }
            if (_firstLoad)
            {
                _firstLoad = false;
                try
                {
                    await Task.Run(async () =>
                    {
                        var vanlliaList = await VanlliaInstaller.EnumerableGameCoreAsync(MirrorDownloadManager.Bmcl);
                        await Dispatcher.BeginInvoke(() =>
                        {
                            var latestRelease = false;
                            _loaded = true;
                            var latestSnapshot = false;
                            foreach (var item in vanlliaList)
                            {
                                if (item.Type == "release")
                                {
                                    ReleaseVanlliaListView.Items.Add(item);
                                    if (!latestRelease)
                                    {
                                        latestRelease = true;
                                        LatestReleaseVersionId.Content = item.Id;
                                        LatestReleaseVersionTime.Content = item.ReleaseTime;
                                    }
                                }
                                else if (item.Type == "snapshot")
                                {
                                    SnapshotVanlliaListView.Items.Add(item);
                                    if (!latestSnapshot)
                                    {
                                        latestSnapshot = true;
                                        LatestSnapshotVersionId.Content = item.Id;
                                        LatestSnapshotVersionTime.Content = item.ReleaseTime;
                                    }
                                }
                                else
                                {
                                    OldVanlliaListView.Items.Add(item);
                                }
                            }
                            ReleaseVanlliaLoading.Visibility = Visibility.Hidden;
                            SnapshotVanlliaLoading.Visibility = Visibility.Hidden;
                            OldVanlliaLoading.Visibility = Visibility.Hidden;
                            VanlliaListGrid.Visibility = Visibility.Visible;
                        });
                    });
                }
                catch
                {
                    Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: LangHelper.Current.GetText("GetInstallableVersionFail"));
                    VanlliaLoading.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(196, 43, 28));
                    VanlliaLoadFail.Visibility = Visibility.Visible;
                    VanlliaLoading.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void VanlliaLoadFail_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VanlliaLoading.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 90, 158));
            VanlliaLoadFail.Visibility = Visibility.Collapsed;
            VanlliaLoading.Visibility = Visibility.Visible;
        }
        private void ReturnToVanlliaList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstallPreView.Visibility = Visibility.Hidden;
            VanlliaScrollViewer.Visibility = Visibility.Visible;
            ReleaseVanlliaListView.SelectedIndex = -1;
            SnapshotVanlliaListView.SelectedIndex = -1;
            OldVanlliaListView.SelectedIndex = -1;
        }
        private void VanlliaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((System.Windows.Controls.ListView)sender).SelectedIndex < 0)
            {
                return;
            }
            var manifest = ((System.Windows.Controls.ListView)sender).SelectedItem as VersionManifestEntry;
            ReadyInstallGame(manifest.Id);
        }
        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OptifineListView.SelectedIndex >= 0)
            {

            }
            else if (ForgeListView.SelectedIndex >= 0)
            {
                var entry = ForgeListView.SelectedItem as ForgeInstallEntry;
                InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text, forgeInstallEntry: entry);
            }
            else if (FabricListView.SelectedIndex >= 0)
            {
                var entry = FabricListView.SelectedItem as FabricBuildEntry;
                InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text, fabricBuildEntry: entry);
            }
            else if (QuiltListView.SelectedIndex >= 0)
            {
                var entry = QuiltListView.SelectedItem as QuiltBuildEntry;
                InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text, quiltBuildEntry: entry);
            }
            else
            {
                InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text);
            }
        }
        private void ModLaderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (System.Windows.Controls.ListView)sender;
            var index = control.SelectedIndex;
            switch (control.Tag)
            {
                case "Optifine":
                    if (index >= 0)
                    {
                        OptifineLabel.Content = (control.SelectedItem as Object).ToString();
                    }
                    else
                    {
                        OptifineLabel.Content = "Null";
                    }
                    break;
                case "Forge":
                    if (index >= 0)
                    {
                        ForgeLabel.Content = (control.SelectedItem as ForgeInstallEntry).ForgeVersion;
                    }
                    else
                    {
                        ForgeLabel.Content = "Null";
                    }
                    break;
                case "Fabric":
                    if (index >= 0)
                    {
                        FabricLabel.Content = (control.SelectedItem as FabricBuildEntry).BuildVersion;
                    }
                    else
                    {
                        FabricLabel.Content = "Null";
                    }
                    break;
                case "Quilt":
                    if (index >= 0)
                    {
                        QuiltLabel.Content = (control.SelectedItem as QuiltBuildEntry).BuildVersion;
                    }
                    else
                    {
                        QuiltLabel.Content = "Null";
                    }
                    break;
                default:
                    return;
            }
            if (control.SelectedIndex < 0)
            {
                return;
            }
            var str = string.Empty;
            switch (control.Tag)
            {
                case "Optifine":
                    FabricListView.SelectedIndex = -1;
                    QuiltListView.SelectedIndex = -1;
                    break;
                case "Forge":
                    FabricListView.SelectedIndex = -1;
                    QuiltListView.SelectedIndex = -1;
                    break;
                case "Fabric":
                    OptifineListView.SelectedIndex = -1;
                    ForgeListView.SelectedIndex = -1;
                    QuiltListView.SelectedIndex = -1;
                    break;
                case "Quilt":
                    OptifineListView.SelectedIndex = -1;
                    ForgeListView.SelectedIndex = -1;
                    FabricListView.SelectedIndex = -1;
                    break;
                default:
                    return;
            }
            if (ForgeListView.SelectedIndex >= 0)
            {
                var forge = control.SelectedItem as ForgeInstallEntry;
                str += $"-Forge {forge.ForgeVersion} ";
            }
            if (OptifineListView.SelectedIndex >= 0)
            {
                var optifine = control.SelectedItem as Object;
                str += $"-Optifine {optifine.ToString()} ";
            }
            if (FabricListView.SelectedIndex >= 0)
            {
                var fabric = control.SelectedItem as FabricBuildEntry;
                str += $"-Fabric {fabric.BuildVersion} ";
            }
            if (QuiltListView.SelectedIndex >= 0)
            {
                var quilt = control.SelectedItem as QuiltBuildEntry;
                str += $"-Quilt {quilt.BuildVersion} ";
            }
            AdditionalInstallText.Text = str;
            HandleCustomId(VersionId.Text, CustomGameIdTextBox.Text);
        }
        private void ViewUpdate_Click(object sender, RoutedEventArgs e)
        {
            //https://minecraft.wiki/w/Java_Edition_1.20.4
            Process.Start("explorer.exe", $"https://minecraft.wiki/w/Java_Edition_{VersionIdLabel.Content}");
        }
        #endregion
        public AutoInstall()
        {
            InitializeComponent();
            InstallPreView.Visibility = Visibility.Hidden;
        }
        public async void InstallGame(string versionId, bool msg, string versionName = null, ForgeInstallEntry forgeInstallEntry = null, FabricBuildEntry fabricBuildEntry = null, QuiltBuildEntry quiltBuildEntry = null)
        {
            var customId = HandleCustomId(VersionId.Text, CustomGameIdTextBox.Text);
            Regex regex = new Regex(@"[\\/:*?""<>|]");
            MatchCollection matches = regex.Matches(customId);
            if (matches.Count > 0)
            {
                var str = string.Empty;
                foreach (Match match in matches)
                {
                    str += match.Value;
                }
                if (msg)
                    Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("IncludeSpecialWord")}：{str}");
                else
                    MessageBoxX.Show($"{LangHelper.Current.GetText("IncludeSpecialWord")}：{str}", "Yu Minecraft Launcher");
                return;
            }
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            var resolver = new GameResolver(setting.MinecraftFolder);
            if (setting.DownloadSource == Public.Class.SettingItem.DownloadSource.BmclApi)
            {
                MirrorDownloadManager.IsUseMirrorDownloadSource = true;
            }
            else
            {
                MirrorDownloadManager.IsUseMirrorDownloadSource = false;
            }
            var vanlliaInstaller = new VanlliaInstaller(resolver, versionId, MirrorDownloadManager.Bmcl);
            if (Directory.Exists(Path.Combine(setting.MinecraftFolder, "versions", customId)))
            {
                if (msg)
                    Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("FileExists")}：{customId}");
                else
                    MessageBoxX.Show($"{LangHelper.Current.GetText("FileExists")}：{customId}", "Yu Minecraft Launcher");
                return;
            }
            ReturnToVanlliaList_PreviewMouseDown(null, null);

            var task = Const.Window.tasks.CreateTask($"{LangHelper.Current.GetText("Install")}：Vanllia - {versionId}", true);
            task.AppendText("-----> Vanllia");

            await Task.Run(async () =>
            {
                try
                {
                    vanlliaInstaller.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            task.AppendText(x.ProgressStatus);
                            task.UpdateProgress(x.Progress * 100);
                        });
                    };

                    var result = await vanlliaInstaller.InstallAsync();

                    if (!result)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Vanllia - {versionId}");
                        });
                        return;
                    }
                    else
                    {
                        if (forgeInstallEntry == null && quiltBuildEntry == null && fabricBuildEntry == null)
                        {
                            Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFinish")}：Vanllia - {versionId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        MessageBoxX.Show($"{LangHelper.Current.GetText("InstallFail")}：Vanllia - {versionId}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                    });
                }
            });//Vanllia
            var game = resolver.GetGameEntity(versionId);
            if (forgeInstallEntry != null)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                        if (javas.Count <= 0)
                        {
                            Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("CannotFandRightJavaText")}");
                        }
                        else
                        {
                            var forgeInstaller = new ForgeInstaller(game, forgeInstallEntry, javas[0].JavaPath, customId, MirrorDownloadManager.Bmcl);
                            await Dispatcher.BeginInvoke(() =>
                            {
                                task.UpdateTaskName($"{LangHelper.Current.GetText("Install")}：Forge - {versionId}");
                                task.AppendText("-----> Forge");
                            });
                            forgeInstaller.ProgressChanged += (_, x) =>
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    task.AppendText(x.ProgressStatus);
                                    task.UpdateProgress(x.Progress * 100);
                                });
                            };

                            var result = await forgeInstaller.InstallAsync();

                            if (result)
                            {
                                await Dispatcher.BeginInvoke(() =>
                                {
                                    Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：Forge-{customId}");
                                    Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFinish")}：Forge-{customId}");
                                });
                            }
                            else
                            {
                                await Dispatcher.BeginInvoke(() =>
                                {
                                    Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Forge-{customId}");
                                    Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFail")}：Forge-{customId}");
                                });
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("InstallFail")}：Forge-{customId}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                        });
                    }
                });
            }//Forge
            if (fabricBuildEntry != null)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var fabricInstaller = new FabricInstaller(game, fabricBuildEntry, customId, MirrorDownloadManager.Bmcl);
                        await Dispatcher.BeginInvoke(() =>
                        {
                            task.UpdateTaskName($"{LangHelper.Current.GetText("Install")}：Fabric - {versionId}");
                            task.AppendText("-----> Fabric");
                        });
                        fabricInstaller.ProgressChanged += (_, x) =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                task.AppendText(x.ProgressStatus);
                                task.UpdateProgress(x.Progress * 100);
                            });
                        };

                        var result = await fabricInstaller.InstallAsync();

                        if (result)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：Fabric-{customId}");
                                Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFinish")}：Fabric-{customId}");
                            });
                        }
                        else
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Fabric-{customId}");
                                Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFail")}：Fabric-{customId}");
                            });
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("InstallFail")}：Fabric-{customId}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                        });
                    }
                });
            }//Fabric
            if (quiltBuildEntry != null)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var quiltInstaller = new QuiltInstaller(game, quiltBuildEntry, customId, MirrorDownloadManager.Bmcl);
                        await Dispatcher.BeginInvoke(() =>
                        {
                            task.UpdateTaskName($"{LangHelper.Current.GetText("Install")}：Quilt - {versionId}");
                            task.AppendText("-----> Quilt");
                        });
                        quiltInstaller.ProgressChanged += (_, x) =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                task.AppendText(x.ProgressStatus);
                                task.UpdateProgress(x.Progress * 100);
                            });
                        };

                        var result = await quiltInstaller.InstallAsync();

                        if (result)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：Quilt-{customId}");
                                Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFinish")}：Quilt-{customId}");
                            });
                        }
                        else
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Quilt-{customId}");
                                Method.ShowWin10Notice($"{LangHelper.Current.GetText("InstallFail")}：Quilt-{customId}");
                            });
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("InstallFail")}：Quilt-{customId}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                        });
                    }
                });
            }//Quilt

            Toast.Show(window: Const.Window.main, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：{customId}");
            task.Destory();
        }
        public void ReadyInstallGame(string versionId)
        {
            VersionId.Text = versionId;
            VersionIdLabel.Content = versionId;
            CustomGameIdTextBox.Text = versionId;
            InstallPreView.Visibility = Visibility.Visible;
            VanlliaScrollViewer.Visibility = Visibility.Hidden;
            OptifineLoading.Visibility = Visibility.Visible;
            ForgeLoading.Visibility = Visibility.Visible;
            FabricLoading.Visibility = Visibility.Visible;
            QuiltLoading.Visibility = Visibility.Visible;
            OptifineListView.SelectedIndex = -1;
            ForgeListView.SelectedIndex = -1;
            FabricListView.SelectedIndex = -1;
            QuiltListView.SelectedIndex = -1;
            OptifineListView.Items.Clear();
            ForgeListView.Items.Clear();
            FabricListView.Items.Clear();
            QuiltListView.Items.Clear();

            //_ = Task.Run(async () =>
            //{
            //    var optifine = await OptifineInstaller.EnumerableFromVersionAsync(versionId);
            //    await Dispatcher.BeginInvoke(() =>
            //    {
            //        foreach (var item in optifine)
            //        {

            //        }
            //        OptifineLoading.Visibility = Visibility.Hidden;
            //    });
            //});
            _ = Task.Run(async () =>
            {
                var forge = await ForgeInstaller.EnumerableFromVersionAsync(versionId);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in forge)
                    {
                        ForgeListView.Items.Add(item);
                    }
                    ForgeLoading.Visibility = Visibility.Hidden;
                });
            });
            _ = Task.Run(async () =>
            {
                var fabric = await FabricInstaller.EnumerableFromVersionAsync(versionId);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in fabric)
                    {
                        FabricListView.Items.Add(item);
                    }
                    FabricLoading.Visibility = Visibility.Hidden;
                });
            });
            _ = Task.Run(async () =>
            {
                var quilt = await QuiltInstaller.EnumerableFromVersionAsync(versionId);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in quilt)
                    {
                        QuiltListView.Items.Add(item);
                    }
                    QuiltLoading.Visibility = Visibility.Hidden;
                });
            });

            InstallPreviewControlGrid.Height = 92;
        }
        public string HandleCustomId(string versionId, string customId)
        {
            if ((ForgeListView.SelectedIndex >= 0 || FabricListView.SelectedIndex >= 0 || QuiltListView.SelectedIndex >= 0) && customId == versionId)
            {
                var value = versionId;
                InstallPreviewControlGrid.Height = 108;
                if (ForgeListView.SelectedIndex >= 0)
                {
                    var entry = ForgeListView.SelectedItem as ForgeInstallEntry;
                    value = $"{versionId}-Forge_{entry.ForgeVersion}";
                }
                else if (FabricListView.SelectedIndex >= 0)
                {
                    var entry = FabricListView.SelectedItem as FabricBuildEntry;
                    value = $"{versionId}-Fabric_{entry.BuildVersion}";
                }
                else if (QuiltListView.SelectedIndex >= 0)
                {
                    var entry = QuiltListView.SelectedItem as QuiltBuildEntry;
                    value = $"{versionId}-Quilt_{entry.BuildVersion}";
                }
                HandleCustomIdWarning.Text = value;
                return value;
            }
            else
            {
                InstallPreviewControlGrid.Height = 92;
                return customId;
            }
        }
    }
}
