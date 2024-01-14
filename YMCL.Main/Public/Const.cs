using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMCL.Main.Public
{
    public class Const
    {
        public static CultureInfo culture { get; set; }

        //Path
        public static string DataRootPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\DaiYu.YMCL";
        public static string PublicDataRootPath { get; } = "C:\\ProgramData\\DaiYu.YMCL";
        public static string SettingDataPath { get; } = DataRootPath + "\\YMCL.Setting.DaiYu";
        public static string LaunchPageXamlPath { get; } = DataRootPath + "\\CustomPage\\YMCL.LaunchPageXaml.DaiYu";
    }
}
