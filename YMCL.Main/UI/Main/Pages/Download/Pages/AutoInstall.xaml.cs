using iNKORE.UI.WPF.Modern.Controls;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Install;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
using ListView = iNKORE.UI.WPF.Modern.Controls.ListView;

namespace YMCL.Main.UI.Main.Pages.Download.Pages
{
    /// <summary>
    /// AutoInstall.xaml 的交互逻辑
    /// </summary>
    public partial class AutoInstall : System.Windows.Controls.Page
    {
        bool _firstLoad = true;
        public AutoInstall()
        {
            InitializeComponent();

            InstallPreView.Visibility = Visibility.Hidden;
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
        public async void InstallGame(string versionId)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            var resolver = new GameResolver(setting.MinecraftFolder);
            var vanlliaInstaller = new VanlliaInstaller(resolver, versionId);

            TaskProgress.TaskProgressWindow taskProgress = new($"Vanllia - {versionId}", true);
            taskProgress.Show();

            await Task.Run(async () =>
            {
                vanlliaInstaller.ProgressChanged += (_, x) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        taskProgress.InsertProgressText(x.ProgressStatus);
                        taskProgress.UpdateProgress(x.Progress);
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
        private async void VanlliaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var manifest = ((ListView)sender).SelectedItem as VersionManifestEntry;
            VersionId.Text = manifest.Id;
            VersionIdLabel.Content = manifest.Id;
            CustomGameIdTextBox.Text = manifest.Id;
            InstallPreView.Visibility = Visibility.Visible;
            VanlliaScrollViewer.Visibility = Visibility.Hidden;

            //_ = Task.Run(async () =>
            //{
            //    var optifine = await OptifineInstaller.EnumerableFromVersionAsync(manifest.Id);
            //    await Dispatcher.BeginInvoke(() =>
            //    {
            //        foreach (var item in optifine)
            //        {

            //        }
            //    });
            //});
            _ = Task.Run(async () =>
            {
                var forge = await ForgeInstaller.EnumerableFromVersionAsync(manifest.Id);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in forge)
                    {

                    }
                });
            });
            _ = Task.Run(async () =>
            {
                var fabric = await FabricInstaller.EnumerableFromVersionAsync(manifest.Id);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in fabric)
                    {

                    }
                });
            });
            _ = Task.Run(async () =>
            {
                var quilt = await QuiltInstaller.EnumerableFromVersionAsync(manifest.Id);
                await Dispatcher.BeginInvoke(() =>
                {
                    foreach (var item in quilt)
                    {

                    }
                });
            });

        }
        private void ReturnToVanlliaList_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstallPreView.Visibility = Visibility.Hidden;
            VanlliaScrollViewer.Visibility = Visibility.Visible;
        }
    }
}
