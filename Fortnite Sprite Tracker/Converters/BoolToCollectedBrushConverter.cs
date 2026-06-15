using System.Globalization;
using System.Windows.Data;

namespace FortniteSpriteTracker.Converters;

public class BoolToCollectedBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Green = new(Color.FromRgb(0x4A, 0xDE, 0x80));
    private static readonly SolidColorBrush Dark  = new(Color.FromRgb(0x25, 0x25, 0x3E));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Green : Dark;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
