using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.UI.Composition;
using Windows.UI.Core;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using SampleGrabberCS.Reference;
using AudioVisualization.Services;

namespace AudioVisualization.Controls.Visualizers
{
    internal class Win2DVisualizer : BaseVisualizer, IDisposable
    {
        CanvasDevice _device;
        CompositionGraphicsDevice _compositionGraphicsDevice;

        CanvasSwapChain _swapChain;
        SpriteVisual _swapChainVisual;
        CancellationTokenSource _drawLoopCancellationTokenSource;


        public Win2DVisualizer()
        {
            CreateDevice();
            _swapChainVisual = _compositor.CreateSpriteVisual();
            _rootVisual.Children.InsertAtTop(_swapChainVisual);
        }

        protected override void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e!=null && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                SetDevice(_device, e.NewSize);
                _swapChainVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            }
        }

        internal override void OnUnloaded(object sender, RoutedEventArgs e)
        {
            base.OnUnloaded(sender, e);
            this.Dispose();
        }

        void SetDevice(CanvasDevice device, Size windowSize)
        {
            _drawLoopCancellationTokenSource?.Cancel();

            _swapChain = new CanvasSwapChain(device, (float)this.ActualWidth, (float)this.ActualHeight, 96);
            _swapChainVisual.Brush = _compositor.CreateSurfaceBrush(CanvasComposition.CreateCompositionSurfaceForSwapChain(_compositor, _swapChain));

            _drawLoopCancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(
                DrawLoop,
                _drawLoopCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }


        void CreateDevice()
        {
            _device = CanvasDevice.GetSharedDevice();
            _device.DeviceLost += Device_DeviceLost;

            if (_compositionGraphicsDevice == null)
            {
                _compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _device);
            }
            else
            {
                CanvasComposition.SetCanvasDevice(_compositionGraphicsDevice, _device);
            }
        }

        void Device_DeviceLost(CanvasDevice sender, object args)
        {
            _device.DeviceLost -= Device_DeviceLost;

            var unwaitedTask = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CreateDevice());
        }

        public void Dispose()
        {
            _drawLoopCancellationTokenSource?.Cancel();
            _swapChain?.Dispose();
        }

        void DrawLoop()
        {
            var canceled = _drawLoopCancellationTokenSource.Token;

            try
            {
                while (!canceled.IsCancellationRequested)
                {
                    DrawSwapChain(_swapChain);
                    _swapChain.WaitForVerticalBlank();
                }

                _swapChain.Dispose();
            }
            catch (Exception e) when (_swapChain.Device.IsDeviceLost(e.HResult))
            {
                _swapChain.Device.RaiseDeviceLost();
            }
        }

        void DrawSwapChain(CanvasSwapChain swapChain)
        {
            using (var ds = swapChain.CreateDrawingSession(Colors.Transparent))
            {
                var size = swapChain.Size.ToVector2();
                var radius = (Math.Min(size.X, size.Y) / 2.0f) - 4.0f;

                var center = size / 2;

                float fillRadius = radius;

                // yuck
                lock(PassthroughEffect.GetBadLock())
                {
                    if (PlayerService.Current.ReferencePropertySet != null &&
                        PlayerService.Current.ReferencePropertySet.ContainsKey("InputDataRaw"))
                    {
                        fillRadius *= (float)PlayerService.Current.ReferencePropertySet["InputDataRaw"];
                    }
                }

                ds.FillCircle(center, fillRadius, Colors.LightGoldenrodYellow);
                ds.DrawCircle(center, radius, Colors.LightGray);
            }

            swapChain.Present();
        }

    }
}
