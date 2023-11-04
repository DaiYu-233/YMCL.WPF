using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Class
{
    internal class NetSongsItem
    {
        public class ArtistsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public double id { get; set; }
            /// <summary>
            /// 赵品霖
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string picUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> @alias { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double albumSize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double picId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fansGroup { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string img1v1Url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double img1v1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string trans { get; set; }
        }

        public class Artist
        {
            /// <summary>
            /// 
            /// </summary>
            public double id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string picUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> @alias { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double albumSize { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double picId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string fansGroup { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string img1v1Url { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double img1v1 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string trans { get; set; }
        }

        public class Album
        {
            /// <summary>
            /// 
            /// </summary>
            public double id { get; set; }
            /// <summary>
            /// 以团之名 第一期
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Artist artist { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double publishTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double size { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double copyrightId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double picId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double mark { get; set; }
        }

        public class SongsItem
        {
            /// <summary>
            /// 
            /// </summary>
            public Int64 id { get; set; }
            /// <summary>
            /// 只因你太美 (Live)
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<ArtistsItem> artists { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Album album { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double duration { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double copyrightId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<string> @alias { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double rtype { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double ftype { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double mvid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double fee { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string rUrl { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double mark { get; set; }
        }

        public class Result
        {
            /// <summary>
            /// 
            /// </summary>
            public List<SongsItem> songs { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string hasMore { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double songCount { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public Result result { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double code { get; set; }
        }


    }
}
