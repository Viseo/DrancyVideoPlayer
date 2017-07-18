namespace MediaPlayer.Models
{
    public class Settings 
    {
        public string ScreenId { get; set; }
        public string CalledURL { get; set; }
        public string SecurityKey { get; set; }
        public string DefaultClipURL { get; set; }

        public string CronUpdateTime
        {
            get;
            set;
        }
        public string ImagesDisplayTime { get; set; }
    }
}
