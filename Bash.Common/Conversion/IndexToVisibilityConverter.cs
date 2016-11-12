using System;
using System.Windows.Data;
using System.Windows;

namespace Bash.Common.Conversion
{
    /// <summary>
    /// Value converter to convert quote indexes with a visibility for the speech bubble pointers.
    /// </summary>
    public class IndexToVisibilityConverter : IValueConverter
    {
        private const string SERVER_ONLY = "server-only";
        private const string INVERTED = "not";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = -1;
            bool isInverted = false;
            bool isVisible = true;
            bool isServerOnly = false;
            if (value is int)
            {
                index = (int)value;
            }
            if (parameter is string)
            {
                string param = parameter as string;

                if (param.StartsWith(INVERTED))
                    isInverted = true;
                if (param.Contains(SERVER_ONLY))
                    isServerOnly = true;
            }

            // server-only
            if (isServerOnly)
            {
                isVisible = index < 0;
            }
            else
            {
                // server
                if (index < 0)
                {
                    if (isServerOnly)
                        isVisible = true;
                    else
                        isVisible = false;
                }
                else
                {
                    if (isServerOnly)
                    {
                        isVisible = false;
                    }
                    else if (index % 2 == 0)
                    {
                        isVisible = true;
                    }
                    else
                    {
                        isVisible = false;
                    }
                }
            }

            if (isInverted)
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
