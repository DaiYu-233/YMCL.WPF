using MinecraftLaunch.Classes.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YMCL.Main.Public.Class
{
    public class Setting
    {
        public double GameWidth { get; set; } = 854;
        public double GameHeight { get; set; } = 480;
        public double MainWidth { get; set; } = 1050;
        public double MainHeight { get; set; } = 600;
        public string? Language { get; set; }
        public string? MinecraftFolder { get; set; } = null;
        public string? MinecraftVersionId { get; set; } = null;
        public string? CustomHomePageNetXamlUrl { get; set; } = string.Empty;
        public string? CustomHomePageNetCSharpUrl { get; set; } = string.Empty;
        public JavaEntry Java { get; set; } = new JavaEntry() { JavaPath = "<Auto>" };
        public int AccountSelectionIndex { get; set; } = 0;
        public double MaxMem { get; set; } = 1024;
        public bool AloneCore { get; set; } = true;
        public bool GetOutput { get; set; } = false;
        public double MaxDownloadThread { get; set; } = 16;
        public SettingItem.Theme Theme { get; set; } = SettingItem.Theme.System;
        public System.Windows.Media.Color ThemeColor { get; set; } = System.Windows.Media.Color.FromRgb(13, 107, 192);
        public SettingItem.CustomHomePage CustomHomePage { get; set; } = SettingItem.CustomHomePage.Disable;
        public SettingItem.GameWindow GameWindow { get; set; } = SettingItem.GameWindow.Default;
        public SettingItem.DownloadSource DownloadSource { get; set; } = SettingItem.DownloadSource.Mojang;
    }

    public class VersionSetting
    {
        public SettingItem.VersionSettingAloneCore AloneCore { get; set; } = SettingItem.VersionSettingAloneCore.Global;
        public JavaEntry Java { get; set; } = new JavaEntry() { JavaPath = "Global" };
        public double MaxMem { get; set; } = -1; // -1 = Global
        public string AutoJoinServerIp { get; set; } = "";
    }

    public class SettingItem
    {
        public enum CustomHomePage
        {
            Disable,
            LocalFile,
            NetFile
        }
        public enum Theme
        {
            System,
            Light,
            Dark
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
        public enum DownloadSource
        {
            Mojang,
            BmclApi
        }
        public enum VersionSettingAloneCore
        {
            Global,
            Off,
            On
        }
    }
}
