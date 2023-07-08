using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Launcher.xaml 的交互逻辑
    /// </summary>
    public partial class Launcher : Page
    {
        public Launcher()
        {
            InitializeComponent();
            if (File.Exists("./YMCL/DisplayInformation.txt"))
            {
                if (File.ReadAllText("./YMCL/DisplayInformation.txt") == "false")
                {
                    displayinf.IsOn = false;
                }
                else
                {
                    displayinf.IsOn = true;
                }

            }
            else
            {
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
        }
        private void TestFolder(string Folder)
        {
            if (System.IO.Directory.Exists(Folder)) { }
            else
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Folder);
                directoryInfo.Create();
            }
        }




        private void displayinf_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void displayinf_MouseLeave(object sender, MouseEventArgs e)
        {
            if (displayinf.IsOn == true)
            {
                TestFolder("./YMCL");
                File.WriteAllText("./YMCL/DisplayInformation.txt", "true");
            }
            else
            {
                TestFolder("./YMCL");
                File.WriteAllText("./YMCL/DisplayInformation.txt", "false");
            }
        }
    }
}
