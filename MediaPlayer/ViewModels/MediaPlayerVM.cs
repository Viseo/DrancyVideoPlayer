using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaPlayer.ViewModels
{
    public class MediaPlayerVM : NotifyPropertyChangedVM
    {
        private bool _newItemsDownloaded;

        private IStorageFile _currentSource;
        private MediaPlaybackList _playlist;

        private MediaPlayerElement _videoPlayer;
        public MediaPlayerElement VideoPlayer
        {
            get { return _videoPlayer; }
            set { _videoPlayer = value; OnPropertyChanged(); }
        }

        private Image _imagePlayer;
        public Image ImagePlayer
        {
            get { return _imagePlayer; }
            set { _imagePlayer = value; OnPropertyChanged(); }
        }

        public MediaPlayerVM(MediaPlayerElement videoPlayer, Image imagePlayer)
        {
            VideoPlayer = videoPlayer;
            ImagePlayer = imagePlayer;

            PlayerOnLaunch();
        }

        private async void PlayerOnLaunch()
        {
            _playlist = new MediaPlaybackList();
            VideoPlayer.Source = _playlist;

            await RetrieveAndPlayMedia();
            await SetDefaultVideo();

            VideoPlayer.MediaPlayer.MediaEnded += MediaPlayedEnded;
            Dependencies.HttpRequestManager.ContentDownloaded += OnDownloadComplete;
        }

        private async void MediaPlayedEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            try
            {
                if (_newItemsDownloaded)
                    await RetrieveAndPlayMedia();
                else
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () => VideoPlayer.Source = _playlist);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private async void OnDownloadComplete(object sender, EventArgs eventArgs)
        {
            _newItemsDownloaded = true;
            if (_playlist.Items.Count == 0)
            {
                await RetrieveAndPlayMedia();
                VideoPlayer.MediaPlayer.Play();
            }
        }

        private async Task SetDefaultVideo()
        {
            try
            {
                if (_playlist.Items.Count == 0)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            var defaultVideo =
                                MediaSource.CreateFromUri(
                                    new Uri(Dependencies.SettingsManager.SettingsState.DefaultClipURL));
                            var defaultMediaItem = new MediaPlaybackItem(defaultVideo);
                            _playlist.Items.Add(defaultMediaItem);
                            VideoPlayer.MediaPlayer.Play();
                        });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private async Task RetrieveAndPlayMedia()
        {
            try
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                 {
                     await RetrievePlayList();
                     if (_playlist.Items.Count > 0)
                         VideoPlayer.MediaPlayer.Play();
                 });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on RetrieveAndPlayMedia " + e);
            }
        }

        private async Task RetrievePlayList()
        {
            _playlist.Items.Clear();

            var directoryFiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            var filteredList = directoryFiles.ToList()
                .Where(f => !Dependencies.ContentManager.DeletionQueue.Contains(f.Name)
                && IsVideoFile(f) && f.Name != "Settings.json")
                .ToList();

            filteredList.ForEach(x => _playlist.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromStorageFile(x))));

            _newItemsDownloaded = false;
        }

        private bool IsVideoFile(IStorageFile file)
        {
            var videoFormats = new List<string> { ".MP4", ".MKV", ".MOV" };

            return videoFormats.Contains(file.FileType.ToUpper());
        }

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

                bitmapImage.SetSource(stream);

                return bitmapImage;
            }
            catch (Exception e)
            {
                return new BitmapImage();
            }
        }


    }
}
