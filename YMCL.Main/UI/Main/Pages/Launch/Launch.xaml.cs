using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Components.Watcher;
using MinecraftLaunch.Extensions;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using YMCL.Main.Public;
using YMCL.Main.UI.Initialize;
using YMCL.Main.Public.Lang;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;
using YMCL.Main.Public.Class;
using System.Windows.Interop;
using YMCL.Main.UI.TaskProgress;
using MinecraftLaunch.Classes.Models.Auth;
using YMCL.Main.UI.Main.Pages.Setting.Pages.Account;
using iNKORE.UI.WPF.Modern.Controls.Primitives;
using System.Text;
using YMCL.Main.UI.Main.Pages.Setting;

namespace YMCL.Main.UI.Main.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        public Launch()
        {
            InitializeComponent();
        }
        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
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
        }

        private void OpenVersionList_Click(object sender, RoutedEventArgs e)
        {
            ReturnHomePageLabel.Content = LangHelper.Current.GetText("Launch_VersionList");
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
                To = new Thickness(0, -30, 80, 0),
                From = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanel.BeginAnimation(MarginProperty, animation1);

            LoadMinecraftVersion();
        }

        private async void ReturnHomePage_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = new Thickness(10, 10, 10, 10),
                To = new Thickness(10, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.35")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                From = new Thickness(0, -30, 80, 0),
                To = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.35")
            };
            VersionListBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanel.BeginAnimation(MarginProperty, animation1);
            await Task.Delay(250);
            VersionListBorder.Visibility = Visibility.Hidden;
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
                Function.CreateFolder(Path.Combine(setting.MinecraftFolder, "versions"));
            }
            catch (Exception ex)
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
                    Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_NeedMinecraftFolder"), ToastPosition.Top);
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
                        Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_ExistsMinecraftFolder"), ToastPosition.Top);
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
            ReturnPanel.BeginAnimation(MarginProperty, animation1);

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

        public async void LaunchClient(string versionId, string minecraftPath = "", bool msg = true, string serverIP = "")
        {
            var mcPath = minecraftPath;
            if (string.IsNullOrEmpty(versionId))
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            var accountJson = JsonConvert.DeserializeObject<List<Public.Class.AccountInfo>>(File.ReadAllText(Const.AccountDataPath))[setting.AccountSelectionIndex];
            MinecraftLaunch.Classes.Models.Auth.Account account = null;
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
                    Toast.Show(message: LangHelper.Current.GetText("VerifyingAccount"), position: ToastPosition.Top, window: Const.Window.mainWindow);

                    var profile = JsonConvert.DeserializeObject<MicrosoftAccount>(accountJson.Data);
                    MicrosoftAuthenticator authenticator = new(profile, Const.AzureClientId, true);
                    account = await authenticator.AuthenticateAsync();
                }
            }
            if (account != null)
            {
                if (string.IsNullOrEmpty(mcPath))
                {
                    mcPath = setting.MinecraftFolder;
                }
                var VersionReadied = false;
                var idList = new List<string>();
                var resolver = new GameResolver(mcPath);
                resolver.GetGameEntitys().ToList().ForEach(ver =>
                {
                    idList.Add(ver.Id);
                });
                if (idList.Contains(versionId))
                {
                    string javaPath = null;
                    var version = resolver.GetGameEntity(versionId);

                    if (setting.Java.JavaPath == "<Auto>" || setting.Java == null || setting.Java.JavaPath == string.Empty)
                    {
                        JavaFetcher javaFetcher = new JavaFetcher();
                        var javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
                        foreach (var item in javas)
                        {
                            if (item.JavaSlugVersion == version.JavaVersion)
                            {
                                javaPath = item.JavaPath;
                                break;
                            }
                        }
                    }
                    else
                    {
                        javaPath = setting.Java.JavaPath;
                    }

                    if (!string.IsNullOrEmpty(javaPath))
                    {
                        bool launchError = false;
                        LaunchConfig config = new();
                        try
                        {
                            var ip = string.Empty;
                            var port = 0;
                            string IP;
                            if (serverIP == "")
                            {
                                //IP = setting.AutoJoinServerIP;
                            }
                            else
                            {
                                IP = serverIP;
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
                            }

                            config = new LaunchConfig
                            {
                                Account = account, //账户信息的获取请使用验证器，使用方法请跳转至验证器文档查看
                                GameWindowConfig = new GameWindowConfig
                                {
                                    Width = (int)setting.GameWidth,
                                    Height = (int)setting.GameHeight,
                                    IsFullscreen = setting.GameWindow == SettingItem.GameWindow.FullScreen
                                },
                                JvmConfig = new JvmConfig(javaPath)
                                {
                                    MaxMemory = Convert.ToInt32(setting.MaxMem)
                                },
                                ServerConfig = new(port, ip),
                                IsEnableIndependencyCore = setting.AloneCore,
                                LauncherName = "YMCL"
                            };
                        }
                        catch (Exception ex)
                        {
                            MessageBoxX.Show($"{LangHelper.Current.GetText("BuildLaunchConfigError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
                            return;
                        }

                        Launcher launcher = new(resolver, config);

                        await Task.Run(async () =>
                        {
                            try
                            {
                                await Dispatcher.BeginInvoke(async () =>
                                {
                                    TaskProgressWindow taskProgress = new TaskProgressWindow($"{version.JarPath} - {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")}", false);

                                    if (setting.GetOutput)
                                    {
                                        taskProgress.Show();
                                    }

                                    var watcher = await launcher.LaunchAsync(version.Id);

                                    watcher.Exited += (sender, args) =>
                                    {
                                        Dispatcher.BeginInvoke(async () =>
                                        {
                                            Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_GameExit"), position: ToastPosition.Top, window: Const.Window.mainWindow);

                                            if (setting.GetOutput)
                                            {
                                                taskProgress.Hide();
                                            }
                                        });
                                    };
                                    watcher.OutputLogReceived += async (sender, args) =>
                                    {
                                        Debug.WriteLine(args.Text);
                                        await Dispatcher.BeginInvoke(async () =>
                                        {
                                            taskProgress.InsertProgressText(args.Text);
                                        });
                                    };
                                    await Dispatcher.BeginInvoke(async () =>
                                    {
                                        Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_FinishLaunch"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                                    });

                                });
                            }
                            catch (Exception ex)
                            {
                                await Dispatcher.BeginInvoke(async () =>
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
                            LaunchBtn.IsEnabled = true;
                            if (msg)
                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava") + version.JavaVersion, position: ToastPosition.Top, window: Const.Window.mainWindow);
                            else
                                MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava"), "Yu Minecraft Launcher");
                        }
                        else
                        {
                            LaunchBtn.IsEnabled = true;
                            if (msg)
                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_JavaError"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                            else
                                MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_JavaError"), "Yu Minecraft Launcher");
                        }
                    }
                }
                else
                {
                    LaunchBtn.IsEnabled = true;
                    if (msg)
                        Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                    else
                        MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), "Yu Minecraft Launcher");
                    GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                }
            }
            else
            {
                LaunchBtn.IsEnabled = true;
                if (msg)
                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_AccountError"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                else
                    MessageBoxX.Show(LangHelper.Current.GetText("Launch_LaunchGame_Click_AccountError"), "Yu Minecraft Launcher");
            }
            LaunchBtn.IsEnabled = true;
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_GameExit"), position: ToastPosition.Top, window: Const.Window.mainWindow);
            });
        }

        List<AccountInfo> accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
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
                var skin = Function.BytesToBase64(bytes);

                SkinHeadImage.Source = Function.Base64ToImage(skin);
            }
        }

        private async void ReturnBtn_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = new Thickness(0),
                To = new Thickness(0, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                From = new Thickness(0, -30, 80, 0),
                To = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionSettingBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanelSetting.BeginAnimation(MarginProperty, animation1);
            await Task.Delay(250);
            VersionSettingBorder.Visibility = Visibility.Hidden;
        }

        private void OpenVersionSettings_Click(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.MinecraftVersionId == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                GameCoreText.Text = LangHelper.Current.GetText("Launch_NoChooseGame");
                return;
            }
            ReturnBtnLabel.Content = LangHelper.Current.GetText("Launch_ToVersionSetting") + " - " + GameCoreText.Text;
            VersionSettingBorder.Visibility = Visibility.Visible;
            VersionListView.IsEnabled = true;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                To = new Thickness(0),
                From = new Thickness(0, PageRoot.ActualHeight, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            ThicknessAnimation animation1 = new ThicknessAnimation()
            {
                To = new Thickness(0, -30, 80, 0),
                From = new Thickness(0, -80, 80, 00),
                Duration = TimeSpan.Parse("0:0:0.25")
            };
            VersionSettingBorder.BeginAnimation(MarginProperty, animation);
            ReturnPanelSetting.BeginAnimation(MarginProperty, animation1);
        }

        private void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            var version = VersionListView.SelectedItem as GameEntry;
            if (version == null)
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.mainWindow);
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
                Toast.Show(message: LangHelper.Current.GetText("SaveCancel"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                return;
            }

            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var bytes = Convert.FromBase64String(accounts[setting.AccountSelectionIndex].Skin);
                stream.Write(bytes, 0, bytes.Length);
                Toast.Show(message: LangHelper.Current.GetText("SaveSuccess"), position: ToastPosition.Top, window: Const.Window.mainWindow);
            }
        }
    }
}
