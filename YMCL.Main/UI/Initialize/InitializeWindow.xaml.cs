using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;
using YMCL.Main.Public.Class;
using YMCL.Main.UI.Main;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ProgressBar = iNKORE.UI.WPF.Modern.Controls.ProgressBar;
using Panuon.WPF.UI;
using System.Security.Principal;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using System.Windows.Documents;
using System.Net;
using System.Timers;
using Path = System.IO.Path;
using Cursors = System.Windows.Input.Cursors;
using Size = System.Windows.Size;
using Microsoft.Win32;
using System.Windows.Shapes;

namespace YMCL.Main.UI.Initialize
{
    /// <summary>
    /// InitializeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InitializeWindow : WindowX
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);
        [DllImport("gdi32")]
        static extern int AddFontResource(string lpFileName);

        int page = 0;
        private System.Timers.Timer timer;
        private int countdown = 40;
        List<DownloadFile> files = new List<DownloadFile>()
            {
                new DownloadFile()
                {
                    Name="MiSans.ttf",
                    Url="https://ymcl.daiyu.fun/Assets/YMCL/MiSans.ttf",
                    MD5="61729D264686DD2DCA5038648EA8C9FE"
                },
                new DownloadFile()
                {
                    Name="FluentIcons.ttf",
                    Url="https://ymcl.daiyu.fun/Assets/YMCL/FluentIcons.ttf",
                    MD5="460A1FFA29FBC20E97861B497601C552"
                },
                new DownloadFile()
                {
                    Name="YMCL.Updater.exe",
                    Url="https://ymcl.daiyu.fun/Assets/YMCL/YMCL.Updater.exe",
                    MD5="6851F1062F8CD545FBEBF2B755CE4DEC"
                },
                new DownloadFile()
                {
                    Name="Icon.ico",
                    Url="https://ymcl.daiyu.fun/Assets/img/YMCL-Icon.ico",
                    MD5="74985843B31E8BD0741B2C62A3C00853"
                },
                new DownloadFile()
                {
                    Name="Icon.png",
                    Url="https://ymcl.daiyu.fun/Assets/img/YMCL-Icon.png",
                    MD5="44953C3905C5EDBC3353AA89D359DC06"
                }
            };
        List<TextBlock> lines = new List<TextBlock>();
        List<ProgressBar> progressBarList = new List<ProgressBar>();
        public InitializeWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上(Resize)
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));

            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
            if (setting.Language != null)
            {
                ChooseLanguagePage.Visibility = Visibility.Hidden;
                FileDownloadPage.Visibility = Visibility.Visible;

                foreach (var file in files)
                {
                    ProgressBar progressBar = new ProgressBar
                    {
                        Background = new SolidColorBrush(Color.FromRgb(191, 197, 202)),
                        Margin = new Thickness(5, 0, 5, 6),
                        VerticalAlignment = VerticalAlignment.Bottom
                    };
                    progressBarList.Add(progressBar);
                    Border border = new()
                    {
                        Margin = new Thickness(10, 10, 10, 0),
                        Background = new SolidColorBrush(Color.FromRgb(246, 249, 251)),
                        CornerRadius = new CornerRadius(5),
                        Height = 35
                    };
                    Grid grid = new Grid();
                    TextBlock text1 = new TextBlock
                    {
                        Text = file.Name,
                        FontFamily = new System.Windows.Media.FontFamily("MiSans Medium"),
                        FontSize = 13,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(7, 4, 0, 0)
                    };
                    string text3 = LangHelper.Current.GetText("InitializeWindow_DownloadWaiting");
                    TextBlock text2 = new TextBlock
                    {
                        FontFamily = new System.Windows.Media.FontFamily("MiSans Medium"),
                        FontSize = 13,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(0, 4, 7, 0),
                        Text = text3
                    };
                    lines.Add(text2);
                    grid.Children.Add(text1);
                    grid.Children.Add(text2);
                    grid.Children.Add(progressBar);
                    border.Child = grid;
                    FileDownloadPage.Children.Add(border);
                }
                Download();
            }
        }
        #region UIResize
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Environment.Exit(0);
        }
        #endregion
        #region Resize
        public class WindowResizeAdorner : Adorner
        {
            //4条边
            Thumb _leftThumb, _topThumb, _rightThumb, _bottomThumb;
            //4个角
            Thumb _lefTopThumb, _rightTopThumb, _rightBottomThumb, _leftbottomThumb;
            //布局容器，如果不使用布局容器，则需要给上述8个控件布局，实现和Grid布局定位是一样的，会比较繁琐且意义不大。
            Grid _grid;
            UIElement _adornedElement;
            Window _window;
            public WindowResizeAdorner(UIElement adornedElement) : base(adornedElement)
            {
                _adornedElement = adornedElement;
                _window = Window.GetWindow(_adornedElement);
                //初始化thumb
                _leftThumb = new Thumb();
                _leftThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _leftThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _leftThumb.Cursor = Cursors.SizeWE;
                _topThumb = new Thumb();
                _topThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                _topThumb.VerticalAlignment = VerticalAlignment.Top;
                _topThumb.Cursor = Cursors.SizeNS;
                _rightThumb = new Thumb();
                _rightThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _rightThumb.Cursor = Cursors.SizeWE;
                _bottomThumb = new Thumb();
                _bottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                _bottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _bottomThumb.Cursor = Cursors.SizeNS;
                _lefTopThumb = new Thumb();
                _lefTopThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _lefTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _lefTopThumb.Cursor = Cursors.SizeNWSE;
                _rightTopThumb = new Thumb();
                _rightTopThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _rightTopThumb.Cursor = Cursors.SizeNESW;
                _rightBottomThumb = new Thumb();
                _rightBottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightBottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _rightBottomThumb.Cursor = Cursors.SizeNWSE;
                _leftbottomThumb = new Thumb();
                _leftbottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _leftbottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _leftbottomThumb.Cursor = Cursors.SizeNESW;
                _grid = new Grid();
                _grid.Children.Add(_leftThumb);
                _grid.Children.Add(_topThumb);
                _grid.Children.Add(_rightThumb);
                _grid.Children.Add(_bottomThumb);
                _grid.Children.Add(_lefTopThumb);
                _grid.Children.Add(_rightTopThumb);
                _grid.Children.Add(_rightBottomThumb);
                _grid.Children.Add(_leftbottomThumb);
                AddVisualChild(_grid);
                foreach (Thumb thumb in _grid.Children)
                {
                    int thumnSize = 10;
                    if (thumb.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
                    {
                        thumb.Width = double.NaN;
                        thumb.Margin = new Thickness(thumnSize, 0, thumnSize, 0);
                    }
                    else
                    {
                        thumb.Width = thumnSize;
                    }
                    if (thumb.VerticalAlignment == VerticalAlignment.Stretch)
                    {
                        thumb.Height = double.NaN;
                        thumb.Margin = new Thickness(0, thumnSize, 0, thumnSize);
                    }
                    else
                    {
                        thumb.Height = thumnSize;
                    }
                    thumb.Background = System.Windows.Media.Brushes.Green;
                    thumb.Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = GetFactory(new SolidColorBrush(Colors.Transparent))
                    };
                    thumb.DragDelta += Thumb_DragDelta;
                }
            }

            protected override Visual GetVisualChild(int index)
            {
                return _grid;
            }
            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
            protected override Size ArrangeOverride(Size finalSize)
            {
                //直接给grid布局，grid内部的thumb会自动布局。
                _grid.Arrange(new Rect(new System.Windows.Point(-(_window.RenderSize.Width - finalSize.Width) / 2, -(_window.RenderSize.Height - finalSize.Height) / 2), _window.RenderSize));
                return finalSize;
            }
            //拖动逻辑
            private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                var c = _window;
                var thumb = sender as FrameworkElement;
                double left, top, width, height;
                if (thumb.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                {
                    left = c.Left + e.HorizontalChange;
                    width = c.Width - e.HorizontalChange;
                }
                else
                {
                    left = c.Left;
                    width = c.Width + e.HorizontalChange;
                }
                if (thumb.HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
                {
                    if (width > 63)
                    {
                        c.Left = left;
                        c.Width = width;
                    }
                }
                if (thumb.VerticalAlignment == VerticalAlignment.Top)
                {

                    top = c.Top + e.VerticalChange;
                    height = c.Height - e.VerticalChange;
                }
                else
                {
                    top = c.Top;
                    height = c.Height + e.VerticalChange;
                }

                if (thumb.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    if (height > 63)
                    {
                        c.Top = top;
                        c.Height = height;
                    }
                }
            }
            //thumb的样式
            FrameworkElementFactory GetFactory(System.Windows.Media.Brush back)
            {
                var fef = new FrameworkElementFactory(typeof(System.Windows.Shapes.Rectangle));
                fef.SetValue(System.Windows.Shapes.Rectangle.FillProperty, back);
                return fef;
            }
        }
        #endregion
        int i = 0;
        async void Download()
        {
            bool noFinishDownload = false;

            Next.Content = LangHelper.Current.GetText("InitializeWindow_NextSetpBtn");
            bool needInstallFont = false;
            Title.Text = LangHelper.Current.GetText("InitializeWindow_Title_Download");


            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            using HttpClient client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 0, 40)
            };

            foreach (var item in files)
            {
                #region MD5
                string md5 = "";
                string strHashData = "";
                byte[] arrbytHashValue;
                FileStream oFileStream = null;
                var oMD5Hasher = MD5.Create();
                try
                {
                    oFileStream = new System.IO.FileStream(Path.Combine(Const.PublicDataRootPath, item.Name), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);//计算指定Stream 对象的哈希值
                    oFileStream.Close();
                    strHashData = System.BitConverter.ToString(arrbytHashValue);
                    strHashData = strHashData.Replace("-", "");
                    md5 = strHashData;
                }
                catch (Exception) { }
                #endregion
                Debug.WriteLine(md5 == item.MD5);
                if (File.Exists(Path.Combine(Const.PublicDataRootPath, item.Name)) && md5 != item.MD5)
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        var message = MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_Download_AdministratorPermissionRequired"), "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                        if (message == MessageBoxResult.OK)
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                UseShellExecute = true,
                                WorkingDirectory = Environment.CurrentDirectory,
                                FileName = System.Windows.Forms.Application.ExecutablePath,
                                Verb = "runas",
                                Arguments = "InstallFont"
                            };
                            Process.Start(startInfo);
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_FailedToObtainAdministratorPrivileges"), "Yu Minecraft Launcher", MessageBoxIcon.Error);
                            Application.Current.Shutdown();
                        }
                    }
                }

                var bat = "set /p ymcl=<%USERPROFILE%\\AppData\\Roaming\\DaiYu.YMCL\\YMCL.ExePath.DaiYu\r\necho %ymcl%\r\necho %1\r\nstart %ymcl% %1";
                File.WriteAllText(Const.YMCLBat, bat);

                try { Registry.ClassesRoot.DeleteSubKey("YMCL"); } catch { }
                try
                {
                    RegistryKey keyRoot = Registry.ClassesRoot.CreateSubKey("YMCL", true);
                    keyRoot.SetValue("", "Yu Minecraft Launcher");
                    keyRoot.SetValue("URL Protocol", Const.YMCLBat);
                    RegistryKey registryKeya = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey("DefaultIcon");
                    registryKeya.SetValue("", Const.YMCLBat);
                    RegistryKey registryKeyb = Registry.ClassesRoot.OpenSubKey("YMCL", true).CreateSubKey(@"shell\open\command");
                    registryKeyb.SetValue("", $"\"{Const.YMCLBat}\" \"%1\"");
                }
                catch { }


                if (File.Exists(Path.Combine(Const.PublicDataRootPath, item.Name)) && md5 == item.MD5)
                {
                    lines[i].Text = LangHelper.Current.GetText("InitializeWindow_DownloadFinish");
                    progressBarList[i].Maximum = 100;
                    progressBarList[i].Value = 100;
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        try
                        {
                            timer = new System.Timers.Timer(1000);
                            timer.Elapsed += TimerElapsed;
                            countdown = 40;
                            timer.Start();
                            using (HttpResponseMessage response = await client.GetAsync(item.Url, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();

                                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                                {
                                    using (var fileStream = new FileStream(Path.Combine(Const.PublicDataRootPath, item.Name), FileMode.Create, FileAccess.Write))
                                    {
                                        byte[] buffer = new byte[8192];
                                        int bytesRead;
                                        long totalBytesRead = 0;
                                        long totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1;

                                        while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                        {
                                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                                            totalBytesRead += bytesRead;

                                            if (totalBytes > 0)
                                            {
                                                double progress = Math.Round((double)totalBytesRead / totalBytes * 100, 1);
                                                await Dispatcher.BeginInvoke(() =>
                                                {
                                                    timer.Stop();
                                                    progressBarList[i].Maximum = 100;
                                                    progressBarList[i].Value = progress;
                                                    lines[i].Text = $"{progress}%";
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                lines[i].Text = $"{LangHelper.Current.GetText("InitializeWindow_ConnectionFailed")}：{e.Message}";
                                timer.Stop();
                                noFinishDownload = true;
                            });
                        }
                        catch (TaskCanceledException ex)
                        {
                            await Dispatcher.BeginInvoke(() =>
                            {
                                lines[i].Text = $"{LangHelper.Current.GetText("InitializeWindow_ConnectionTimeout")}：{ex.Message}";
                                noFinishDownload = true;
                            });
                        }
                    });

                    if (!noFinishDownload)
                    {
                        lines[i].Text = LangHelper.Current.GetText("InitializeWindow_DownloadFinish");
                    }

                    if (item.Name == "MiSans.ttf" || item.Name == "FluentIcons.ttf")
                    {
                        needInstallFont = true;
                    }
                }
                i++;
            }

            if (noFinishDownload)
            {
                return;
            }

            if (!needInstallFont && File.Exists(Path.Combine(Const.PublicDataRootPath, "FontHasBeenInstalled")) && !App.StartupArgs.Contains("InstallFont"))
            {
                MainWindow main = Const.Window.main;
                Hide();
                main.Show();
            }
            else
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var message = MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_Download_AdministratorPermissionRequired"), "Yu Minecraft Launcher", MessageBoxButton.OKCancel, MessageBoxIcon.Info);
                    if (message == MessageBoxResult.OK)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory,
                            FileName = System.Windows.Forms.Application.ExecutablePath,
                            Verb = "runas",
                            Arguments = "InstallFont"
                        };
                        Process.Start(startInfo);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_FailedToObtainAdministratorPrivileges"), "Yu Minecraft Launcher", MessageBoxIcon.Error);
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    var fontFilePath = "C:\\ProgramData\\DaiYu.YMCL\\MiSans.ttf";
                    string fontPath = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "fonts", $"YMCL_{Function.GetTimeStamp()}_" + Path.GetFileName(fontFilePath));
                    File.Copy(fontFilePath, fontPath, true); //font是程序目录下放字体的文件夹
                    AddFontResource(fontPath);
                    WriteProfileString("fonts", Path.GetFileNameWithoutExtension(fontFilePath) + "(TrueType)", $"YMCL_{Function.GetTimeStamp()}_" + Path.GetFileName(fontFilePath));


                    var fontFilePath1 = "C:\\ProgramData\\DaiYu.YMCL\\FluentIcons.ttf";
                    string fontPath1 = Path.Combine(Environment.GetEnvironmentVariable("WINDIR"), "fonts", $"YMCL_{Function.GetTimeStamp()}_" + Path.GetFileName(fontFilePath1));
                    File.Copy(fontFilePath1, fontPath1, true); //font是程序目录下放字体的文件夹
                    AddFontResource(fontPath1);
                    WriteProfileString("fonts", Path.GetFileNameWithoutExtension(fontFilePath1) + "(TrueType)", $"YMCL_{Function.GetTimeStamp()}_" + Path.GetFileName(fontFilePath1));

                    File.WriteAllText(Path.Combine(Const.PublicDataRootPath, "FontHasBeenInstalled"), "");
                }
                Next.IsEnabled = true;
            }
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            countdown--;
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    if (countdown > 0)
                    {
                        lines[i].Text = $"{LangHelper.Current.GetText("InitializeWindow_Connecting")} - {countdown}s";
                    }
                    else
                    {
                        lines[i].Text = LangHelper.Current.GetText("InitializeWindow_ConnectionTimeout");
                    }
                }
                catch { }
            });

            Debug.WriteLine($"Remaining time: {countdown}s");
            if (countdown <= 0)
            {
                //cts.Cancel();
                timer.Stop();
            }
        }
        string Lang = string.Empty;
        private void ChooseLanguageBtn_Click(object sender, RoutedEventArgs e)
        {
            var buttons = Languages.Children;
            foreach (var item in buttons)
            {
                var control = item as ToggleButton;
                control.IsChecked = false;
                var stackPanel = (StackPanel)control.Content;
                var blocks = stackPanel.Children;
                foreach (var block in blocks)
                {
                    var a = block as TextBlock;
                    a.Foreground = new SolidColorBrush(Color.FromRgb(49, 50, 50));
                }
            }
            var send = (ToggleButton)sender;
            send.IsChecked = true;
            var main = (StackPanel)send.Content;
            var el = main.Children;
            foreach (var item in el)
            {
                var control = item as TextBlock;
                control.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            Lang = send.Tag.ToString();
            Next.Content = LangHelper.Current.GetText("InitializeWindow_NextSetpBtn", Lang);
            Next.IsEnabled = true;
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next.IsEnabled = false;
            if (page == 0)
            {
                var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));
                setting.Language = Lang;
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = System.Windows.Forms.Application.ExecutablePath,
                };
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }
            if (page == 1)
            {
                MessageBoxX.Show(LangHelper.Current.GetText("InitializeWindow_InitializeFinish"), "Yu Minecraft Launcher", MessageBoxIcon.Success);
                MainWindow main = Const.Window.main;
                Hide();
                main.Show();
            }
            page++;
        }
    }
}
