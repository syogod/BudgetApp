using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace BudgetApp.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Not used for OneWayToSource, but required by interface
            return value?.ToString();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s && int.TryParse(s, out var result))
                return result;
            return null; // Default to null if empty or invalid
        }
    }
}