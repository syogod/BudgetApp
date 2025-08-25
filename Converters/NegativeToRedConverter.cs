using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace BudgetApp.Converters
{
    // This value converter is used in Avalonia UI to change the foreground color of a bound value to red
    // if the value is negative, and to the default color otherwise. 
    //
    // Usage: Bind the Foreground property of a control to a numeric value using this converter in XAML.
    public class NegativeToRedConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is < 0 ? Brushes.Red : Brushes.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}