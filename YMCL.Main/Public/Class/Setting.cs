using MinecraftLaunch.Classes.Models.Game;
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
        public string? MinecraftVersionId { get; set; } = null;
        public JavaEntry Java { get; set; } = new JavaEntry()
        {
            JavaPath= "<Auto>"
        };
        public int AccountSelectionIndex { get; set; } = 0;
        public double MaxMem { get; set; } = 1024;
        public bool AloneCore { get; set; } = true;
    }
}
