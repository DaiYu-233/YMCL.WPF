using Newtonsoft.Json;
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
using Natsurainko;
using Natsurainko.FluentCore.Extension.Windows.Service;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Panuon.WPF.UI;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Page
    {
        public List<SettingInfo> settingInfos = new List<SettingInfo>();
        List<string> JavaList = new List<string>();

        struct MEMORYSTATUSEX{
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
        [DllImport("kernel32.dll")]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX LpBuffer);
        public Main()
        {
            
            InitializeComponent();
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            
            JavaList = (List<string>)JavaHelper.SearchJavaRuntime();
            foreach (var item in JavaList)
            {
                JavaCombo.Items.Add(item);
            }
            if (System.IO.File.ReadAllText(@".\YMCL\logs\setting\java.log") != "" || !System.IO.File.Exists(@".\YMCL\logs\setting\java.log"))
            {
                if (JavaCombo.Items.Count >= 1)
                {
                    JavaCombo.SelectedItem = JavaCombo.Items[0];
                }
            }

            UpdateMem();
            SettingInitialization();


        }

        private void SettingInitialization()
        {
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            TestFolder("./YMCL/logs/setting/save");
            try
            {
                JavaCombo.SelectedItem = System.IO.File.ReadAllText(@".\YMCL\logs\setting\save\java.log");
                SilderBox.Value = Convert.ToDouble(System.IO.File.ReadAllText(@".\YMCL\logs\setting\save\mem.log"));
            }
            catch { }
                

            
        }







        private void UpdateMem()
        {
            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);
            MexMemTextBl.Text = status.ullTotalPhys / 1024 / 1024 + "M";
            SurMemTextBl.Text = status.ullAvailPhys / 1024 / 1024 + "M";
            UsedMemTextBl.Text = (status.ullTotalPhys-status.ullAvailPhys) * 100 / status.ullTotalPhys + "%";
            SilderBox.Maximum = status.ullTotalPhys / 1024 / 1024;
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



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JavaCombo.Items.Clear();
            JavaList = (List<string>)JavaHelper.SearchJavaRuntime();
            foreach (var item in JavaList)
            {
                JavaCombo.Items.Add(item);
            }
            Toast.Show("扫描成功,已发现" + JavaCombo.Items.Count.ToString() + "个Java", ToastPosition.Top);
            //Toast.Show("扫描成功,已发现"+ JavaCombo.Items.Count.ToString() + "个Java", new ToastOptions { Icon = ToastIcons.Information, ToastMargin = new Thickness(10), Time = 5000, Location = ToastLocation.OwnerTopCenter });
            if (JavaCombo.Items.Count >= 1)
            {
                JavaCombo.SelectedItem = JavaCombo.Items[0];
            }
        }

        private void JavaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            System.IO.File.WriteAllText(@".\YMCL\logs\setting\java.log", (string?)JavaCombo.SelectedItem);
        }

        private void AddCustomJavaBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "选择Java(javaw.exe)",
                Filter = "Java|javaw.exe",
                CheckFileExists = true
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileNames.ToString().Trim().Equals("") || openFileDialog.FileNames.Length == 0)
            {
                return;
            }
            
            string JavaFilePath = openFileDialog.FileName;

            JavaCombo.Items.Add(JavaFilePath);
            JavaCombo.SelectedItem = JavaFilePath;
        }



        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            //Toast.Show("点击任意处可刷新内存信息", new ToastOptions { Icon = ToastIcons.Warning, ToastMargin = new Thickness(10), Time = 5000, Location = ToastLocation.OwnerTopCenter });
            UpdateMem();
        }

        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateMem();
        }

        private void Silder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value,0);
            SilderInfo.Text = SilderBox.Value.ToString() + "M";
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            System.IO.File.WriteAllText(@".\YMCL\logs\setting\mem.log", SilderBox.Value.ToString());
        }

        private void SaveSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            TestFolder("./YMCL");
            TestFolder("./YMCL/logs");
            TestFolder("./YMCL/logs/setting");
            TestFolder("./YMCL/logs/setting/save");
            System.IO.File.WriteAllText(@".\YMCL\logs\setting\save\java.log", (string?)JavaCombo.SelectedItem);
            System.IO.File.WriteAllText(@".\YMCL\logs\setting\save\mem.log", SilderBox.Value.ToString());
            Panuon.WPF.UI.Toast.Show("已保存设置", ToastPosition.Top);

            //Toast.Show("已保存设置", new ToastOptions { Icon = ToastIcons.Information, ToastMargin = new Thickness(10), Time = 5000, Location = ToastLocation.OwnerTopCenter });

        }
    }
}

