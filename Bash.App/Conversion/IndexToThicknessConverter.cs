using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bash.App.Conversion
{
    /// <summary>
    /// The value converer to convert a index to a margin thickness for the speech bubbles.
    /// </summary>
    public class IndexToThicknessConverter : IValueConverter
    {
        private static readonly int[] ORDER = { 0, 80, 10, 70, 20, 60, 30, 50, 40 };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = -1;
            if (value is int)
            {
                index = (int)value;
            }

            if (index < 0)
            {
                // server
                return new Thickness();
            }

            index = index % ORDER.Length;
            return new Thickness(ORDER[index], 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
