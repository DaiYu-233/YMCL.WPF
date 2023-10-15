using MinecraftLaunch.Modules.Installer;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVersions();
        }


    }
}
