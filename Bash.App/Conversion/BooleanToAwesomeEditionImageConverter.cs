using System;
using System.Windows.Data;

namespace Bash.App.Conversion
{
    public class BooleanToAwesomeEditionImageConverter : IValueConverter
    {
        private readonly Uri LOCKED_URI = new Uri("/Assets/warte_locked.png", UriKind.Relative);
        private readonly Uri UNLOCKED_URI = new Uri("/Assets/warte.png", UriKind.Relative);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                var boolValue = (bool)value;

                if (!boolValue)
                    return LOCKED_URI;
            }

            return UNLOCKED_URI;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
