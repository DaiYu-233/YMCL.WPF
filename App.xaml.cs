using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        //入口点
        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (!Directory.Exists(Const.YMCLDataRoot))
            {
                DirectoryInfo directoryInfo = new(Const.YMCLDataRoot);
                directoryInfo.Create();
            }
            if (!Directory.Exists(Const.YMCLDataRoot + "\\Temp"))
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Const.YMCLDataRoot + "\\Temp");
                directoryInfo.Create();
            }

            if (!File.Exists(Const.YMCLMinecraftFolderDataPath))
            {
                var obj = new List<string>()
                {
                    ".minecraft"
                };
                var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(Const.YMCLMinecraftFolderDataPath, data);
            }
            if (!File.Exists(Const.YMCLSettingDataPath))
            {
                var obj = new Class.Setting()
                {
                    MinecraftPath = ".minecraft"
                };
                var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(Const.YMCLSettingDataPath, data);
            }
            if (!File.Exists(Const.YMCLJavaDataPath))
            {
                File.WriteAllText(Const.YMCLJavaDataPath, "[]");
            }
        }


        //全局异常处理
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxX.Show($"\n{e.Exception.Message}\n\n详细信息：\n{e.Exception.ToString()}","Yu Minecraft Launcher - 异常");
        }
    }
}
