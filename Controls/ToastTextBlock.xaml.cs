using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
/*
 * https://blog.csdn.net/u012046379/article/details/119517156
 * https://gitee.com/DLGCY_Clone/WpfToast
 */
namespace WpfToast.Controls
{
    public partial class ToastTextBlock : TextBlock
    {
        public ToastTextBlock()
        {
            InitializeComponent();
        }

        #region 点击弹出Toast

        private void ToastTextBlock_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsTextTrimmed)
            {
                return;
            }

            string msg = Text;
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            Toast.Show(msg, new ToastOptions()
            {
                Icon = ToastIcons.Information,
                Location = ToastLocation.ScreenCenter,
                Time = 3000,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Click = ToastClick,
                TextWidth = 1000,
            });
        }

        private void ToastClick(object sender, EventArgs e)
        {
            Toast toast = sender as Toast;
            toast?.Close();
        }

        #endregion

        #region 判断当前文字是否被截断

        /// <summary>
        /// 当前文字是否被截取
        /// </summary>
        public bool IsTextTrimmed
        {
            get { return (bool)GetValue(IsTextTrimmedProperty); }
            private set { SetValue(IsTextTrimmedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextTrimming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextTrimmedProperty =
            DependencyProperty.Register("IsTextTrimmed", typeof(bool), typeof(TextBlock), new PropertyMetadata(false));

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            IsTextTrimmed = GetIsTrimming();
            return base.GetLayoutClip(layoutSlotSize);
        }

        bool GetIsTrimming()
        {
            if (TextTrimming == TextTrimming.None)
            {
                return false;
            }

            if (TextWrapping == TextWrapping.NoWrap)  //比较长度
            {
                Size size = new Size(double.MaxValue, RenderSize.Height);
                var needsize = MeasureOverride(size);

                if (needsize.Width > RenderSize.Width)
                {
                    MeasureOverride(RenderSize);
                    return true;
                }
            }
            else  //比较高度
            {
                Size size = new Size(RenderSize.Width, double.MaxValue);
                var needsize = MeasureOverride(size);
                if (needsize.Height > RenderSize.Height)
                {
                    MeasureOverride(RenderSize);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
