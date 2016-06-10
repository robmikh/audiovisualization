using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Services;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace AudioVisualization.Controls.Visualizers
{
    class CompositionVisualizer : BaseVisualizer
    {
        SpriteVisual _backgroundVisual;
        CompositionBrush backgroundBrush;

        SpriteVisual _barVisual;

        public CompositionVisualizer()
        {
            _backgroundVisual = _compositor.CreateSpriteVisual();
            _rootVisual.Children.InsertAtTop(_backgroundVisual);
            backgroundBrush = _compositor.CreateColorBrush(Colors.LightGray);
            _backgroundVisual.Brush = backgroundBrush;

            SetupVisualizer();
        }

        protected override void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual != null)
            {
                _backgroundVisual.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }

        private void SetupVisualizer()
        {
            _barVisual = _compositor.CreateSpriteVisual();
            _barVisual.Size = new Vector2(50, 0);
            _barVisual.AnchorPoint = new Vector2(0.5f, 1);
            _barVisual.Brush = _compositor.CreateColorBrush(Colors.Red);

            var sizeExpression = _compositor.CreateExpressionAnimation();
            sizeExpression.Expression = "propertySet.InputData";
            sizeExpression.SetReferenceParameter("propertySet", PlayerService.Current.CompositionPropertySet);
            _barVisual.StartAnimation(nameof(Visual.Size) + ".Y", sizeExpression);

            var offsetExpression = _compositor.CreateExpressionAnimation();
            offsetExpression.Expression = "Vector3(visual.Size.X / 2, visual.Size.Y, 0)";
            offsetExpression.SetReferenceParameter("visual", _backgroundVisual);
            _barVisual.StartAnimation(nameof(Visual.Offset), offsetExpression);

            _backgroundVisual.Children.InsertAtTop(_barVisual);
        }
    }
}
