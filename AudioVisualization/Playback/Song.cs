using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AudioVisualization.Playback
{
    class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public uint TrackNumber { get; set; }
        public IList<string> Genres { get; }

        public BitmapImage AlbumArt { get; set; }
        public MediaPlaybackItem PlaybackItem { get; set; }

        public Song()
        {
            Genres = new List<string>();
        }
    }
}
