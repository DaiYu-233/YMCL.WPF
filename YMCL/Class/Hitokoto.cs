using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Class
{
    internal class Hitokoto
    {
        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? uuid { get; set; }
            /// <summary>
            /// 就很犯困很困很困~很忙还是很困！
            /// </summary>
            public string? hitokoto { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? type { get; set; }
            /// <summary>
            /// 上班
            /// </summary>
            public string? from { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? from_who { get; set; }
            /// <summary>
            /// 美美的鱼香肉丝
            /// </summary>
            public string? creator { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int creator_uid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int reviewer { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? commit_from { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? created_at { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int length { get; set; }
        }

    }
}
