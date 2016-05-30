using AudioVisualization.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AudioVisualization.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingPage : BasePage
    {
        public NowPlayingPage()
        {
            CreateStaticResources();

            this.InitializeComponent();
        }

        void CreateStaticResources() {
            this.Resources.Add("StartWin2d", new DelegateCommand(
               (obj) => {
                   CheckBox sender = obj as CheckBox;

                   if (sender?.IsChecked.Value==true)
                   {
                       Services.PlayerService.Current.StartVisualization(win2dVisualizer);
                   }
                   else
                   {
                       Debug.WriteLine("Checked");
                   }
               },
           (obj) => {
               return true;
           }));
        }

    }
}
