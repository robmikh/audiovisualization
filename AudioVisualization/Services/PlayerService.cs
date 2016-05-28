using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioVisualization.Playback;
using Windows.Media.Playback;
using Windows.Foundation.Collections;
using AudioVisualization.AutoProcessing;
using System.Diagnostics;

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

            _mediaPlayer.MediaFailed += _mediaPlayer_MediaFailed;

            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;

            _mediaPlayer.CurrentStateChanged += _mediaPlayer_CurrentStateChanged;

            _mediaPlayer.Source = Playlist.PlaybackList;

            //Not working
            //AddEchoEffect(_mediaPlayer);
        }

        private void _mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine(sender.CurrentState.ToString());
        }

        private void _mediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("Ended");
        }

        private void _mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine(args.ErrorMessage);
        }

        public void Play()
        {
            _mediaPlayer.Play();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
        }

        private void AddEchoEffect(MediaPlayer player)
        {
            // Create a property set and add a property/value pair 
            PropertySet echoProperties = new PropertySet();
            echoProperties.Add("Mix", 0.5f);

            // Instantiate the custom effect defined in the 'CustomEffect' project 
            player.AddAudioEffect(typeof(AudioEchoEffect).FullName, true, echoProperties);
        }

    }
}
