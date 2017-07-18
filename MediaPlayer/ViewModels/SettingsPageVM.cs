using MediaPlayer.Managers;
using MediaPlayer.Models;

namespace MediaPlayer.ViewModels
{
    public class SettingsPageVM : NotifyPropertyChangedVM
    {
        private ISettingsManager _settingsManager;

        private Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; OnPropertyChanged(); }
        }

        public SettingsPageVM()
        {
            _settingsManager = new SettingsManager();
            InitSettings();
        }

        private async void InitSettings()
        {
            Settings = await _settingsManager.TryGetSettingsFile();
        }

        public void SaveSettings()
        {
            _settingsManager.CreateSettingsFile(Settings);
        }

    }
}
