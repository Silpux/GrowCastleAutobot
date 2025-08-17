using System.Globalization;
using System.Windows.Data;

namespace gca.Classes.Converters
{
    public class AddConverter : IValueConverter
    {
        public double Add { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double original)
            {
                return original + Add;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
