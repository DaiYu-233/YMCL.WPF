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

namespace YMCL.Pages
{
    /// <summary>
    /// Lógica de interacción para PaymentPage.xaml
    /// </summary>
    public partial class PaymentPage : Page
    {
        Tools.DownloaderPage downloaderPage = new();
        public PaymentPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://daiyu-233.top");
        }

        private void GithubBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/daiyu-233/YMCL");
        }

        private void rdHome_Checked(object sender, RoutedEventArgs e)
        {
            
        // powershell script execution
        var processInfo = new System.Diagnostics.ProcessStartInfo
        {
            Verb = "runas",
            LoadUserProfile = true,
            FileName = "powershell.exe",
            Arguments = "C:\\YMCL\\shuzijihuo.ps1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

            var p = System.Diagnostics.Process.Start(processInfo);
        }

        private void rdDownloader_Checked(object sender, RoutedEventArgs e)
        {
            //ToolsFarme.Content = downloaderPage;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //ToolsFarme.Content = downloaderPage;

        }
    }
}
