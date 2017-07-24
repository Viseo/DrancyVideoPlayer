using Newtonsoft.Json;

namespace MediaPlayer.Models
{
    public class PlaylistItem
    {
        [JsonProperty("UniqueId")]
        public string Id { get; set; }

        [JsonProperty("Content")]
        public string AccessPath { get; set; }

        public bool IsDowloaded { get; set; }
    }
}
