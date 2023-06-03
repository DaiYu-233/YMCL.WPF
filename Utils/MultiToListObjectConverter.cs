using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace WPFTemplateLib.WpfConverters
{
    /// <summary>
    /// 多重绑定转为List《object》转换器
    /// </summary>
    public class MultiToListObjectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}