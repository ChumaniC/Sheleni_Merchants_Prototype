using System;
using Xamarin.Forms;

namespace Sheleni_Merchants.Converters
{
    public class OrBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && parameter is bool)
            {
                bool visibilityValue = (bool)value || (bool)parameter;
                return visibilityValue ? true : false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null; // You might not need this, just return null
        }
    }
}
