using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using YMCL.Public;
using YMCL.UI.Lang;
using YMCL.Public.Class;
using YMCL.UI.Main;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ProgressBar = ModernWpf.Controls.ProgressBar;

namespace YMCL.UI.Initialize
{
    /// <summary>
    /// InitializeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InitializeWindow : Window
    {
        List<InitializeFile> files = new List<InitializeFile>()
            {
                new InitializeFile()
                {
                    Name="MiSans.ttf",
                    Url="https://ymcl.daiyu.fun/MiSans.ttf",
                    MD5="61729D264686DD2DCA5038648EA8C9FE"
                },
                new InitializeFile()
                {
                    Name="YMCL-Updater.exe",
                    Url="https://ymcl.daiyu.fun/YMCL-Updater.exe",
                    MD5="2A526334DB2E669C02D9792357D5DD55"
                }
            };
        List<TextBlock> lines = new List<TextBlock>();
        List<ProgressBar> progressBarList = new List<ProgressBar>();
        public InitializeWindow()
        {
            InitializeComponent();
            Function.CreateFolder(Const.PublicDataRootPath);
            Function.CreateFolder(Const.DataRootPath);
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
                TextBlock text2 = new TextBlock
                {
                    FontFamily = new System.Windows.Media.FontFamily("MiSans Medium"),
                    FontSize = 13,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 4, 7, 0),
                    Text = "等待中"
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
        async void Download()
        {
            bool needInstallFont = false;
            Title.Text = LangHelper.Current.GetText("InitializeWindow_Title_Download");
            var i = 0;
            foreach (var item in files)
            {
                #region MD5
                string md5 = "";
                string strHashData = "";
                byte[] arrbytHashValue;
                FileStream oFileStream = null;
                var oMD5Hasher = new MD5CryptoServiceProvider();
                try
                {
                    oFileStream = new System.IO.FileStream(Path.Combine(Const.PublicDataRootPath,item.Name), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    arrbytHashValue = oMD5Hasher.ComputeHash(oFileStream);//计算指定Stream 对象的哈希值
                    oFileStream.Close();
                    strHashData = System.BitConverter.ToString(arrbytHashValue);
                    strHashData = strHashData.Replace("-", "");
                    md5 = strHashData;
                }
                catch (Exception ex) { }
                #endregion
                Debug.WriteLine(md5 == item.MD5);
                if (File.Exists(Path.Combine(Const.PublicDataRootPath, item.Name)) && md5 == item.MD5)
                {
                    lines[i].Text = $"下载完成";
                    progressBarList[i].Maximum = 100;
                    progressBarList[i].Value = 100;
                }
                else
                {
                    await Task.Run(async () =>
                    {
                        using HttpClient client = new HttpClient();
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
                                                progressBarList[i].Maximum = 100;
                                                progressBarList[i].Value = progress;
                                                lines[i].Text = $"{progress}%";
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    });
                    lines[i].Text = $"下载完成";
                    if (item.Name == "MiSans.ttf")
                    {
                        needInstallFont = true;
                    }
                }
                i++;
            }

            if (needInstallFont || !File.Exists(Path.Combine(Const.PublicDataRootPath,"FontHasBeenInstalled")))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = System.Windows.Forms.Application.ExecutablePath,
                    Arguments = "InstallFont"
                };
                Process.Start(startInfo);
                Application.Current.Shutdown();
            }

        }
    }
}
