using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Front;

/// <summary>
/// Returns true when count > 0 (items exist).
/// Pass ConverterParameter="empty" to invert (true when count == 0).
/// </summary>
public class CountToBoolConverter : IValueConverter
{
    public static readonly CountToBoolConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int count = value is int i ? i : 0;
        bool hasItems = count > 0;
        return parameter is string p && p == "empty" ? !hasItems : hasItems;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
