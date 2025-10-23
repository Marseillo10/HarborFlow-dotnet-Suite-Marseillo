using System;
using System.Globalization;
using System.Windows.Data;

namespace HarborFlow.Wpf.Converters
{
    public class MultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Return a clone of the values array to allow the command to receive them.
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
