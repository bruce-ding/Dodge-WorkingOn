using OpenXLive;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Controls;
using OpenXLive.UI;
using Windows.Graphics.Display;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace DodgeXaml
{
          
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
       
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            DisplayProperties.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var gamePage = Window.Current.Content as GamePage;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (gamePage == null)
            {
                // Create a main GamePage
                gamePage = new GamePage(args.Arguments);
                
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the GamePage in the current Window
                Window.Current.Content = gamePage;
            }

            // Ensure the current window is active
            Window.Current.Activate();
            

            // Modify the OpenXLive pages background
            XLiveUIManager.Background = "ms-appx:///Assets/Background.jpg";

            GameSession session = XLiveGameManager.CreateSession("RfudEBA8TvNkYsqKJHUGMeWy");

            try
            {
                AsyncProcessResult result = await session.OpenAsync();

                if (result.ReturnValue)
                {
                    // Create game session succeeded
                }
                else
                {
                    // Create game session failed
                    MessageDialog message = new MessageDialog(result.ErrorMessage, "Exception Caught");
                    await message.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                // Exception caught during the request
                MessageDialog message = new MessageDialog(ex.Message, "Exception Caught");
                message.ShowAsync();
            }

        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity

            if (XLiveGameManager.CurrentSession != null && XLiveGameManager.CurrentSession.IsValid)
            {
                await XLiveGameManager.CurrentSession.CloseAsync();
            }

            deferral.Complete();
        }

    }
}
