using AudioVisualization.AutoProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace AudioVisualization.Controls.Visualizers
{
    abstract internal class BaseVisualizer : Control
    {
        // Audio processing fields
        MediaPlayer _mediaPlayer;
        PropertySet _audioData;

        // Composition fields
        protected Compositor _compositor;
        protected ContainerVisual _rootVisual;

        internal BaseVisualizer()
        {
            var elementVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            _compositor = elementVisual.Compositor;
            _rootVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this as UIElement, _rootVisual);

            this.Loading += OnLoading;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            this.SizeChanged += OnSizeChanged;
            OnSizeChanged(this, null);
        }

        abstract protected void OnSizeChanged(object sender, SizeChangedEventArgs e);
        

        internal virtual void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= OnSizeChanged;
        }

        internal void SetAudioDataSource(PropertySet audioData)
        {
            _audioData = audioData;
        }
    }
}
