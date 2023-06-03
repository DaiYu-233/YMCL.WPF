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


namespace YMCL.Pages
{
    /// <summary>
    /// LaunchPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchPage : Page
    {

        public static KMCCC.Launcher.Version[] versions;
        public static LauncherCore Core = LauncherCore.Create();
        public LaunchPage()
        {
            InitializeComponent();
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
            
            LoginNameText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginName.log");
            LoginTypeText.Text = System.IO.File.ReadAllText("./YMCL/logs/setting/save/LoginType.log");
            KMCCC.Launcher.Version[] versions = Core.GetVersions().ToArray();
            VerListView.ItemsSource = versions;


        }
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
            
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {

        }






        private void GameVerTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VerListBorder.Visibility = Visibility.Visible;
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



    }
}
