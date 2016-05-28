using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AudioVisualization.Navigation
{
    class NavigationBarMenuItem : INavigationBarMenuItem
    {
        public object Arguments { get; private set; }

        public Type DestPage { get; private set; }

        public ImageSource Image { get; private set; }

        public string Label { get; private set; }

        public NavigationBarItemPosition Position { get; private set; }

        public char SymbolAsChar { get; private set; }

        public NavigationBarMenuItem(
            Type destinationPage,
            object arguments,
            NavigationBarItemPosition position,
            String label,
            String image,
            Char symbol)
        {
            DestPage = destinationPage;
            Arguments = arguments;
            Position = position;
            Label = label;
            SymbolAsChar = symbol;

            if (image != null)
            {
                Image = new BitmapImage(new Uri(image));
            }
        }
    }
}
