using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MediaPlayer.ViewModels;

namespace MediaPlayer.Views
{
    public sealed partial class MediaPlayer : Page
    {
        private MediaPlayerVM ViewModel { get; set; }

        public MediaPlayer()
        {
            this.InitializeComponent();
            this.ViewModel = new MediaPlayerVM(mediaPlayer, imagesView);
            this.DataContext = ViewModel;  
            this.CustomizeTitleBar();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                mediaPlayer.MediaPlayer.Dispose();
            });
        }

        private void CustomizeTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(BackgroundElement);
        }
        
        private void SettingsNavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
    }
}
