using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace AudioVisualization.Composition
{
    static class CompositionManager
    {
        private static Compositor _compositor;
        public static Compositor Compositor
        {
            get
            {
                if (_compositor == null)
                {
                    _compositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
                }

                return _compositor;
            }
        }
    }
}
