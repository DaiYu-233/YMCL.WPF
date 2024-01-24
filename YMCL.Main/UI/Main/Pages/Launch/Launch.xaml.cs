using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
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
using YMCL.Main.UI.Lang;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

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

            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));

            if (setting.UseCustomHomePage)
            {
                LoadCustomPage();
            }

            LoadMinecraftVersion();
        }

        async void LoadCustomPage()
        {
            try
            {
                FileStream fs = new FileStream(Const.LaunchPageXamlPath, FileMode.Open);
                DependencyObject rootElement = (DependencyObject)XamlReader.Load(fs);
                this.PageRoot.Content = rootElement;
            }
            catch (Exception ex)
            {
                MessageBoxX.Show($"\n{LangHelper.Current.GetText("Launch_Launch_CustomPageSourceError")}：{ex.Message}\n\n{ex.ToString()}", "Yu Minecraft Launcher");
            }
        }


        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
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

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = "https://v1.hitokoto.cn";
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Public.Class.Hitokoto.Root>(data);
                    if (obj != null)
                    {
                        var res = string.Empty;
                        if (obj.from_who != null)
                        {
                            res = obj.hitokoto + $"    ——「{obj.from} ({obj.from_who})」";
                        }
                        else
                        {
                            res = obj.hitokoto + "    ——「" + obj.from + "」";
                        }
                        await Dispatcher.BeginInvoke(() => { Toast.Show(message: obj.hitokoto, position: ToastPosition.Top, window: Const.Window.mainWindow); });
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void OpenVersionList_Click(object sender, RoutedEventArgs e)
        {
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

        private void ReturnHomePage_Click(object sender, RoutedEventArgs e)
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

        private async void LaunchGame_Click(iNKORE.UI.WPF.Modern.Controls.SplitButton sender, iNKORE.UI.WPF.Modern.Controls.SplitButtonClickEventArgs args)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            var accountJson = JsonConvert.DeserializeObject<List<Public.Class.AccountInfo>>(File.ReadAllText(Const.AccountDataPath))[setting.AccountSelectionIndex];
            MinecraftLaunch.Classes.Models.Auth.Account account = null;
            if (accountJson != null)
            {
                if (accountJson.AccountType == "Offline")
                {
                    if (accountJson.Name != null && accountJson.Name != string.Empty)
                    {
                        OfflineAuthenticator authenticator = new(accountJson.Name);
                        account = authenticator.Authenticate();
                    }
                }
            }
            if (account != null)
            {
                if (VersionListView.SelectedItem != null)
                {
                    string javaPath = null;
                    var version = VersionListView.SelectedItem as GameEntry;

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
                        LaunchConfig config = new LaunchConfig
                        {
                            Account = account, //账户信息的获取请使用验证器，使用方法请跳转至验证器文档查看
                            GameWindowConfig = new GameWindowConfig
                            {
                                IsFullscreen = false
                            },
                            JvmConfig = new JvmConfig(javaPath)
                            {
                                MaxMemory = Convert.ToInt32(setting.MaxMem)
                            },
                            IsEnableIndependencyCore = setting.AloneCore
                        };
                        var resolver = new GameResolver(setting.MinecraftFolder);
                        Launcher launcher = new(resolver, config);

                        await Task.Run(async () =>
                        {
                            var gameProcessWatcher = new IGameProcessWatcher();
                            gameProcessWatcher = await launcher.LaunchAsync(version.Id);

                            gameProcessWatcher.Exited += (sender, args) =>
                            {
                                Dispatcher.BeginInvoke(() =>
                               {
                                   Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_GameExit"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                               });
                            };

                            gameProcessWatcher.OutputLogReceived += (sender, args) =>
                            {
                                Debug.WriteLine(args.Text);
                            };

                            await Dispatcher.BeginInvoke(async () =>
                            {
                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_FinishLaunch"), position: ToastPosition.Top, window: Const.Window.mainWindow);

                                await gameProcessWatcher.Process.WaitForExitAsync();

                                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_FinishLaunch"), position: ToastPosition.Top, window: Const.Window.mainWindow);

                                Debug.WriteLine("-------------------------------- Launched");
                            });

                        });
                    }
                    else
                    {
                        if (setting.Java.JavaPath == "<Auto>" || setting.Java == null || setting.Java.JavaPath == string.Empty)
                        {
                            Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_CannotFandRightJava") + version.JavaVersion, position: ToastPosition.Top, window: Const.Window.mainWindow);
                        }
                        else
                        {
                            Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_JavaError"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                        }
                    }
                }
                else
                {
                    Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_NoChooseGame"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                }
            }
            else
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_AccountError"), position: ToastPosition.Top, window: Const.Window.mainWindow);
            }

        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Toast.Show(message: LangHelper.Current.GetText("Launch_LaunchGame_Click_GameExit"), position: ToastPosition.Top, window: Const.Window.mainWindow);
            });
        }
    }
}
