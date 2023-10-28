using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.RegularExpressions;
using YMCL.Class;

namespace YMCL.Pages.Windows
{
    /// <summary>
    /// ToolsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayerWindow : WindowX
    {
        string musicLoopType;
        string downloadUrl;
        bool isMovingSilder = false;
        bool isGettingMusic = false;
        bool isOpenVolume = false;

        private List<Lyrics> lyrics;
        private List<Run> lyricRuns;
        private DispatcherTimer timer1;
        public MusicPlayerWindow()
        {
            InitializeComponent();
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            musicLoopType = setting.MusicLoopType;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.2);
            player.MediaEnded += Player_MediaEnded;


        }
        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            //将装饰器添加到窗口的Content控件上(Resize)
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));
            LoadMusicLoop();
            LoadPlayList();
            VolumeSlider.Value = setting.Volume;
        }

        bool isPlaying = false;
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
        #endregion
        #region UI
        private async void DownloadIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Class.PlayMusicListItem song = PlayListView.SelectedItem as Class.PlayMusicListItem;
            if (song == null)
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "尚未选择音乐", ToastPosition.Top);
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
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "已取消保存", ToastPosition.Top);
                return;
            }
            if (song.Type == "本地")
            {
                if (!File.Exists(song.Path))
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "源文件不存在", ToastPosition.Top);
                    return;
                }
                string sourceFile = song.Path;
                string destinationFile = save.FileName;
                try
                {
                    System.IO.File.Copy(sourceFile, destinationFile, true);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "复制文件失败：" + ex.Message, ToastPosition.Top);
                }
            }
            else
            {
                try
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "开始下载", ToastPosition.Top);
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
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "下载完成", ToastPosition.Top);
                }
                catch (Exception ex)
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "下载失败：" + ex.Message, ToastPosition.Top);
                }
            }
        }
        private void DelSongBtn_Click(object sender, RoutedEventArgs e)
        {
            playList.RemoveAt(PlayListView.SelectedIndex);
            string str = JsonConvert.SerializeObject(playList, Formatting.Indented);
            File.WriteAllText(Const.YMCLSongPlayListDataPath, str);
            LoadPlayList();


        }
        private void PlayBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPlaying)
            {
                PlayBtnIcon.Glyph = "\uF5B0";
                PlayBtnIcon.Margin = new Thickness(0, 0, -2, 0);

                isPlaying = false;
                player.Pause();
            }
            else
            {
                isPlaying = true;
                PlayBtnIcon.Glyph = "\uF8AE";
                PlayBtnIcon.Margin = new Thickness(0);
                player.Play();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;

            }
            else
            {
                this.WindowState = WindowState.Maximized;

            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WindowX_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            //PlayBtn.Background = new SolidColorBrush((Color)setting.ThemeColor);
        }

        bool isOpenPlayList = false;
        private void OpenPlayListBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isOpenPlayList)
            {
                PlayListBorder.BeginAnimation(MarginProperty, new System.Windows.Media.Animation.ThicknessAnimation()
                {
                    To = new Thickness(0, 35, 10, 80),
                    From = new Thickness(0, 35, -265, 80),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                MainGrid.BeginAnimation(MarginProperty, new System.Windows.Media.Animation.ThicknessAnimation()
                {
                    To = new Thickness(0, 30, 270, 75),
                    From = new Thickness(0, 30, 0, 75),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                isOpenPlayList = true;
            }
            else
            {
                PlayListBorder.BeginAnimation(MarginProperty, new System.Windows.Media.Animation.ThicknessAnimation()
                {
                    From = new Thickness(0, 35, 10, 80),
                    To = new Thickness(0, 35, -265, 80),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                MainGrid.BeginAnimation(MarginProperty, new System.Windows.Media.Animation.ThicknessAnimation()
                {
                    From = new Thickness(0, 30, 270, 75),
                    To = new Thickness(0, 30, 0, 75),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                isOpenPlayList = false;
            }
        }
        private void Player_MediaEnded(object? sender, EventArgs e)
        {
            if (isGettingMusic)
            {
                return;
            }
            if (musicLoopType == "RepeatOff")
            {
                return;
            }
            else if (musicLoopType == "RepeatOne")
            {
                player.Position = new TimeSpan(0);
            }
            else if (musicLoopType == "RepeatAll")
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
        private void NextMusic_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void BackMusic_MouseDown(object sender, MouseButtonEventArgs e)
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
        private void Timer_Tick(object? sender, EventArgs e)
        {
            var nowtime = Millisecond_to_minutes(player.Position.TotalMilliseconds);
            TimeText.Text = $"{nowtime.Split(".")[0]}/{Millisecond_to_minutes(PlaySlider.Maximum)}";
            if (!isMovingSilder)
            {
                PlaySlider.Value = player.Position.TotalMilliseconds;
            }
        }

        private void SearchBox_QuerySubmitted(ModernWpf.Controls.AutoSuggestBox sender, ModernWpf.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Search(SearchBox.Text, 0);
        }

        private void PlaySlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMovingSilder = true;
        }

        private void PlaySlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMovingSilder = false;
            var item = PlayListView.SelectedItem as Class.PlayMusicListItem;
            player.Position = new TimeSpan(0, 0, 0, 0, milliseconds: (int)PlaySlider.Value);
            if (item.Type == "网易云")
            {
                lyricRuns.ForEach(x =>
                {
                    x.Foreground = Brushes.Black;
                    x.FontWeight = FontWeights.Normal;
                });
            }

        }
        private async void PlayListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LyricBlock.Text = "";
            isGettingMusic = true;
            var item = PlayListView.SelectedItem as Class.PlayMusicListItem;
            if (PlayListView.SelectedIndex >= 0)
            {
                DelSongBtn.IsEnabled = true;
                if (item == null)
                {
                    isGettingMusic = false;
                    return;
                }
                if (item.Type == "本地")
                {
                    if (!File.Exists(item.Path))
                    {
                        Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, $"源文件不存在，它可能被移动、重命名或删除", ToastPosition.Top);
                        isGettingMusic = false;
                        player.Close();
                        timer.Stop();
                        PlayBtnIcon.Glyph = "\uF5B0";
                        PlayBtnIcon.Margin = new Thickness(0, 0, -2, 0);
                        isPlaying = false;
                        PlaySlider.Maximum = 0;
                        TimeText.Text = $"00:00/00:00";
                        PlaySlider.Value = 0;
                        PlayingSongAuthors.Text = "";
                        PlayingSongName.Text = "";
                        return;
                    }
                    Play(item.Duration, item.Authors, item.SongName, item.Path);
                }
                else
                {
                    var availability = false;
                    #region CheckSong
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "检测音乐可用性...", ToastPosition.Top);
                    using var client = new HttpClient();
                    var res = "";
                    var url = $"http://music.api.daiyu.fun/check/music?id={item.SongID}";
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        res = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception)
                    {
                        Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "无法连接到api", ToastPosition.Top);
                        SearchBox.IsEnabled = true;
                        isGettingMusic = false;
                        return;
                    }
                    var obj = JsonConvert.DeserializeObject<Class.Usefulness>(res);
                    if (!obj.success)
                    {
                        Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "音乐不可用", ToastPosition.Top);
                        availability = false;
                    }
                    else
                    {
                        availability = true;
                    }
                    if (!availability)
                    {
                        isGettingMusic = false;
                        return;
                    }
                    #endregion
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "获取音乐...", ToastPosition.Top);
                    using var client1 = new HttpClient();
                    var res1 = "";
                    var url1 = $"http://music.api.daiyu.fun/song/url?id={item.SongID}";
                    try
                    {
                        HttpResponseMessage response = await client1.GetAsync(url1);
                        response.EnsureSuccessStatusCode();
                        res1 = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception)
                    {
                        Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "无法连接到api", ToastPosition.Top);
                        isGettingMusic = false;
                        return;
                    }
                    var obj1 = JsonConvert.DeserializeObject<Class.NetSongUrl>(res1);

                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "获取歌词...", ToastPosition.Top);
                    using var client2 = new HttpClient();
                    var res2 = "";
                    try
                    {
                        var url2 = $"https://music.api.daiyu.fun/lyric?id={item.SongID}";
                        HttpResponseMessage response = await client2.GetAsync(url2);
                        response.EnsureSuccessStatusCode();
                        res2 = await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception)
                    {
                        Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "无法连接到api", ToastPosition.Top);
                        SearchBox.IsEnabled = true;
                        return;
                    }
                    var obj2 = JsonConvert.DeserializeObject<Class.LyricApi>(res2);
                    var data = obj2.lrc.lyric;

                    lyrics = ParseLyrics(data); // 歌词数据

                    // 创建一个Run元素来展示每一句歌词
                    lyricRuns = new List<Run>();
                    foreach (var lyric in lyrics)
                    {
                        var run = new Run(lyric.Text + "\n");
                        LyricBlock.Inlines.Add(run);
                        lyricRuns.Add(run);
                    }

                    timer1 = new DispatcherTimer();
                    timer1.Interval = TimeSpan.FromSeconds(1);
                    timer1.Tick += Timer1_Tick; ;
                    timer1.Start();

                    Play(obj1.data[0].time, item.Authors, item.SongName, obj1.data[0].url);
                    isGettingMusic = false;
                }
            }
            else
            {
                isGettingMusic = false;
                DelSongBtn.IsEnabled = false;

                player.Close();
                timer.Stop();
                PlayBtnIcon.Glyph = "\uF5B0";
                PlayBtnIcon.Margin = new Thickness(0, 0, -2, 0);
                isPlaying = false;
                PlaySlider.Maximum = 0;
                TimeText.Text = $"00:00/00:00";
                PlaySlider.Value = 0;
                PlayingSongAuthors.Text = "";
                PlayingSongName.Text = "";
            }
        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {
            // 获取当前播放的歌曲进度
            TimeSpan currentTime = TimeSpan.FromMilliseconds(PlaySlider.Value);

            // 找到当前应该显示的歌词
            for (int i = 0; i < lyrics.Count; i++)
            {
                LyricBlock.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    From = LyricBlock.Margin,
                    To = new Thickness(0, MainGrid.ActualHeight / 2 - i * 26, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.2")
                });
                if (lyrics[i].Time > currentTime)
                {
                    // 更新Run元素的样式
                    lyricRuns[i - 1].Foreground = Brushes.Black;
                    lyricRuns[i - 1].FontWeight = FontWeights.Normal;

                    lyricRuns[i].Foreground = Brushes.Red;
                    lyricRuns[i].FontWeight = FontWeights.Normal;
                    break;
                }
            }
        }
        #endregion UI
        #region Save&LoadData
        void LoadPlayList()
        {
            PlayListView.Items.Clear();
            playList.Clear();
            var obj = JsonConvert.DeserializeObject<List<Class.PlayMusicListItem>>(File.ReadAllText(Const.YMCLSongPlayListDataPath));
            foreach (var item in obj)
            {
                PlayListView.Items.Add(item);
                playList.Add(item);
            }
        }
        private void LoopBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (musicLoopType == "RepeatOff")
            {
                musicLoopType = "RepeatOne";
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "单曲循环", ToastPosition.Top);
                LoopBtn.Glyph = "\uE8ED";
            }
            else if (musicLoopType == "RepeatOne")
            {
                musicLoopType = "RepeatAll";
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "顺序播放", ToastPosition.Top);
                LoopBtn.Glyph = "\uE8EE";
            }
            else if (musicLoopType == "RepeatAll")
            {
                musicLoopType = "RepeatOff";
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "不循环", ToastPosition.Top);
                LoopBtn.Glyph = "\uF5E7";
            }
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.MusicLoopType = musicLoopType;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void VolumeBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isOpenVolume)
            {
                VolumeSlider.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    From = new Thickness(350, 0, 0, 0),
                    To = new Thickness(350, 70, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.15")
                });
                isOpenVolume = false;
            }
            else
            {
                VolumeSlider.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    To = new Thickness(350, 0, 0, 0),
                    From = new Thickness(350, 70, 0, 0),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                isOpenVolume = true;
            }
        }
        void LoadMusicLoop()
        {
            if (musicLoopType == "RepeatOff")
            {
                musicLoopType = "RepeatOne";
                LoopBtn.Glyph = "\uE8ED";
            }
            else if (musicLoopType == "RepeatOne")
            {
                musicLoopType = "RepeatAll";
                LoopBtn.Glyph = "\uE8EE";
            }
            else if (musicLoopType == "RepeatAll")
            {
                musicLoopType = "RepeatOff";
                LoopBtn.Glyph = "\uF5E7";
            }

        }


        private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, $"{Math.Round(VolumeSlider.Value * 100, 0)} %", ToastPosition.Top);
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.Volume = VolumeSlider.Value;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }


        #endregion
        #region Funtions
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
        #endregion
        #region GetLocalSongs

        List<Class.PlayMusicListItem> playList = new List<Class.PlayMusicListItem>();
        MediaPlayer player_for_duration = new MediaPlayer();
        private void AddLocalFileBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "音频文件|*.wav;*.mp3;*.flac"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileNames.ToString().Trim().Equals("") || openFileDialog.FileNames.Length == 0)
            {
                return;
            }
            string[] Array = openFileDialog.FileName.Split(@"\");

            player_for_duration.Open(new Uri(openFileDialog.FileName));
            bool HasTimeSpan = false;
            Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "加载中...", ToastPosition.Top);
            while (!HasTimeSpan)
            {
                if (player_for_duration.NaturalDuration.HasTimeSpan)
                {
                    HasTimeSpan = true;
                }
            }
            var time = player_for_duration.NaturalDuration.TimeSpan.TotalMilliseconds;

            playList.Add(new Class.PlayMusicListItem()
            {
                Path = openFileDialog.FileName,
                SongName = Array[Array.Length - 1].Split(".")[0],
                Type = "本地",
                Authors = Array[Array.Length - 1].Split(".")[1],
                Duration = time,
                DisplayDuration = Millisecond_to_minutes(time),
                Pic = null,
                SongID = -1
            });

            string str = JsonConvert.SerializeObject(playList, Formatting.Indented);
            File.WriteAllText(Const.YMCLSongPlayListDataPath, str);
            LoadPlayList();
        }






        #endregion
        #region GetNetSongs

        async void Search(string keyword, int offset)
        {
            SearchBox.IsEnabled = false;
            SongsListView.Items.Clear();
            Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "搜索中...", ToastPosition.Top);
            using var client = new HttpClient();
            var res = "";
            try
            {
                var url = $"http://music.api.daiyu.fun/search?keywords={keyword}&offset={offset * 30}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                res = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "无法连接到api", ToastPosition.Top);
                SearchBox.IsEnabled = true;
                return;
            }


            var obj = JsonConvert.DeserializeObject<Class.NetSongsItem.Root>(res);
            if (obj.code == 200)
            {
                if (obj.result.songCount > 0)
                {
                    var songs = obj.result.songs.ToArray();
                    foreach (var item in songs)
                    {
                        var strAuthors = "";
                        var authors = item.artists.ToArray();
                        foreach (var author in authors)
                        {
                            strAuthors += author.name + " ";
                        }
                        SongsListView.Items.Add(new Class.SearchMusicListItem()
                        {
                            SongName = item.name,
                            Authors = strAuthors,
                            DisplayDuration = Millisecond_to_minutes(item.duration),
                            SongID = item.id,
                            Duration = item.duration
                        });

                    }
                }
                else
                {
                    Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "搜索无结果", ToastPosition.Top);
                }
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(Const.Window.musicPlayer, "搜索失败", ToastPosition.Top);
            }
            SearchBox.IsEnabled = true;
        }

        private void SongsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SongsListView.SelectedItem as Class.SearchMusicListItem;
            playList.Add(new Class.PlayMusicListItem()
            {
                Path = null,
                SongName = item.SongName,
                Type = "网易云",
                Authors = item.Authors,
                Duration = item.Duration,
                DisplayDuration = item.DisplayDuration,
                Pic = null,
                SongID = item.SongID
            });

            string str = JsonConvert.SerializeObject(playList, Formatting.Indented);
            File.WriteAllText(Const.YMCLSongPlayListDataPath, str);
            LoadPlayList();

            if (PlayListView.Items.Count > 0)
            {
                PlayListView.SelectedIndex = PlayListView.Items.Count - 1;
            }
        }


        #endregion


        DispatcherTimer timer = new DispatcherTimer();
        MediaPlayer player = new MediaPlayer();
        void Play(double time, string authors, string name, string url)
        {
            downloadUrl = url;
            player.Open(new Uri(url));
            timer.Start();
            PlayBtnIcon.Glyph = "\uF8AE";
            PlayBtnIcon.Margin = new Thickness(0);
            isPlaying = true;
            player.Play();
            PlaySlider.Maximum = time;
            TimeText.Text = $"00:00/{Millisecond_to_minutes(time)}";
            PlaySlider.Value = 0;
            PlayingSongAuthors.Text = authors;
            PlayingSongName.Text = name;
        }

        bool isOpenLyric = true;
        private void LyricIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Lyric.Visibility = Visibility.Visible;
            var height = Lyric.ActualHeight + 100;
            if (isOpenLyric)
            {
                Lyric.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    To = new Thickness(5, 5, 5, 0),
                    From = new Thickness(5, 5, 5, height),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                isOpenLyric = false;
            }
            else
            {
                Lyric.BeginAnimation(MarginProperty, new ThicknessAnimation()
                {
                    From = new Thickness(5, 5, 5, 0),
                    To = new Thickness(5, 5, 5, height),
                    Duration = TimeSpan.Parse("0:0:0.3")
                });
                isOpenLyric = true;
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
    }
}

