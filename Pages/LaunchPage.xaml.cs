using System;
using System.Collections.Generic;
using System.Linq;
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
using KMCCC.Launcher;
using System.Net;
using System.IO;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Models.Auth;
using KMCCC.Authentication;
using MinecaftOAuth;
using MinecraftLaunch.Modules.Toolkits;
using Newtonsoft.Json;
using YMCL.Pages.SettingPages;
using System.Diagnostics;
using Panuon.WPF.UI;

namespace YMCL.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page
    {
        public static LaunchConfig launchConfig { get; } = new LaunchConfig();
        public MinecraftLaunch.Modules.Models.Auth.Account UserInfo { get; private set; }

        public static KMCCC.Launcher.Version[] versions;
        public static LauncherCore Core = LauncherCore.Create();
        public LaunchPage()
        {
            InitializeComponent();
            #region
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            TestFolder("./YMCL/logs/setting/save");
            if (System.IO.File.Exists("./YMCL/logs/setting/save/LoginName.log"))
            {

            }
            else
            {
                System.IO.File.WriteAllText(@".\YMCL\logs\setting\save\LoginName.log", "Steve");
                LoginNameText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginName.log");
            }
            if (System.IO.File.Exists("./YMCL/logs/LoginType.log"))
            {

            }
            else
            {
                System.IO.File.WriteAllText(@"./YMCL/logs/setting/save/LoginType.log", "离线登录");
                LoginTypeText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginType.log");
            }
            try
            {
                JavaText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/java.log");
            }
            catch { }

            TestFolder("./YMCL");
            if (File.Exists("./YMCL/DisplayInformation.txt"))
            {
                if (File.ReadAllText("./YMCL/DisplayInformation.txt") == "false")
                {
                    LoginBr.Visibility = Visibility.Hidden;
                    JavaBr.Visibility = Visibility.Hidden;
                }

            }
            else
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
            LoginNameText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginName.log");
            LoginTypeText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginType.log");
            #endregion


            KMCCC.Launcher.Version[] versions = Core.GetVersions().ToArray();
            VerListView.ItemsSource = versions;
            
        }

        private async void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            if (VerListView.SelectedIndex >= 0)
            {
                LaunchGame.IsEnabled = false;
                if (File.ReadAllText("./YMCL/logs/LoginType.log") == "离线登录")
                {
                    MinecaftOAuth.OfflineAuthenticator authenticator = new(File.ReadAllText("./YMCL/logs/LoginName.log"));
                    launchConfig.Account = authenticator.Auth();
                    launchConfig.JvmConfig = File.ReadAllText("./YMCL/logs/setting/java.log");
                    launchConfig.JvmConfig.MaxMemory = Convert.ToInt32(File.ReadAllText("./YMCL/logs/setting/mem.log"));
                    JavaMinecraftLauncher javaMinecraftLauncher = new JavaMinecraftLauncher(launchConfig, new GameCoreToolkit(".minecraft"),true);
                    using var res = await javaMinecraftLauncher.LaunchTaskAsync(VerListView.SelectedValue.ToString());
                    if (res.State is MinecraftLaunch.Modules.Enum.LaunchState.Succeess)
                    {
                        Panuon.WPF.UI.Toast.Show("启动成功,等待游戏窗口出现", ToastPosition.Top);
                        NoticeBox.Show("启动成功,等待游戏窗口出现", "提示", MessageBoxIcon.Success);
                        //Toast.Show("启动成功,等待游戏窗口出现", new ToastOptions { Icon = ToastIcons.Information, ToastMargin = new Thickness(10), Time = 5000, Location = ToastLocation.OwnerTopCenter });
                        LaunchGame.IsEnabled = true;
                        await Task.Run(res.WaitForExit);
                        MessageBoxX.Show("游戏已退出", "游戏已退出");

                    }
                    else
                    {
                        NoticeBox.Show("启动错误", "提示", MessageBoxIcon.Error);
                        MessageBoxX.Show("错误信息:\n" + res.State.ToString(), "启动错误");
                        
                        LaunchGame.IsEnabled = true;
                    }
                }
                else if (File.ReadAllText("./YMCL/logs/LoginType.log") == "微软登录")
                {
                    UserInfo = JsonConvert.DeserializeObject<MinecraftLaunch.Modules.Models.Auth.Account>(File.ReadAllText("./YMCL/Accounts/Microsoft-" + File.ReadAllText("./YMCL/logs/LoginName.log") + ".json"));
                    if (UserInfo == null) { MessageBoxX.Show("请尝试删除此账户并重新登录","登录失败"); return; }
                    launchConfig.Account = UserInfo;
                    launchConfig.JvmConfig = File.ReadAllText("./YMCL/logs/setting/java.log");
                    launchConfig.JvmConfig.MaxMemory = Convert.ToInt32(File.ReadAllText("./YMCL/logs/setting/mem.log"));
                    
                    JavaMinecraftLauncher javaMinecraftLauncher = new JavaMinecraftLauncher(launchConfig,new GameCoreToolkit(".minecraft"),true);
                    using var res = await javaMinecraftLauncher.LaunchTaskAsync(VerListView.SelectedValue.ToString());
                    if (res.State is MinecraftLaunch.Modules.Enum.LaunchState.Succeess) 
                    {
                        Panuon.WPF.UI.Toast.Show("启动成功,等待游戏窗口出现", ToastPosition.Top);
                        NoticeBox.Show("启动成功,等待游戏窗口出现", "提示", MessageBoxIcon.Success);
                        //Toast.Show("启动成功,等待游戏窗口出现", new ToastOptions { Icon = ToastIcons.Information, ToastMargin = new Thickness(10), Time = 5000, Location = ToastLocation.OwnerTopCenter });
                        LaunchGame.IsEnabled = true;
                        await Task.Run(res.WaitForExit);
                        MessageBoxX.Show("游戏已退出", "游戏已退出");
                    }
                    else
                    {
                        NoticeBox.Show("启动错误", "提示", MessageBoxIcon.Error);
                        MessageBoxX.Show("错误信息:\n" + res.State.ToString(), "启动错误");
                        
                        LaunchGame.IsEnabled = true;
                    }
                }
            }
            else
            {
                MessageBoxX.Show("请选择游戏核心");
            }

        }






        #region

        private void TestFolder(string Folder)
        {
            if (System.IO.Directory.Exists(Folder)) { }
            else
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Folder);
                directoryInfo.Create();
            }
        }
        public void UpdateLogin()
        {
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            TestFolder("./YMCL/logs/setting/save");
            try {
                LoginNameText.Text = System.IO.File.ReadAllText("./YMCL/logs/LoginName.log");
                LoginTypeText.Text = System.IO.File.ReadAllText("./YMCL/logs/LoginType.log");
                JavaText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/java.log");
            }
            catch { }


            if (File.Exists("./YMCL/DisplayInformation.txt"))
            {
                if (File.ReadAllText("./YMCL/DisplayInformation.txt") == "false")
                {
                    LoginBr.Visibility = Visibility.Hidden;
                    JavaBr.Visibility = Visibility.Hidden;
                }
                else
                {
                    LoginBr.Visibility = Visibility.Visible;
                    JavaBr.Visibility = Visibility.Visible;
                }

            }
            else
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
        }








        private void GameVerTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
            KMCCC.Launcher.Version[] versions = Core.GetVersions().ToArray();
            VerListView.ItemsSource = versions;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
            KMCCC.Launcher.Version[] versions = Core.GetVersions().ToArray();
            VerListView.ItemsSource = versions;
        }

        private void VerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
            GameVerTextBlock.Text = VerListView.SelectedValue.ToString();
        }

        private void Page_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateLogin();
        }


        #endregion


    }
}
