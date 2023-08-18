using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
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
using static PInvoke.Kernel32;
using static YMCL.Class.NeteasyCloudMusic;

namespace YMCL.Pages.MorePages
{
    /// <summary>
    /// MusicPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MusicPlayer : Page
    {
        List<DisplaySongs> displaySongs = new List<DisplaySongs>();
        string VarSongID = "";
        string VarSongName = "";
        string VarArtists = "";
        string VarAlbumName = "";
        string VarDuration = "";
        string SongUrl = "";
        bool IsPlay = false;
        int Page = 1;
        bool IsNoPlay = false;
        bool Playing = false;
        MediaPlayer player = new MediaPlayer();//实例化绘图媒体

        public MusicPlayer()
        {
            InitializeComponent();
        }
        public class Response
        {
            public bool success { get; set; }
            public string message { get; set; }
        }
        class DisplaySongs
        {
            public string? SongID { get; set; }
            public string? SongName { get; set; }
            public string? Artists { get; set; }
            public string? AlbumName { get; set; }
            public string? Duration { get; set; }
        }

        void Player()
        {
            if (IsNoPlay)
            {
                return;
            }
            player.Open(new Uri(SongUrl));
            IsPlay = true;
            player.Play();//播放媒体
            Playing = true;
            PlayerBtn.IsEnabled = true;
            PlayerBtn.Content = "暂停";
        }
        async void Search()
        {
            PlayerBtn.IsEnabled = false;
            //displaySongs.Clear();
            //SongsListView.ItemsSource = displaySongs;
            SongsListView.SelectedIndex = -1;
            SongsListView.Items.Clear();
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                Panuon.WPF.UI.Toast.Show($"搜索内容不可为空", ToastPosition.Top);
                return;
            }
            SearchBtn.IsEnabled = false;
            NextPageBtn.IsEnabled = false;
            BackPageBtn.IsEnabled = false;
            Panuon.WPF.UI.Toast.Show($"搜索中...", ToastPosition.Top);
            var SearchText = SearchTextBox.Text;
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
                                Dispatcher.BeginInvoke(() => { Toast.Show("请求错误", ToastPosition.Top); });
                                return;
                            }
                            if (root.result.songCount == 0)
                            {
                                Dispatcher.BeginInvoke(() => { Toast.Show("搜索无结果", ToastPosition.Top); });
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

                                VarSongID = song.id.ToString();
                                VarSongName = song.name;
                                VarArtists = artists_str;
                                VarAlbumName = song.album.name;
                                VarDuration = time;

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

            SearchBtn.IsEnabled = true;
            NextPageBtn.IsEnabled = true;
            BackPageBtn.IsEnabled = true;
        }

        private async void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Page = 1;
            PageText.Text = Page.ToString();
            Search();
            
        }

        private async void SongsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayerBtn.IsEnabled = false;
            IsNoPlay = false;
            if (SongsListView.SelectedIndex < 0)
            {
                return;
            }
            if (IsPlay)
            {
                player.Close();
                IsPlay = false;
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
                            Dispatcher.BeginInvoke(() => { Toast.Show("请求错误", ToastPosition.Top); });
                            return;
                        }
                        SongUrl = root.data[0].url;


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
            Player();
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

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Page = 1;
                PageText.Text = Page.ToString();
                Search();
            }
        }

        private void PlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Playing)
            {
                player.Pause();
                Playing = false;
                PlayerBtn.Content = "播放";
            }
            else
            {
                player.Play();
                
                Playing = true;
                PlayerBtn.Content = "暂停";
            }
        }
    }
}
