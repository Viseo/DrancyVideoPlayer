using System;
using System.ServiceModel;


namespace MediaPlayer.Models
{
    public class Settings
    {
        private static Settings _instance;

        public string ScreenId { get; set; }
        public string CalledURL { get; set; }
        public string SecurityKey { get; set; }
        public string DefaultClipURL { get; set; }
        public int CronUpdateTime { get; set; }
        public int ImagesDisplayTime { get; set; }

        public bool AreNonNumericFieldsValid()
        {
            return !string.IsNullOrEmpty(this.ScreenId)
                   && !string.IsNullOrEmpty(this.CalledURL)
                   && !string.IsNullOrEmpty(this.SecurityKey)
                   && !string.IsNullOrEmpty(this.DefaultClipURL);
        }

        public bool AreNumericFieldsValid()
        {
            return (CronUpdateTime > 0)
                && (ImagesDisplayTime > 0);
        }

        public void SetDefaultValue()
        {
            if (CronUpdateTime == 0)
                CronUpdateTime = 15;
            if (ImagesDisplayTime == 0)
                ImagesDisplayTime = 30;
        }

        public Settings Clone()
        {
            return (Settings) MemberwiseClone();
        }

        public static Settings Instance
        {
            get => _instance.Clone();
            set => _instance = value;
        }
    }
}
