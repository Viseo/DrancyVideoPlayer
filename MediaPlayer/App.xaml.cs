using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MediaPlayer.Managers;

namespace MediaPlayer
{

    sealed partial class App : Application
    {
        private DispatcherTimer _dispatcherTimer;
        private SettingsManager settingsManager;
        private PlanningManager planningManager;
        private ContentManager contentManager;
        private HttpRequestManager httpRequestManager;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            SetFullScreenModeOnLaunch();
            InitDependencies();
            LaunchBackgroundTasks();
        }

        private void SetFullScreenModeOnLaunch()
        {
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }

        private void InitDependencies()
        {
            Dependencies.SettingsManager = settingsManager = new SettingsManager();
            Dependencies.PlanningManager = planningManager = new PlanningManager();
            Dependencies.ContentManager = contentManager = new ContentManager();
            Dependencies.HttpRequestManager = httpRequestManager = new HttpRequestManager(Dependencies.SettingsManager.SettingsState.CalledURL);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                GetFirstFrameToDisplay(rootFrame, e);
                Window.Current.Activate();
            }
        }

        private async void GetFirstFrameToDisplay(Frame rootFrame, LaunchActivatedEventArgs e)
        {
            if (rootFrame.Content == null)
            {
                if (await DoesValidSettingsExist())
                    rootFrame.Navigate(typeof(Views.MediaPlayer), e.Arguments);
                else
                    rootFrame.Navigate(typeof(Views.SettingsPage), e.Arguments);
            }
        }

        private Task CleanTmpFolder()
        {
            return contentManager.CleanTemporaryFolder();
        }

        private async Task<bool> DoesValidSettingsExist()
        {
            if (await settingsManager.IsSettingsFileExist()
                && settingsManager.SettingsState.AreNumericFieldsValid()
                && settingsManager.SettingsState.AreNonNumericFieldsValid())
                return true;
            return false;
        }

        private async void LaunchBackgroundTasks()
        {
            try
            {
                await CleanTmpFolder();
                await DoBackgroundWork();
                contentManager.ManageDownloadQueue(httpRequestManager);
                SetupTimerRetrievingPlaylist();
            }
            catch (Exception ex)
            {
                //todo error handling
            }
        }

        private void SetupTimerRetrievingPlaylist()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += RetrievePlaylistTick;
            _dispatcherTimer.Interval = new TimeSpan(0, settingsManager.SettingsState.CronUpdateTime, 0);
            _dispatcherTimer.Start();
        }

        private void RetrievePlaylistTick(object sender, object e)
        {
            DoBackgroundWork();
        }

        private async Task DoBackgroundWork()
        {
            try
            {
                var settings = settingsManager.SettingsState;
                await planningManager
                    .RetrievePlaylist(settings.ScreenId
                        , settings.SecurityKey
                        , settings.DefaultClipURL
                        , httpRequestManager);

                await contentManager.CheckIfPlaylistItemsAreDownloaded(planningManager.PlayListState.PlaylistItems);

                await contentManager.FillDeletionQueue(planningManager.PlayListState.PlaylistItems);

                contentManager.FillDownloadQueue(planningManager.PlayListState.PlaylistItems);
            }
            catch (Exception)
            {
                //todo error handling
            }
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
