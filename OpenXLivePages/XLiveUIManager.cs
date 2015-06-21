using OpenXLive;
using OpenXLivePages;
using OpenXLivePages.Common;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenXLive.UI
{
    public enum XLivePages
    {
        HomePage
    }

    public static class XLiveUIManager
    {
        public static void Show(XLivePages page)
        {
            GameSession session = XLiveGameManager.CurrentSession;
            if (session != null && session.IsValid)
            {
                UIElement p = null;

                if (Window.Current.Content is Page || Window.Current.Content is SwapChainBackgroundPanel)
                {
                    if (page == XLivePages.HomePage)
                    {
                        HomePage homePage = new HomePage();

                        var service = NavigationService.GetService();
                        service.NavigateTo(homePage);
                    }
                }
                else if (Window.Current.Content is Frame)
                {
                    p = ((Window.Current.Content as Frame).Content as Page).Content;
                    var frame = Window.Current.Content as Frame;
                    if (page == XLivePages.HomePage)
                        frame.Navigate(typeof(HomePage), "AllGroups");
                }
                else return;
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Game session is not valid, please try later");
                dlg.ShowAsync();
            }
        }

        /// <summary>
        /// Gets or sets the background image for OpenXLive.
        /// </summary>
        public static string Background
        {
            get { return XLiveParameters.Parameters.Background; }
            set
            {
                XLiveParameters.Parameters.Background = value;
            }
        }

        /// <summary>
        /// Gets or sets the game name for OpenXLive.
        /// </summary>
        public static string GameName
        {
            get { return XLiveParameters.Parameters.GameName; }
            set
            {
                XLiveParameters.Parameters.GameName = value;
            }
        }

        /// <summary>
        /// Gets or sets the OpenXLive version
        /// </summary>
        public static string Version
        {
            get { return XLiveParameters.Parameters.Version; }
            set
            {
                XLiveParameters.Parameters.Version = value;
            }
        }
    }
}
