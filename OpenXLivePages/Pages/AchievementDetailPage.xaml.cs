using OpenXLive.UI;
using OpenXLivePages.Common;
using OpenXLivePages.Data;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace OpenXLivePages
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class AchievementDetailPage : OpenXLivePages.Common.LayoutAwarePage
    {
        public AchievementDetailPage()
        {
            this.InitializeComponent();
            LoadData();
        }

        public static string Id { get; set; }
        public static Page fromPage { get; set; }
        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        private void LoadData()
        {
            var item = OpenXLiveDataSource.GetItem(Id);
            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;
            this.flipView.SelectedItem = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (OpenXLiveDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }

        protected override void GoBack(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Window.Current.Content is Page || Window.Current.Content is SwapChainBackgroundPanel)
            {
                var service = NavigationService.GetService();
                service.GoBack();
            }
            else if (Window.Current.Content is Frame)
            {
                base.GoBack(sender, e);
            }
        }
    }
}
