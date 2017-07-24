using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using MediaPlayer.Models;
using Newtonsoft.Json;

namespace MediaPlayer.Managers
{
    public interface ISettingsManager
    {
        Settings SettingsState { get; set; }
        Task<bool> IsSettingsFileExist();
        Task<Settings> TryGetSettingsFile();
        void CreateSettingsFile();
        Settings DeserializeFile(string stream);
    }

    public class SettingsManager : ISettingsManager
    {
        private const string FileName = "Settings.json";
        private readonly StorageFolder _localFolder;

        public Settings SettingsState { get; set; }

        public SettingsManager()
        {
            _localFolder = ApplicationData.Current.LocalFolder;
            Initialize();
        }

        private async void Initialize()
        {
            await TryGetSettingsFile();
        }

        public async Task<bool> IsSettingsFileExist()
        {
            var item = await _localFolder.TryGetItemAsync(FileName);

            if (item != null)
                return true;
            return false;
        }

        public async Task<Settings> TryGetSettingsFile()
        {
            if (await IsSettingsFileExist())
            {
                var settingsFile = await _localFolder.GetFileAsync(FileName);
                SettingsState = await ExctractStream(settingsFile);
                return SettingsState;
            }
            SettingsState = new Settings();
            return SettingsState;
        }

        private async Task<Settings> ExctractStream(IRandomAccessStreamReference file)
        {
            var data = await file.OpenReadAsync();
            using (StreamReader r = new StreamReader(data.AsStream()))
                return DeserializeFile(r.ReadToEnd());
        }

        public Settings DeserializeFile(string stream)
        {
            try
            {
                var deserializedObj = JsonConvert.DeserializeObject<Settings>(stream);
                return deserializedObj ?? new Settings();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new Settings();
            }

        }

        public async void CreateSettingsFile()
        {
            var file = await _localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            var data = await file.OpenStreamForWriteAsync();

            using (StreamWriter r = new StreamWriter(data))
            {
                var serelizedfile = JsonConvert.SerializeObject(SettingsState);
                r.Write(serelizedfile);
            }
        }


    }
}
