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
                var route = httpRequestManager.GenerateUriWithParams("/get-test", requestParams);
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
                PlayListState.PlaylistItems = playlist;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error on Method DeserializeResponseAsPlaylist => {0}", e);
                var defaultPlaylist = new List<PlaylistItem>();
                defaultPlaylist.Add(new PlaylistItem()
                {
                    AccessPath = defaultClipUrl,
                    Id = "_settings.SettingsState.DefaultClipURL",
                    IsDowloaded = false
                });
                PlayListState.PlaylistItems = defaultPlaylist;
            }
        }



    }
}
