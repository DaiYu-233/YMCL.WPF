using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.IO;
using System.Reflection;
using System.Windows;
using YMCL.Main.Public;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.UI.Lang;
using Application = System.Windows.Application;

namespace YMCL.Main
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
            Function.CreateFolder(Const.DataRootPath + "\\CustomPage");

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            var obj = new Setting();
            if (!File.Exists(Const.SettingDataPath) || JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.SettingDataPath)) == null)
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
                LangHelper.Current.ChangedCulture(setting.Language);
            }

            if (!File.Exists(Const.LaunchPageXamlPath) || File.ReadAllText(Const.LaunchPageXamlPath) == null)
            {
                Type type = MethodBase.GetCurrentMethod().DeclaringType;
                string _namespace = type.Namespace;
                Assembly _assembly = Assembly.GetExecutingAssembly();
                string resourceName = _namespace + ".Public.Text.LaunchPage.xaml";
                Stream stream = _assembly.GetManifestResourceStream(resourceName);
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    File.WriteAllText(Const.LaunchPageXamlPath, result);
                }
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxX.Show($"\n{LangHelper.Current.GetText("App_UnhandledException")}：{e.Exception.Message}\n\n{e.Exception.ToString()}", "Yu Minecraft Launcher");
        }
    }

}
