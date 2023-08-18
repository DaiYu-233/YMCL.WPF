using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Class
{
    class NeteasyCloudMusic
    {
        public class Artist
        {
            public int id { get; set; }
            public string name { get; set; }
            public object picUrl { get; set; }
            public object[] alias { get; set; }
            public int albumSize { get; set; }
            public long picId { get; set; }
            public object fansGroup { get; set; }
            public string img1v1Url { get; set; }
            public int img1v1 { get; set; }
            public object trans { get; set; }
        }

        public class Album
        {
            public int id { get; set; }
            public string name { get; set; }
            public Artist artist { get; set; }
            public long publishTime { get; set; }
            public int size { get; set; }
            public int copyrightId { get; set; }
            public int status { get; set; }
            public long picId { get; set; }
            public int mark { get; set; }
        }

        public class Song
        {
            public int id { get; set; }
            public string name { get; set; }
            public Artist[] artists { get; set; }
            public Album album { get; set; }
            public int duration { get; set; }
            public int copyrightId { get; set; }
            public int status { get; set; }
            public object[] alias { get; set; }
            public int rtype { get; set; }
            public int ftype { get; set; }
            public int mvid { get; set; }
            public int fee { get; set; }
            public object rUrl { get; set; }
            public int mark { get; set; }
        }

        public class Result
        {
            public Song[] songs { get; set; }
            public bool hasMore { get; set; }
            public int songCount { get; set; }
        }

        public class Root
        {
            public Result result { get; set; }
            public int code { get; set; }
        }


    }
}
