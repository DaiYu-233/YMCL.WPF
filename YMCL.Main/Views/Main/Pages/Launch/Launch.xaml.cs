using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using iNKORE.UI.WPF.Modern.Controls.Primitives;
using Microsoft.VisualBasic.FileIO;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Utilities;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using UpdateD;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
using YMCL.Main.Views.TaskManage.TaskProgress;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;
using Method = YMCL.Main.Public.Method;
using SearchOption = System.IO.SearchOption;
using Path = System.IO.Path;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Components.Checker;
using System;
using MinecraftLaunch.Components.Analyzer;

namespace YMCL.Main.Views.Main.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        bool _firstLoadCustomHomePage = true;
        bool _firstLoadPage = true;
        private ObservableCollection<ModListEntry> modObservableCollection;
        private CollectionView modCollectionView;
        List<AccountInfo> accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        #region UI
        private void PageRoot_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Const.Window.main.DropMethod(e);
        }
        private void NoticeBar_CloseButtonClick(iNKORE.UI.WPF.Modern.Controls.InfoBar sender, object args)
        {
            NoticeBar.Visibility = Visibility.Collapsed;
        }
        private void UpdateBar_CloseButtonClick(iNKORE.UI.WPF.Modern.Controls.InfoBar sender, object args)
        {
            UpdateBar.Visibility = Visibility.Collapsed;
        }
        private void RefreshMod_Click(object sender, RoutedEventArgs e)
        {
            LoadVersionMods();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstLoadPage)
            {
                Task task = new Task(() =>
                {
                    LoadNoticeAsync();
                });
                task.Start();
                _firstLoadPage = false;
            }
            Const.Window.main.ReturnlVersionSettingPane.Visibility = Visibility.Hidden;
            VersionSettingBorder.Visibility = Visibility.Hidden;
            minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
            MinecraftFolderComboBox.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            foreach (var item in minecraftFolder)
            {
                MinecraftFolderComboBox.Items.Add(item);
            }
            if (setting.MinecraftFolder == null || !minecraftFolder.Contains(setting.MinecraftFolder))
            {
                MinecraftFolderComboBox.SelectedIndex = 0;
            }
            else
            {
                MinecraftFolderComboBox.SelectedItem = setting.MinecraftFolder;
            }
            LoadMinecraftVersion();
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null | setting.MinecraftVersionId == null)
            {
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
            LoadAccounts();

            if (setting.CustomHomePage == SettingItem.CustomHomePage.LocalFile && !File.Exists(Const.CustomHomePageXamlPath))
            {
                string resourceName = "YMCL.Main.Public.Text.CustomHomePageDefault.xaml";
                Assembly _assembly = Assembly.GetExecutingAssembly();
                Stream stream = _assembly.GetManifestResourceStream(resourceName);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(Const.CustomHomePageXamlPath, result);
                }
            }

            if (setting.CustomHomePage == SettingItem.CustomHomePage.LocalFile && !File.Exists(Const.CustomHomePageCSharpPath))
            {
                File.WriteAllText(Const.CustomHomePageCSharpPath, "-------\r\npublic class YMCLRunner\r\n{\r\n    public static void Main()\r\n    {\r\n        \r\n    }\r\n}");
            }
            if (_firstLoadCustomHomePage)
            {
                _firstLoadCustomHomePage = false;
                if (setting.CustomHomePage == SettingItem.CustomHomePage.LocalFile)
                {
                    try
                    {
                        FileStream fs = new FileStream(Const.CustomHomePageXamlPath, FileMode.Open);
                        UIElement rootElement = (UIElement)XamlReader.Load(fs);
                        this.CustomHomePageRoot.Child = rootElement;

                        //var cs = File.ReadAllText(Const.CustomHomePageCSharpPath).Split("-------");
                        //Function.RunCodeByString(cs[1], dlls: cs[0].Split("\r\n"));
                    }
                    catch (Exception ex)
                    {
                        MessageBoxX.Show($"\n{LangHelper.Current.GetText("Launch_Launch_CustomPageSourceError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                    }
                }
                if (setting.CustomHomePage == SettingItem.CustomHomePage.NetFile)
                {
                    try
                    {
                        WebRequest request = WebRequest.Create(setting.CustomHomePageNetXamlUrl);
                        WebResponse response = request.GetResponse();
                        Stream dataStream = response.GetResponseStream();
                        UIElement rootElement = (UIElement)XamlReader.Load(dataStream);
                        this.CustomHomePageRoot.Child = rootElement;
                    }
                    catch (Exception ex)
                    {

                        MessageBoxX.Show($"\n{LangHelper.Current.GetText("Launch_Launch_CustomPageSourceError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                    }
                }
            }
        }
        private void OpenVersionList_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.main.ReturnHomePageLabel.Content = LangHelper.Current.GetText("Launch_VersionList");
            VersionListBorder.Visibility = Visibility.Visible;
            VersionListView.IsEnabled = true;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                To = new Thickness(10, 10, 10, 10),
                From = new Thickness(10, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                To = new Thickness(0, 0, 80, 0),
                From = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            Const.Window.main.ReturnVersionListPanel.BeginAnimation(MarginProperty, animation1);

            LoadMinecraftVersion();
        }
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMinecraftVersion();
        }
        private void DelMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            var index = MinecraftFolderComboBox.SelectedIndex;
            minecraftFolder.RemoveAt(index);
            MinecraftFolderComboBox.Items.RemoveAt(index);
            if (minecraftFolder.Count == 0)
            {
                minecraftFolder.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
                MinecraftFolderComboBox.Items.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
            }
            MinecraftFolderComboBox.SelectedIndex = 0;
            File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MinecraftFolder = MinecraftFolderComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void AddMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string path = string.Empty;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;//获取选中文件路径
                string[] array = path.Split(@"\");
                if (array[array.Length - 1] != ".minecraft")
                {
                    Toast.Show(Const.Window.main, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_NeedMinecraftFolder"), ToastPosition.Top);
                    return;
                }
                var isIncludePath = false;
                var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.MinecraftFolderDataPath));
                if (dataArray != null)
                {
                    foreach (var item in dataArray)
                    {
                        if (item == path)
                        {
                            isIncludePath = true;
                        }
                    }
                    if (isIncludePath)
                    {
                        Toast.Show(Const.Window.main, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_ExistsMinecraftFolder"), ToastPosition.Top);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                    else
                    {
                        minecraftFolder.Add(path);
                        File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
                        MinecraftFolderComboBox.Items.Add(path);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                }
                else
                {
                    var obj = new List<string>()
                    {
                        System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft")
                    };
                    var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    File.WriteAllText(Const.MinecraftFolderDataPath, data);
                }
            }
        }
        private void VersionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (version != null)
            {
                GameCoreText.Text = version.Id;
            }
            if (version == null || version.Id == setting.MinecraftVersionId)
            {
                return;
            }

            VersionListView.IsEnabled = false;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = new Thickness(10, 10, 10, 10),
                To = new Thickness(10, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.20")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                From = new Thickness(0, -30, 80, 0),
                To = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.15")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            Const.Window.main.ReturnVersionListPanel.BeginAnimation(MarginProperty, animation1);

            setting.MinecraftVersionId = version.Id;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void MinecraftFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (MinecraftFolderComboBox.SelectedItem == null || MinecraftFolderComboBox.SelectedItem.ToString() == setting.MinecraftFolder)
            {
                return;
            }
            setting.MinecraftFolder = MinecraftFolderComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadMinecraftVersion();
        }
        private void OpenVersionSettings_Click(object sender, RoutedEventArgs e)
        {
            Const.Window.main.ReturnlVersionSettingPane.Visibility = Visibility.Visible;
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.MinecraftVersionId == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            LoadVersionMods();
            var version = VersionListView.SelectedItem as GameEntry;
            LoadVersionSettings(version, true);

            if (version == null)
            {
                return;
            }
            VersionSettingPageVersionId.Text = version.Id;
            VersionSettingPageGameDescriptionVersion.Text = version.Version;
            VersionSettingPageGameDescriptionJava.Text = version.JavaVersion.ToString();
            VersionSettingPageGameDescriptionLoaderType.Text = version.MainLoaderType.ToString();
            Const.Window.main.ReturnBtnLabel.Content = LangHelper.Current.GetText("Launch_ToVersionSetting") + " - " + GameCoreText.Text;
            VersionSettingBorder.Visibility = Visibility.Visible;
            VersionListView.IsEnabled = true;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                To = new Thickness(0, 0, 0, 0),
                From = new Thickness(0, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                To = new Thickness(0, 0, 80, 0),
                From = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionSettingBorder.BeginAnimation(MarginProperty, animation);
            Const.Window.main.ReturnlVersionSettingPane.BeginAnimation(MarginProperty, animation1);
        }
        private void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            LaunchClient(version.Id);
        }
        private void AccountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (AccountComboBox.SelectedIndex == setting.AccountSelectionIndex || AccountComboBox.SelectedIndex == -1)
            {
                return;
            }
            setting.AccountSelectionIndex = AccountComboBox.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadAccounts();
        }
        private void SkinHeadImage_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
        private void SaveSkinBtn_Click(object sender, RoutedEventArgs e)
        {
            accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = LangHelper.Current.GetText("SaveSkin");
            dialog.FileName = accounts[setting.AccountSelectionIndex].Name;
            dialog.Filter = "SkinFile | *.png";
            dialog.ShowDialog();
            string path = dialog.FileName;

            if (path == accounts[setting.AccountSelectionIndex].Name)
            {
                Toast.Show(message: LangHelper.Current.GetText("SaveCancel"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var bytes = Convert.FromBase64String(accounts[setting.AccountSelectionIndex].Skin);
                stream.Write(bytes, 0, bytes.Length);
                Toast.Show(message: LangHelper.Current.GetText("SaveSuccess"), position: ToastPosition.Top, window: Const.Window.main);
            }
        }
        private void OpenVersionFolder_Click(object sender, RoutedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version != null)
            {
                var gameFolder = Path.GetDirectoryName(version.JarPath);
                System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
                var path = Path.Combine(gameFolder, button.Tag.ToString());
                Method.CreateFolder(path);
                Process.Start("explorer.exe", path);
            }
        }
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            VersionModListView.SelectAll();
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", MinecraftFolderComboBox.Text);
        }
        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            VersionModListView.SelectedIndex = -1;
        }
        private void VersionModListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = LangHelper.Current.GetText("Launch_SelectedModCount").Split("{|}");
            SelectedModCount.Text = $"{text[0]}{VersionModListView.SelectedItems.Count}{text[1]}";
        }
        private void DisableSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            var mods = VersionModListView.SelectedItems;
            foreach (var item in mods)
            {
                var mod = item as ModListEntry;
                if (mod.Name.ToString().Length > 0)
                {
                    if (Path.GetExtension(mod.File) == ".jar")
                    {
                        File.Move(mod.File, mod.File + ".disabled");
                    }
                }
            }
            LoadVersionMods();
        }
        private void EnableSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            var mods = VersionModListView.SelectedItems;
            foreach (var item in mods)
            {
                var mod = item as ModListEntry;
                if (mod.Name.ToString().Length > 0)
                {
                    if (Path.GetExtension(mod.File) == ".disabled")
                    {
                        File.Move(mod.File, $"{Path.GetDirectoryName(mod.File)}\\{mod.Name}.jar");
                    }
                }
            }
            LoadVersionMods();
        }
        private void ModSearchBox_TextChanged(iNKORE.UI.WPF.Modern.Controls.AutoSuggestBox sender, iNKORE.UI.WPF.Modern.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            modCollectionView.Filter = item =>
            {
                ModListEntry entry = item as ModListEntry;
                return entry.Name.IndexOf(ModSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            };
        }
        private void DeleteSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            var mods = VersionModListView.SelectedItems;
            var text = string.Empty;
            foreach (var item in mods)
            {
                var mod = item as ModListEntry;
                text += $"{Path.GetFileName(mod.File)}\n";
            }
            var message = MessageBoxX.Show($"{LangHelper.Current.GetText("SureDeleteMod")}\n\n{text}", "Yu Minecraft Launcher", MessageBoxButton.OKCancel);
            if (message == MessageBoxResult.OK)
            {
                foreach (var item in mods)
                {
                    var mod = item as ModListEntry;
                    FileSystem.DeleteFile(mod.File, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
                }
            }
            LoadVersionMods();
        }
        private void AloneCoreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchBtn_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            var filePath = Path.Combine(Path.GetDirectoryName(version.JarPath), Const.VersionSettingFileName);
            var versionSetting = LoadVersionSettings(version);
            if ((int)versionSetting.AloneCore == AloneCoreComboBox.SelectedIndex)
            {
                return;
            }
            versionSetting.AloneCore = (SettingItem.VersionSettingAloneCore)AloneCoreComboBox.SelectedIndex;
            File.WriteAllText(filePath, JsonConvert.SerializeObject(versionSetting, Formatting.Indented));
        }
        private void JavaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchBtn_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            var filePath = Path.Combine(Path.GetDirectoryName(version.JarPath), Const.VersionSettingFileName);
            var versionSetting = LoadVersionSettings(version);
            if (JavaComboBox.SelectedItem == null || JavaComboBox.SelectedItem.ToString() == versionSetting.Java.JavaPath)
            {
                return;
            }
            if (JavaComboBox.SelectedIndex == 0 && versionSetting.Java.JavaPath == "Global")
            {
                return;
            }
            if (JavaComboBox.SelectedIndex == 1 && versionSetting.Java.JavaPath == "<Auto>")
            {
                return;
            }
            if (JavaComboBox.SelectedIndex == 0)
            {
                versionSetting.Java = new JavaEntry()
                {
                    JavaPath = "Global"
                };
            }
            else if (JavaComboBox.SelectedIndex == 1)
            {
                versionSetting.Java = new JavaEntry()
                {
                    JavaPath = "<Auto>"
                };
            }
            else
            {
                versionSetting.Java = JavaComboBox.SelectedItem as JavaEntry;
            }
            File.WriteAllText(filePath, JsonConvert.SerializeObject(versionSetting, Formatting.Indented));
        }
        void LoadMem(double value)
        {
            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);
            SilderBox.Maximum = status.ullTotalPhys / 1024 / 1024;
            SilderBox.Minimum = -1;
            SilderBox.Value = value;
        }
        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value);
            if (SilderBox.Value < 0)
            {
                SilderInfo.Text = $"{LangHelper.Current.GetText("UseGlobalSetting")}";
            }
            else
            {
                SilderInfo.Text = $"{SilderBox.Value}M";
            }
        }
        private void SilderBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchBtn_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            var filePath = Path.Combine(Path.GetDirectoryName(version.JarPath), Const.VersionSettingFileName);
            var versionSetting = LoadVersionSettings(version);
            versionSetting.MaxMem = Math.Round(SilderBox.Value);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(versionSetting, Formatting.Indented));
        }
        private void AutoJoinServerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchBtn_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            var filePath = Path.Combine(Path.GetDirectoryName(version.JarPath), Const.VersionSettingFileName);
            var versionSetting = LoadVersionSettings(version);
            versionSetting.AutoJoinServerIp = AutoJoinServerTextBox.Text;
            File.WriteAllText(filePath, JsonConvert.SerializeObject(versionSetting, Formatting.Indented));
        }
        #endregion
        #region Model
        struct MEMORYSTATUSEX
        {
            public int dwLength;
            public int dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        };
        [DllImport("kernel32.dll")]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX LpBuffer);
        class ModListEntry
        {
            public string Name { get; set; }
            public string File { get; set; }
            public TextDecorationCollection Decorations { get; set; }
        }
        #endregion
        public Launch()
        {
            InitializeComponent();
            var text = LangHelper.Current.GetText("Launch_SelectedModCount").Split("{|}");
            SelectedModCount.Text = $"{text[0]}0{text[1]}";
            VersionSettingBorder.Visibility = Visibility.Hidden;
        }
        public async Task LoadNoticeAsync()
        {
            return;
            Update updater = new();
            var url = string.Empty;
            var json = string.Empty;
            await Task.Run(() => { try { Dispatcher.BeginInvoke(() => { json = updater.GetUpdateFile(Const.UpdaterId); var obj = JsonConvert.DeserializeObject<_2018k>(json); if (obj != null) { if (!string.IsNullOrEmpty(obj.Notice.Msg)) { NoticeBar.Visibility = Visibility.Visible; NoticeBar.Message = obj.Notice.Msg.Replace(@"\n", "\n"); if (obj.Notice.Type == _2018k.InfoBar.InfoType.Informational) { NoticeBar.Severity = iNKORE.UI.WPF.Modern.Controls.InfoBarSeverity.Informational; } else if (obj.Notice.Type == _2018k.InfoBar.InfoType.Warning) { NoticeBar.Severity = iNKORE.UI.WPF.Modern.Controls.InfoBarSeverity.Warning; } else if (obj.Notice.Type == _2018k.InfoBar.InfoType.Success) { NoticeBar.Severity = iNKORE.UI.WPF.Modern.Controls.InfoBarSeverity.Success; } else if (obj.Notice.Type == _2018k.InfoBar.InfoType.Error) { NoticeBar.Severity = iNKORE.UI.WPF.Modern.Controls.InfoBarSeverity.Error; } NoticeBar.IsOpen = true; } } if (updater.GetUpdate(Const.UpdaterId, Const.Version) == true) { UpdateBar.Message = $"{Const.Version} / {updater.GetVersionInternet(Const.UpdaterId)}"; UpdateBar.IsOpen = true; UpdateBar.Visibility = Visibility.Visible; } }); } catch { Dispatcher.BeginInvoke(() => { Toast.Show(message: LangHelper.Current.GetText("GetNoticeFail"), position: ToastPosition.Top, window: Const.Window.main); }); } });
        }
        public void LoadVersionMods()
        {
            modObservableCollection = new ObservableCollection<ModListEntry>();
            var disabledMod = new List<ModListEntry>();
            var version = VersionListView.SelectedItem as GameEntry;
            if (version != null)
            {
                modObservableCollection.Clear();
                disabledMod.Clear();
                Task.Run(async () =>
                {
                    var mods = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(version.JarPath), "mods"), "*.*", SearchOption.AllDirectories);
                    foreach (var mod in mods)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            if (Path.GetExtension(mod) == ".jar")
                            {
                                modObservableCollection.Add(new ModListEntry
                                {
                                    Name = Path.GetFileName(mod).Substring(0, Path.GetFileName(mod).Length - 4),
                                    File = mod
                                });
                            }
                            if (Path.GetExtension(mod) == ".disabled")
                            {
                                disabledMod.Add(new ModListEntry
                                {
                                    Name = Path.GetFileName(mod).Substring(0, Path.GetFileName(mod).Length - 13),
                                    Decorations = TextDecorations.Strikethrough,
                                    File = mod
                                });
                            }
                        });
                    }
                    var i = 0;
                    foreach (var item in disabledMod)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            modObservableCollection.Insert(i, item);
                        });
                        i++;
                    }
                });
                modCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(modObservableCollection);
                VersionModListView.ItemsSource = modCollectionView;
                modCollectionView.Filter = item =>
                {
                    ModListEntry entry = item as ModListEntry;
                    return entry.Name.IndexOf(ModSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }
        void LoadMinecraftVersion()
        {
            List<GameEntry> versions = new();
            VersionListView.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.MinecraftFolder == null)
            {
                return;
            }
            try
            {
                Method.CreateFolder(Path.Combine(setting.MinecraftFolder, "versions"));
            }
            catch (Exception)
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var message = MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_Download_AdministratorPermissionRequired"), "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                    if (message == MessageBoxResult.OK)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory,
                            FileName = System.Windows.Forms.Application.ExecutablePath,
                            Verb = "runas"
                        };
                        Process.Start(startInfo);
                        System.Windows.Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_FailedToObtainAdministratorPrivileges"), "Yu Minecraft Launcher", MessageBoxIcon.Error);
                        System.Windows.Application.Current.Shutdown();
                    }
                }
            }
            var list = new List<string>();
            GameResolver resolver = new(setting.MinecraftFolder);
            try
            {
                versions = resolver.GetGameEntitys().ToList();
                versions.ForEach(version =>
                {
                    VersionListView.Items.Add(version);
                    list.Add(version.Id);
                    if (version.Id == setting.MinecraftVersionId)
                    {
                        VersionListView.SelectedItem = version;
                        GameCoreText.Text = version.Id;
                    }
                });
                if (!list.Contains(setting.MinecraftVersionId))
                {
                    setting.MinecraftVersionId = null;
                    File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
                }
            }
            catch
            {
                Toast.Show(message: MainLang.GetVersionListFail, position: ToastPosition.Top, window: Const.Window.main);
            }
        }
        void LoadAccounts()
        {
            accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
            AccountComboBox.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            accounts.ForEach(x =>
            {
                AccountComboBox.Items.Add($"{x.AccountType} {x.Name}");
            });

            if (AccountComboBox.Items.Count > 0)
            {
                if (setting.AccountSelectionIndex <= AccountComboBox.Items.Count)
                {
                    AccountComboBox.SelectedIndex = setting.AccountSelectionIndex;
                }
                else
                {
                    AccountComboBox.SelectedItem = AccountComboBox.Items[0];
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(new List<AccountInfo>() { new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = null,
                    Name = "Steve"
                }}, Formatting.Indented));
                LoadAccounts();
            }

            if (setting.AccountSelectionIndex != -1)
            {
                MinecraftLaunch.Skin.SkinResolver SkinResolver = new(Convert.FromBase64String(accounts[setting.AccountSelectionIndex].Skin));
                var bytes = MinecraftLaunch.Skin.ImageHelper.ConvertToByteArray(SkinResolver.CropSkinHeadBitmap());
                var skin = Method.BytesToBase64(bytes);

                SkinHeadImage.Source = Method.Base64ToImage(skin);
            }
        }
        VersionSetting LoadVersionSettings(GameEntry version, bool loadUI = false)
        {
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchBtn_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                return null;
            }
            var filePath = Path.Combine(Path.GetDirectoryName(version.JarPath), Const.VersionSettingFileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(new VersionSetting(), Formatting.Indented));
            }
            var versionSetting = JsonConvert.DeserializeObject<VersionSetting>(File.ReadAllText(filePath));
            if (loadUI)
            {
                AloneCoreComboBox.SelectedIndex = (int)versionSetting.AloneCore;

                List<JavaEntry> javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                JavaComboBox.Items.Clear();
                JavaComboBox.Items.Add(new JavaEntry()
                {
                    JavaPath = LangHelper.Current.GetText("UseGlobalSetting")
                });
                JavaComboBox.Items.Add(new JavaEntry()
                {
                    JavaPath = LangHelper.Current.GetText("Launch_AutoSelectJava")
                });
                foreach (var item in javas)
                {
                    JavaComboBox.Items.Add(item);
                }
                if (versionSetting.Java == null || versionSetting.Java.JavaPath == "Global" || versionSetting.Java.JavaPath == string.Empty)
                {
                    JavaComboBox.SelectedIndex = 0;
                }
                else if (versionSetting.Java.JavaPath == "<Auto>")
                {
                    JavaComboBox.SelectedIndex = 1;
                }
                else if (!javas.Contains(versionSetting.Java) && versionSetting.Java.JavaPath != "Global" && versionSetting.Java.JavaPath != "<Auto>")
                {
                    JavaComboBox.SelectedIndex = 0;
                }
                else
                {
                    JavaComboBox.SelectedItem = versionSetting.Java;
                }

                LoadMem(versionSetting.MaxMem);

                AutoJoinServerTextBox.Text = versionSetting.AutoJoinServerIp;
            }
            return versionSetting;
        }
        public async void LaunchClient(string versionId, string minecraftPath = "", bool msg = true, string serverIP = "")
        {
            var mcPath = minecraftPath;
            if (string.IsNullOrEmpty(versionId))
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));

            if (string.IsNullOrEmpty(mcPath))
            {
                mcPath = setting.MinecraftFolder;
            }
            var idList = new List<string>();
            var resolver = new GameResolver(mcPath);
            resolver.GetGameEntitys().ToList().ForEach(ver =>
            {
                idList.Add(ver.Id);
            });
            var version = resolver.GetGameEntity(versionId);
            var versionSetting = LoadVersionSettings(version);
            if (!idList.Contains(versionId))
            {
                if (msg)
                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                else
                    MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), "Yu Minecraft Launcher");
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            var accountJson = JsonConvert.DeserializeObject<List<Public.Class.AccountInfo>>(File.ReadAllText(Const.AccountDataPath))[setting.AccountSelectionIndex];
            Account account = null;
            LaunchBtn.IsEnabled = false;
            TaskProgressWindow taskProgress = new TaskProgressWindow($"{version.JarPath} - {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")}", false);

            taskProgress.Show();
            taskProgress.InsertProgressText("-----> YMCLOutputLog", false);
            taskProgress.InsertProgressText("YMCL: " + LangHelper.Current.GetText("VerifyingAccount"));

            if (accountJson != null)
            {
                if (accountJson.AccountType == SettingItem.AccountType.Offline)
                {
                    if (accountJson.Name != null && accountJson.Name != string.Empty)
                    {
                        OfflineAuthenticator authenticator = new(accountJson.Name);
                        account = authenticator.Authenticate();
                    }
                }
                else if (accountJson.AccountType == SettingItem.AccountType.Microsoft)
                {
                    var profile = JsonConvert.DeserializeObject<MicrosoftAccount>(accountJson.Data);
                    MicrosoftAuthenticator authenticator = new(profile, Const.AzureClientId, true);
                    try
                    {
                        account = await authenticator.AuthenticateAsync();
                    }
                    catch
                    {

                    }
                }
                else if (accountJson.AccountType == SettingItem.AccountType.ThirdParty)
                {
                    account = JsonConvert.DeserializeObject<YggdrasilAccount>(accountJson.Data);
                }
            }
            if (account != null)
            {
                if (idList.Contains(versionId))
                {
                    string javaPath = null;
                    if (versionSetting.Java.JavaPath == "Global" || versionSetting == null)
                    {
                        if (setting.Java.JavaPath == "<Auto>" || setting.Java == null || setting.Java.JavaPath == string.Empty)
                        {
                            JavaFetcher javaFetcher = new JavaFetcher();
                            var javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                            try
                            {
                                javaPath = JavaUtil.GetCurrentJava(javas, version).JavaPath;
                            }
                            catch (Exception)
                            {
                                LaunchBtn.IsEnabled = true; taskProgress.Hide();
                                if (msg)
                                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava") + version.JavaVersion, position: ToastPosition.Top, window: Const.Window.main);
                                else
                                    MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava"), "Yu Minecraft Launcher");
                            }
                        }
                        else
                        {
                            javaPath = setting.Java.JavaPath;
                        }
                    }
                    else
                    {
                        if (versionSetting.Java.JavaPath == "<Auto>" || versionSetting.Java == null || versionSetting.Java.JavaPath == string.Empty)
                        {
                            JavaFetcher javaFetcher = new JavaFetcher();
                            var javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                            try
                            {
                                javaPath = JavaUtil.GetCurrentJava(javas, version).JavaPath;
                            }
                            catch (Exception)
                            {
                                LaunchBtn.IsEnabled = true; taskProgress.Hide();
                                if (msg)
                                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava") + version.JavaVersion, position: ToastPosition.Top, window: Const.Window.main);
                                else
                                    MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava"), "Yu Minecraft Launcher");
                            }
                        }
                        else
                        {
                            javaPath = versionSetting.Java.JavaPath;
                        }
                    }
                    if (Convert.ToInt32(version.Version.Split(".")[1]) >= 17)
                    {
                        List<JavaEntry> javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                        foreach (var item in javas)
                        {
                            if (item.JavaSlugVersion >= 17)
                            {
                                javaPath = item.JavaPath;
                                break;
                            }
                        }
                    }
                    bool aloneCore = true;
                    if (!string.IsNullOrEmpty(javaPath))
                    {
                        LaunchConfig config = new();
                        try
                        {
                            var ip = string.Empty;
                            var port = 0;
                            string IP;
                            if (serverIP == "")
                            {
                                IP = versionSetting.AutoJoinServerIp;
                            }
                            else
                            {
                                IP = serverIP;
                            }
                            var server = IP.Split(":");
                            if (!string.IsNullOrEmpty(IP))
                            {
                                ip = server[0];
                                if (server.Length > 1)
                                {
                                    port = Convert.ToInt32(server[1]);
                                }
                                else
                                {
                                    port = 25565;
                                }
                            }
                            double maxMem = 0;
                            if (versionSetting.MaxMem == -1)
                            {
                                maxMem = setting.MaxMem;
                            }
                            else
                            {
                                maxMem = versionSetting.MaxMem;
                            }

                            if (versionSetting.AloneCore == SettingItem.VersionSettingAloneCore.Global)
                            {
                                aloneCore = setting.AloneCore;
                            }
                            else
                            {
                                if (versionSetting.AloneCore == SettingItem.VersionSettingAloneCore.Off)
                                {
                                    aloneCore = false;
                                }
                                else
                                {
                                    aloneCore = true;
                                }
                            }
                            config = new LaunchConfig
                            {
                                Account = account,
                                GameWindowConfig = new GameWindowConfig
                                {
                                    Width = (int)setting.GameWidth,
                                    Height = (int)setting.GameHeight,
                                    IsFullscreen = setting.GameWindow == SettingItem.GameWindow.FullScreen
                                },
                                JvmConfig = new JvmConfig(javaPath)
                                {
                                    MaxMemory = Convert.ToInt32(maxMem)
                                },
                                ServerConfig = new(port, ip),
                                IsEnableIndependencyCore = aloneCore,
                                LauncherName = "YMCL"
                            };
                        }
                        catch (Exception ex)
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("BuildLaunchConfigError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                            return;
                        }
                        Launcher launcher = new(resolver, config);

                        //var task = Const.Window.tasks.CreateTask($"{version.JarPath}", false);

                        await Task.Run(async () =>
                        {
                            try
                            {
                                await Dispatcher.BeginInvoke(async () =>
                                {
                                    var watcher = await launcher.LaunchAsync(version.Id);

                                    watcher.Exited += (_, args) =>
                                    {
                                        Dispatcher.BeginInvoke(() =>
                                        {
                                            Toast.Show(message: $"{LangHelper.Current.GetText("Launch_LaunchGame_Click_GameExit")}：{args.ExitCode}", position: ToastPosition.Top, window: Const.Window.main);
                                            taskProgress.Hide();
                                            Const.Window.main.Focus();

                                            if (args.ExitCode == 1)
                                            {
                                                var crashAnalyzer = new GameCrashAnalyzer(version, aloneCore);
                                                var reports = crashAnalyzer.AnalysisLogs();
                                                var msg = string.Empty;
                                                foreach (var report in reports)
                                                {
                                                    msg += $"\n{report.CrashCauses}";
                                                }
                                                MessageBoxX.Show($"{MainLang.MinecraftCrash}\n{msg}", "Yu Minecraft Launcher");
                                            }
                                        });
                                    };
                                    watcher.OutputLogReceived += async (_, args) =>
                                    {
                                        Debug.WriteLine(args.Log);
                                        await Dispatcher.BeginInvoke(() =>
                                        {
                                            //task.AppendText(args.Text, false);
                                            taskProgress.InsertProgressText(args.Original, false);
                                        });
                                    };

                                    await Dispatcher.BeginInvoke(() =>
                                    {
                                        taskProgress.InsertProgressText("YMCL: " + LangHelper.Current.GetText("WaitForGameWindowAppear"));
                                        //watcher.Process.WaitForInputIdle();
                                        //taskProgress.Hide();
                                        if (!setting.GetOutput)
                                        {
                                            taskProgress.Hide();
                                        }
                                        taskProgress.InsertProgressText("-----> JvmOutputLog", false);
                                        Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_FinishLaunch"), position: ToastPosition.Top, window: Const.Window.main);
                                    });

                                });
                            }
                            catch (Exception ex)
                            {
                                await Dispatcher.BeginInvoke(() =>
                                {
                                    MessageBoxX.Show($"{LangHelper.Current.GetText("LaunchFail")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                                });
                            }
                        });
                    }
                    else
                    {
                        if (setting.Java.JavaPath == "<Auto>" || setting.Java == null || setting.Java.JavaPath == string.Empty)
                        {
                            LaunchBtn.IsEnabled = true; taskProgress.Hide();
                            if (msg)
                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava") + version.JavaVersion, position: ToastPosition.Top, window: Const.Window.main);
                            else
                                MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava"), "Yu Minecraft Launcher");
                        }
                        else
                        {
                            LaunchBtn.IsEnabled = true; taskProgress.Hide();
                            if (msg)
                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_JavaError"), position: ToastPosition.Top, window: Const.Window.main);
                            else
                                MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_JavaError"), "Yu Minecraft Launcher");
                        }
                    }
                }
                else
                {
                    LaunchBtn.IsEnabled = true; taskProgress.Hide();
                    if (msg)
                        Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.main);
                    else
                        MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), "Yu Minecraft Launcher");
                    GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                }
            }
            else
            {
                LaunchBtn.IsEnabled = true;
                taskProgress.Hide();
                if (msg)
                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_AccountError"), position: ToastPosition.Top, window: Const.Window.main);
                else
                    MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_AccountError"), "Yu Minecraft Launcher");
            }
            LaunchBtn.IsEnabled = true;
            //taskProgress.Hide();
        }
        private async void CheckMissingResource_Click(object sender, RoutedEventArgs e)
        {
            var resourceChecker = new ResourceChecker(VersionListView.SelectedItem as GameEntry);
            var result = await resourceChecker.CheckAsync();

            if (!result)
            {
                MessageBoxX.Show($"{MainLang.ResourceAreMissing}：{resourceChecker.MissingResources.Count}", "Yu Minecraft Launcher");
                Console.WriteLine();
            }
            else
            {
                Toast.Show(message: MainLang.NoResourceAreMissing, position: ToastPosition.Top, window: Const.Window.main);
            }
        }
        public void AddModInThisVersion(string mod)
        {
            if (Path.GetExtension(mod) != ".jar")
                return;
            var version = VersionListView.SelectedItem as GameEntry;
            if (version != null)
            {
                var path = Path.Combine(Path.GetDirectoryName(version.JarPath), "mods");
                try
                {
                    File.Copy(path, mod);
                }
                catch (UnauthorizedAccessException)
                {
                    Method.ObtainAdministratorPrivileges($"{MainLang.AccessWasDenied}，{MainLang.SureObtainAdministratorPrivileges}", false);
                }
            }
        }
    }
}
