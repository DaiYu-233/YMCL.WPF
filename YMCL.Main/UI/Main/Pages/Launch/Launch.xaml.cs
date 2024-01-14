using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using YMCL.Main.Public;

namespace YMCL.Main.UI.Main.Pages.Launch
{
    /// <summary>
    /// Launch.xaml 的交互逻辑
    /// </summary>
    public partial class Launch : Page
    {
        public Launch()
        {
            InitializeComponent();

            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));

            if (setting.UseCustomHomePage)
            {
                FileStream fs = new FileStream(Const.LaunchPageXamlPath, FileMode.Open);
                DependencyObject rootElement = (DependencyObject)XamlReader.Load(fs);
                this.PageRoot.Content = rootElement;
            }

        }
    }
}
