using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Class
{
    public class Lyrics
    {
        public TimeSpan Time { get; set; }
        public string Text { get; set; }
        public int Index { get; set; } // 添加了这一行
    }
    internal class SearchMusicListItem
    {
        public string? SongName { get; set; }
        public string? DisplayDuration { get; set; }
        public string? Authors { get; set; }
        public Int64 SongID { get; set; }
        public double Duration { get; set; }
    }
    public class Usefulness
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class Lrc
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Klyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Tlyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Romalrc
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class LyricApi
    {
        /// <summary>
        /// 
        /// </summary>
        public string sgc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Lrc lrc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Klyric klyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Tlyric tlyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Romalrc romalrc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

}
