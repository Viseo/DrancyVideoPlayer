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
            this.ViewModel = new SettingsPageVM();
            this.DataContext = this.ViewModel;
        }

        private void OnSaveClicked(object sender, TappedRoutedEventArgs e)
        {
            this.ViewModel.SaveSettings();
        }

    }
}
