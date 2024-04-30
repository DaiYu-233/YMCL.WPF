using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using iNKORE.UI.WPF.Modern.Controls;
using Panuon.WPF.UI;
using YMCL.Main.Public;
using YMCL.Main.Public.Lang;
using Cursors = System.Windows.Input.Cursors;
using Size = System.Windows.Size;

namespace YMCL.Main.Views.TaskManage.TaskCenter
{
    /// <summary>
    /// Tasks.xaml 的交互逻辑
    /// </summary>
    public partial class Tasks : WindowX
    {
        public Tasks()
        {
            InitializeComponent();
            Async();
        }
        #region WindowUI
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void WindowX_Loaded(object sender, RoutedEventArgs e)
        {
            //将装饰器添加到窗口的Content控件上(Resize)
            var c = this.Content as UIElement;
            var layer = AdornerLayer.GetAdornerLayer(c);
            layer.Add(new WindowResizeAdorner(c));
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion
        #region Resize
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
                _leftThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _leftThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _leftThumb.Cursor = Cursors.SizeWE;
                _topThumb = new Thumb();
                _topThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                _topThumb.VerticalAlignment = VerticalAlignment.Top;
                _topThumb.Cursor = Cursors.SizeNS;
                _rightThumb = new Thumb();
                _rightThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightThumb.VerticalAlignment = VerticalAlignment.Stretch;
                _rightThumb.Cursor = Cursors.SizeWE;
                _bottomThumb = new Thumb();
                _bottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                _bottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _bottomThumb.Cursor = Cursors.SizeNS;
                _lefTopThumb = new Thumb();
                _lefTopThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                _lefTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _lefTopThumb.Cursor = Cursors.SizeNWSE;
                _rightTopThumb = new Thumb();
                _rightTopThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightTopThumb.VerticalAlignment = VerticalAlignment.Top;
                _rightTopThumb.Cursor = Cursors.SizeNESW;
                _rightBottomThumb = new Thumb();
                _rightBottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _rightBottomThumb.VerticalAlignment = VerticalAlignment.Bottom;
                _rightBottomThumb.Cursor = Cursors.SizeNWSE;
                _leftbottomThumb = new Thumb();
                _leftbottomThumb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
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
                    if (thumb.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch)
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
                    thumb.Background = System.Windows.Media.Brushes.Green;
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
                _grid.Arrange(new Rect(new System.Windows.Point(-(_window.RenderSize.Width - finalSize.Width) / 2, -(_window.RenderSize.Height - finalSize.Height) / 2), _window.RenderSize));
                return finalSize;
            }
            //拖动逻辑
            private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
            {
                var c = _window;
                var thumb = sender as FrameworkElement;
                double left, top, width, height;
                if (thumb.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                {
                    left = c.Left + e.HorizontalChange;
                    width = c.Width - e.HorizontalChange;
                }
                else
                {
                    left = c.Left;
                    width = c.Width + e.HorizontalChange;
                }
                if (thumb.HorizontalAlignment != System.Windows.HorizontalAlignment.Stretch)
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
            FrameworkElementFactory GetFactory(System.Windows.Media.Brush back)
            {
                var fef = new FrameworkElementFactory(typeof(System.Windows.Shapes.Rectangle));
                fef.SetValue(System.Windows.Shapes.Rectangle.FillProperty, back);
                return fef;
            }
        }
        #endregion
        public void Async()
        {
            UpdateTaskNumber();
        }
        public async void UpdateTaskNumber()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(700);
                    Dispatcher.Invoke(() =>
                    {
                        var tasks = TaskContainer.Children;
                        var index = 1;
                        foreach (var item in tasks)
                        {
                            var task = item as TaskEntry;
                            task.TaskNumber.Text = $"#{index}";
                            index++;
                        }
                    });
                }
            });
        }
        public async void CreateTaskInfoBar()
        {
            await Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(async () =>
                {
                    InfoBar infoBar = new InfoBar
                    {
                        Title = $" ",
                        Message = $" ",
                        Severity = InfoBarSeverity.Success,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        IsOpen = true,
                        Margin = new Thickness(10),
                        FontFamily = new System.Windows.Media.FontFamily("MiSans Medium")
                    };
                    HyperlinkButton hyperlinkButton = new HyperlinkButton
                    {
                        Content = $"{LangHelper.Current.GetText("OpenTaskCenter")}",
                        FontFamily = new System.Windows.Media.FontFamily("MiSans Medium")
                    };
                    hyperlinkButton.Click += (_, _) =>
                    {
                        Const.Window.tasks.WindowState = WindowState.Normal;
                        Const.Window.tasks.Show();
                        Const.Window.tasks.Activate();
                    };
                    infoBar.ActionButton = hyperlinkButton;
                    Const.Window.main.InfoBarShowArea.Children.Add(infoBar);
                    await Task.Delay(3000);
                    infoBar.IsOpen = false;
                });
            });
        }

        public TaskEntry CreateTask(string taskName, bool showProgressBar = false, double height = 210)
        {
            var uiRoot = new TaskEntry(taskName, showProgressBar);
            uiRoot.Height = height;
            TaskContainer.Children.Add(uiRoot);
            CreateTaskInfoBar();
            return uiRoot;
        }
        public void Destory(TaskEntry entry)
        {
            TaskContainer.Children.Remove(entry);
        }
    }
}
