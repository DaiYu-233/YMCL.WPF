using Microsoft.Win32;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using YMCL.Class;

namespace YMCL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public class WindowResizeAdorner : Adorner
        {
            //4条边
            Thumb _leftThumb, _topThumb, _rightThumb, _bottomThumb;
            //4个角
            Thumb _lefTopThumb, _rightTopThumb, _rightBottomThumb, _leftbottomThumb;
            //布局容器，如果不使用布局容器，则需要给上述8个控件布局，实现和Grid布局定位是一样的，会比较繁琐且意义不大。
            Grid _grid;
            UIElement _adornedElement;
            Window _window;
            public WindowResizeAdorner(UIElement adornedElement) : base(adornedElement)
            {
                _adornedElement = adornedElement;
                _window = Window.GetWindow(_adornedElement);
                //初始化thumb
                _leftThumb = new Thumb();
                _leftThumb.HorizontalAlignment = HorizontalAlignment.Left;
                _leftThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _leftThumb.Cursor = Cursors.SizeWE;
                _topThumb = new Thumb();
                _topThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                _topThumb.VerticalAlignment = VerticalAlignment.Top;
                _topThumb.Cursor = Cursors.SizeNS;
                _rightThumb = new Thumb();
                _rightThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _rightThumb.Cursor = Cursors.SizeWE;
                _bottomThumb = new Thumb();
                _bottomThumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                _bottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _bottomThumb.Cursor = Cursors.SizeNS;
                _lefTopThumb = new Thumb();
                _lefTopThumb.HorizontalAlignment = HorizontalAlignment.Left;
                _lefTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _lefTopThumb.Cursor = Cursors.SizeNWSE;
                _rightTopThumb = new Thumb();
                _rightTopThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _rightTopThumb.Cursor = Cursors.SizeNESW;
                _rightBottomThumb = new Thumb();
                _rightBottomThumb.HorizontalAlignment = HorizontalAlignment.Right;
                _rightBottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _rightBottomThumb.Cursor = Cursors.SizeNWSE;
                _leftbottomThumb = new Thumb();
                _leftbottomThumb.HorizontalAlignment = HorizontalAlignment.Left;
                _leftbottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _leftbottomThumb.Cursor = Cursors.SizeNESW;
                _grid = new Grid();
                _grid.Children.Add(_leftThumb);
                _grid.Children.Add(_topThumb);
                _grid.Children.Add(_rightThumb);
                _grid.Children.Add(_bottomThumb);
                _grid.Children.Add(_lefTopThumb);
                _grid.Children.Add(_rightTopThumb);
                _grid.Children.Add(_rightBottomThumb);
                _grid.Children.Add(_leftbottomThumb);
                AddVisualChild(_grid);
                foreach (Thumb thumb in _grid.Children)
                {
                    int thumnSize = 10;
                    if (thumb.HorizontalAlignment == HorizontalAlignment.Stretch)
                    {
                        thumb.Width = double.NaN;
                        thumb.Margin = new Thickness(thumnSize, 0, thumnSize, 0);
                    }
                    else
                    {
                        thumb.Width = thumnSize;
                    }
                    if (thumb.VerticalAlignment == VerticalAlignment.Stretch)
                    {
                        thumb.Height = double.NaN;
                        thumb.Margin = new Thickness(0, thumnSize, 0, thumnSize);
                    }
                    else
                    {
                        thumb.Height = thumnSize;
                    }
                    thumb.Background = Brushes.Green;
                    thumb.Template = new ControlTemplate(typeof(Thumb))
                    {
                        VisualTree = GetFactory(new SolidColorBrush(Colors.Transparent))
                    };
                    thumb.DragDelta += Thumb_DragDelta;
                }
            }

            protected override Visual GetVisualChild(int index)
            {
                return _grid;
            }
            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
            protected override Size ArrangeOverride(Size finalSize)
            {
                //直接给grid布局，grid内部的thumb会自动布局。
                _grid.Arrange(new Rect(new Point(-(_window.RenderSize.Width - finalSize.Width) / 2, -(_window.RenderSize.Height - finalSize.Height) / 2), _window.RenderSize));
                return finalSize;
            }
            //拖动逻辑
            private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                var c = _window;
                var thumb = sender as FrameworkElement;
                double left, top, width, height;
                if (thumb.HorizontalAlignment == HorizontalAlignment.Left)
                {
                    left = c.Left + e.HorizontalChange;
                    width = c.Width - e.HorizontalChange;
                }
                else
                {
                    left = c.Left;
                    width = c.Width + e.HorizontalChange;
                }
                if (thumb.HorizontalAlignment != HorizontalAlignment.Stretch)
                {
                    if (width > 63)
                    {
                        c.Left = left;
                        c.Width = width;
                    }
                }
                if (thumb.VerticalAlignment == VerticalAlignment.Top)
                {

                    top = c.Top + e.VerticalChange;
                    height = c.Height - e.VerticalChange;
                }
                else
                {
                    top = c.Top;
                    height = c.Height + e.VerticalChange;
                }

                if (thumb.VerticalAlignment != VerticalAlignment.Stretch)
                {
                    if (height > 63)
                    {
                        c.Top = top;
                        c.Height = height;
                    }
                }
            }
            //thumb的样式
            FrameworkElementFactory GetFactory(Brush back)
            {
                var fef = new FrameworkElementFactory(typeof(Rectangle));
                fef.SetValue(Rectangle.FillProperty, back);
                return fef;
            }
        }

        double w;
        double h;
        Pages.LaunchPage launch = new Pages.LaunchPage();
        Frame setting = new Frame() { Content = new Pages.SettingPage() };
        Frame download = new Frame() { Content = new Pages.DownloadPage() };
        Frame more = new Frame() { Content = new Pages.MorePage() };

        void Comein()
        {
            //var h = MainFrame.Height;
            //ThicknessAnimation Comein = new ThicknessAnimation()
            //{
            //    From = new Thickness(60, 30 + h, -h, 0),
            //    To = new Thickness(60, 30, 0, 0),
            //    Duration = TimeSpan.Parse("0:0:1")
            //};
            //MainFrame.BeginAnimation(MarginProperty, Comein);
        }

        public MainWindow()
        {
            InitializeComponent();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "YMCL.exe";

            ServicePointManager.DefaultConnectionLimit = 512;
            MainFrame.Content = launch;

            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            Width = obj.MainWindowWidth;
            Height = obj.MainWindowHeight;
            w = Width;
            h = Height;

            //Trigger trigger = new Trigger();
            //trigger.Property = RadioButton.IsCheckedProperty;
            //trigger.Value = true;
            //Setter setter1 = new Setter();
            //setter1.TargetName = "txtName";
            //setter1.Property = RadioButton.ForegroundProperty;
            //setter1.Value = obj.ThemeColor;
            //trigger.Setters.Add(setter1);
            //rdLaunch.Style.Triggers.Add(trigger);
        }



        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            System.Environment.Exit(0);
        }

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (ToLaunch.IsSelected)
            {
                MainFrame.Content = launch;
            }
            else if (ToSetting.IsSelected)
            {
                MainFrame.Content = setting;
            }
            else if (ToDownload.IsSelected)
            {
                MainFrame.Content = download;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void rdLaunch_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = launch;
            Comein();
            launch.VerListBorder.Visibility = Visibility.Hidden;
        }

        private void rdSetting_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = setting; Comein();
        }

        private void rdDownload_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = download; Comein();
        }

        private void rdMore_Checked(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = more; Comein();
        }

        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));


        }

        private void WindowX_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void WindowX_MouseMove(object sender, MouseEventArgs e)
        {
            if (w == Width && h == Height)
            {
                return;
            }
            var obj = JsonConvert.DeserializeObject<Class.SettingInfo>(File.ReadAllText(        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
            obj.MainWindowWidth = Width;
            obj.MainWindowHeight = Height;
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));
        }
    }
}
