using System;
using System.Windows.Media;

namespace YMCL.Class
{
    public class PlayMusicListItem
    {
        public string? SongName { get; set; }
        public string? DisplayDuration { get; set; }
        public double Duration { get; set; }
        public string? Type { get; set; }
        public string? Authors { get; set; }
        public string? Path { get; set; }
        public Int64 SongID { get; set; }
        public string? Pic { get; set; }
    }
}
