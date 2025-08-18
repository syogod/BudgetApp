using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace BudgetApp.Converters
{
    public class NegativeToRedConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int intValue && intValue < 0)
                return Brushes.Red;
            return Brushes.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}