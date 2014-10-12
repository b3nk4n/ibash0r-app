using Bash.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Bash.Common.Conversion
{
    /// <summary>
    /// The converter to get different colors for speech bubbles.
    /// </summary>
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = 0;
            if (value is int)
            {
                int valueInt = (int)value;
                index = valueInt % AppConstants.COLORS.Length;
            }

            if (index >= 0)
            {
                return new SolidColorBrush(AppConstants.COLORS[index]);
            }
            else
            {
                return new SolidColorBrush(AppConstants.SERVER_COLOR);
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
