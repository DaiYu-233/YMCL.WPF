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
        public string? MinecraftFolder { get; set; } = null;
        public string? MinecraftVersionId { get; set; } = null;
        public JavaEntry Java { get; set; } = new JavaEntry()
        {
            JavaPath = "<Auto>"
        };
        public int AccountSelectionIndex { get; set; } = 0;
        public double MaxMem { get; set; } = 1024;
        public bool AloneCore { get; set; } = true;
        public double MainWidth { get; set; } = 1050;
        public double MainHeight { get; set; } = 600;
        public bool GetOutput { get; set; } = false;
        public double GameWidth { get; set; } = 854;
        public double GameHeight { get; set; } = 480;
        public SettingItem.Theme Theme { get; set; } = SettingItem.Theme.System;
        public SettingItem.GameWindow GameWindow { get; set; } = SettingItem.GameWindow.Default;
    }

    public class SettingItem
    {
        public enum Theme
        {
            System = 0,
            Light = 1,
            Dark = 2
        }
        public enum AccountType
        {
            Offline,
            Microsoft,
            ThirdParty
        }
        public enum GameWindow
        {
            Default,
            FullScreen,
            Custom
        }
    }
}
