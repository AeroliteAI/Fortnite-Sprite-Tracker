using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace FortniteSpriteTracker.Converters;

public class BoolToCollectedForegroundConverter : IValueConverter
{
    private static readonly IBrush Green = new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80));
    private static readonly IBrush Grey  = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Green : Grey;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
