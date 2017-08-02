using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediaPlayer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaPlayer.Managers
{
    public interface IPlanningManager
    {
        Playlist PlayListState { get; set; }

        Task RetrievePlaylist(string screenId, string securityKey
            , string defaultClipUrl, IHttpRequestManager httpRequestManager);

        List<PlaylistItem> SetOldFlagToPreviousPlaylistItems(List<PlaylistItem> oldPlaylist
            , List<PlaylistItem> newPlaylist);
    }

    public class PlanningManager : IPlanningManager
    {
        public Playlist PlayListState { get; set; }

        public PlanningManager()
        {
            PlayListState = new Playlist();
        }

        public async Task RetrievePlaylist(string screenId, string securityKey
            , string defaultClipUrl, IHttpRequestManager httpRequestManager)
        {
            try
            {
                var requestParams = new Dictionary<string, string>
                {
                    {"screenName", screenId},
                    {"code", securityKey}
                };
                var route = httpRequestManager.GenerateUriWithParams("/get-playlist", requestParams);
                var response = await httpRequestManager.Get<JToken>(route);
                DeserializeResponseAsPlaylist(response, defaultClipUrl);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on RetrievePlaylist : " + e);
            }
        }

        private void DeserializeResponseAsPlaylist(JToken response, string defaultClipUrl)
        {
            try
            {
                var playlist = new List<PlaylistItem>();
                response["Groups"].Children().ToList()
                    .ForEach(grp => grp["Items"].ToList()
                    .ForEach(it => playlist.Add(JsonConvert.DeserializeObject<PlaylistItem>(it.ToString()))));
                PlayListState.PlaylistItems = SetOldFlagToPreviousPlaylistItems(PlayListState.PlaylistItems, playlist);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on Method DeserializeResponseAsPlaylist => {0}", e);
                var defaultPlaylist = new List<PlaylistItem>
                {
                    new PlaylistItem()
                    {
                        AccessPath = defaultClipUrl,
                        Id = "_settings.SettingsState.DefaultClipURL",
                        IsDowloaded = false
                    }
                };
                PlayListState.PlaylistItems = defaultPlaylist;
            }
        }

        public List<PlaylistItem> SetOldFlagToPreviousPlaylistItems(List<PlaylistItem> oldPlaylist, List<PlaylistItem> newPlaylist)
        {
            var oldIds = oldPlaylist.Select(item => item.Id).ToList();
            var newIds = newPlaylist.Select(item => item.Id).ToList();
            var isSamePlaylist = oldIds.SequenceEqual(newIds);

            if (!isSamePlaylist)
            {
                oldPlaylist.ForEach(item => item.IsFromPreviousPlaylist = true);
                return oldPlaylist.Concat(newPlaylist).ToList();
            }
            return newPlaylist;
        }



    }
}
