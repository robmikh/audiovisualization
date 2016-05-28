using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AudioVisualization.Views
{
    /// <summary>
    /// A base UI page.
    /// </summary>
    public class BasePage : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BasePage()
        {
        }

        /// <summary>
        /// Notifies that the property has changed.
        /// </summary>
        /// <param name="propertyName">The property name. Optional.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
