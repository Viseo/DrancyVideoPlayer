using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using MediaPlayer.Models;
using Newtonsoft.Json;

namespace MediaPlayer.Managers
{
    public interface ISettingsManager
    {
        Task<Settings> TryGetSettingsFile();
        void CreateSettingsFile(Settings file);
    }

    public class SettingsManager : ISettingsManager
    {
        private StorageFolder _localFolder;
        private const string FileName = "Settings.json";

        public SettingsManager()
        {
            _localFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine(_localFolder.Path);
        }

        public async Task<Settings> TryGetSettingsFile()
        {
            var item = await _localFolder.TryGetItemAsync(FileName);
            if (item != null)
            {
                var settingsFile = await _localFolder.GetFileAsync(FileName);
                return await DeserializeSettingsFile(settingsFile);
            }
            return new Settings();
        }

        public async Task<Settings> DeserializeSettingsFile(IStorageFile file)
        {
            try
            {
                var data = await file.OpenReadAsync();
                using (StreamReader r = new StreamReader(data.AsStream()))
                {
                    var content = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<Settings>(content);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SettingsManager : DeserializeSettingsFile error => " + e);
                return new Settings();
            }
        }

        public async void CreateSettingsFile(Settings settings)
        {
            try
            {
                var file = await _localFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                var data = await file.OpenStreamForWriteAsync();

                using (StreamWriter r = new StreamWriter(data))
                {
                    var serelizedfile = JsonConvert.SerializeObject(settings);
                    r.Write(serelizedfile);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SettingsManager : CreateSettingsFile error => " + e);
            }
        }

    }
}
