using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AudioVisualization.Playback;
using AudioVisualization.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class LibraryPage : BasePage
    {
        private LibraryViewModel _viewModel;

        public LibraryPage()
        {
            this.InitializeComponent();

            _viewModel = new LibraryViewModel();
            this.DataContext = _viewModel;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = e.ClickedItem as Song;
            var index = _viewModel.Songs.IndexOf(song);

            _viewModel.PlayFromIndex(index);
        }
    }
}
