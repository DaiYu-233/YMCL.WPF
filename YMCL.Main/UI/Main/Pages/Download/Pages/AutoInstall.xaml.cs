using MinecraftLaunch;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;
namespace YMCL.Main.UI.Main.Pages.Download.Pages
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
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstLoad)
            {
                _firstLoad = false;
                try
                {
                    await Task.Run(async () =>
                    {
                        var vanlliaList = await VanlliaInstaller.EnumerableGameCoreAsync();
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
                        });
                    });
                }
                catch
                {
                    Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: LangHelper.Current.GetText("GetInstallableVersionFail"));
                }
            }
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

            }
            else if (FabricListView.SelectedIndex >= 0)
            {
                var entry = FabricListView.SelectedItem as FabricBuildEntry;
                InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text, fabricBuildEntry: entry);
            }
            else if (QuiltListView.SelectedIndex >= 0)
            {

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
                        ForgeLabel.Content = (control.SelectedItem as ForgeInstallEntry).Build;
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
                str += $"-Forge {forge.Build} ";
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
        public async void InstallGame(string versionId, bool msg, string versionName = null, FabricBuildEntry fabricBuildEntry = null, QuiltBuildEntry quiltBuildEntry = null)
        {
            var customId = string.Empty;
            customId = string.IsNullOrEmpty(versionName) ? versionId : versionName;
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
                    Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("IncludeSpecialWord")}：{str}");
                else
                    MessageBoxX.Show($"{LangHelper.Current.GetText("IncludeSpecialWord")}：{str}", "Yu Minecraft Launcher");
                return;
            }
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            var resolver = new GameResolver(setting.MinecraftFolder);
            var vanlliaInstaller = new VanlliaInstaller(resolver, versionId);
            if (Directory.Exists(Path.Combine(setting.MinecraftFolder, "versions", customId)))
            {
                if (msg)
                    Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("FileExists")}：{customId}");
                else
                    MessageBoxX.Show($"{LangHelper.Current.GetText("FileExists")}：{customId}", "Yu Minecraft Launcher");
                return;
            }
            TaskProgress.TaskProgressWindow taskProgress = new($"Vanllia - {versionId}", true);
            taskProgress.Show();
            ReturnToVanlliaList_PreviewMouseDown(null, null);
            taskProgress.InsertProgressText("-----> Vanllia");
            await Task.Run(async () =>
            {
                try
                {
                    vanlliaInstaller.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            taskProgress.InsertProgressText(x.ProgressStatus);
                            taskProgress.UpdateProgress(x.Progress * 100);
                        });
                    };

                    var result = await vanlliaInstaller.InstallAsync();

                    if (!result)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Vanllia - {versionId}");
                        });
                        return;
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
            var game = resolver.GetGameEntity(customId);
            if (fabricBuildEntry != null)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        var fabricInstaller = new FabricInstaller(game, fabricBuildEntry, customId);
                        await Dispatcher.BeginInvoke(() =>
                        {
                            taskProgress.UpdateTitle("Fabric");
                            taskProgress.InsertProgressText("-----> Fabric");
                        });
                        fabricInstaller.ProgressChanged += (_, x) =>
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                taskProgress.InsertProgressText(x.ProgressStatus);
                                taskProgress.UpdateProgress(x.Progress * 100);
                            });
                        };

                        var result = await fabricInstaller.InstallAsync();

                        if (result)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：Fabric");
                            });
                        }
                        else
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFail")}：Fabric");
                            });
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("InstallFail")}：Fabric\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                        });
                    }
                });
            }//Fabric

            Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"{LangHelper.Current.GetText("InstallFinish")}：{customId}");
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
            //_ = Task.Run(async () =>
            //{
            //    var forge = await ForgeInstaller.EnumerableFromVersionAsync(versionId);
            //    await Dispatcher.BeginInvoke(() =>
            //    {
            //        foreach (var item in forge)
            //        {

            //        }
            //        ForgeLoading.Visibility = Visibility.Hidden;
            //    });
            //});
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
        }
    }
}
