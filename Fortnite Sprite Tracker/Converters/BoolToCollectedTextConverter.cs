using System.Globalization;
using Avalonia.Data.Converters;

namespace FortniteSpriteTracker.Converters;

public class BoolToCollectedTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "Collected" : "Collect";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
