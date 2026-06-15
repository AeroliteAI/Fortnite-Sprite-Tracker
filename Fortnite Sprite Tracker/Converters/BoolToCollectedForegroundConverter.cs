using System.Globalization;
using System.Windows.Data;

namespace FortniteSpriteTracker.Converters;

public class BoolToCollectedForegroundConverter : IValueConverter
{
    private static readonly SolidColorBrush Dark = new(Color.FromRgb(0x0D, 0x0D, 0x15));
    private static readonly SolidColorBrush Grey = new(Color.FromRgb(0x94, 0xA3, 0xB8));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Dark : Grey;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
