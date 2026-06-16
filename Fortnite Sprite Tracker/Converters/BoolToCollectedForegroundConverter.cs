using System.Globalization;
using System.Windows.Data;

namespace FortniteSpriteTracker.Converters;

public class BoolToCollectedForegroundConverter : IValueConverter
{
    private static readonly SolidColorBrush Green = new(Color.FromRgb(0x4A, 0xDE, 0x80));
    private static readonly SolidColorBrush Grey  = new(Color.FromRgb(0x94, 0xA3, 0xB8));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Green : Grey;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
