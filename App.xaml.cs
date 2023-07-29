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
using YMCL.Class;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
                    AloneCore = "True",
                    DisplayInformation = "True",
                    Java = "Null",
                    LoginIndex = "0",
                    LoginName = "Steve",
                    LoginType = "离线登录",
                    MaxMem = "1024",
                    Theme = "Light",
                    DownloadSoure = "Mcbbs",
                    MaxDownloadThreads = "64",
                    MinecraftPath = ".minecraft"
                };
                File.WriteAllText("./YMCL/YMCL.Setting.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
        }
    }
}
