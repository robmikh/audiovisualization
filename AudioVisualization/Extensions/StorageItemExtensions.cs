using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Playback;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AudioVisualization.Extensions
{
    static class StorageItemExtensions
    {
        public static async Task<Song> ToSong(this StorageFile file)
        {
            var musicProperties = await file.Properties.GetMusicPropertiesAsync();
            var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView);

            var source = MediaSource.CreateFromStorageFile(file);
            var mediaItem = new MediaPlaybackItem(source);

            var displayProperties = mediaItem.GetDisplayProperties();
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromStream(thumbnail);
            displayProperties.MusicProperties.AlbumArtist = musicProperties.AlbumArtist;
            displayProperties.MusicProperties.AlbumTitle = musicProperties.Album;
            displayProperties.MusicProperties.TrackNumber = musicProperties.TrackNumber;
            displayProperties.MusicProperties.Title = musicProperties.Title;
            displayProperties.MusicProperties.Artist = musicProperties.Artist;

            var song = new Song();

            song.AlbumArtist = musicProperties.AlbumArtist;
            song.Album = musicProperties.Album;
            song.TrackNumber = musicProperties.TrackNumber;
            song.Title = musicProperties.Title;
            song.Artist = musicProperties.Artist;
            foreach (var genre in musicProperties.Genre)
            {
                song.Genres.Add(genre);
                displayProperties.MusicProperties.Genres.Add(genre);
            }

            mediaItem.ApplyDisplayProperties(displayProperties);

            song.PlaybackItem = mediaItem;
            song.AlbumArt = new BitmapImage();
            await song.AlbumArt.SetSourceAsync(thumbnail);

            return song;
        }
    }
}
