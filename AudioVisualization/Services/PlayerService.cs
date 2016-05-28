using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Playback;
using Windows.Media.Playback;

namespace AudioVisualization.Services
{
    class PlayerService
    {
        private static PlayerService _current;
        public static PlayerService Current { get { if (_current == null) { _current = new PlayerService(); } return _current; } }

        private MediaPlayer _mediaPlayer;

        public Playlist Playlist { get; }

        private PlayerService()
        {
            Playlist = new Playlist();

            InitializeMediaPlayer();
        }

        private void InitializeMediaPlayer()
        {
            _mediaPlayer = new MediaPlayer();

            _mediaPlayer.Source = Playlist.PlaybackList;
        }

        public void Play()
        {
            _mediaPlayer.Play();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
        }
    }
}
