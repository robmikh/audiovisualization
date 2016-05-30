using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace AudioVisualization.Controls.Visualizers
{
    class CompositionVisualizer : BaseVisualizer
    {
        SpriteVisual _backgroundVisual;
        CompositionBrush backgroundBrush;

        public CompositionVisualizer()
        {
            _backgroundVisual = _compositor.CreateSpriteVisual();
            _rootVisual.Children.InsertAtTop(_backgroundVisual);
            backgroundBrush = _compositor.CreateColorBrush(Colors.LightGray);
            _backgroundVisual.Brush = backgroundBrush;
        }

        protected override void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual != null)
            {
                _backgroundVisual.Size = new System.Numerics.Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }
    }
}
