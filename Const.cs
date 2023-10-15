﻿using Newtonsoft.Json;
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
        public class Font : ResourceDictionary
        {
            public Font()
            {
                Add("Font", YMCLFontPath + "#MiSans Medium");
            }
        }
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

        //Path
        public static string YMCLDataRoot { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL";
        public static string YMCLFontPath { get; } = "C:\\ProgramData\\MiSans.ttf";
        public static string YMCLSettingDataPath { get; } = YMCLDataRoot + "\\YMCL.Setting.DaiYu";
        public static string YMCLAccountDataPath { get; } = YMCLDataRoot + "\\YMCL.Account.DaiYu";
        public static string YMCLMinecraftFolderDataPath { get; } = YMCLDataRoot + "\\YMCL.MinecraftFolder.DaiYu";
        public static string YMCLJavaDataPath { get; } = YMCLDataRoot + "\\YMCL.Java.DaiYu";


        //Temp
        public static string YMCLTempDataRoot { get; } = YMCLDataRoot + "\\Temp";
        public static string YMCLTempAccountDataPath { get; } = YMCLTempDataRoot + "\\YMCL.Account.DaiYu"; 
        public static string YMCLTempSetupArgsDataPath { get; } = YMCLTempDataRoot + "\\YMCL.SetupArgs.DaiYu";


        //String
        public static string ApiKey { get; } = "$2a$10$ndSPnOpYqH3DRmLTWJTf5Ofm7lz9uYoTGvhSj0OjJWJ8WdO4ZTsr.";
        public static string YMCLDefaultAccount { get; } = "[\r\n  {\r\n    \"AccountType\": \"离线模式\",\r\n    \"Name\": \"Steve\",\r\n    \"AddTime\": \"23/10/14 20:05:46\",\r\n    \"Data\": null\r\n  }\r\n]";
        public static string YMCLClientId { get; } = "c06d4d68-7751-4a8a-a2ff-d1b46688f428";
    }
}
