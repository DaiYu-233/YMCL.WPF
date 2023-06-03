using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Point = System.Windows.Point;

namespace WpfToast.Utils
{
    /// <summary>
    /// WPF 对 Winform 的 Screen 类的包装
    /// https://stackoverflow.com/a/2118993 by Nils in 
    /// （https://stackoverflow.com/questions/1927540/how-to-get-the-size-of-the-current-screen-in-wpf/68152137#68152137）
    /// </summary>
    public class WpfScreen
    {
        public static IEnumerable<WpfScreen> AllScreens()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                yield return new WpfScreen(screen);
            }
        }

        public static WpfScreen GetScreenFrom(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = Screen.FromHandle(windowInteropHelper.Handle);
            WpfScreen wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen GetScreenFrom(Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = Screen.FromPoint(drawingPoint);
            WpfScreen wpfScreen = new WpfScreen(screen);

            return wpfScreen;
        }

        public static WpfScreen Primary => new WpfScreen(Screen.PrimaryScreen);

        private readonly Screen screen;

        internal WpfScreen(Screen screen)
        {
            this.screen = screen;
        }

        public Rect DeviceBounds => GetRect(screen.Bounds);

        public Rect WorkingArea => GetRect(screen.WorkingArea);

        //private Rect GetRect(Rectangle value)
        //{
        //    // should x, y, width, height be device-independent-pixels ??
        //    return new Rect
        //    {
        //        X = value.X,
        //        Y = value.Y,
        //        Width = value.Width,
        //        Height = value.Height
        //    };
        //}

        /// <summary>
        /// Jürgen Bayer extended the GetRect method to return the Rect in device independent pixels
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Rect GetRect(Rectangle value)
        {
            var pixelWidthFactor = SystemParameters.WorkArea.Width / screen.WorkingArea.Width;
            var pixelHeightFactor = SystemParameters.WorkArea.Height / screen.WorkingArea.Height;
            return new Rect
            {
                X = value.X * pixelWidthFactor,
                Y = value.Y * pixelHeightFactor,
                Width = value.Width * pixelWidthFactor,
                Height = value.Height * pixelHeightFactor
            };
        }

        public bool IsPrimary => screen.Primary;

        public string DeviceName => screen.DeviceName;
    }

    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class WpfScreenExtensions
    {
        /// <summary>
        /// 获取当前屏幕的工作区（未解决获取非主屏幕的需求）
        /// https://stackoverflow.com/a/68152137
        /// </summary>
        /// <param name="window">窗口对象</param>
        /// <returns>工作区</returns>
        //public static Rect GetCurrentScreenWorkArea(this Window window)
        //{
        //    var screen = Screen.FromPoint(new System.Drawing.Point((int)window.Left, (int)window.Top));
        //    var dpiScale = VisualTreeHelper.GetDpi(window);

        //    return new Rect
        //    {
        //        Width = screen.WorkingArea.Width / dpiScale.DpiScaleX,
        //        Height = screen.WorkingArea.Height / dpiScale.DpiScaleY
        //    };
        //}

        /// <summary>
        /// 获取当前屏幕的工作区
        /// </summary>
        /// <param name="window">窗口对象</param>
        /// <returns>工作区</returns>
        public static Rect GetCurrentScreenWorkArea(this Window window)
        {
            return WpfScreen.GetScreenFrom(window).WorkingArea;
        }
    }
}
