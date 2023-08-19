using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using YMCL.Class;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        class MinecraftPathCalss
        {
            public string? MCPath;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //入口点
            if (!Directory.Exists("./YMCL"))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo("./YMCL");
                directoryInfo.Create();
            }
            if (!Directory.Exists("./YMCL/Temp"))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo("./YMCL/Temp");
                directoryInfo.Create();
            }
            if (!Directory.Exists("./YMCL/Accounts"))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo("./YMCL/Accounts");
                directoryInfo.Create();
            }

            if (!File.Exists("./YMCL/YMCL.Setting.json"))
            {
                var obj = new SettingInfo()
                {
                    AloneCore = true,
                    DisplayInformation = "True",
                    Java = "Null",
                    LoginIndex = "0",
                    LoginName = "Steve",
                    LoginType = "离线登录",
                    MaxMem = 1024,
                    Theme = "Light",
                    DownloadSoure = "Mcbbs",
                    MaxDownloadThreads = "64",
                    MinecraftPath = ".minecraft",
                    MainWindowHeight = 521,
                    MainWindowWidth = 900,
                    PlayerWindowHeight = 521,
                    PlayerWindowWidth = 900,
                    PlayerVolume = 0.5
                };
                File.WriteAllText("./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
            if (!File.Exists("./YMCL/YMCL.MinecraftPath.json"))
            {
                List<string> MinecraftPathList = new List<string> { ".minecraft" };
                File.WriteAllText("./YMCL/YMCL.MinecraftPath.json", JsonConvert.SerializeObject(MinecraftPathList, Formatting.Indented));
            }
            if (!File.Exists("./YMCL/YMCL.PlayList.json"))
            {
                File.WriteAllText("./YMCL/YMCL.PlayList.json", "");
            }
        }
    }
}
