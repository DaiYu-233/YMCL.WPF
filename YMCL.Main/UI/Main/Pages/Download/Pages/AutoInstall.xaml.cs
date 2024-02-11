using MinecraftLaunch;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;
using Newtonsoft.Json;
using Panuon.WPF.UI;
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
        #region UI
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
                            foreach (var item in vanlliaList)
                            {
                                if (item.Type == "release")
                                {
                                    ReleaseVanlliaListView.Items.Add(item);
                                }
                                else if (item.Type == "snapshot")
                                {
                                    SnapshotVanlliaListView.Items.Add(item);
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
                    Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: "Get Vanllia List Fail");
                }
            }
        }
        private void ReturnToVanlliaList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstallPreView.Visibility = Visibility.Hidden;
            VanlliaScrollViewer.Visibility = Visibility.Visible;
        }
        private void VanlliaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var manifest = ((System.Windows.Controls.ListView)sender).SelectedItem as VersionManifestEntry;
            VersionId.Text = manifest.Id;
            VersionIdLabel.Content = manifest.Id;
            CustomGameIdTextBox.Text = manifest.Id;
            InstallPreView.Visibility = Visibility.Visible;
            VanlliaScrollViewer.Visibility = Visibility.Hidden;
            OptifineLoading.Visibility = Visibility.Visible;
            ForgeLoading.Visibility = Visibility.Visible;
            FabricLoading.Visibility = Visibility.Visible;
            QuiltLoading.Visibility = Visibility.Visible;

            //_ = Task.Run(async () =>
            //{
            //    var optifine = await OptifineInstaller.EnumerableFromVersionAsync(manifest.Id);
            //    await Dispatcher.BeginInvoke(() =>
            //    {
            //        foreach (var item in optifine)
            //        {

            //        }
            //          OptifineLoading.Visibility = Visibility.Hidden;
            //    });
            //});
            //_ = Task.Run(async () =>
            //{
            //    var forge = await ForgeInstaller.EnumerableFromVersionAsync(manifest.Id);
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
                var fabric = await FabricInstaller.EnumerableFromVersionAsync(manifest.Id);
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
                var quilt = await QuiltInstaller.EnumerableFromVersionAsync(manifest.Id);
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
        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            InstallGame(VersionId.Text, true, CustomGameIdTextBox.Text);
        }
        #endregion
        public AutoInstall()
        {
            InitializeComponent();

            InstallPreView.Visibility = Visibility.Hidden;
        }
        public async void InstallGame(string versionId, bool msg, string versionName = null)
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

            await Task.Run(async () =>
            {
                vanlliaInstaller.ProgressChanged += (_, x) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        taskProgress.InsertProgressText(x.ProgressStatus);
                        taskProgress.UpdateProgress(x.Progress*100);
                    });
                };

                var result = await vanlliaInstaller.InstallAsync();

                if (result)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        taskProgress.Hide();
                        Toast.Show(window: Const.Window.mainWindow, position: ToastPosition.Top, message: $"Vanllia - {versionId} Install Successfully");
                    });
                }
            });
        }

    }
}
