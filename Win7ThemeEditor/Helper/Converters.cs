using System;
using System.Globalization;
using System.Windows.Data;

namespace Win7ThemeEditor
{
    /// <summary>
    /// 取反
    /// </summary>
    public class BoolReverseConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool?) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool?) value;
        }

        #endregion
    }

    public class DefaultSrtingConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (string) value == "")
            {
                if (parameter == null) return App.FindString("DefNone");
                return (string) parameter == "cur" ? App.FindString("DefCur") : value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}