using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using MediaPlayer.Models;

namespace MediaPlayer.Managers
{
    public interface IContentManager
    {
        Task CheckIfPlaylistItemsAreDownloaded(List<PlaylistItem> playlist);
        void ManageDownloadQueue(IHttpRequestManager httpRequestManager);
        void FillDownloadQueue(List<PlaylistItem> playlist);
    }

    public class ContentManager : IContentManager
    {
        private ConcurrentQueue<PlaylistItem> _downloadQueue;

        public ContentManager()
        {
            _downloadQueue = new ConcurrentQueue<PlaylistItem>();
        }

        public async Task CheckIfPlaylistItemsAreDownloaded(List<PlaylistItem> playlist)
        {
            try
            {
                await Task.WhenAll(playlist
                    .Select(async item => item.IsDowloaded = await IsItemDownloaded(item.AccessPath))
                    .ToList());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on CheckIfPlaylistItemsAreDownloaded " + e);
                return;
            };
        }

        public void FillDownloadQueue(List<PlaylistItem> playlist)
        {
            var nonDownloadedItems = playlist.Where(item => !item.IsDowloaded).ToList();

            _downloadQueue = new ConcurrentQueue<PlaylistItem>(nonDownloadedItems);
        }

        private async Task<bool> IsItemDownloaded(string fileName)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
                return (file != null);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async void ManageDownloadQueue(IHttpRequestManager httpRequestManager)
        {
            int i = 0;
            await Task.Run(async () =>
            {
                while (true)
                {
                    PlaylistItem item;

                    if (_downloadQueue.TryDequeue(out item) && item.AccessPath != null)
                    {
                        try
                        {
                             await httpRequestManager.DownloadContent(item.AccessPath
                                 ,string.Format("{0:X}", item.Id.GetHashCode()) 
                                 + Path.GetExtension(item.AccessPath));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error on ManageDownloadQueue " + e);
                            _downloadQueue.Enqueue(item);
                        }
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                    i++;
                }
            });
        }


    }
}
