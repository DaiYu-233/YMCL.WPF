using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Fetcher;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        List<string> minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
        List<JavaEntry> javas = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
        List<string> javasPath = new();
        public Launch()
        {
            InitializeComponent();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            LoadJavas();
            LoadMem(setting.MaxMem);
            AloneCoreToggle.IsOn = setting.AloneCore;
            OutputLogToggle.IsOn = setting.GetOutput;

            foreach (var item in javas)
            {
                javasPath.Add(item.JavaPath);
            }

            GameWindowComboBox.SelectedIndex = (int)setting.GameWindow;
        }
        void LoadMinecraftFolder()
        {
            minecraftFolder = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(Const.MinecraftFolderDataPath));
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
            JavaComboBox.Items.Add(new JavaEntry()
            {
                JavaPath = LangHelper.Current.GetText("Launch_AutoSelectJava")
            });
            foreach (var item in javas)
            {
                JavaComboBox.Items.Add(item);
            }
            if (setting.Java == null || setting.Java.JavaPath == "<Auto>" || setting.Java.JavaPath == string.Empty)
            {
                JavaComboBox.SelectedIndex = 0;
            }
            else
            {
                JavaComboBox.SelectedItem = setting.Java;
            }
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
            if (MinecraftFolderComboBox.SelectedItem.ToString() != setting.MinecraftFolder)
            {
                setting.MinecraftVersionId = null;
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
            var temp = new List<JavaEntry>();
            var java = javaFetcher.Fetch().ToList();
            java = java.Distinct().ToList();
            foreach (var item in java)
            {
                if (javasPath.Contains(item.JavaPath))
                {
                    includeItem++;
                }
                else
                {
                    temp.Add(item);
                }
            }
            var str = LangHelper.Current.GetText("Launch_AutoFindJava_Click_ScanCompleted").Split("{|}");
            temp.ForEach(javas.Add);
            foreach (var item in javas)
            {
                javasPath.Add(item.JavaPath);
            }
            Toast.Show(Const.Window.mainWindow, $"{str[0]}{javas.Count}{str[1]}{includeItem}{str[2]}", ToastPosition.Top);
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
            if (JavaComboBox.SelectedIndex == 0 && setting.Java.JavaPath == "<Auto>")
            {
                return;
            }
            if (JavaComboBox.SelectedIndex == 0)
            {
                setting.Java = new JavaEntry()
                {
                    JavaPath = "<Auto>"
                };
            }
            else
            {
                setting.Java = JavaComboBox.SelectedItem as JavaEntry;
            }
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
            var dataArray = JsonConvert.DeserializeObject<List<JavaEntry>>(File.ReadAllText(Const.JavaDataPath));
            if (dataArray != null)
            {
                foreach (var item in dataArray)
                {
                    if (item.JavaPath == path)
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
                    javas.Add(new JavaEntry()
                    {
                        JavaPath = path,
                        JavaSlugVersion = -1
                    });
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
            if (GameWindowComboBox.SelectedIndex == 2)
            {
                CustomGameWindow.Visibility = Visibility.Visible;
                GameWindowComboBox.Margin = new Thickness(18, 0, 0, 0);
            }
            else
            {
                CustomGameWindow.Visibility = Visibility.Hidden;
                GameWindowComboBox.Margin = new Thickness(18, 0, -1 * CustomGameWindow.ActualWidth - 6.8, 0);
            }
        }
        private void AloneCoreToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (AloneCoreToggle.IsOn == setting.AloneCore)
            {
                return;
            }
            setting.AloneCore = AloneCoreToggle.IsOn;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void GameWindowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameWindowComboBox.SelectedIndex == 2)
            {
                CustomGameWindow.Visibility = Visibility.Visible;
                GameWindowComboBox.Margin = new Thickness(18, 0, 0, 0);
            }
            else
            {
                CustomGameWindow.Visibility = Visibility.Hidden;
                GameWindowComboBox.Margin = new Thickness(18, 0, -1 * CustomGameWindow.ActualWidth - 6.8, 0);
            }
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (GameWindowComboBox.SelectedIndex != (int)setting.GameWindow)
            {
                setting.GameWindow = (SettingItem.GameWindow)GameWindowComboBox.SelectedIndex;
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
        }
        private void OutputLogToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (OutputLogToggle.IsOn == setting.GetOutput)
            {
                return;
            }
            setting.GetOutput = OutputLogToggle.IsOn;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void GameHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.GameHeight = Convert.ToDouble(GameHeight.Text);
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
        private void GameWidth_LostFocus(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            setting.GameWidth = Convert.ToDouble(GameWidth.Text);
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }
    }
}
