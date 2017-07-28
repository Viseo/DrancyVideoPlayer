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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace MediaPlayer.ViewModels
{
    public class MediaPlayerVM : NotifyPropertyChangedVM
    {
        private bool _newItemsDownloaded;
        private DispatcherTimer _dispatcherTimer;

        private IStorageFile _currentSource;
        private MediaPlaybackList _playlist;
        private List<StorageFile> _imagePlaylist;

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
            _imagePlaylist = new List<StorageFile>();
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
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        async () =>
                        {
                            await PlayPictures();
                            VideoPlayer.Source = _playlist;
                        });
                }
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
                     await RetrievePlayLists();
                     if (_playlist.Items.Count > 0)
                         VideoPlayer.MediaPlayer.Play();
                 });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on RetrieveAndPlayMedia " + e);
            }
        }

        private async Task PlayPictures()
        {
            DisplayImagePlayer(true);
            foreach (var storageFile in _imagePlaylist)
            {
                _imagePlayer.Source = await LoadImage(storageFile);
                await Task.Delay(Dependencies.SettingsManager.SettingsState.ImagesDisplayTime * 1000);
            }
            DisplayImagePlayer(false);
        }

        private void DisplayImagePlayer(bool diplayImagePlayer)
        {
            if (diplayImagePlayer)
            {
                VideoPlayer.IsFullWindow = false;
                ImagePlayer.Visibility = Visibility.Visible;
                VideoPlayer.Visibility = Visibility.Collapsed;
            }
            else
            {
                VideoPlayer.IsFullWindow = true;
                ImagePlayer.Visibility = Visibility.Collapsed;
                VideoPlayer.Visibility = Visibility.Visible;
            }
        }

        private async Task RetrievePlayLists()
        {
            _playlist.Items.Clear();
            _imagePlaylist.Clear();

            var directoryFiles = await ApplicationData.Current.LocalFolder.GetFilesAsync();

            var videosList = directoryFiles.ToList()
                .Where(f => !Dependencies.ContentManager.DeletionQueue.Contains(f.Name)
                && IsVideoFile(f) && f.Name != "Settings.json")
                .ToList();

            _imagePlaylist = directoryFiles.ToList()
                .Where(f => !Dependencies.ContentManager.DeletionQueue.Contains(f.Name)
                            && !IsVideoFile(f) && f.Name != "Settings.json")
                .ToList();

            videosList.ForEach(x => _playlist.Items.Add(new MediaPlaybackItem(MediaSource.CreateFromStorageFile(x))));

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
