using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Front;

/// <summary>
/// Returns true when the bound value is an instance of the target type.
/// Used in XAML as:
///   Converter={x:Static local:IsTypeConverter.HomeTab}
/// </summary>
public class IsTypeConverter : IValueConverter
{
    public static readonly IsTypeConverter HomeTab = new(typeof(HomeTabSentinel));

    private readonly Type _targetType;

    public IsTypeConverter(Type targetType) => _targetType = targetType;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value != null && _targetType.IsInstanceOfType(value);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
