using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AudioVisualization.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="bool" />.
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts the value to <see cref="Visibility" />.
        /// </summary>
        /// <param name="value">The value converted to Visibility.</param>
        /// <returns>Visible, if true. Otherwise, collapsed.</returns>
        public static Visibility ToVisibility(this bool value)
        {
            if (value)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }
    }
}
