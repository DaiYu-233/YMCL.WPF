using iNKORE.UI.WPF.Modern;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;
using YMCL.Main.Public;
using YMCL.Main.Views.MusicPlayer.DesktopLyric;
using Size = System.Windows.Size;
using Cursors = System.Windows.Input.Cursors;
using static YMCL.Main.Public.Class.PlaySongListViewItemEntry;

namespace YMCL.Main.Views.Main.Pages.MusicPlayer
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : Page
    {
        int page = 0;
        string key = string.Empty;
        List<PlaySongListViewItemEntry> playSongListViewItemEntries = [];
        PlaySongListViewItemEntry playingSong = null;
        DispatcherTimer timer = new DispatcherTimer();
        MediaPlayer player_for_duration = new MediaPlayer();
        MediaPlayer player = new MediaPlayer();
        string downloadUrl = string.Empty;
        bool gettingMusic = false;
        bool movingSilder = false;
        bool playing = false;
        DesktopLyric desktopLyric = new();
        Repeat repeat = Public.Class.Repeat.RepeatOff;
        private List<Lyrics> lyrics;
        private List<Run> lyricRuns;
        bool enabledDesktopLyric = false;
        private DispatcherTimer timerForLyric;
        public MusicPlayer()
        {
            InitializeComponent();
            playSongListViewItemEntries = JsonConvert.DeserializeObject<List<PlaySongListViewItemEntry>>(File.ReadAllText(Const.PlayListDataPath));
            playSongListViewItemEntries.ForEach(song =>
            {
                PlayListView.Items.Add(song);
            });
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.2);
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            repeat = setting.PlayerRepeat;
            UpdateRepeatUI(false);
            player.MediaEnded += (_, _) =>
            {
                if (repeat == Public.Class.Repeat.RepeatAll)
                {
                    if (PlayListView.Items.Count > 1)
                    {
                        Next_MouseDown(null, null);
                    }
                    else
                    {
                        player.Position = new TimeSpan(0);
                    }
                }
                else if (repeat == Public.Class.Repeat.RepeatOne)
                {
                    player.Position = new TimeSpan(0);
                }
            };
        }
        #region UI
        private void DesktopLyric_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Const.Window.desktopLyric.Change();
        }
        private void Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var index = PlayListView.SelectedIndex;
            var count = PlayListView.Items.Count;
            if (index == 0)
            {
                PlayListView.SelectedItem = PlayListView.Items[count - 1];
            }
            else
            {
                if (index < 0)
                {
                    PlayListView.SelectedItem = PlayListView.Items[count - 1];
                }
                else
                {
                    PlayListView.SelectedItem = PlayListView.Items[index - 1];
                }
            }
        }
        private void Lyric_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Lyric.Visibility == Visibility.Visible)
            {
                Lyric.Visibility = Visibility.Collapsed;
            }
            else
            {
                Lyric.Visibility = Visibility.Visible;
            }
        }
        private void Repeat_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (repeat == Public.Class.Repeat.RepeatOff)
            {
                repeat = Public.Class.Repeat.RepeatAll;
            }
            else if (repeat == Public.Class.Repeat.RepeatAll)
            {
                repeat = Public.Class.Repeat.RepeatOne;
            }
            else if (repeat == Public.Class.Repeat.RepeatOne)
            {
                repeat = Public.Class.Repeat.RepeatOff;
            }
            UpdateRepeatUI();
        }
        private void Next_MouseDown(object sender, MouseButtonEventArgs e)
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
        private void PlaySlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            movingSilder = true;
        }
        private void PlaySlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            movingSilder = false;
            player.Position = new TimeSpan(0, 0, 0, 0, milliseconds: (int)PlaySlider.Value);
        }
        private void Play_Click(object sender, MouseButtonEventArgs e)
        {
            if (playing)
            {
                UpdatePlayStatus(false);
                playing = false;
            }
            else
            {
                playing = true;
                UpdatePlayStatus(true);
            }
        }
        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上(Resize)
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));
        }
        private void AutoSuggestBox_QuerySubmitted(iNKORE.UI.WPF.Modern.Controls.AutoSuggestBox sender, iNKORE.UI.WPF.Modern.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            page = 0;
            key = SearchBox.Text;
            SearchForListView(key, page);
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
        public async void SearchForListView(string key, int page)
        {
            LoadMore.Visibility = Visibility.Collapsed;
            Loading.Visibility = Visibility.Visible;
            SearchBox.IsEnabled = false;
            SongsListView.Items.Clear();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36 Edg/91.0.864.54");
            var json = string.Empty;
            try
            {
                var url = $"http://music.api.daiyu.fun/cloudsearch?keywords={key}&offset={page * 30}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                SearchBox.IsEnabled = true;
                Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchFail}：{ex}", position: ToastPosition.Top, window: Const.Window.main);
                Loading.Visibility = Visibility.Collapsed;
                return;
            }
            var obj = JsonConvert.DeserializeObject<SearchSongEntry.Root>(json);
            if (obj.code == 200)
            {
                if (obj.result.songCount > 0)
                {
                    var songs = obj.result.songs.ToArray();
                    foreach (var song in songs)
                    {
                        var authors = string.Empty;
                        foreach (var author in song.ar)
                        {
                            authors += $"{author.name} ";
                        }
                        SongsListView.Items.Add(new SearchSongListViewItemEntry()
                        {
                            SongId = song.id,
                            SongName = song.name,
                            Authors = authors,
                            Img = song.al.picUrl,
                            DisplayDuration = Method.MsToTime(Convert.ToInt32(song.dt))
                        });
                    }
                }
                else
                {
                    Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchNoResult}", position: ToastPosition.Top, window: Const.Window.main);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchFail}", position: ToastPosition.Top, window: Const.Window.main);
            }
            if (SongsListView.Items.Count > 0)
            {
                LoadMore.Visibility = Visibility.Visible;
            }
            SearchBox.IsEnabled = true;
            Loading.Visibility = Visibility.Collapsed;
        }
        private async void LoadMore_Click(object sender, RoutedEventArgs e)
        {
            LoadingMore.Visibility = Visibility.Visible;
            LoadMore.Visibility = Visibility.Collapsed;
            SongsListViewScroll.ScrollToEnd();
            page++;
            SearchBox.IsEnabled = false;
            LoadMore.IsEnabled = false;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.114 Safari/537.36 Edg/91.0.864.54");
            var json = string.Empty;
            try
            {
                var url = $"http://music.api.daiyu.fun/cloudsearch?keywords={key}&offset={page * 30}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                SearchBox.IsEnabled = true;
                LoadMore.IsEnabled = true;
                Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchFail}：{ex}", position: ToastPosition.Top, window: Const.Window.main);
                LoadingMore.Visibility = Visibility.Collapsed;
                LoadMore.Visibility = Visibility.Visible;
                return;
            }
            var obj = JsonConvert.DeserializeObject<SearchSongEntry.Root>(json);
            if (obj.code == 200)
            {
                if (obj.result.songCount > 0)
                {
                    var songs = obj.result.songs.ToArray();
                    foreach (var song in songs)
                    {
                        var authors = string.Empty;
                        foreach (var author in song.ar)
                        {
                            authors += $"{author.name} ";
                        }
                        SongsListView.Items.Add(new SearchSongListViewItemEntry()
                        {
                            SongId = song.id,
                            SongName = song.name,
                            Authors = authors,
                            Img = song.al.picUrl,
                            DisplayDuration = Method.MsToTime(Convert.ToInt32(song.dt))
                        });
                    }
                }
                else
                {
                    Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchNoResult}", position: ToastPosition.Top, window: Const.Window.main);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(message: $"{MainLang.SearchFail}", position: ToastPosition.Top, window: Const.Window.main);
            }
            if (SongsListView.Items.Count > 0)
            {
                LoadMore.Visibility = Visibility.Visible;
            }
            SearchBox.IsEnabled = true;
            LoadMore.IsEnabled = true;
            LoadingMore.Visibility = Visibility.Collapsed;
            LoadMore.Visibility = Visibility.Visible;
        }
        private async void SongsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsListView.SelectedIndex < 0) return;
            var song = SongsListView.SelectedItem as SearchSongListViewItemEntry;
            using (var httpClient = new HttpClient())
            {
                byte[] bytes = await httpClient.GetByteArrayAsync(song.Img);
                await Dispatcher.BeginInvoke(() =>
                {
                    PlayListView.Items.Add(new PlaySongListViewItemEntry
                    {
                        SongId = song.SongId,
                        SongName = song.SongName,
                        Authors = song.Authors,
                        DisplayDuration = song.DisplayDuration,
                        Img = Method.BytesToBase64(bytes),
                        Type = PlaySongListViewItemEntry.PlaySongListViewItemEntryType.Network
                    }); playSongListViewItemEntries.Add(new PlaySongListViewItemEntry
                    {
                        SongId = song.SongId,
                        SongName = song.SongName,
                        Authors = song.Authors,
                        DisplayDuration = song.DisplayDuration,
                        Img = Method.BytesToBase64(bytes),
                        Type = PlaySongListViewItemEntry.PlaySongListViewItemEntryType.Network
                    });
                    if (PlayListView.Items.Count > 0)
                    {
                        PlayListView.SelectedIndex = PlayListView.Items.Count - 1;
                    }
                    else
                    {
                        PlayListView.SelectedIndex = -1;
                    }
                });
                File.WriteAllText(Const.PlayListDataPath, JsonConvert.SerializeObject(playSongListViewItemEntries, Formatting.Indented));
            }
        }
        string _theLastLocalSong = string.Empty;
        public async void AddLocalSong(string path)
        {
            if (_theLastLocalSong == path)
                return;
            _theLastLocalSong = path;
            string[] Array = path.Split(@"\");
            player_for_duration.Open(new Uri(path));
            bool HasTimeSpan = false;
            Toast.Show(Const.Window.main, MainLang.Account_Loading, ToastPosition.Top);
            while (!HasTimeSpan)
            {
                await Task.Delay(500);
                if (player_for_duration.NaturalDuration.HasTimeSpan)
                {
                    HasTimeSpan = true;
                }
            }
            var time = player_for_duration.NaturalDuration.TimeSpan.TotalMilliseconds;

            playSongListViewItemEntries.Add(new PlaySongListViewItemEntry()
            {
                Path = path,
                SongName = Array[Array.Length - 1].Split(".")[0],
                Type = PlaySongListViewItemEntry.PlaySongListViewItemEntryType.Local,
                Authors = Array[Array.Length - 1].Split(".")[1],
                Duration = time,
                DisplayDuration = Method.MsToTime(time),
                SongId = -1
            }); PlayListView.Items.Add(new PlaySongListViewItemEntry()
            {
                Path = path,
                SongName = Array[Array.Length - 1].Split(".")[0],
                Type = PlaySongListViewItemEntry.PlaySongListViewItemEntryType.Local,
                Authors = Array[Array.Length - 1].Split(".")[1],
                Duration = time,
                DisplayDuration = Method.MsToTime(time),
                SongId = -1
            });
            File.WriteAllText(Const.PlayListDataPath, JsonConvert.SerializeObject(playSongListViewItemEntries, Formatting.Indented));
        }
        private void AddLocalSongBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = $"{MainLang.SongFile}|*.wav;*.mp3;*.flac"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileNames.ToString().Trim().Equals("") || openFileDialog.FileNames.Length == 0)
            {
                return;
            }
            AddLocalSong(openFileDialog.FileName);
        }
        private void DelSongFromPlayList_Click(object sender, RoutedEventArgs e)
        {
            if (playSongListViewItemEntries.Count == 0) return;
            playSongListViewItemEntries.RemoveAt(PlayListView.SelectedIndex);
            PlayListView.Items.RemoveAt(PlayListView.SelectedIndex);
            if (PlayListView.Items.Count > 0)
            {
                PlayListView.SelectedIndex = 0;
            }
            else
            {
                PlayListView.SelectedIndex = -1;
            }
            UpdatePlayStatus(false);
            File.WriteAllText(Const.PlayListDataPath, JsonConvert.SerializeObject(playSongListViewItemEntries, Formatting.Indented));
        }
        private async void PlayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var song = PlayListView.SelectedItem as PlaySongListViewItemEntry;
            if (song == null) return;
            LyricBlock.Inlines.Clear();
            if (song.Type == PlaySongListViewItemEntryType.Local)
            {
                if (!File.Exists(song.Path))
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.SourceFileNotExists, ToastPosition.Top);
                    playing = false;
                    PlaySlider.Maximum = 0;
                    TimeText.Text = $"00:00/00:00";
                    PlaySlider.Value = 0;
                    PlayingSongAuthor.Text = string.Empty;
                    PlayingSongName.Text = string.Empty;
                    return;
                }
                else
                {
                    Play(song.Duration, song, song.Path);
                }
            }
            else
            {
                var availability = false;
                #region CheckSong
                Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.CheckSongAvailability, ToastPosition.Top);
                using var client = new HttpClient();
                var res = "";
                var url = $"http://music.api.daiyu.fun/check/music?id={song.SongId}";
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    res = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.CannotConnectToApi, ToastPosition.Top);
                    SearchBox.IsEnabled = true;
                    gettingMusic = false;
                    return;
                }
                var obj = JsonConvert.DeserializeObject<Usefulness>(res);
                if (!obj.success)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.MusicUnavailable, ToastPosition.Top);
                    availability = false;
                }
                else
                {
                    availability = true;
                }
                if (!availability)
                {
                    gettingMusic = false;
                    if (repeat == Public.Class.Repeat.RepeatAll)
                    {
                        Next_MouseDown(null, null);
                    }
                    return;
                }
                #endregion
                Toast.Show(Const.Window.main, MainLang.GettingMusic, ToastPosition.Top);
                using var client1 = new HttpClient();
                var res1 = "";
                var url1 = $"http://music.api.daiyu.fun/song/url?id={song.SongId}";
                try
                {
                    HttpResponseMessage response = await client1.GetAsync(url1);
                    response.EnsureSuccessStatusCode();
                    res1 = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    Toast.Show(Const.Window.main, MainLang.CannotConnectToApi, ToastPosition.Top);
                    gettingMusic = false;
                    return;
                }
                var obj1 = JsonConvert.DeserializeObject<NetWorkSong>(res1);

                Toast.Show(Const.Window.main, MainLang.GettingLyric, ToastPosition.Top);
                using var client2 = new HttpClient();
                var res2 = "";
                try
                {
                    var url2 = $"https://music.api.daiyu.fun/lyric?id={song.SongId}";
                    HttpResponseMessage response = await client2.GetAsync(url2);
                    response.EnsureSuccessStatusCode();
                    res2 = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.CannotConnectToApi, ToastPosition.Top);
                    SearchBox.IsEnabled = true;
                    return;
                }
                var obj2 = JsonConvert.DeserializeObject<LyricApi>(res2);
                var data = obj2.lrc.lyric;

                var seletedSong = PlayListView.SelectedItem as PlaySongListViewItemEntry;
                if ((song.Type == PlaySongListViewItemEntryType.Network && seletedSong.SongId != song.SongId) || (song.Type == PlaySongListViewItemEntryType.Local && seletedSong.Path != song.Path))
                {
                    return;
                }

                lyrics = ParseLyrics(data); // 歌词数据

                // 创建一个Run元素来展示每一句歌词
                lyricRuns = new List<Run>();
                ResourceDictionary appResources = System.Windows.Application.Current.Resources;
                foreach (var lyric in lyrics)
                {
                    var run = new Run(lyric.Text + "\n");
                    long milliseconds = (long)lyric.Time.TotalMilliseconds;
                    run.Tag = milliseconds;
                    run.Foreground = (SolidColorBrush)appResources["TextColor"];
                    run.MouseDown += Run_MouseDown; ;
                    LyricBlock.Inlines.Add(run);
                    lyricRuns.Add(run);

                }

                timerForLyric = new DispatcherTimer();
                timerForLyric.Interval = TimeSpan.FromSeconds(0.2);
                timerForLyric.Tick += TimerForLyric_Tick; ; ;
                timerForLyric.Start();

                Play(obj1.data[0].time, song, obj1.data[0].url);
                gettingMusic = false;
            }
        }
        private void Run_MouseDown(object send, MouseButtonEventArgs e)
        {
            var sender = send as Run;
            var res = Convert.ToUInt64(sender.Tag.ToString());
            PlaySlider.Value = res;
            var item = PlayListView.SelectedItem as PlaySongListViewItemEntry;
            player.Position = new TimeSpan(0, 0, 0, 0, milliseconds: (int)PlaySlider.Value);
            ResourceDictionary appResources = System.Windows.Application.Current.Resources;
            lyricRuns.ForEach(x =>
            {
                x.Foreground = (SolidColorBrush)appResources["TextColor"];
                x.FontWeight = FontWeights.Normal;
            });
        }
        private void TimerForLyric_Tick(object? sender, EventArgs e)
        {
            if (PlayListView.SelectedIndex == -1)
            {
                player.Close();
                timer.Stop();
                PlayBtnIcon.Glyph = "\uF5B0";
                PlayBtnIcon.Margin = new Thickness(0, 0, -2, 0);
                playing = false;
                PlaySlider.Maximum = 0;
                TimeText.Text = $"00:00/00:00";
                PlaySlider.Value = 0;
                PlayingSongAuthor.Text = "";
                Pic.Source = null;
                PlayingSongName.Text = "";
            }
            // 获取当前播放的歌曲进度
            TimeSpan currentTime = TimeSpan.FromMilliseconds(PlaySlider.Value);
            // 找到当前应该显示的歌词
            for (int i = 1; i < lyrics.Count; i++)
            {
                //if (i == 0) { continue; }
                LyricBlock.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    From = LyricBlock.Margin,
                    To = new Thickness(0, LyricRoot.ActualHeight / 2 - i * 24, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.2")
                });
                ResourceDictionary appResources = System.Windows.Application.Current.Resources;
                if (movingSilder)
                {
                    lyricRuns.ForEach(x =>
                    {
                        x.Foreground = (SolidColorBrush)appResources["TextColor"];
                        x.FontWeight = FontWeights.Normal;
                    });
                }
                if (lyrics[i].Time > currentTime)
                {
                    // 更新Run元素的样式
                    lyricRuns.ForEach(x =>
                    {
                        x.Foreground = (SolidColorBrush)appResources["TextColor"];
                        x.FontWeight = FontWeights.Normal;
                    });
                    lyricRuns[i - 1].Foreground = new SolidColorBrush((System.Windows.Media.Color)ThemeManager.Current.AccentColor);
                    Const.Window.desktopLyric.Lyric.Text = lyricRuns[i - 1].Text;
                    Const.Window.desktopLyric.Lyric.Foreground = new SolidColorBrush((System.Windows.Media.Color)ThemeManager.Current.AccentColor);
                    Const.Window.desktopLyric.Topmost = true;
                    lyricRuns[i - 1].FontWeight = FontWeights.Normal;
                    break;
                }
            }
        }
        void Play(double time, PlaySongListViewItemEntry song, string url)
        {
            var seletedSong = PlayListView.SelectedItem as PlaySongListViewItemEntry;
            if ((song.Type == PlaySongListViewItemEntryType.Network && seletedSong.SongId != song.SongId) || (song.Type == PlaySongListViewItemEntryType.Local && seletedSong.Path != song.Path))
            {
                return;
            }
            if (song == playingSong)
            {
                return;
            }
            downloadUrl = url;
            player.Open(new Uri(url));
            timer.Start();
            PlayBtnIcon.Glyph = "\uF8AE";
            PlayBtnIcon.Margin = new Thickness(0);
            playing = true;
            player.Play();
            PlaySlider.Maximum = time;
            TimeText.Text = $"00:00/{Method.MsToTime(time)}";
            PlaySlider.Value = 0;
            PlayingSongAuthor.Text = song.Authors;
            PlayingSongName.Text = song.SongName;
            PlaySlider.IsEnabled = true;
            player.Position = new TimeSpan(0);
            try
            {
                Pic.Source = Method.Base64ToImage(song.Img);
            }
            catch { Pic.Source = null; }
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            var nowtime = Method.MsToTime(player.Position.TotalMilliseconds);
            TimeText.Text = $"{nowtime.Split(".")[0]}/{Method.MsToTime(PlaySlider.Maximum)}";
            if (!movingSilder)
            {
                PlaySlider.Value = player.Position.TotalMilliseconds;
            }
        }
        public void UpdatePlayStatus(bool status)
        {
            var x = gettingMusic;
            while (x)
            {
                if (!gettingMusic)
                {
                    x = false;
                }
            }
            if (!status)
            {
                PlayBtnIcon.Glyph = "\uF5B0";
                PlayBtnIcon.Margin = new Thickness(2, 0, 0, 0);
                PlayBtnIcon.FontSize = 22;
                player.Pause();
                timer.Stop();
                playing = false;
            }
            else
            {
                playing = true;
                player.Play();
                timer.Start();
                PlayBtnIcon.Glyph = "\uF8AE";
                PlayBtnIcon.Margin = new Thickness(0);
                PlayBtnIcon.FontSize = 20;
            }
        }
        public List<Lyrics> ParseLyrics(string lyricsText)
        {
            var lines = lyricsText.Split('\n');
            var lyrics = new List<Lyrics>();
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var parts = line.Split(']');
                if (parts.Length < 2) continue;
                var timeText = parts[0].TrimStart('[');
                var time = ParseTime(timeText);
                var text = parts[1];
                if (!string.IsNullOrEmpty(text))
                {
                    lyrics.Add(new Lyrics { Time = time, Text = text, Index = i });
                }
            }
            return lyrics;
        }
        TimeSpan ParseTime(string timeText)
        {
            var parts = timeText.Split(':');
            var minutes = int.Parse(parts[0]);
            var secondsAndMilliseconds = parts[1].Split('.');
            var seconds = int.Parse(secondsAndMilliseconds[0]);
            var milliseconds = int.Parse(secondsAndMilliseconds[1]);
            return new TimeSpan(0, 0, minutes, seconds, milliseconds);
        }
        public void UpdateRepeatUI(bool save = true)
        {
            if (repeat == Public.Class.Repeat.RepeatOff)
            {
                Repeat.Glyph = "\uF5E7";
            }
            else if (repeat == Public.Class.Repeat.RepeatAll)
            {
                Repeat.Glyph = "\uE8EE";
            }
            else if (repeat == Public.Class.Repeat.RepeatOne)
            {
                Repeat.Glyph = "\uE8ED";
            }
            if (save)
            {
                var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
                setting.PlayerRepeat = repeat;
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            }
        }
        private async void Download_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var song = PlayListView.SelectedItem as PlaySongListViewItemEntry;
            if (song == null)
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.NoChooseSong, ToastPosition.Top);
                return;
            }
            var SaveFileName = song.SongName;
            SaveFileDialog save = new SaveFileDialog();
            save.Title = MainLang.SaveMusicFile;
            save.Filter = "File (*.mp3)|*.mp3|File (*.wav)|*.wav|All Files (*.*)|*.*";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            save.FileName = SaveFileName;
            save.ShowDialog();
            if (save.FileName == SaveFileName)
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.SaveCancel, ToastPosition.Top);
                return;
            }
            if (song.Type == PlaySongListViewItemEntry.PlaySongListViewItemEntryType.Local)
            {
                if (!File.Exists(song.Path))
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.SourceFileNotExists, ToastPosition.Top);
                    return;
                }
                string sourceFile = song.Path;
                string destinationFile = save.FileName;
                try
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, $"{MainLang.CopyFileFail}：" + ex.Message, ToastPosition.Top);
                }
            }
            else
            {
                try
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.BeginDownload, ToastPosition.Top);
                    await Task.Run(async () =>
                    {
                        using (var client = new HttpClient())
                        {
                            var response = await client.GetAsync(downloadUrl);
                            using (var fs = new FileStream(save.FileName, FileMode.CreateNew))
                            {
                                await response.Content.CopyToAsync(fs);
                            }
                        }
                    });
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, MainLang.InitializeWindow_DownloadFinish, ToastPosition.Top);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.main, $"{MainLang.DownloadFail}：" + ex.Message, ToastPosition.Top);
                }
            }
        }

        private void Page_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Const.Window.main.DropMethod(e);
        }

        private void Voiume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = Voiume.Value / 100;
        }
    }
}
