using MediaPlayer.Managers;

namespace MediaPlayer
{
    public static class Dependencies
    {
        public static IContentManager ContentManager { get; set; }
        public static ISettingsManager SettingsManager { get; set; }
        public static IPlanningManager PlanningManager { get; set; }
        public static IHttpRequestManager HttpRequestManager { get; set; }
    }
}
