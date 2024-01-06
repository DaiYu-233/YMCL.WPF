using System.Globalization;
using System.Xml.Linq;

namespace YMCL.Public
{
    public static class Const
    {
        public static CultureInfo culture;

        //Path
        public static string DataRootPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.";
        public static string PublicDataRootPath { get; } = "C:\\ProgramData\\";
        public static string SettingDataPath { get; } = DataRootPath + "\\.Setting.DaiYu";
        public static string AssetsPath { get; } = DataRootPath + "\\Assets";
        public static string AccountDataPath { get; } = DataRootPath + "\\.Account.DaiYu";
        public static string MinecraftFolderDataPath { get; } = DataRootPath + "\\.MinecraftFolder.DaiYu";
        public static string JavaDataPath { get; } = DataRootPath + "\\.Java.DaiYu";
        public static string SongPlayListDataPath { get; } = DataRootPath + "\\.SongPlayList.DaiYu";

        static Const()
        {
            culture = CultureInfo.GetCultureInfo("zh-CN");
        }
    }
}
