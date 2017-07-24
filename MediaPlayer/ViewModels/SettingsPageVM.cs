using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using MediaPlayer.Managers;
using MediaPlayer.Models;

namespace MediaPlayer.ViewModels
{
    public class SettingsPageVM : NotifyPropertyChangedVM
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ResourceLoader _stringLoader;

        private string _errorMsg;
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { _errorMsg = value; OnPropertyChanged(); }
        }

        private Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
            set { _settings = value; OnPropertyChanged(); }
        }

        public SettingsPageVM(ISettingsManager manager)
        {
            _settingsManager = manager;
            _stringLoader = new ResourceLoader();
            Settings = _settingsManager.SettingsState;
        }

        public async Task<bool> DoesSettingsExist()
        {
            if (await _settingsManager.IsSettingsFileExist() && Settings.AreNonNumericFieldsValid())
            {
                if (!Settings.AreNumericFieldsValid())
                    Settings.SetDefaultValue();
                return true;
            }
            return false;
        }

        public bool SaveSettings()
        {
            ErrorMsg = "";
            if (Settings.AreNonNumericFieldsValid())
            {
                if (!Settings.AreNumericFieldsValid())
                    Settings.SetDefaultValue();
                _settingsManager.SettingsState = Settings;
                _settingsManager.CreateSettingsFile();
                return true;
            }
            ErrorMsg = _stringLoader.GetString("errorMessageInvalidFields");
            return false;
        }

    }
}
