using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Class
{
    public class SettingInfo
    {

            public string? Java { get; set; }
            public string? MaxMem { get; set; }
            public string? AloneCore { get; set; }
            public string? MinecraftPath { get; set; }

            public string? LoginIndex { get; set; }
            public string? LoginName { get; set; }
            public string? LoginType { get; set; }


        public string? Theme { get; set; }
        public string? DisplayInformation { get; set; }
        public double MainWindowWidth { get; set; }
        public double MainWindowHeight { get; set; }
        public double PlayerWindowWidth { get; set; }
        public double PlayerWindowHeight { get; set; }

        public string? DownloadSoure { get; set; }
        public string? MaxDownloadThreads { get; set; }
    }
}
