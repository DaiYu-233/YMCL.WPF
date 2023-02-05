using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
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
using KMCCC.Authentication;
using Panuon.UI.Silver;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using Natsurainko.FluentCore.Extension.Windows.Service;
using MinecaftOAuth;
using System.Data;
//using Microsoft.Win32;

namespace YMCL.Pages
{

    public partial class HomePage : Page
    {
        LaunchOptions launchoptions = new LaunchOptions(); //判断启动模式
        public static LaunchConfig launchConfig { get; } = new LaunchConfig();
        public static Account UserInfo { get; private set; }

        Pages.SoundsPage settingpage = new Pages.SoundsPage();
        //Pages.VersionSelection versionSelection = new Pages.VersionSelection();
        LoginUI.offline offline = new LoginUI.offline();
        Pages.VersionSelection versionselectionpage = new Pages.VersionSelection();
        LoginUI.Microsoft microsoft = new LoginUI.Microsoft();
        LoginUI.external external = new LoginUI.external();
        LoginUI.Mojang mojang = new LoginUI.Mojang();
        public int launchMode;
        
        public static LauncherCore Core = LauncherCore.Create();
        
        public HomePage()
        {
            InitializeComponent();
            var versions = Core.GetVersions().ToArray();
            VersionListbox.ItemsSource = Core.GetVersions();

            launchConfig.JvmConfig = new();
            launchMode = 1;
        }


        public void GameLaunch()
        {
            
            switch (launchMode)
            {
                case 1://离线
                    launchoptions.Authenticator = new KMCCC.Authentication.OfflineAuthenticator(offline.PlayerId.Text);
                    KMCCCLaunch();
                    break;
                case 2://mojang
                    launchoptions.Authenticator = new YggdrasilLogin(mojang.mojangacount.Text, mojang.mojangpassword.Text, false);
                    KMCCCLaunch();
                    break;
                case 3://Microsoft
                    MicrosoftLaunch();
                    break;

            }
        }
        public async void MicrosoftLaunch()
        {
            launchConfig.Account = UserInfo;//这是非刷新验证方式
            launchConfig.JvmConfig.JavaPath = new(settingpage.JavaListComboSetting.Text);
            JavaClientLauncher javaClientLauncher = new(launchConfig, new GameCoreToolkit(".minecraft"));
            using var res = await javaClientLauncher.LaunchTaskAsync(VersionListbox.SelectedItem.ToString());
            if (res.State is MinecraftLaunch.Modules.Enum.LaunchState.Succeess)
            {
                MessageBoxX.Show("正在等待游戏窗口出现", "启动成功");
                await Task.Run(res.Process.WaitForInputIdle);
            }
            else
            {
                MessageBoxX.Show($"异常信息：{res.Exception}", res.State.ToString());
            }
        }

        public void KMCCCLaunch()
        {
            // 离线,mojang启动 (KMCCC)
            launchoptions.MaxMemory = Convert.ToInt32(settingpage.MaxMemTextBox.Text) * 1024;
            //launchoptions.MaxMemory = 2048;
            Core.JavaPath = settingpage.JavaListComboSetting.Text;
            if (VersionListbox.SelectedItem.ToString() != string.Empty & launchMode == 1 || launchMode == 2)
            {
                var ver = (KMCCC.Launcher.Version)VersionListbox.SelectedItem;
                launchoptions.Version = ver;
                var result = Core.Launch(launchoptions);
                if (!result.Success)
                {
                    switch (result.ErrorType)
                    {
                        case ErrorType.NoJAVA:
                            MessageBoxX.Show("Java错误\n详细信息: " + result.ErrorMessage, "错误!");
                            break;
                        case ErrorType.AuthenticationFailed:
                            MessageBoxX.Show("登录错误\n详细信息: " + result.ErrorMessage, "错误!");
                            break;
                        case ErrorType.UncompressingFailed:
                            MessageBoxX.Show("文件错误\n详细信息: " + result.ErrorMessage, "错误!");
                            break;
                        default:
                            MessageBoxX.Show("YMCL也不知道哪里出了问题awa\n详细信息: " + result.ErrorMessage, "错误!");
                            break;
                    }
                }
            }
            else
            {
                MessageBoxX.Show("请选择游戏版本", "错误!");
            }
        }









        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ymcl.daiyu-233.top");
        }


        private void offlineButton_Click(object sender, RoutedEventArgs e) //离线
        {
            LoginUIFrame.Content = offline;
            launchMode = 1;
            microsoftLoginButton.Visibility = Visibility.Hidden;
        }

        private void microsoftButton_Click(object sender, RoutedEventArgs e) //微软
        {
            LoginUIFrame.Content = microsoft;
            launchMode = 3;
            microsoftLoginButton.Visibility = Visibility.Visible;
        }

        private void mojangButton_Click(object sender, RoutedEventArgs e) //mojang
        {
            LoginUIFrame.Content = mojang;
            launchMode = 2;
            microsoftLoginButton.Visibility = Visibility.Hidden;
        }

        private void exteralButton_Click(object sender, RoutedEventArgs e) //外置
        {
            LoginUIFrame.Content = external;
            microsoftLoginButton.Visibility = Visibility.Hidden;
            launchMode = 4;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }



        public async Task McrosoftLoginAsync()
        {
            var V = MessageBoxX.Show("确定开始验证您的账户", "验证", MessageBoxButton.OKCancel);
            MicrosoftAuthenticator microsoftAuthenticator = new(MinecaftOAuth.Module.Enum.AuthType.Access)
            {
                ClientId = "ed0e15b9-fa1e-489b-b83d-7a66ff149abd"
            };
            var code = await microsoftAuthenticator.GetDeviceInfo();
            MessageBoxX.Show(code.UserCode, "你的一次性访问代码 确定开始验证账户");
            Debug.WriteLine("Link:{0} - Code:{1}", code.VerificationUrl, code.UserCode);
            if (V == MessageBoxResult.OK)
            {
                Process.Start(new ProcessStartInfo(code.VerificationUrl)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            var token = await microsoftAuthenticator.GetTokenResponse(code);
            var user = await microsoftAuthenticator.AuthAsync(x =>
            {
                Debug.WriteLine(x);
            });
            UserInfo = user;
            microsoft.MicrosoftPlayerName.Text = user.Name;
            microsoft.MicrosoftPlayerUUID.Text = user.Uuid.ToString();
        }

        private void microsoftLoginButton_Click(object sender, RoutedEventArgs e)
        {
            _ = McrosoftLoginAsync();
        }



        private void KMCCClaunch_Click(object sender, RoutedEventArgs e)
        {
            GameLaunch();
        }

        private void VersionSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            VersionListGrid.Visibility = Visibility.Visible;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VersionListGrid.Visibility = Visibility.Hidden;
        }

        private void VersionListBoxRefresh_Click(object sender, RoutedEventArgs e)
        {
            VersionListbox.ItemsSource = Core.GetVersions();
        }

        private void VersionListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VersionNameButtonTextBlock.Text = VersionListbox.SelectedItem.ToString();
        }
    }
}
