using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MediaPlayer.Managers
{
    public interface IHttpRequestManager
    {
        Task<T> Get<T>(string route);
        string GenerateUriWithParams(string route, Dictionary<string, string> parameters);
        Task DownloadContent(string route, string fileName);
        event EventHandler ContentDownloaded;
    }

    public class HttpRequestManager : IHttpRequestManager
    {
        private readonly string _apiUrl;
        private readonly HttpClient _httpClient;

        public event EventHandler ContentDownloaded;

        public HttpRequestManager(string apiUrl)
        {
            _apiUrl = apiUrl;
            _httpClient = new HttpClient();
        }

        public string GenerateUriWithParams(string route, Dictionary<string, string> parameters)
        {
            if (parameters.Count > 0)
            {
                var uri = route + "?";
                foreach (var item in parameters)
                {
                    uri += item.Key + "=" + item.Value;
                    uri += "&";
                }
                return uri.Remove(uri.Length - 1);
            }
            return route;
        }

        public async Task<T> Get<T>(string route)
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, _apiUrl + route));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
        
        public async Task DownloadContent(string route, string fileName)
        {
            var uri = new Uri(route);

            var storageFile = await ApplicationData.Current.TemporaryFolder
                .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var mediaBytes = await new HttpClient().GetByteArrayAsync(uri);

            var buffer = mediaBytes.AsBuffer();

            await FileIO.WriteBufferAsync(storageFile, buffer);

            var file = await ApplicationData.Current.TemporaryFolder.GetFileAsync(fileName);

            await file.MoveAsync(ApplicationData.Current.LocalFolder);

            ContentDownloaded?.Invoke(null, new EventArgs());
        }
    }
}
