using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Windows;
using YMCL.Public;
using YMCL.Public.Class;
using YMCL.UI.Lang;
using Application = System.Windows.Application;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] StartupArgs;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            StartupArgs = e.Args;
            var args = e.Args;

            Function.CreateFolder(Const.PublicDataRootPath);
            Function.CreateFolder(Const.DataRootPath);

            var obj = new Setting();
            if (!File.Exists(Const.SettingDataPath))
            {
                var data = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(Const.SettingDataPath, data);
            }

            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath));

            if (setting.Language == null || setting.Language == "zh-CN")
            {
                LangHelper.Current.ChangedCulture("");
            }
            else
            {
                LangHelper.Current.ChangedCulture("en-US");
            }
        }
    }

}
