using System.Globalization;
using Avalonia.Data.Converters;

namespace FortniteSpriteTracker.Converters;

// Generic "value equals parameter" converter — works for FilterMode, GroupMode, or any enum.
public class FilterActiveConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not null && parameter is not null && value.ToString() == parameter.ToString();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
