using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YMCL.Class;
using static YMCL.Pages.MorePages.MusicPlayer;

namespace YMCL.Pages.Forms
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : WindowX
    {
        string VarSongID = "";
        string VarSongName = "";
        string VarArtists = "";
        string VarAlbumName = "";
        string VarDuration = "";
        string VarPic = "";
        string SongUrl = "";
        double w;
        double h;
        bool IsPlayListViewGrClose = true;
        bool IsPlaying = false;
        bool IsNoPlay = false;
        bool del = false;
        int Page = 1;
        MediaPlayer player = new MediaPlayer();//实例化绘图媒体
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
                _leftThumb.HorizontalAlignment = HorizontalAlignment.Left;
                _leftThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _leftThumb.Cursor = Cursors.SizeWE;
                _topThumb = new Thumb();
                _topThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                _topThumb.VerticalAlignment = VerticalAlignment.Top;
                _topThumb.Cursor = Cursors.SizeNS;
                _rightThumb = new Thumb();
                _rightThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _rightThumb.Cursor = Cursors.SizeWE;
                _bottomThumb = new Thumb();
                _bottomThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                _bottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _bottomThumb.Cursor = Cursors.SizeNS;
                _lefTopThumb = new Thumb();
                _lefTopThumb.HorizontalAlignment = HorizontalAlignment.Left;
                _lefTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _lefTopThumb.Cursor = Cursors.SizeNWSE;
                _rightTopThumb = new Thumb();
                _rightTopThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _rightTopThumb.Cursor = Cursors.SizeNESW;
                _rightBottomThumb = new Thumb();
                _rightBottomThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightBottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _rightBottomThumb.Cursor = Cursors.SizeNWSE;
                _leftbottomThumb = new Thumb();
                _leftbottomThumb.HorizontalAlignment = HorizontalAlignment.Left;
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
                    if (thumb.HorizontalAlignment == HorizontalAlignment.Stretch)
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
                    thumb.Background = Brushes.Green;
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
                _grid.Arrange(new Rect(new Point(-(_window.RenderSize.Width - finalSize.Width) / 2, -(_window.RenderSize.Height - finalSize.Height) / 2), _window.RenderSize));
                return finalSize;
            }
            //拖动逻辑
            private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                var c = _window;
                var thumb = sender as FrameworkElement;
                double left, top, width, height;
                if (thumb.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    left = c.Left + e.HorizontalChange;
                    width = c.Width - e.HorizontalChange;
                }
                else
                {
                    left = c.Left;
                    width = c.Width + e.HorizontalChange;
                }
                if (thumb.HorizontalAlignment != HorizontalAlignment.Stretch)
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
            FrameworkElementFactory GetFactory(Brush back)
            {
                var fef = new FrameworkElementFactory(typeof(Rectangle));
                fef.SetValue(Rectangle.FillProperty, back);
                return fef;
            }
        }
        class DisplaySongs
        {
            public string? SongID { get; set; }
            public string? SongName { get; set; }
            public string? Artists { get; set; }
            public string? AlbumName { get; set; }
            public string? Duration { get; set; }
        }
        class PlayListClass
        {
            public string? Type { get; set; }
            public string? Uri { get; set; }
            public string? SongName { get; set; }
            public string? Authors { get; set; }
            public string? Duration { get; set; }
            public string? Pic { get; set; }
        }
        List<PlayListClass> playList = new List<PlayListClass>();
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        public MusicPlayer()
        {
            InitializeComponent();
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            Width = obj.PlayerWindowWidth;
            Height = obj.PlayerWindowHeight;
            w = Width;
            h = Height;
            RefreshPlayList();
        }
        void RefreshPlayList()
        {
            playList.Clear();
            PlayListView.Items.Clear();
            var list = JsonConvert.DeserializeObject<List<PlayListClass>>(File.ReadAllText("./YMCL/YMCL.PlayList.json"));
            list.ForEach(x =>
            {
                playList.Add(new PlayListClass() { Authors = x.Authors, Duration = x.Duration, Type = x.Type, Pic = x.Pic, SongName = x.SongName, Uri = x.Uri });
            });
            playList.ForEach(y =>
            {
                PlayListView.Items.Add(y);
            });

        }
        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));
        }

        private void WindowX_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (w == Width && h == Height)
            {
                return;
            }
            var obj = JsonConvert.DeserializeObject<Class.SettingInfo>(File.ReadAllText("./YMCL/YMCL.Setting.json"));
            obj.PlayerWindowWidth = Width;
            obj.PlayerWindowHeight = Height;
            File.WriteAllText(@"./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var _mainWindow = Application.Current.Windows
            .Cast<Window>()
            .FirstOrDefault(window => window is MainWindow) as MainWindow;

            _mainWindow.WindowState = WindowState.Normal;

            Close();
        }


        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FontIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPlayListViewGrClose)
            {
                ThicknessAnimation Comein = new ThicknessAnimation()
                {
                    To = new Thickness(0, 40, 10, 80),
                    From = new Thickness(0, 40, -290, 80),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                ThicknessAnimation SearchGrComein = new ThicknessAnimation()
                {
                    To = new Thickness(10, 10, 300, 13),
                    From = new Thickness(10, 10, 10, 13),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                PlayListViewGr.BeginAnimation(MarginProperty, Comein);
                SearchGr.BeginAnimation(MarginProperty, SearchGrComein);
                IsPlayListViewGrClose = false;
            }
            else
            {
                IsPlayListViewGrClose = true;
                ThicknessAnimation Comeout = new ThicknessAnimation()
                {
                    From = new Thickness(0, 40, 10, 80),
                    To = new Thickness(0, 40, -290, 80),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                ThicknessAnimation SearchGrComeout = new ThicknessAnimation()
                {
                    From = new Thickness(10, 10, 300, 13),
                    To = new Thickness(10, 10, 10, 13),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                PlayListViewGr.BeginAnimation(MarginProperty, Comeout);
                SearchGr.BeginAnimation(MarginProperty, SearchGrComeout);
            }
        }


        private void AddLocalFileBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "音频文件|*.wav;*.mp3";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileNames.ToString().Trim().Equals("") || openFileDialog.FileNames.Length == 0)
            {
                return;
            }

            string[] Array = openFileDialog.FileName.Split(@"\");

            playList.Add(new PlayListClass()
            {
                Uri = openFileDialog.FileName,
                SongName = Array[Array.Length - 1],
                Type = "本地",
                Authors = "",
                Duration = "",
                Pic = ""
            });
            string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@".\YMCL\YMCL.PlayList.json", str);
            RefreshPlayList();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPlaying)
            {
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Hidden;
                IsPlaying = false;
            }
            else
            {
                PlayIcon.Visibility = Visibility.Hidden;
                PauseIcon.Visibility = Visibility.Visible;
                IsPlaying = true;
            }
        }

        private void DelObjBtn_Click(object sender, RoutedEventArgs e)
        {
            var index = PlayListView.SelectedIndex;
            playList.RemoveAt(PlayListView.SelectedIndex);
            string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(@".\YMCL\YMCL.PlayList.json", str);
            RefreshPlayList();
            player.Close();
        }

        private void PlayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayListView.SelectedIndex >= 0)
            {
                DelObjBtn.IsEnabled = true;
            }
            else
            {
                DelObjBtn.IsEnabled = false;
            }
            Play();
        }

        async void Search()
        {
            SearchBox.IsEnabled = false;
            //displaySongs.Clear();
            //SongsListView.ItemsSource = displaySongs;
            SongsListView.SelectedIndex = -1;
            SongsListView.Items.Clear();
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                Panuon.WPF.UI.Toast.Show($"搜索内容不可为空", ToastPosition.Top);
                return;
            }
            NextPageBtn.IsEnabled = false;
            BackPageBtn.IsEnabled = false;
            Panuon.WPF.UI.Toast.Show($"搜索中...", ToastPosition.Top);
            var SearchText = SearchBox.Text;
            var offset = (Page - 1) * 30;
            await Task.Run(async () =>
            {
                string url = $"https://music.api.daiyu.fun/search?keywords={SearchText}&type=1&offset={offset}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;
                    try
                    {
                        response = await client.GetAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求失败：" + ex, ToastPosition.Top); });
                        return;
                    }

                    string responseData;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            responseData = await response.Content.ReadAsStringAsync();
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求失败：" + ex, ToastPosition.Top); });
                            return;
                        }

                        // 输出响应数据
                        string json = responseData;
                        try
                        {
                            YMCL.Class.NeteasyCloudMusic.Root root = JsonConvert.DeserializeObject<YMCL.Class.NeteasyCloudMusic.Root>(json);

                            Debug.WriteLine("Code: " + root.code);
                            Debug.WriteLine("Song Count: " + root.result.songCount);

                            if (root.code != 200)
                            {
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求错误", ToastPosition.Top); });
                                return;
                            }
                            if (root.result.songCount == 0)
                            {
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("搜索无结果", ToastPosition.Top); });
                                return;
                            }
                            foreach (var song in root.result.songs)
                            {
                                string artists_str = "";
                                string time = "";
                                //Debug.WriteLine("Song ID: " + song.id);
                                //Debug.WriteLine("Song Name: " + song.name);

                                foreach (var artist in song.artists)
                                {
                                    artists_str += $"{artist.name} ";
                                    //Debug.WriteLine("Artist ID: " + artist.id);
                                    //Debug.WriteLine("Artist Name: " + artist.name);
                                    // 艺术家属性...
                                }

                                //Debug.WriteLine("Album ID: " + song.album.id);
                                //Debug.WriteLine("Album Name: " + song.album.name);
                                // 其他专辑属性...

                                //Debug.WriteLine("Duration: " + song.duration);
                                // 其他歌曲属性...

                                double getsecond = song.duration * 1.0 / 1000;
                                double getdoubleminuth = Math.Floor(getsecond / 60);
                                string minuthTIme = string.Empty;
                                string secondTime = string.Empty;
                                string resultShow = string.Empty;
                                if (getdoubleminuth >= 1)
                                {
                                    minuthTIme = getdoubleminuth >= 10 ? $"{getdoubleminuth}" : $"0{getdoubleminuth}";
                                    double getmtemp = getdoubleminuth * 60;
                                    double getmtemp2 = getsecond - getmtemp;
                                    double timemiao = Math.Floor(getmtemp2);
                                    secondTime = $"{(timemiao >= 10 ? timemiao.ToString() : "0" + timemiao)}";
                                    resultShow = $"{minuthTIme}:{secondTime}";
                                }
                                else
                                {
                                    secondTime = getsecond >= 10 ? getsecond.ToString() : ("0" + getsecond);
                                    resultShow = $"00:{secondTime}";
                                }
                                time = resultShow;

                                await Dispatcher.BeginInvoke(() =>
                                {
                                    SongsListView.Items.Add(new DisplaySongs()
                                    {
                                        SongID = song.id.ToString(),
                                        SongName = song.name,
                                        Artists = artists_str,
                                        AlbumName = song.album.name,
                                        Duration = time
                                    });
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("解析错误：" + ex, ToastPosition.Top); });
                            return;
                        }

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求失败：" + response.StatusCode, ToastPosition.Top); });
                    }
                }
            });

            //SongsListView.ItemsSource = displaySongs;

            SearchBox.IsEnabled = true;
            NextPageBtn.IsEnabled = true;
            BackPageBtn.IsEnabled = true;
        }

        void Play()
        {
            PlayListClass song = PlayListView.SelectedItem as PlayListClass;
            if (song==null)
            {
                return;
            }
            if (song.Type == "本地")
            {
                if (!File.Exists(song.Uri))
                {
                    Panuon.WPF.UI.Toast.Show($"此文件不存在，它可能被移动、重命名或删除", ToastPosition.Top);
                    return;
                }
                PlayingSongName.Text = song.SongName;
                PlayingSongAuthors.Text = song.Authors;
                Player();
            }
            else
            {
                PlayingSongName.Text = song.SongName;
                PlayingSongAuthors.Text = song.Authors;
                Player();
            }
        }
        void Player()
        {
            PlayListClass song = PlayListView.SelectedItem as PlayListClass;
            player.Open(new Uri(song.Uri));
            IsPlaying = true;
            player.Play();//播放媒体
            SearchBox.IsEnabled = true;
            PlayIcon.Visibility = Visibility.Visible;
            PauseIcon.Visibility = Visibility.Hidden;
        }
        private void AutoSuggestBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Page = 1;
            PageText.Text = Page.ToString();
            Search();
        }

        private async void SongsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchBox.IsEnabled = false;
            if (SongsListView.SelectedIndex < 0)
            {
                return;
            }
            if (IsPlaying)
            {
                player.Close();
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Hidden;
            }
            DisplaySongs song = SongsListView.SelectedItem as DisplaySongs;
            var SearchText = song.SongID;
            Panuon.WPF.UI.Toast.Show($"正在获取音乐...", ToastPosition.Top);
            await Task.Run(async () =>
            {
                string checkurl = $"https://music.api.daiyu.fun/check/music?id={SearchText}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(checkurl);

                    string responseData = await response.Content.ReadAsStringAsync();

                    Response res = JsonConvert.DeserializeObject<Response>(responseData);

                    if (res.success == false)
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求失败，网易云音乐返回信息：" + res.message, ToastPosition.Top); });
                        IsNoPlay = true;
                    }

                }
                if (IsNoPlay)
                {
                    return;
                }
                string url = $"https://music.api.daiyu.fun/song/url?id={SearchText}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        YMCL.Class.NeteasyCloudMusicUrl.Root root = JsonConvert.DeserializeObject<YMCL.Class.NeteasyCloudMusicUrl.Root>(responseData);
                        if (root.code != 200)
                        {
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求错误", ToastPosition.Top); });
                            return;
                        }
                        SongUrl = root.data[0].url;

                        playList.Add(new PlayListClass()
                        {
                            Uri = SongUrl,
                            SongName = song.SongName,
                            Type = "网易云",
                            Authors = song.Artists,
                            Duration = song.Duration,
                        });

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show("请求失败：" + response.StatusCode, ToastPosition.Top); });
                    }
                }
            });
            if (IsNoPlay)
            {
                return;
            }
            else
            {
                string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(@".\YMCL\YMCL.PlayList.json", str);
                RefreshPlayList();
            }

        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Page++;
            PageText.Text = Page.ToString();
            Search();
        }

        private void BackPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Page == 1)
            {
                return;
            }
            Page--;
            PageText.Text = Page.ToString();
            Search();
        }
    }
}
