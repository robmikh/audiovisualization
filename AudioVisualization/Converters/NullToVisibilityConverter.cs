using AudioVisualization.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AudioVisualization.Converters
{
    /// <summary>
    /// A converter that returns Visibility.Visible if the input value
    /// is not null and Visibility.Collapsed otherwise.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var invert = (parameter != null) && System.Convert.ToBoolean(parameter);

            var isVisible = value != null;

            if (invert)
            {
                isVisible = !isVisible;
            }

            return isVisible.ToVisibility();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
