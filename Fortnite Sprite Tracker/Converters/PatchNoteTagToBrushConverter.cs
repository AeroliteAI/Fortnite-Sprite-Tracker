using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace FortniteSpriteTracker.Converters;

public class PatchNoteTagToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value as string) switch
        {
            "Added"   => new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80)),
            "Changed" => new SolidColorBrush(Color.FromRgb(0x60, 0xA5, 0xFA)),
            "Fixed"   => new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
            "Removed" => new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71)),
            _         => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8)),
        };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
