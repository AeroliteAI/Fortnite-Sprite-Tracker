using System.Globalization;
using Avalonia.Data.Converters;

namespace FortniteSpriteTracker.Converters;

// Avalonia uses bool IsVisible instead of WPF's Visibility enum.
public class BoolToVisibleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true;
}
