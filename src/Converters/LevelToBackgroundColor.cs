
using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace event_log_monitor.Converters;
public class LevelToBackgroundColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var _default = System.Windows.SystemColors.WindowBrush;

        if (value is not string stringValue) return _default;
        if (string.IsNullOrWhiteSpace(stringValue)) return _default;
        
        switch (stringValue)
        {
            case "Error": return Brushes.LightSalmon;
            case "Warning": return Brushes.LightYellow;
            case "Information": return _default;
            case "SuccessAudit": return _default;
            case "FailureAudit": return Brushes.LightSalmon;
            default: return _default;
        }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
