using MinecraftLaunch.Modules.Installer;
using Panuon.UI.Silver;
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
using System.Windows.Shapes;

namespace YMCL.Pages
{
    /// <summary>
    /// JavaDownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class JavaDownloadPage : Window
    {
        public JavaDownloadPage()
        {
            InitializeComponent();
        }

        public void aa()
        {
            jdk11btn.Content = "安装中...";
            jdk17btn.Content = "安装中...";
            jdk18btn.Content = "安装中...";
            jdk8btn.Content = "安装中...";
            jdk11btn.IsEnabled = false;
            jdk17btn.IsEnabled = false;
            jdk18btn.IsEnabled = false;
            jdk8btn.IsEnabled = false;
        }

        public void bb()
        {
            jdk11btn.Content = "OpenJDK11";
            jdk17btn.Content = "OpenJDK17";
            jdk18btn.Content = "OpenJDK18";
            jdk8btn.Content = "OpenJDK8";
            jdk11btn.IsEnabled = true;
            jdk17btn.IsEnabled = true;
            jdk18btn.IsEnabled = true;
            jdk8btn.IsEnabled = true;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            aa();
            JavaInstaller javaInstaller = new(
            MinecraftLaunch.Modules.Enum.JdkDownloadSource.JdkJavaNet,
            MinecraftLaunch.Modules.Enum.OpenJdkType.OpenJdk8, ewd.Text);
            var javares = await javaInstaller.InstallAsync(x =>
            {
                qwq.Text = (x.Item1 * 100).ToString() + "%";
            });
            if (javares.Success)
            {
                qwq.Text = "0%";
                MessageBoxX.Show("安装成功");
                bb();
            }
            else
            {
                bb();
                qwq.Text = "0%";
                MessageBoxX.Show("安装失败");
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void jdk11btn_Click(object sender, RoutedEventArgs e)
        {
            aa();
            JavaInstaller javaInstaller = new(
            MinecraftLaunch.Modules.Enum.JdkDownloadSource.JdkJavaNet,
            MinecraftLaunch.Modules.Enum.OpenJdkType.OpenJdk11, ewd.Text);
            var javares = await javaInstaller.InstallAsync(x =>
            {
                qwq.Text = (x.Item1 * 100).ToString() + "%";
            });
            if (javares.Success)
            {
                qwq.Text = "0%";
                MessageBoxX.Show("安装成功");
                bb();
            }
            else
            {
                bb();
                qwq.Text = "0%";
                MessageBoxX.Show("安装失败");
            }
        }

        private async void jdk17btn_Click(object sender, RoutedEventArgs e)
        {
            aa();
            JavaInstaller javaInstaller = new(
            MinecraftLaunch.Modules.Enum.JdkDownloadSource.JdkJavaNet,
            MinecraftLaunch.Modules.Enum.OpenJdkType.OpenJdk17, ewd.Text);
            var javares = await javaInstaller.InstallAsync(x =>
            {
                qwq.Text = (x.Item1 * 100).ToString() + "%";
            });
            if (javares.Success)
            {
                qwq.Text = "0%";
                MessageBoxX.Show("安装成功");
                bb();
            }
            else
            {
                bb();
                qwq.Text = "0%";
                MessageBoxX.Show("安装失败");
            }
        }

        private async void jdk18btn_Click(object sender, RoutedEventArgs e)
        {
            aa();
            JavaInstaller javaInstaller = new(
            MinecraftLaunch.Modules.Enum.JdkDownloadSource.JdkJavaNet,
            MinecraftLaunch.Modules.Enum.OpenJdkType.OpenJdk18, ewd.Text);
            var javares = await javaInstaller.InstallAsync(x =>
            {
                qwq.Text = (x.Item1 * 100).ToString() + "%";
            });
            if (javares.Success)
            {
                qwq.Text = "0%";
                MessageBoxX.Show("安装成功");
                bb();
            }
            else
            {
                bb();
                qwq.Text = "0%";
                MessageBoxX.Show("安装失败");
            }
        }
    }
}
