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
using Panuon.UI.Silver;
using YMCL.Class;

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
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));

            #region


                LoginNameText.Text = obj.LoginName;


                LoginTypeText.Text = obj.LoginType;

                JavaText.Text = obj.Java;



                if (obj.DisplayInformation == "False")
                {
                    LoginBr.Visibility = Visibility.Hidden;
                    JavaBr.Visibility = Visibility.Hidden;
                }


            #endregion


            KMCCC.Launcher.Version[] versions = Core.GetVersions().ToArray();
            VerListView.ItemsSource = versions;
            
        }

        private async void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            if (VerListView.SelectedIndex >= 0)
            {
                LaunchGame.IsEnabled = false;
                if (File.ReadAllText("./YMCL/Temp/LoginType.log") == "离线登录")
                {
                }
                else if (File.ReadAllText("./YMCL/Temp/LoginType.log") == "微软登录")
                {
                }
            }
            else
            {
                MessageBoxX.Show("请选择游戏核心");
            }

        }






        #region


        public void UpdateLogin()
        {
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));

            try {
                LoginNameText.Text = System.IO.File.ReadAllText("./YMCL/Temp/LoginName.log");
                LoginTypeText.Text = System.IO.File.ReadAllText("./YMCL/Temp/LoginType.log");
                JavaText.Text = System.IO.File.ReadAllText("./YMCL/Temp/Java.log");
            }
            catch { }


                if (obj.DisplayInformation == "False")
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
