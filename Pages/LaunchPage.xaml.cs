using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YMCL.Class;

namespace YMCL.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page
    {
        
        public LaunchPage()
        {
            InitializeComponent();
            hitokoto();
            LoadMinecraftFolders();

            HitokotoTextBlock.MouseRightButtonDown += (sender, e) =>
            {
                System.Windows.Clipboard.SetText(HitokotoTextBlock.Text);
                Toast.Show("已复制到剪切板", ToastPosition.Top);
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MinecraftVersionBorderCloseButton_Click(sender, e);
        }

        #region MinecraftFolder
        List<string> minecraftFolders = new();
        void LoadMinecraftFolders()
        {
            MinecraftPathComboBox.Items.Clear();
            minecraftFolders = JsonConvert.DeserializeObject<List<string>>
                (File.ReadAllText(Const.YMCLMinecraftFolderDataPath));
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            foreach (var item in minecraftFolders)
            {
                MinecraftPathComboBox.Items.Add(item);
            }
            MinecraftPathComboBox.SelectedItem = setting.MinecraftPath;

            if (setting.SelectedVersion != null)
            {
                VersionListView.SelectedItem = setting.SelectedVersion;
            }
            else
            {
                VersionListView.SelectedIndex = -1;
            }

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
                    Toast.Show(Const.Window.main, "需选择 .minecraft 文件夹", ToastPosition.Top);
                    return;
                }
                var isIncludePath = false;
                var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.YMCLMinecraftFolderDataPath));
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
                        Toast.Show(Const.Window.main, "已存在此文件夹", ToastPosition.Top);
                        MinecraftPathComboBox.SelectedItem = path;
                    }
                    else
                    {
                        minecraftFolders.Add(path);
                        File.WriteAllText(Const.YMCLMinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolders, Formatting.Indented));
                        MinecraftPathComboBox.Items.Add(path);
                        MinecraftPathComboBox.SelectedItem = path;
                    }
                }
                else
                {
                    var obj = new List<string>()
                    {
                        ".minecraft"
                    };
                    var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    File.WriteAllText(Const.YMCLMinecraftFolderDataPath, data);
                }
            }
        }

        private void DelMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            var index = MinecraftPathComboBox.SelectedIndex;
            minecraftFolders.RemoveAt(index);
            MinecraftPathComboBox.Items.RemoveAt(index);
            if (minecraftFolders.Count == 0)
            {
                minecraftFolders.Add(".minecraft");
                MinecraftPathComboBox.Items.Add(".minecraft");
            }
            MinecraftPathComboBox.SelectedIndex = 0;
            File.WriteAllText(Const.YMCLMinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolders, Formatting.Indented));
        }

        private void MinecraftPathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VersionListView.SelectedIndex = -1;
            var item = MinecraftPathComboBox.SelectedItem as string;
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.MinecraftPath = item;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            LoadMinecraftVersion();
        }
        #endregion
        #region MinecreaftVersion

        void LoadMinecraftVersion()
        {
            List<string> gameCores = new();
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            MinecraftLaunch.Modules.Utils.GameCoreUtil gameCoreUtil = new(setting.MinecraftPath);
            gameCoreUtil.GetGameCores().ToList().ForEach(gameCore =>
            {
                gameCores.Add(gameCore.Id);
            });
            VersionListView.ItemsSource = gameCores;
        }

        private void ToVersionList_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;

            HitokotoTextBlock.Visibility = Visibility.Hidden;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                To = new Thickness(10, 10, 10, 10),
                From = new Thickness(10, 350, 0, 0),
                Duration = TimeSpan.Parse("0:0:0.15")
            };
            VerListBorder.BeginAnimation(MarginProperty, animation);
            LoadMinecraftVersion();
        }

        private void MinecraftVersionBorderCloseButton_Click(object sender, RoutedEventArgs e)
        {
            var height = Root.ActualHeight;
            ThicknessAnimation animation = new ThicknessAnimation()
            {
                From = new Thickness(10, 10, 10, 10),
                To = new Thickness(10, height, 0, 10),
                Duration = TimeSpan.Parse("0:0:0.10")
            };
            //VerListBorder.BeginAnimation(OpacityProperty, doubleAnimation);
            VerListBorder.BeginAnimation(MarginProperty, animation);

            HitokotoTextBlock.Visibility = Visibility.Visible;
        }

        private async void VersionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VersionListView.IsEnabled = false;
            if (VersionListView.SelectedIndex == -1)
            {
                GameCoreText.Text = "<未选择游戏核心>";
                var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
                setting.SelectedVersion = null;
                File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
            else
            {
                MinecraftVersionBorderCloseButton_Click(sender, e);
                GameCoreText.Text = VersionListView.SelectedItem.ToString();
                var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
                setting.SelectedVersion = VersionListView.SelectedItem.ToString();
                File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
            await Task.Run(() =>
            {
                Thread.Sleep(500);
            });
            VersionListView.IsEnabled = true;
        }


        #endregion


        //获取一言(https://hitokoto.cn/)
        public async void hitokoto(object? sender = null, MouseButtonEventArgs? e = null)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string url = "https://v1.hitokoto.cn";
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string data = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<Class.Hitokoto.Root>(data);
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
                        await Dispatcher.BeginInvoke(() => { HitokotoTextBlock.Text = res; });
                    }
                }
            }
            catch (Exception ex)
            {
                await Dispatcher.BeginInvoke(() => { HitokotoTextBlock.Text = ex.Message; });
            }
        }

        private async void LaunchGame_Click(ModernWpf.Controls.SplitButton sender, ModernWpf.Controls.SplitButtonClickEventArgs args)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            LaunchConfig launchConfig = new()
            {
                GameWindowConfig = new GameWindowConfig()
                {
                    IsFullscreen = false
                },
                JvmConfig = new JvmConfig(setting.Java)
                {
                    MaxMemory = (int)setting.MamMem,
                    MinMemory = 256
                },
                LauncherName = "Yu_Minecraft_Launcher",
                IsChinese = true,
                NativesFolder = null,
                IsEnableIndependencyCore = true
            };
            var account = JsonConvert.DeserializeObject<AccountInfo>(File.ReadAllText(Const.YMCLTempAccountDataPath));
            switch (account.AccountType)
            {
                case "离线模式":
                    launchConfig.Account = new MinecraftOAuth.Authenticator.OfflineAuthenticator(account.Name).Auth();
                    break;
            }
            GameCoreUtil gameCoreUtil = new(setting.MinecraftPath); //默认为.minecraft
            JavaMinecraftLauncher javaMinecraftLauncher = new(launchConfig, gameCoreUtil);

            await Task.Run(async () =>
            {
                using var res = await javaMinecraftLauncher.LaunchTaskAsync(setting.SelectedVersion, x =>
                {
                    Debug.WriteLine(x.Item1);
                    Debug.WriteLine(x.Item2);
                });

                if (res.State is LaunchState.Succeess)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, "启动成功", ToastPosition.Top);
                    });
                    Debug.WriteLine("启动成功");
                }
                else
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        MessageBoxX.Show("详细异常信息：\n" + res.Exception, "Yu Minecraft Launcher - 启动失败");
                    });
                    Debug.WriteLine("启动失败");
                    Debug.WriteLine("详细异常信息：{0}", res.Exception);
                }
            });
        }
        public async void LaunchAssignMC(string mcPath, string id)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            LaunchConfig launchConfig = new()
            {
                GameWindowConfig = new GameWindowConfig()
                {
                    IsFullscreen = false
                },
                JvmConfig = new JvmConfig(setting.Java)
                {
                    MaxMemory = (int)setting.MamMem,
                    MinMemory = 256
                },
                LauncherName = "Yu_Minecraft_Launcher",
                NativesFolder = null,
                IsEnableIndependencyCore = true
            };
            var account = JsonConvert.DeserializeObject<AccountInfo>(File.ReadAllText(Const.YMCLTempAccountDataPath));
            switch (account.AccountType)
            {
                case "离线模式":
                    launchConfig.Account = new MinecraftOAuth.Authenticator.OfflineAuthenticator(account.Name).Auth();
                    break;
            }
            GameCoreUtil gameCoreUtil = new(mcPath);
            JavaMinecraftLauncher javaMinecraftLauncher = new(launchConfig, gameCoreUtil);

            await Task.Run(async () =>
            {
                using var res = await javaMinecraftLauncher.LaunchTaskAsync(id, x =>
                {
                    Debug.WriteLine(x.Item1);
                    Debug.WriteLine(x.Item2);
                });

                if (res.State is LaunchState.Succeess)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, "启动成功", ToastPosition.Top);
                    });
                    Debug.WriteLine("启动成功");
                }
                else
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        MessageBoxX.Show("详细异常信息：\n" + res.Exception, "Yu Minecraft Launcher - 启动失败");
                    });
                    Debug.WriteLine("启动失败");
                    Debug.WriteLine("详细异常信息：{0}", res.Exception);
                }
            });
        }

        private async void ImportModPackBtn_Click(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));

            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Title = "选择整合包压缩文件",
                Filter = "整合包压缩文件|*.zip",
                CheckFileExists = true
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileNames.ToString().Trim().Equals("") || openFileDialog.FileNames.Length == 0)
            {
                return;
            }
            string path = openFileDialog.FileName;

            var fileName = System.IO.Path.GetFileName(path);

            DisplayProgressWindow displayProgressWindow = new DisplayProgressWindow();
            displayProgressWindow.Show();

            displayProgressWindow.CurrentStepTextBlock.Text = "安装原版MC";

            await Task.Run(async () =>
            {
                ModsPacksInstaller modsPacksInstaller = new(path, setting.MinecraftPath, Const.ApiKey);
                var info = await modsPacksInstaller.GetModsPacksInfoAsync();
                GameCoreInstaller gameCoreInstaller = new(setting.MinecraftPath, info.Minecraft.Version);
                gameCoreInstaller.ProgressChanged += (_, x) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProBar.Value = x.Progress * 100;
                        displayProgressWindow.TaskProText.Text = Math.Round(x.Progress * 100, 2).ToString() + "%";
                        displayProgressWindow.TaskProgressTextBox.Text += $"{x.ProgressDescription}\n";
                    });
                    Console.WriteLine(x.ProgressDescription);
                };

                var result = await gameCoreInstaller.InstallAsync();

                if (result.Success)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        Toast.Show(Const.Window.main, $"游戏核心 {result.GameCore.Id} 安装成功", ToastPosition.Top);
                    });
                    Console.WriteLine($"游戏核心 {result.GameCore.Id} 安装成功");

                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.CurrentStepTextBlock.Text = "安装整合包";
                    });

                    modsPacksInstaller.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProBar.Value = x.Progress * 100;
                            displayProgressWindow.TaskProText.Text = Math.Round(x.Progress * 100, 2).ToString() + "%";
                            displayProgressWindow.TaskProgressTextBox.Text += $"{x.ProgressDescription}\n";
                        });
                        Console.WriteLine(x.ProgressDescription);
                    };

                    var modsResult = await modsPacksInstaller.InstallAsync();

                    if (modsResult.Success)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(Const.Window.main, $"整合包 {fileName} 安装成功", ToastPosition.Top);
                        });
                        Console.WriteLine($"整合包 {fileName} 安装成功");
                    }
                    else
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"整合包  {fileName}  安装失败", "Yu Minecraft Launcher - 安装整合包");
                        });
                    }

                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.Hide();
                        displayProgressWindow.Close();
                    });
                }
            });

        }

        public async void InstallAssignPack(string url)
        {

            DisplayProgressWindow displayProgressWindow = new DisplayProgressWindow();
            displayProgressWindow.Show();
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            Toast.Show(Const.Window.main, $"开始安装整合包", ToastPosition.Top);
            var path = Const.YMCLTempDataRoot + "\\Pack.zip";
            var fileName = System.IO.Path.GetFileName(path);
            displayProgressWindow.CurrentStepTextBlock.Text = "下载整合包";
            displayProgressWindow.TaskProgressTextBox.Text += $"开始下载整合包\n";
            try
            {
                await Task.Run(async () =>
                {
                    using HttpClient client = new HttpClient();
                    using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var downloadStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                            {
                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                long totalBytesRead = 0;
                                long totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1;

                                while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    totalBytesRead += bytesRead;

                                    if (totalBytes > 0)
                                    {
                                        double progress = ((double)totalBytesRead / totalBytes) * 100;
                                        await Dispatcher.BeginInvoke(() =>
                                        {
                                            displayProgressWindow.TaskProBar.Value = progress;
                                            displayProgressWindow.TaskProText.Text = Math.Round(progress, 2).ToString() + "%";
                                        });
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {
                MessageBoxX.Show($"整合包  {fileName}  安装失败", "Yu Minecraft Launcher - 安装整合包");
                Toast.Show(Const.Window.main, $"整合包下载失败", ToastPosition.Top);
                displayProgressWindow.Hide();
                displayProgressWindow.Close();
                return;
            }

            displayProgressWindow.CurrentStepTextBlock.Text = "安装原版MC";

            await Task.Run(async () =>
            {
                ModsPacksInstaller modsPacksInstaller = new(path, setting.MinecraftPath, Const.ApiKey);
                var info = await modsPacksInstaller.GetModsPacksInfoAsync();
                GameCoreInstaller gameCoreInstaller = new(setting.MinecraftPath, info.Minecraft.Version);
                gameCoreInstaller.ProgressChanged += (_, x) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.TaskProBar.Value = x.Progress * 100;
                        displayProgressWindow.TaskProText.Text = Math.Round(x.Progress * 100, 2).ToString() + "%";
                        displayProgressWindow.TaskProgressTextBox.Text += $"{x.ProgressDescription}\n";
                    });
                    Console.WriteLine(x.ProgressDescription);
                };

                var result = await gameCoreInstaller.InstallAsync();

                if (result.Success)
                {
                    await Dispatcher.BeginInvoke(() =>
                    {
                        //Toast.Show(Const.Window.main, $"游戏核心 {result.GameCore.Id} 安装成功", ToastPosition.Top);
                    });
                    Console.WriteLine($"游戏核心 {result.GameCore.Id} 安装成功");

                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.CurrentStepTextBlock.Text = "安装整合包";
                    });

                    modsPacksInstaller.ProgressChanged += (_, x) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            displayProgressWindow.TaskProBar.Value = x.Progress * 100;
                            displayProgressWindow.TaskProText.Text = Math.Round(x.Progress * 100, 2).ToString() + "%";
                            displayProgressWindow.TaskProgressTextBox.Text += $"{x.ProgressDescription}\n";
                        });
                        Console.WriteLine(x.ProgressDescription);
                    };

                    var modsResult = await modsPacksInstaller.InstallAsync();

                    if (modsResult.Success)
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            Toast.Show(Const.Window.main, $"整合包 {fileName} 安装成功", ToastPosition.Top);
                        });
                        Console.WriteLine($"整合包 {fileName} 安装成功");
                    }
                    else
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            MessageBoxX.Show($"整合包  {fileName}  安装失败", "Yu Minecraft Launcher - 安装整合包");
                        });
                    }

                    await Dispatcher.BeginInvoke(() =>
                    {
                        displayProgressWindow.Hide();
                        displayProgressWindow.Close();
                    });
                }
            });
        }
    }
}
