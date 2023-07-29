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
using System.Net;
using System.IO;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Models.Auth;
using MinecaftOAuth;
using MinecraftLaunch.Modules.Toolkits;
using Newtonsoft.Json;
using YMCL.Pages.SettingPages;
using System.Diagnostics;
using YMCL.Class;
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


            if (!Directory.Exists(obj.MinecraftPath))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(obj.MinecraftPath);
                directoryInfo.Create();
            }


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
                //MessageBoxX.Show("请选择游戏核心");
                Toast.Show("请选择游戏核心", ToastPosition.Top);
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


        public void GetVers()
        {
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            var Versionpath = obj.MinecraftPath + "\\versions";
            if (!Directory.Exists(Versionpath))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Versionpath);
                directoryInfo.Create();
            }
            DirectoryInfo dir = new DirectoryInfo(Versionpath);
            DirectoryInfo[] dii = dir.GetDirectories();  //获取.minecraft\versions的所有子目录
            foreach (DirectoryInfo VersionDir in dii)
            {
                var Index = VersionDir.FullName.Split(@"\");
                var VersionName = Index[Index.Length - 1];
                if (File.Exists(Versionpath + @"\" + VersionName + @"\" + VersionName + ".json"))  //检查是否符合mc版本
                {
                    VerListView.Items.Add(VersionName);
                }
            }
        }





        private void GameVerTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
            GetVers();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
        }

        private void VerListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
            GameVerTextBlock.Text = VerListView.SelectedItem.ToString();
        }

        private void Page_MouseEnter(object sender, MouseEventArgs e)
        {
            UpdateLogin();
        }


        #endregion


    }
}
