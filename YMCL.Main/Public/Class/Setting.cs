using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Main.Public.Class
{
    public class Setting
    {
        public string? Language { get; set; }
        public bool UseCustomHomePage { get; set; } = false;
        public string? MinecraftFolder { get; set; } = null;
        public string? MinecraftVersion { get; set; } = null;
        public string? Java { get; set; } = null;
        public int AccountSelectionIndex { get; set; } = 0;
        public double MaxMem { get; set; } = 1024;
        public bool AloneCore { get; set; } = true;
    }
}
