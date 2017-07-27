using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using MediaPlayer.Models;

namespace MediaPlayer.Managers
{
    public interface IContentManager
    {
        ConcurrentQueue<PlaylistItem> DownloadQueue { get; set; }
        ConcurrentQueue<string> DeletionQueue { get; set; }

        Task CheckIfPlaylistItemsAreDownloaded(List<PlaylistItem> playlist);
        void ManageDownloadQueue(IHttpRequestManager httpRequestManager);
        void FillDownloadQueue(List<PlaylistItem> playlist);
        Task FillDeletionQueue(List<PlaylistItem> playlist);
        Task CleanTemporaryFolder();
    }

    public class ContentManager : IContentManager
    {
        public ConcurrentQueue<PlaylistItem> DownloadQueue { get; set; }
        public ConcurrentQueue<string> DeletionQueue { get; set; }

        public ContentManager()
        {
            DownloadQueue = new ConcurrentQueue<PlaylistItem>();
            DeletionQueue = new ConcurrentQueue<string>();
        }

        public async Task CheckIfPlaylistItemsAreDownloaded(List<PlaylistItem> playlist)
        {
            try
            {
                await Task.WhenAll(playlist
                    .Select(async item => item.IsDowloaded = await IsItemDownloaded(item))
                    .ToList());
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on CheckIfPlaylistItemsAreDownloaded " + e);
                return;
            };
        }

        private async Task<List<string>> RetrieveLostElements(List<PlaylistItem> playlist)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            var fileNames = files.Select(file => file.Name).ToList();
            var playlistItemFileNames = playlist.Select(item
                => HashFileName(item.AccessPath) + Path.GetExtension(item.AccessPath)).ToList();

            var lostedFiles = fileNames.Where(f => f != "Settings.json" && playlistItemFileNames.All(p2 => p2 != f)).ToList();

            return lostedFiles;
        }

        private async Task DeleteFile(string fileName)
        {
            var fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);

            await fileToDelete.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public async Task CleanTemporaryFolder()
        {
            var filesToDelete = await ApplicationData.Current.TemporaryFolder.GetFilesAsync();

            filesToDelete.ToList().ForEach(async x => await x.DeleteAsync());
        }

        private string HashFileName(string seed)
        {
            return BitConverter.ToString(SHA512.Create()
                    .ComputeHash(new UTF8Encoding().GetBytes(seed)))
                .Replace("-", string.Empty);
        }

        public async Task FillDeletionQueue(List<PlaylistItem> playlist)
        {
            var itemToDelete = playlist
                .Where(item => item.IsFromPreviousPlaylist)
                .Select(oldItem => HashFileName(oldItem.AccessPath) + Path.GetExtension(oldItem.AccessPath)).ToList();

            var lostElements = await RetrieveLostElements(playlist);

            DeletionQueue = new ConcurrentQueue<string>(itemToDelete.Concat(lostElements).ToList());
        }

        public void FillDownloadQueue(List<PlaylistItem> playlist)
        {
            var nonDownloadedItems = playlist.Where(item => !item.IsDowloaded).ToList();

            DownloadQueue = new ConcurrentQueue<PlaylistItem>(nonDownloadedItems);
        }

        private async Task<bool> IsItemDownloaded(PlaylistItem item)
        {
            try
            {
                var hashedFileName = HashFileName(item.AccessPath) + Path.GetExtension(item.AccessPath);
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync(hashedFileName);
                return (file != null);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async void ManageDownloadQueue(IHttpRequestManager httpRequestManager)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    while (DeletionQueue.Any())
                    {
                        var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                        bool settingsFileAndOneVideoExists = files.Count > 2;
                        if (DeletionQueue.TryDequeue(out string fileToDelete) && settingsFileAndOneVideoExists)
                            await DeleteFile(fileToDelete);
                    }
                    while (DownloadQueue.Any())
                    {
                        if (DownloadQueue.TryDequeue(out PlaylistItem item) && item.AccessPath != null)
                        {
                            try
                            {
                                await httpRequestManager.DownloadContent(item.AccessPath
                                    , HashFileName(item.AccessPath)
                                      + Path.GetExtension(item.AccessPath));
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Error on ManageDownloadQueue " + e);
                                DownloadQueue.Enqueue(item);
                            }
                        }
                        else
                            await Task.Delay(200);
                    }
                }
            });
        }


    }
}
