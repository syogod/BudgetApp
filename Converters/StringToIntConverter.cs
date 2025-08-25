using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace BudgetApp.Converters
{
    // This value converter is used to convert between string and integer values for data binding.
    // It allows binding a TextBox or similar control to an integer property in the ViewModel, handling parsing and formatting.
    // Usage: Bind the Text property of a TextBox to an int property using this converter in XAML.
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
            return null; 
        }
    }
}