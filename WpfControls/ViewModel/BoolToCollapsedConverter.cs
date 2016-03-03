using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CommonWpfControls
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool _isVisible = (bool)value;
            return _isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility _visibility = (Visibility)value;
            return _visibility == Visibility.Visible;
        }
    }
}
