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
        public JavaEntry Java { get; set; } = null;
        public double MaxMem { get; set; } = -1;
    }
}
