using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using YMCL.Class;

namespace YMCL.Pages.Forms
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : WindowX
    {
        string SongUri = "";
        double w;
        double h;
        bool IsPlayListViewGrClose = true;
        bool IsVolumeSliderClose = true;
        bool IsPlaying = false;
        bool IsNoPlay = false;
        bool IsGettingMusic = false;
        bool IsMovingSlider = false;
        bool del = false;
        int Page = 1;
        MediaPlayer player_for_duration = new MediaPlayer();
        MediaPlayer player = new MediaPlayer();//实例化绘图媒体
        DispatcherTimer timer = new DispatcherTimer();
        public class Response
        {
            public bool success { get; set; }
            public string message { get; set; }
        }
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
            public double Duration { get; set; }
            public string? DisplayDuration { get; set; }
        }
        class PlayListClass
        {
            public string? Type { get; set; }
            public string? Uri { get; set; }
            public string? SongID { get; set; }
            public string? SongName { get; set; }
            public string? Authors { get; set; }
            public double Duration { get; set; }
            public string? DisplayDuration { get; set; }
            public string? Pic { get; set; }
        }
        List<PlayListClass> playList = new List<PlayListClass>();
        string MaxPlayingTime = "";
        string MusicLoop = "RepeatOff";  //RepeatOff RepeatOne RepeatAll
        double MaxPlayingDuration = 0;
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        public MusicPlayer()
        {
            InitializeComponent();
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            Width = obj.PlayerWindowWidth;
            Height = obj.PlayerWindowHeight;
            w = Width;
            h = Height;
            RefreshPlayList();

            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(0.2);   //设置刷新的间隔时间

            VolumeSlider.Value = obj.PlayerVolume;
            player.Volume = obj.PlayerVolume;

            player.MediaEnded += Player_MediaEnded;
        }

        private void Player_MediaEnded(object? sender, EventArgs e)
        {
            if (IsGettingMusic)
            {
                return;
            }
            if (MusicLoop == "RepeatOff")
            {
                return;
            }
            else if (MusicLoop == "RepeatOne")
            {
                player.Position = new TimeSpan(0);
            }
            else if (MusicLoop == "RepeatAll")
            {
                var index = PlayListView.SelectedIndex;
                var count = PlayListView.Items.Count;
                if (index == count - 1)
                {
                    PlayListView.SelectedItem = PlayListView.Items[0];
                }
                else
                {
                    PlayListView.SelectedItem = PlayListView.Items[index + 1];
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            var nowtime = Millisecond_to_minutes(player.Position.TotalMilliseconds);
            TimeText.Text = $"{nowtime.Split(".")[0]}/{MaxPlayingTime}";
            if (!IsMovingSlider)
            {
                PlaySlider.Value = player.Position.TotalMilliseconds;
            }
        }


        void RefreshPlayList()
        {
            playList.Clear();
            PlayListView.Items.Clear();
            var list = JsonConvert.DeserializeObject<List<PlayListClass>>(File.ReadAllText(      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.PlayList.json"));
            if (list == null)
            {
                return;
            }
            list.ForEach(x =>
            {
                playList.Add(new PlayListClass() { Authors = x.Authors, Duration = x.Duration, Type = x.Type, Pic = x.Pic, SongName = x.SongName, Uri = x.Uri, DisplayDuration = x.DisplayDuration, SongID = x.SongID });
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
            var obj = JsonConvert.DeserializeObject<Class.SettingInfo>(File.ReadAllText(        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            obj.PlayerWindowWidth = Width;
            obj.PlayerWindowHeight = Height;
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
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
                    To = new Thickness(10, 50, 300, 13),
                    From = new Thickness(10, 50, 10, 13),
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
                    From = new Thickness(10, 50, 300, 13),
                    To = new Thickness(10, 50, 10, 13),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                PlayListViewGr.BeginAnimation(MarginProperty, Comeout);
                SearchGr.BeginAnimation(MarginProperty, SearchGrComeout);
            }
        }

        string Millisecond_to_minutes(double time)
        {
            double getsecond = time * 1.0 / 1000;
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
            return resultShow;
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

            player_for_duration.Open(new Uri(openFileDialog.FileName));

            bool HasTimeSpan = false;
            while (!HasTimeSpan)
            {
                if (player_for_duration.NaturalDuration.HasTimeSpan)
                {
                    HasTimeSpan = true;
                }
            }

            var time = player_for_duration.NaturalDuration.TimeSpan.TotalMilliseconds;


            playList.Add(new PlayListClass()
            {
                Uri = openFileDialog.FileName,
                SongName = Array[Array.Length - 1].Split(".")[0],
                Type = "本地",
                Authors = "." + Array[Array.Length - 1].Split(".")[1],
                Duration = time,
                DisplayDuration = Millisecond_to_minutes(time),
                Pic = null,
                SongID = null
            });
            string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.PlayList.json", str);
            RefreshPlayList();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPlaying)
            {
                PlayIcon.Visibility = Visibility.Hidden;
                PauseIcon.Visibility = Visibility.Visible;
                player.Pause();
                IsPlaying = false;
            }
            else
            {
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Hidden;
                player.Play();
                timer.Start();
                IsPlaying = true;
            }
        }

        private void DelObjBtn_Click(object sender, RoutedEventArgs e)
        {
            var index = PlayListView.SelectedIndex;
            playList.RemoveAt(PlayListView.SelectedIndex);
            string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.PlayList.json", str);
            RefreshPlayList();
            player.Close();
            timer.Stop();
            PlaySlider.Value = 0;
            PlaySlider.Maximum = 0;
            PlayingSongAuthors.Text = "";
            PlayingSongName.Text = "";
            TimeText.Text = "00:00/00:00";
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
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,$"搜索内容不可为空", ToastPosition.Top);
                return;
            }
            NextPageBtn.IsEnabled = false;
            BackPageBtn.IsEnabled = false;
            Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,$"搜索中...", ToastPosition.Top);
            var SearchText = SearchBox.Text;
            var offset = (Page - 1) * 30;
            await Task.Run(async () =>
            {
                string url = $"https://music.api.daiyu.fun/search?keywords={SearchText}&type=1&limit=100";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response;
                    try
                    {
                        response = await client.GetAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"请求失败：" + ex.Message, ToastPosition.Top); });
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
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"请求失败：" + ex.Message, ToastPosition.Top); });
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
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"请求错误", ToastPosition.Top); });
                                return;
                            }
                            if (root.result.songCount == 0)
                            {
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"搜索无结果", ToastPosition.Top); });
                                return;
                            }
                            foreach (var song in root.result.songs)
                            {
                                string artists_str = "";
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


                                await Dispatcher.BeginInvoke(() =>
                                {
                                    SongsListView.Items.Add(new DisplaySongs()
                                    {
                                        SongID = song.id.ToString(),
                                        SongName = song.name,
                                        Artists = artists_str,
                                        AlbumName = song.album.name,
                                        Duration = song.duration,
                                        DisplayDuration = Millisecond_to_minutes(song.duration)
                                    });
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"解析错误：" + ex.Message, ToastPosition.Top);
                                MessageBoxX.Show(ex.Message); 
                            });
                            return;
                        }

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"请求失败：" + response.StatusCode, ToastPosition.Top); });
                    }
                }
            });

            //SongsListView.ItemsSource = displaySongs;

            SearchBox.IsEnabled = true;
            NextPageBtn.IsEnabled = true;
            BackPageBtn.IsEnabled = true;
        }

        async void Play()
        {
            PlayListClass song = PlayListView.SelectedItem as PlayListClass;
            if (song == null)
            {
                return;
            }
            if (song.Type == "本地")
            {
                if (!File.Exists(song.Uri))
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,$"源文件不存在，它可能被移动、重命名或删除", ToastPosition.Top);
                    return;
                }
                SongUri = song.Uri;
                PlayingSongName.Text = song.SongName;
                PlayingSongAuthors.Text = song.Authors;
                Player();
            }
            else
            {
                bool isPlay;
                IsGettingMusic = true;
                isPlay = true;
                var SearchText = song.SongID;
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,$"正在获取音乐...", ToastPosition.Top);
                try
                {
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
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "请求失败，网易云音乐返回信息：" + res.message, ToastPosition.Top); });
                                isPlay = false;
                                IsGettingMusic = false;
                            }

                        }
                        if (!isPlay)
                        {
                            IsGettingMusic = false;
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
                                    Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "请求错误", ToastPosition.Top); });
                                    IsGettingMusic = false;
                                    return;
                                }
                                SongUri = root.data[0].url;
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "请求失败：" + response.StatusCode, ToastPosition.Top); });
                                IsGettingMusic = false;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "请求失败：" + ex.Message, ToastPosition.Top);
                    return;
                }
                
                if (isPlay)
                {
                    PlayingSongName.Text = song.SongName;
                    PlayingSongAuthors.Text = song.Authors;
                    Player();

                }

            }

        }
        async void Player()
        {
            string time;
            PlayListClass song = PlayListView.SelectedItem as PlayListClass;
            MaxPlayingTime = song.DisplayDuration;
            player.Open(new Uri(SongUri));
            bool HasTimeSpan = false;
            while (!HasTimeSpan)
            {
                if (player.NaturalDuration.HasTimeSpan)
                {
                    HasTimeSpan = true;
                }
            }
            IsGettingMusic = false;
            IsPlaying = true;
            MaxPlayingDuration = song.Duration;
            PlaySlider.Maximum = song.Duration;
            player.Play();//播放媒体
            timer.Start();
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
            IsNoPlay = false;
            SearchBox.IsEnabled = false;
            if (SongsListView.SelectedIndex < 0)
            {
                return;
            }
            if (IsPlaying)
            {
                player.Close();
                timer.Stop();
                PlayIcon.Visibility = Visibility.Visible;
                PauseIcon.Visibility = Visibility.Hidden;
            }
            DisplaySongs song = SongsListView.SelectedItem as DisplaySongs;
            var SearchText = song.SongID;
            Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, $"正在检查音乐可用性...", ToastPosition.Top);
            try
            {
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
                            Dispatcher.BeginInvoke(() => { Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "音乐不可用，网易云音乐返回信息：" + res.message, ToastPosition.Top); });
                            IsNoPlay = true;
                        }

                    }
                    if (IsNoPlay)
                    {
                        return;
                    }
                    playList.Add(new PlayListClass()
                    {
                        Uri = null,
                        SongName = song.SongName,
                        Type = "网易云",
                        SongID = song.SongID,
                        Authors = song.Artists,
                        Duration = song.Duration,
                        DisplayDuration = Millisecond_to_minutes(song.Duration)
                    });
                });
            }
            catch (Exception ex)
            {
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer, "请求失败：" + ex.Message, ToastPosition.Top);
            }
            
            
            if (IsNoPlay)
            {
                return;
            }
            else
            {
                string str = JsonConvert.SerializeObject(playList, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.PlayList.json", str);
                RefreshPlayList();
                PlayListView.SelectedItem = PlayListView.Items[PlayListView.Items.Count - 1];
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

        private void PlaySlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            player.Position = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0, milliseconds: (int)PlaySlider.Value);
            IsMovingSlider = false;
        }

        private void PlaySlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMovingSlider = true;
        }

        private async void DownloadIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlayListClass song = PlayListView.SelectedItem as PlayListClass;
            if (song == null)
            {
                return;
            }
            var SaveFileName = song.SongName;
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.Title = "保存音频文件";
            save.Filter = "音频文件 (*.mp3)|*.mp3|音频文件 (*.wav)|*.wav|All Files (*.*)|*.*";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            save.FileName = SaveFileName;
            save.ShowDialog();
            if (save.FileName == SaveFileName)
            {
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"已取消保存", ToastPosition.Top);
                return;
            }
            if (song.Type == "本地")
            {
                if (!File.Exists(song.Uri))
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"源文件不存在", ToastPosition.Top);
                    return;
                }
                string sourceFile = song.Uri;
                string destinationFile = save.FileName;
                try
                {
                    System.IO.File.Copy(sourceFile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"复制文件失败：" + ex.Message, ToastPosition.Top);
                }
            }
            else
            {
                try
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"开始下载", ToastPosition.Top);
                    await Task.Run(async () =>
                    {
                        using (var client = new HttpClient())
                        {
                            var response = await client.GetAsync(SongUri);
                            using (var fs = new FileStream(save.FileName, FileMode.CreateNew))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                    });
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"下载完成", ToastPosition.Top);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"下载失败：" + ex.Message, ToastPosition.Top);
                }
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = VolumeSlider.Value;

        }

        private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,Math.Round(VolumeSlider.Value * 100, 0).ToString() + "%", ToastPosition.Top);
            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            obj.PlayerVolume = VolumeSlider.Value;
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }

        private void Volume_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsVolumeSliderClose)
            {
                ThicknessAnimation Comein = new ThicknessAnimation()
                {
                    To = new Thickness(350, 0, 0, 0),
                    From = new Thickness(350, 70, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                VolumeSlider.BeginAnimation(MarginProperty, Comein);
                IsVolumeSliderClose = false;
            }
            else
            {
                IsVolumeSliderClose = true;
                ThicknessAnimation Comeout = new ThicknessAnimation()
                {
                    From = new Thickness(350, 0, 0, 0),
                    To = new Thickness(350, 70, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.3")
                };
                VolumeSlider.BeginAnimation(MarginProperty, Comeout);
            }
        }

        private void NextMusic_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var index = PlayListView.SelectedIndex;
            var count = PlayListView.Items.Count;
            if (index == count - 1)
            {
                PlayListView.SelectedItem = PlayListView.Items[0];
            }
            else
            {
                PlayListView.SelectedItem = PlayListView.Items[index + 1];
            }
        }

        private void BackMusic_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var index = PlayListView.SelectedIndex;
            var count = PlayListView.Items.Count;
            if (index == 0)
            {
                PlayListView.SelectedItem = PlayListView.Items[count - 1];
            }
            else
            {
                PlayListView.SelectedItem = PlayListView.Items[index - 1];
            }
        }

        private void LoopIcon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MusicLoop == "RepeatOff")
            {
                MusicLoop = "RepeatOne";
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"单曲循环", ToastPosition.Top);
                LoopIcon.Glyph = "\uE8ED";
            }
            else if (MusicLoop == "RepeatOne")
            {
                MusicLoop = "RepeatAll";
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"顺序播放", ToastPosition.Top);
                LoopIcon.Glyph = "\uE8EE";
            }
            else if (MusicLoop == "RepeatAll")
            {
                MusicLoop = "RepeatOff";
                Panuon.WPF.UI.Toast.Show(GlobalWindow.form_musicplayer,"不循环", ToastPosition.Top);
                LoopIcon.Glyph = "\uF5E7";
            }
        }
    }
}


