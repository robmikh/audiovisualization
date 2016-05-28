using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace AudioVisualization.Navigation
{
    /// <summary>
    /// Defines a menu item for the navigation bar.
    /// </summary>
    public interface INavigationBarMenuItem
    {
        /// <summary>
        /// Gets the arguments that can be passed optionally to the target page.
        /// </summary>
        object Arguments { get; }

        /// <summary>
        /// Gets the type of the destination page.
        /// </summary>
        Type DestPage { get; }

        /// <summary>
        /// Gets the image that is displayed in the navigation bar.
        /// </summary>
        ImageSource Image { get; }

        /// <summary>
        /// Gets the title displayed in the navigation bar.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the positions of the current item in the navigation bar.
        /// </summary>
        NavigationBarItemPosition Position { get; }

        /// <summary>
        /// Gets the symbol character that is displayed in the
        /// navigation bar.
        /// </summary>
        char SymbolAsChar { get; }
    }
}
