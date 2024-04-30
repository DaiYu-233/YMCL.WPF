using System.Windows;

namespace YMCL.Main.Public.Control.ModFileExpander
{
    /// <summary>
    /// ExpanderWithListView.xaml 的交互逻辑
    /// </summary>
    public partial class ModFileExpander : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty ExpanderHeaderProperty =
        DependencyProperty.Register("ExpanderHeader", typeof(string), typeof(ModFileExpander), new PropertyMetadata(string.Empty));

        public string ExpanderHeader
        {
            get { return (string)GetValue(ExpanderHeaderProperty); }
            set { SetValue(ExpanderHeaderProperty, value); }
        }
        public ModFileExpander(string haeder, System.Windows.Controls.ListView listView = null)
        {
            InitializeComponent();
            Expander.Header = haeder;
            //Grid.Children.Add(listView);
        }
    }
}
