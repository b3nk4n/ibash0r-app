﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bash.App.Conversion
{
    /// <summary>
    /// Prepends a PLUS before a positive number.
    /// </summary>
    public class PositivePlusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                var intValue = (int)value;
                if (intValue > 0)
                    return string.Format("+{0}", intValue);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
