using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using Natsurainko.FluentCore.Extension.Windows.Service;
using Newtonsoft.Json;
using Panuon.UI.Silver;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        /// <summary>
        /// 数据保存
        /// </summary>
        #region
        string ConfigPath = @"YMCL.config.json";
        LauncherSetting setting = new LauncherSetting();//链接设置
        public class LauncherSetting
        {
            public string Ram = "1024";
        }

        //设置初始化
        public void LauncherSettingInitialization()
        {
            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(setting));
            }
            else
            {
                setting = JsonConvert.DeserializeObject<LauncherSetting>(File.ReadAllText(ConfigPath));
                MaxMemTextBox.Text = setting.Ram;
            }
        }




        #endregion

        bool colorMode = true;
        Pages.NotesPage downloadpage = new();
        Pages.JavaDownloadPage javaDownloadPage = new();
        List<string> JavaList = new List<string>();

        /// <summary>
        /// 内存检测
        #region
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
        #endregion
        public SoundsPage()
        {
            InitializeComponent();
            LauncherSettingInitialization();
            //内存
            #region
            MemoryInfo MemInfo = new MemoryInfo();
            GlobalMemoryStatus(ref MemInfo);

            double totalMb = MemInfo.TotalPhysical / 1024 / 1024;
            double avaliableMb = MemInfo.AvailablePhysical / 1024 / 1024;

            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);

            MaxMemSlider.Maximum = status.ullAvailPhys / 1024 / 1024;

            //内存显示
            Memory.Text = "物理内存: " + status.ullTotalPhys / 1024 / 1024 + "MB";
            MemoryRem.Text = "可用内存: " + status.ullAvailPhys / 1024 / 1024 + "MB " + ($"{Math.Round((avaliableMb / totalMb) * 100, 2)}%");
            MemoryUse.Text = "使用内存: " + ((status.ullTotalPhys / 1024 / 1024) - (status.ullAvailPhys / 1024 / 1024)) + "MB";
            #endregion

            //初始化
            #region

            //寻找Java
            JavaList = (List<string>)JavaHelper.SearchJavaRuntime();
            JavaListComboSetting.ItemsSource = JavaList;

            //Java下拉框初始化
            JavaListComboSetting.SelectedItem = JavaListComboSetting.Items[0];


            #endregion

            //设置
            #region

            //设置读取


            MaxMemTextBox.Text = setting.Ram;

            #endregion



            


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
            setting.Ram = MaxMemTextBox.Text;
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(setting));
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

        private void github_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/DaiYu-233/YMCL");
        }

        private void gw_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ymcl.daiyu-233.top");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Update.exe");
            }
            catch
            {
                MessageBoxX.Show("启动更新程序失败\n请尝试使用管理员身份运行YMCL", "检查更新失败!");
            }
            
        }
    }
}
