using System.Globalization;
using System.Xml.Linq;

namespace YMCL.Public
{
    public static class Const
    {
        public static CultureInfo culture { get; set; }

        //Path
        public static string DataRootPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL";
        public static string PublicDataRootPath { get; } = "C:\\ProgramData\\DaiYu.YMCL";
        public static string SettingDataPath { get; } = DataRootPath + "\\YMCL.Setting.DaiYu";
        public static string AssetsPath { get; } = DataRootPath + "\\Assets";
        public static string AccountDataPath { get; } = DataRootPath + "\\YMCL.Account.DaiYu";
        public static string MinecraftFolderDataPath { get; } = DataRootPath + "\\YMCL.MinecraftFolder.DaiYu";
        public static string JavaDataPath { get; } = DataRootPath + "\\YMCL.Java.DaiYu";
        public static string SongPlayListDataPath { get; } = DataRootPath + "\\YMCL.SongPlayList.DaiYu";
    }
}
