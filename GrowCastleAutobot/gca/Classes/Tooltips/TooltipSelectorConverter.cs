using System.Globalization;
using System.Windows.Data;

namespace gca.Classes.Tooltips
{
    public class TooltipSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnabled = (bool)values[0];
            string enabledTip = values[1]?.ToString() ?? "";
            string disabledTip = values[2]?.ToString() ?? "";
            return isEnabled ? enabledTip : disabledTip;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
