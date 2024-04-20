using iNKORE.UI.WPF.Modern.Controls;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Download;
using MinecraftLaunch.Components.Fetcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using YMCL.Main.Public;
using YMCL.Main.Public.Control.ModFileExpander;
using ListView = iNKORE.UI.WPF.Modern.Controls.ListView;

namespace YMCL.Main.Views.Main.Pages.Download.Pages.Mods
{
    /// <summary>
    /// Mods.xaml 的交互逻辑
    /// </summary>
    /// 

    public class VersionComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string[] versionPartsX = x.Split('.');
            string[] versionPartsY = y.Split('.');

            int minLength = Math.Min(versionPartsX.Length, versionPartsY.Length);

            for (int i = 0; i < minLength; i++)
            {
                int partX = int.Parse(versionPartsX[i]);
                int partY = int.Parse(versionPartsY[i]);

                if (partX != partY)
                {
                    return partX.CompareTo(partY);
                }
            }
            // 如果所有相同位置的版本号都相同，但长度不同，则较长的版本号应该更大  
            return versionPartsX.Length.CompareTo(versionPartsY.Length);
        }
    }
    public partial class Mods : System.Windows.Controls.Page
    {
        CurseForgeFetcher curseForgeFetcher = new(Const.CurseForgeApiKey);
        public Mods()
        {
            InitializeComponent();
        }
        private async void ModSearchBox_QuerySubmitted(iNKORE.UI.WPF.Modern.Controls.AutoSuggestBox sender, iNKORE.UI.WPF.Modern.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ModsListView.Items.Clear();
            ModSearchBox.IsEnabled = false;
            LoadingMore.Visibility = Visibility.Visible;
            List<CurseForgeResourceEntry> entries = new();
            LoaderType loaderType = (LoaderType)LoaderTypeComboBox.SelectedIndex;
            if (string.IsNullOrEmpty(MinecraftVersionBox.Text))
            {
                entries = (await curseForgeFetcher.SearchResourcesAsync(ModSearchBox.Text, modLoaderType: loaderType)).ToList();
            }
            else
            {
                entries = (await curseForgeFetcher.SearchResourcesAsync(ModSearchBox.Text, gameVersion: MinecraftVersionBox.Text, modLoaderType: loaderType)).ToList();
            }
            LoadingMore.Visibility = Visibility.Collapsed;
            ModSearchBox.IsEnabled = true;
            entries.ForEach(entry =>
            {
                ModsListView.Items.Add(entry);
            });
        }

        private void ModsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModDetailsVersionPanel.Children.Clear();
            if (ModsListView.SelectedIndex == -1) return;
            var item = ModsListView.SelectedItem as CurseForgeResourceEntry;
            ModDetailsName.Content = item.Name;
            ModDetails.Visibility = Visibility.Visible;
            List<string> mcVersions = new();
            foreach (var file in item.Files)
            {
                if (!mcVersions.Contains(file.McVersion))
                {
                    mcVersions.Add(file.McVersion);
                }
            }
            mcVersions.Sort(new VersionComparer());
            mcVersions.Reverse();
            mcVersions.ForEach(mcVersion =>
            {
                var expander = new ModFileExpander(mcVersion);
                expander.Margin = new Thickness(0, 0, 0, 10);
                expander.Name = $"mod_{mcVersion.Replace(".","_")}";
                expander.ModFileListView.SelectionChanged += ModFileListView_SelectionChanged;
                ModDetailsVersionPanel.Children.Add(expander);
                foreach (var file in item.Files)
                {
                    if (file.McVersion == mcVersion)
                    {
                        expander.ModFileListView.Items.Add(file);
                    }
                }
            });
        }

        private void ModFileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as ListView;
            var item = control.SelectedItem as CurseFileEntry;
            if (item != null)
            {
                
            }
        }

        private void ReturnToModSearch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ModDetails.Visibility = Visibility.Collapsed;
        }
    }
}
