using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Utils;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            InitializeComponent();
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
            id = version.Id;
            CustomIdInput.Text = id;
            InstallVerBlock.Text = id;
            VersionView.Visibility = Visibility.Hidden;
            InstallPreview.Visibility = Visibility.Visible;
            ForgeLoading.Visibility = Visibility.Visible;
            ForgeExpander.Header = "Forge";
            OptifineLoading.Visibility = Visibility.Visible;
            OptifineExpander.Header = "Optifine";
            FabricLoading.Visibility = Visibility.Visible;
            FabricExpander.Header = "Fabric";
            QuileLoading.Visibility = Visibility.Visible;
            QuileExpander.Header = "Quile";
            GetAddition();
        }

        async void GetAddition()
        {
            ForgeList.Items.Clear();
            await Task.Run(async () =>
            {
                var builds = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(id)).ToList();
                builds.ForEach(async x =>
                {
                    await Dispatcher.BeginInvoke(() => { ForgeList.Items.Add(x); });
                });
            });
            ForgeLoading.Visibility = Visibility.Hidden;
            if (ForgeList.Items.Count <= 0)
            {
                ForgeExpander.Header = "Forge (无可用版本)";
            }

            OptifineList.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(id)).ToList();
                res.ForEach(async x =>
                {
                    await Dispatcher.BeginInvoke(() => { OptifineList.Items.Add(x); });
                });
            });
            OptifineLoading.Visibility = Visibility.Hidden;
            if (OptifineList.Items.Count <= 0)
            {
                OptifineExpander.Header = "Optifine (无可用版本)";
            }

            FabricList.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await FabricInstaller.GetFabricBuildsByVersionAsync(id)).ToList();
                res.ForEach(async x =>
                {
                    await Dispatcher.BeginInvoke(() => { FabricList.Items.Add(x); });
                });
            });
            FabricLoading.Visibility = Visibility.Hidden;
            if (FabricList.Items.Count <= 0)
            {
                FabricExpander.Header = "Fabric (无可用版本)";
            }

            QuileList.Items.Clear();
            await Task.Run(async () =>
            {
                var res = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(id)).ToList();
                res.ForEach(async x =>
                {
                    await Dispatcher.BeginInvoke(() => { QuileList.Items.Add(x); });
                });
            });
            QuileLoading.Visibility = Visibility.Hidden;
            if (QuileList.Items.Count <= 0)
            {
                QuileExpander.Header = "Quile (无可用版本)";
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
            QuileList.IsEnabled = true;
            switch (senderObj.Name)
            {
                case "ForgeList":
                    var forgeItem = ForgeList.SelectedItem as ForgeInstallEntity;
                    FabricList.IsEnabled = false;
                    QuileList.IsEnabled = false;
                    FabricList.SelectedIndex = -1;
                    QuileList.SelectedIndex = -1;
                    if (ForgeList.SelectedIndex != -1)
                        AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Forge {forgeItem.ForgeVersion}  ";
                    break;
                case "OptifineList":
                    var optifineItem = OptifineList.SelectedItem as OptiFineInstallEntity;
                    FabricList.IsEnabled = false;
                    QuileList.IsEnabled = false;
                    FabricList.SelectedIndex = -1;
                    QuileList.SelectedIndex = -1;
                    if (OptifineList.SelectedIndex != -1)
                        AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Optifine {optifineItem.FileName}  ";
                    break;
                case "FabricList":
                    var FabricItem = FabricList.SelectedItem as FabricInstallBuild;
                    ForgeList.IsEnabled = false;
                    OptifineList.IsEnabled = false;
                    QuileList.IsEnabled = false;
                    ForgeList.SelectedIndex = -1;
                    OptifineList.SelectedIndex = -1;
                    QuileList.SelectedIndex = -1;
                    if (FabricList.SelectedIndex != -1)
                        AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Fabric {FabricItem.Loader.Version}  ";
                    break;
                case "QuileList":
                    var QuileItem = QuileList.SelectedItem as QuiltInstallBuild;
                    ForgeList.IsEnabled = false;
                    OptifineList.IsEnabled = false;
                    FabricList.IsEnabled = false;
                    ForgeList.SelectedIndex = -1;
                    OptifineList.SelectedIndex = -1;
                    FabricList.SelectedIndex = -1;
                    if (QuileList.SelectedIndex != -1)
                        AdditionInstallBlock.Text = AdditionInstallBlock.Text + $"Quilt {QuileItem.Loader.Version}  ";
                    break;
            }
        }

        private void CancelAddition_Click(object sender, RoutedEventArgs e)
        {
            ForgeList.SelectedIndex = -1;
            OptifineList.SelectedIndex = -1;
            QuileList.SelectedIndex = -1;
            FabricList.SelectedIndex = -1;
            ForgeList.IsEnabled = true;
            OptifineList.IsEnabled = true;
            FabricList.IsEnabled = true;
            QuileList.IsEnabled = true;
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
            });
            displayProgressWindow.Hide();
            Toast.Show(Const.Window.main, $"Vanllia {id} 安装完成", ToastPosition.Top);
        }
    }
}
