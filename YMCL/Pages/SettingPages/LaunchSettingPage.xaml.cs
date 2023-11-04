using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
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

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// LaunchSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchSettingPage : Page
    {
        public LaunchSettingPage()
        {
            var value = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath)).MamMem;
            InitializeComponent();
            LoadJavas();
            LoadMem(value);
        }


        #region Java
        List<string> javas = new();


        void LoadJavas()
        {
            JavaPathComboBox.Items.Clear();
            javas = JsonConvert.DeserializeObject<List<string>>
                (File.ReadAllText(Const.YMCLJavaDataPath));
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            foreach (var item in javas)
            {
                JavaPathComboBox.Items.Add(item);
            }
            JavaPathComboBox.SelectedItem = setting.Java;
        }
        private void AddJava_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
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

            string path = openFileDialog.FileName;

            var isIncludePath = false;
            var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.YMCLJavaDataPath));
            if (dataArray != null)
            {
                foreach (var item in dataArray)
                {
                    if (item == path)
                    {
                        isIncludePath = true;
                    }
                }
                if (isIncludePath)
                {
                    Toast.Show(Const.Window.main, "已存在此Java", ToastPosition.Top);
                    JavaPathComboBox.SelectedItem = path;
                }
                else
                {
                    javas.Add(path);
                    File.WriteAllText(Const.YMCLJavaDataPath, JsonConvert.SerializeObject(javas, Formatting.Indented));
                    JavaPathComboBox.Items.Add(path);
                    JavaPathComboBox.SelectedItem = path;
                }
            }
        }

        private void AutoScanJava_Click(object sender, RoutedEventArgs e)
        {
            var backItem = string.Empty;
            var includeItem = 0;
            foreach (var java in MinecraftLaunch.Modules.Utilities.JavaUtil.GetJavas())
            {
                var isInclude = false;


                if (java.JavaPath == backItem)
                {
                    continue;
                }
                else
                {
                    backItem = java.JavaPath;
                }

                foreach (var item in javas)
                {
                    if (item == java.JavaPath)
                    {
                        isInclude = true;
                        continue;
                    }

                }
                if (!isInclude)
                {
                    javas.Add(java.JavaPath);
                    JavaPathComboBox.Items.Add(java.JavaPath);
                }
                else
                {
                    includeItem++;
                }
            };
            if (javas.Count > 0) { JavaPathComboBox.SelectedIndex = 0; }
            Toast.Show(Const.Window.main, $"扫描完成：发现{javas.Count}个Java，其中已有{includeItem}个存在列表中", ToastPosition.Top);
            File.WriteAllText(Const.YMCLJavaDataPath, JsonConvert.SerializeObject(javas, Formatting.Indented));
        }

        private void JavaPathComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = JavaPathComboBox.SelectedItem as string;
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.Java = item;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        #endregion
        #region MaxMem
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
        [DllImport("kernel32.dll")]
        static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX LpBuffer);

        void LoadMem(double value)
        {
            MEMORYSTATUSEX status = new MEMORYSTATUSEX();
            status.dwLength = 0x40;
            GlobalMemoryStatusEx(ref status);
            SilderBox.Maximum = status.ullTotalPhys / 1024 / 1024;
            SilderBox.Value = value;
        }
        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value);
            SilderInfo.Text = SilderBox.Value.ToString() + "M";
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.MamMem = SilderBox.Value;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        #endregion

    }
}
