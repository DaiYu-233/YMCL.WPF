using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using Natsurainko.FluentCore.Extension.Windows.Service;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YMCL.Pages
{
    /// <summary>
    /// Lógica de interacción para SoundsPage.xaml
    /// </summary>
    public partial class SoundsPage : Page
    {
        bool colorMode = true;
        Pages.NotesPage downloadpage = new();
        Pages.JavaDownloadPage javaDownloadPage = new();
        List<string> JavaList = new List<string>();
        //Java

        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryInfo
        {
            public uint Length;
            public uint MemoryLoad;
            public ulong TotalPhysical;//总内存
            public ulong AvailablePhysical;//可用物理内存
            public ulong TotalPageFile;
            public ulong AvailablePageFile;
            public ulong TotalVirtual;
            public ulong AvailableVirtual;
        }
        struct MEMORYSTATUSEX
        {
            public int dwLength;
            public int dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        };
        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref MemoryInfo meminfo);

        [DllImport("kernel32.dll")]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        public SoundsPage()
        {
            

            MemoryInfo MemInfo = new MemoryInfo();
            GlobalMemoryStatus(ref MemInfo);

            double totalMb = MemInfo.TotalPhysical / 1024 / 1024;
            double avaliableMb = MemInfo.AvailablePhysical / 1024 / 1024;

            InitializeComponent();

            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);

            MaxMemSlider.Maximum = status.ullAvailPhys / 1024 / 1024;

            //内存显示
            Memory.Text = "物理内存: " + status.ullTotalPhys / 1024 / 1024  + "MB";
            MemoryRem.Text = "可用内存: " + status.ullAvailPhys / 1024 / 1024 + "MB " + ($"{Math.Round((avaliableMb / totalMb) * 100, 2)}%");
            MemoryUse.Text = "使用内存: " + ((status.ullTotalPhys / 1024 / 1024) - (status.ullAvailPhys / 1024 / 1024)) + "MB";
            JavaList = (List<string>)JavaHelper.SearchJavaRuntime();
            //JavaList.Add(@"C:\Program Files\Java\jre1.8.0_341\bin\javaw.exe");
            JavaListComboSetting.ItemsSource = JavaList;

            //下拉框初始化
            JavaListComboSetting.SelectedItem = JavaListComboSetting.Items[0];
        }

        private void seletJavaPathButton_Click(object sender, RoutedEventArgs e)//Java自定义选择
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "选择Java(javaw.exe)",
                Filter = "Java|javaw.exe",
                CheckFileExists = true
            };
            openFileDialog.ShowDialog();
            string JavaFilePath = openFileDialog.FileName;
            
            JavaList.Add(JavaFilePath);
            JavaListComboSetting.ItemsSource = JavaList;
            JavaListComboSetting.SelectedItem = JavaFilePath;
        }

        private void MemoryRefresh_Click(object sender, RoutedEventArgs e)
        {
            MemoryInfo MemInfo = new MemoryInfo();
            GlobalMemoryStatus(ref MemInfo);

            double totalMb = MemInfo.TotalPhysical / 1024 / 1024;
            double avaliableMb = MemInfo.AvailablePhysical / 1024 / 1024;

            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);
            //内存显示

            Memory.Text = "物理内存: " + status.ullTotalPhys / 1024 / 1024 + "MB";
            MemoryRem.Text = "可用内存: " + status.ullAvailPhys / 1024 / 1024 + "MB " + ($"{Math.Round((avaliableMb / totalMb) * 100, 2)}%");
            MemoryUse.Text = "使用内存: " + ((status.ullTotalPhys / 1024 / 1024) - (status.ullAvailPhys / 1024 / 1024)) + "MB";

        }


        private void MaxMemSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);
            MaxMemTextBox.Text = MaxMemSlider.Value.ToString();
            MaxMemSlider.Maximum = status.ullAvailPhys / 1024 / 1024;
            GameMemUse.Text = ((Convert.ToInt32(MaxMemTextBox.Text)/ Convert.ToInt32((status.ullAvailPhys / 1024 / 1024)))*100).ToString();
            //MemUsePercent.Text = MaxMemSlider.Value.ToString() + "MB / " + status.ullTotalPhys / 1024 / 1024 + "MB";
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            javaDownloadPage.Show();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (colorMode == true)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Themes/Dark.xaml") });
                colorMode = false;
            }
            else
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/YMCL;component/Themes/Light.xaml") });
                colorMode = true;
            }
        }
    }
}
