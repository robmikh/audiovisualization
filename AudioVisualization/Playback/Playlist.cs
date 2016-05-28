using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Extensions;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AudioVisualization.Playback
{
    class Playlist : ObservableCollection<Song>
    {
        public MediaPlaybackList PlaybackList { get; }

        public Playlist()
        {
            PlaybackList = new MediaPlaybackList();
        }

        protected override void InsertItem(int index, Song item)
        {
            base.InsertItem(index, item);

            PlaybackList.Items.Insert(index, item.PlaybackItem);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            PlaybackList.Items.RemoveAt(index);
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            PlaybackList.Items.Clear();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new InvalidOperationException();
        }

        protected override void SetItem(int index, Song item)
        {
            throw new InvalidOperationException();
        }

        public async void AddStorageItems(IEnumerable<StorageFile> files)
        {
            foreach (var file in files)
            {
                var song = await file.ToSong();
                Add(song);
            }
        }
    }
}
