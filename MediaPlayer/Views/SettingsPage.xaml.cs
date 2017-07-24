using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MediaPlayer.ViewModels;

namespace MediaPlayer.Views
{
    public sealed partial class SettingsPage : Page
    {
        private SettingsPageVM ViewModel { get; set; }

        public SettingsPage()
        {
            this.InitializeComponent();
            this.ViewModel = new SettingsPageVM(Dependencies.SettingsManager);
            this.DataContext = this.ViewModel;
            this.CustomizeTitleBar();
        }

        private void CustomizeTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(BackgroundElement);
        }

        private void OnSaveClicked(object sender, TappedRoutedEventArgs e)
        {
            if (this.ViewModel.SaveSettings())
                this.Frame.Navigate(typeof(MediaPlayer));
        }

        private void MediaPlayerNavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
            this.Frame.Navigate(typeof(MediaPlayer));
        }
    }
}
