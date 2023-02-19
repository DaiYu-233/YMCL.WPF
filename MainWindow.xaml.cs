using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YMCL.LoginUI;
using Panuon.UI.Silver;
using YMCL.Pages;
using System.IO;

namespace YMCL
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LoginUI.offline offLine = new LoginUI.offline();
        Pages.HomePage homePage = new Pages.HomePage();
        Frame Launch = new Frame() { Content = new Pages.HomePage() };
        Frame Download = new Frame() { Content = new Pages.NotesPage() };
        Frame Setting = new Frame() { Content = new Pages.SoundsPage() };
        Frame More = new Frame() { Content = new Pages.PaymentPage() };

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            InitializeComponent();


            InstallFont();

        }

        /// <summary>
        /// 通过文件获取字体
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns>字体</returns>
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                     string lpFileName);
        public void InstallFont()
        {
            // Try install the font.
            AddFontResource("NotoSansMonoCJKsc-VF.ttf");
            AddFontResource("NotoSansCJKsc-VF");
            AddFontResource("NotoSansSC-VF.ttf");
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void rdHome_Click(object sender, RoutedEventArgs e)
        {
            // PagesNavigation.Navigate(new HomePage());

            //PagesNavigation.Navigate(new System.Uri("Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
            PagesNavigation.Content = Launch;
        }

        private void rdSounds_Click(object sender, RoutedEventArgs e)
        {
            //PagesNavigation.Navigate(new System.Uri("Pages/SettingPage.xaml", UriKind.RelativeOrAbsolute));
            PagesNavigation.Content = Setting;
        }

        private void rdNotes_Click(object sender, RoutedEventArgs e)
        {
            //PagesNavigation.Navigate(new System.Uri("Pages/DownloadPage.xaml", UriKind.RelativeOrAbsolute));
            PagesNavigation.Content = Download;
        }

        private void rdPayment_Click(object sender, RoutedEventArgs e)
        {
            //PagesNavigation.Navigate(new System.Uri("Pages/MorePage.xaml", UriKind.RelativeOrAbsolute));
            PagesNavigation.Content = More;
        }

        private void rdHome_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void home_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            //homePage.LoginUIFrame.Content = offLine;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            //PagesNavigation.Navigate(new System.Uri("Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
            PagesNavigation.Content = Launch;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            home.DragMove();
        }
    }
}
