using System.Globalization;
using System.Windows.Data;
using FortniteSpriteTracker.Models;

namespace FortniteSpriteTracker.Converters;

public class FilterActiveConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FilterMode current && parameter is string param &&
            Enum.TryParse<FilterMode>(param, out var target))
            return current == target;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
