using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YMCL.Pages;

namespace YMCL
{
    public class Const
    {
        public class Window
        {
            public static MainWindow main { get; set; }
            static Window()
            {
                foreach (System.Windows.Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))//使用窗体类进行匹配查找
                    {
                        #pragma warning disable CS8601
                        main = window as MainWindow;
                    }
                }
            }
        }
        public static string YMCLDataRoot { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL";
        public static string YMCLSettingDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL\\YMCL.Setting.DaiYu";
        public static string YMCLMinecraftFolderDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL\\YMCL.MinecraftFolder.DaiYu";
        public static string YMCLJavaDataPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL\\YMCL.Java.DaiYu";
    }
}
