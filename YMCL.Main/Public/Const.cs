using YMCL.Main.Views.MusicPlayer;
using YMCL.Main.Views.Initialize;
using YMCL.Main.Views.Main;
using YMCL.Main.Views.MusicPlayer.DesktopLyric;
using YMCL.Main.Views.MusicPlayer.Main;
using YMCL.Main.Views.TaskManage.TaskCenter;

namespace YMCL.Main.Public
{
    public class Const
    {
        public class Window
        {
            public static MainWindow main { get; set; } = new MainWindow();
            public static InitializeWindow initialize { get; set; } = new InitializeWindow();
            public static Tasks tasks { get; set; } = new Tasks();
            public static MusicPlayer musicPlayer { get; set; } = new MusicPlayer();
            public static DesktopLyric desktopLyric { get; set; } = new DesktopLyric();
        }

        public static string CurseForgeApiKey { get; } = "$2a$10$ndSPnOpYqH3DRmLTWJTf5Ofm7lz9uYoTGvhSj0OjJWJ8WdO4ZTsr.";
        public static string Version { get; } = "2024.04.20.0";
        public static string UpdaterId { get; } = "97B62D3AD1724EFA9AFC7A8D8971BBB1";
        public static string AzureClientId { get; } = "c06d4d68-7751-4a8a-a2ff-d1b46688f428";
        public static string VersionSettingFileName { get; } = "YMCL.Setting.DaiYu";

        //Path
        public static string DataRootPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL";
        public static string PublicDataRootPath { get; } = "C:\\ProgramData\\DaiYu.YMCL";
        public static string JavaDataPath { get; } = DataRootPath + "\\YMCL.Java.DaiYu";
        public static string SettingDataPath { get; } = DataRootPath + "\\YMCL.Setting.DaiYu";
        public static string CustomHomePageXamlPath { get; } = DataRootPath + "\\YMCL.CustomHomePageXaml.DaiYu";
        public static string CustomHomePageDllPath { get; } = DataRootPath + "\\YMCL.CustomHomePageXaml.DaiYu";
        public static string CustomHomePageCSharpPath { get; } = DataRootPath + "\\YMCL.CustomHomePageCSharp.DaiYu";
        public static string AccountDataPath { get; } = DataRootPath + "\\YMCL.Account.DaiYu";
        public static string PlayListDataPath { get; } = DataRootPath + "\\YMCL.PlayList.DaiYu";
        public static string MinecraftFolderDataPath { get; } = DataRootPath + "\\YMCL.MinecraftFolder.DaiYu";
        public static string YMCLPathData { get; } = DataRootPath + "\\YMCL.ExePath.DaiYu";
        public static string YMCLBat { get; } = PublicDataRootPath + "\\launch.bat";
    }
}
