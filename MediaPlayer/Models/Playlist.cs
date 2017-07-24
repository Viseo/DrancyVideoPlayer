using System.Collections.Generic;

namespace MediaPlayer.Models
{
    public class Playlist
    {
        private static Playlist _instance;

        public List<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();

        public Playlist Clone()
        {
            return (Playlist) MemberwiseClone();
        }

        public static Playlist Instance
        {
            get => _instance.Clone();
            set => _instance = value;
        }
    }
}
