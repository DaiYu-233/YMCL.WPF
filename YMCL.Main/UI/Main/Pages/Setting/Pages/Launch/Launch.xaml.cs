using MinecraftLaunch.Components.Fetcher;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using YMCL.Main.Public;
using YMCL.Main.UI.Lang;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        List<string> javas = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.JavaDataPath));
        public Launch()
        {
            InitializeComponent();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            LoadJavas();
            LoadMem(setting.MaxMem);
        }

        void LoadMinecraftFolder()
        {
            MinecraftFolderComboBox.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            foreach (var item in minecraftFolder)
            {
                MinecraftFolderComboBox.Items.Add(item);
            }
            if (setting.MinecraftFolder == null || !minecraftFolder.Contains(setting.MinecraftFolder))
            {
                MinecraftFolderComboBox.SelectedIndex = 0;
            }
            else
            {
                MinecraftFolderComboBox.SelectedItem = setting.MinecraftFolder;
            }
        }
        void LoadJavas()
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            JavaComboBox.Items.Clear();
            foreach (var item in javas)
            {
                JavaComboBox.Items.Add(item);
            }
            JavaComboBox.SelectedItem = setting.Java;
            if (setting.MinecraftFolder == null || !minecraftFolder.Contains(setting.MinecraftFolder))
            {
                JavaComboBox.SelectedIndex = 0;
            }
            else
            {
                JavaComboBox.SelectedItem = setting.Java;
            }
        }

        private void MinecraftFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (MinecraftFolderComboBox.SelectedItem == null || MinecraftFolderComboBox.SelectedItem.ToString() == setting.MinecraftFolder)
            {
                return;
            }
            setting.MinecraftFolder = MinecraftFolderComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void AddMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string path = string.Empty;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;//获取选中文件路径
                string[] array = path.Split(@"\");
                if (array[array.Length - 1] != ".minecraft")
                {
                    Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_NeedMinecraftFolder"), ToastPosition.Top);
                    return;
                }
                var isIncludePath = false;
                var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.MinecraftFolderDataPath));
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
                        Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_AddMinecraftFolder_Click_ExistsMinecraftFolder"), ToastPosition.Top);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                    else
                    {
                        minecraftFolder.Add(path);
                        File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
                        MinecraftFolderComboBox.Items.Add(path);
                        MinecraftFolderComboBox.SelectedItem = path;
                    }
                }
                else
                {
                    var obj = new List<string>()
                    {
                        System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft")
                    };
                    var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                    File.WriteAllText(Const.MinecraftFolderDataPath, data);
                }
            }
        }

        private void DelMinecraftFolder_Click(object sender, RoutedEventArgs e)
        {
            var index = MinecraftFolderComboBox.SelectedIndex;
            minecraftFolder.RemoveAt(index);
            MinecraftFolderComboBox.Items.RemoveAt(index);
            if (minecraftFolder.Count == 0)
            {
                minecraftFolder.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
                MinecraftFolderComboBox.Items.Add(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, ".minecraft"));
            }
            MinecraftFolderComboBox.SelectedIndex = 0;
            File.WriteAllText(Const.MinecraftFolderDataPath, JsonConvert.SerializeObject(minecraftFolder, Formatting.Indented));
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MinecraftFolder = MinecraftFolderComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void AutoFindJava_Click(object sender, RoutedEventArgs e)
        {
            JavaFetcher javaFetcher = new JavaFetcher();
            var includeItem = 0;
            var temp = new List<string>();
            var java = javaFetcher.Fetch().ToList();
            java = java.Distinct().ToList();
            foreach (var item in java)
            {
                if (javas.Contains(item.JavaPath))
                {
                    includeItem++;
                }
                else
                {
                    temp.Add(item.JavaPath);
                }
            }
            var str = LangHelper.Current.GetText("Launch_AutoFindJava_Click_ScanCompleted").Split("{|}");
            Toast.Show(Const.Window.mainWindow, $"{str[0]}{javas.Count}{str[1]}{includeItem}{str[2]}", ToastPosition.Top);
            temp.ForEach(item =>
            {
                javas.Add(item);
            });
            File.WriteAllText(Const.JavaDataPath, JsonConvert.SerializeObject(javas, Formatting.Indented));
            LoadJavas();
            if (javas.Count > 0) { JavaComboBox.SelectedIndex = 0; }
        }

        private void JavaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (JavaComboBox.SelectedItem == null || JavaComboBox.SelectedItem.ToString() == setting.MinecraftFolder)
            {
                return;
            }
            setting.Java = JavaComboBox.SelectedItem.ToString();
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void ManualAddJava_Click(object sender, RoutedEventArgs e)
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
            var dataArray = JsonConvert.DeserializeObject<string[]>(File.ReadAllText(Const.JavaDataPath));
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
                    Toast.Show(Const.Window.mainWindow, LangHelper.Current.GetText("Launch_ManualAddJava_Click_ExistsJava"), ToastPosition.Top);
                    JavaComboBox.SelectedItem = path;
                }
                else
                {
                    javas.Add(path);
                    File.WriteAllText(Const.JavaDataPath, JsonConvert.SerializeObject(javas, Formatting.Indented));
                    JavaComboBox.Items.Add(path);
                    JavaComboBox.SelectedItem = path;
                }
            }
        }

        private void SilderBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SilderBox.Value = Math.Round(SilderBox.Value);
            SilderInfo.Text = $"{SilderBox.Value}M";
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

        private void SilderBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.MaxMem = Math.Round(SilderBox.Value);
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMinecraftFolder();
        }
    }
}
