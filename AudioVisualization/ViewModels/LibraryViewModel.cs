using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Extensions;
using AudioVisualization.Playback;
using AudioVisualization.Services;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace AudioVisualization.ViewModels
{
    class LibraryViewModel : ViewModel
    {
        public ObservableCollection<Song> Songs { get; }

        public LibraryViewModel()
        {
            Songs = new ObservableCollection<Song>();
            GetMusicLibrary();
        }

        private async void GetMusicLibrary()
        {
            var files = await StorageService.Current.GetMusicLibrary();
            System.Diagnostics.Debug.WriteLine(files.Count);
            foreach (var file in files)
            {
                var song = await file.ToSong();
                Songs.Add(song);
            }
        }

        public void PlayFromIndex(int index)
        {
            PlayerService.Current.Playlist.Clear();
            foreach(var song in Songs)
            {
                PlayerService.Current.Playlist.Add(song);
            }

            PlayerService.Current.Playlist.PlaybackList.MoveTo((uint)index);
            PlayerService.Current.Play();
        }
    }
}
