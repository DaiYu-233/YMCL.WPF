using System;
using System.Windows.Media;

namespace YMCL.Class
{
    public class Setting
    {
        public string? Java { get; set; }
        public string? MinecraftPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + ".minecraft";
        public double MamMem { get; set; } = 1024;
        public double Volume { get; set; } = 0;
        public string? SelectedVersion { get; set; }
        public int AccountSelectionIndex { get; set; }
        public string? Theme { get; set; } = "System";
        public string? MusicLoopType { get; set; } = "RepeatOff";
        public Color? ThemeColor { get; set; } = Color.FromArgb(255, 0, 120, 215);
        public Color? DesktopLyricColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
    }

}
