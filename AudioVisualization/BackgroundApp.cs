using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Xaml;

namespace AudioVisualization
{
    sealed partial class App : Application
    {
        bool _isInBackgroundMode;

        partial void Construct()
        {
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            EnteredBackground += App_EnteredBackground;
            LeavingBackground += App_LeavingBackground;

            Suspending += App_Suspending;
            Resuming += App_Resuming;
        }

        private void App_Resuming(object sender, object e)
        {

        }

        private void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        private void App_LeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
        {
            var oldState = _isInBackgroundMode;
            _isInBackgroundMode = false;

            if (oldState && Window.Current.Content == null)
            {
                CreateRootFrame(ApplicationExecutionState.Running, string.Empty);
            }
        }

        private void App_EnteredBackground(object sender, Windows.ApplicationModel.EnteredBackgroundEventArgs e)
        {
            _isInBackgroundMode = true;
            UnloadViewContentIfNeeded();
        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            UnloadViewContentIfNeeded();
        }

        public void UnloadViewContentIfNeeded()
        {
            var level = MemoryManager.AppMemoryUsageLevel;

            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
            {
                if (_isInBackgroundMode && Window.Current.Content != null)
                {
                    Window.Current.Content = null;
                    GC.Collect();
                }
            }
        }
    }
}
