using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Newtonsoft.Json;

namespace MediaPlayer.Managers
{
    public interface IHttpRequestManager
    {
        Task<T> Get<T>(string route);
        Task DownloadContent(string route, string fileName);
        string GenerateUriWithParams(string route, Dictionary<string, string> parameters);
    }

    public class HttpRequestManager : IHttpRequestManager
    {
        private readonly string _apiUrl;
        private readonly HttpClient _httpClient;

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
            try
            {
                var source = new Uri(route);            
                var destination = await ApplicationData.Current.TemporaryFolder
                    .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var downloader = new BackgroundDownloader();
                var download = downloader.CreateDownload(source, destination);

                await download.StartAsync();

                var file = await ApplicationData.Current.TemporaryFolder.GetFileAsync(fileName);

                await file.MoveAsync(ApplicationData.Current.LocalFolder);
            }
            catch (TaskCanceledException tce)
            {
                Debug.WriteLine("Download Cancelled {0}", tce);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Download Error {0}", e);
                var item = await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(fileName);
                if (item != null)
                {
                    var file = await ApplicationData.Current.TemporaryFolder.GetFileAsync(fileName);
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
        }

    }
}
