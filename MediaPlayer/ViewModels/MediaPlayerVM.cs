using Windows.UI.Xaml.Controls;
using MediaPlayer.Managers;

namespace MediaPlayer.ViewModels
{
    public class MediaPlayerVM : NotifyPropertyChangedVM
    {
        private MediaPlayerElement _videoPlayer;
        public MediaPlayerElement VideoPlayer
        {
            get { return _videoPlayer; }
            set { _videoPlayer = value; OnPropertyChanged(); }
        }

        private FlipView _imagePlayer;
        public FlipView ImagePlayer
        {
            get { return _imagePlayer; }
            set { _imagePlayer = value; OnPropertyChanged(); }
        }

        public MediaPlayerVM(MediaPlayerElement videoPlayer, FlipView imagePlayer)
        {
            VideoPlayer = videoPlayer;
            ImagePlayer = imagePlayer;           
        }

    }
}
