using MinecraftLaunch.Classes.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Main.Public.Class
{
    public class VersionSetting
    {
        public JavaEntry Java { get; set; } = new JavaEntry()
        {
            JavaPath = "Global"
        };
        public double MaxMem { get; set; } = -1;
    }
}
