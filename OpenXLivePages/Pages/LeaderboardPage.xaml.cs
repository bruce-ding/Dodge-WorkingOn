using OpenXLive;
using OpenXLive.UI;
using OpenXLivePages.Common;
using OpenXLivePages.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace OpenXLivePages
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class LeaderboardPage : OpenXLivePages.Common.LayoutAwarePage
    {
        ProgressBar bar = new ProgressBar();

        public LeaderboardPage()
        {
            this.InitializeComponent();
            LoadData();
        }

        public static string Title { get; set; }
        public static string Id { get; set; }

        #region Page state management

        Leaderboard lb;

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
            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
        }

        private async void LoadData()
        {
            GameSession session = XLiveGameManager.CurrentSession;

            if (session != null && session.IsValid)
            {
                bar.IsIndeterminate = true;
                bar.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                mainGrid.Children.Add(bar);

                ModeText.Text = Title;

                lb = new Leaderboard(session, Id);
                try
                {
                    AsyncProcessResult result = await lb.GetHighScoresAsync();

                    if (result.ReturnValue)
                    {
                        if (lb != null)
                        {
                            var group3 = new OpenXLiveDataGroup("Leaderboard", "Leaderboard", "", "", "");

                            foreach (var item in lb.Scores)
                            {
                                group3.Items.Add(new OpenXLiveDataItem(
                                    item.IsAnonymous ? item.AnonymousID : item.PlayerID,
                                    item.Title,
                                    item.Value.ToString(),
                                    item.Image.Url.ToString(),
                                    item.Rank.ToString(),
                                    item.Account,
                                    group3));
                            }

                            this.DefaultViewModel["Items"] = group3.Items;
                        }

                        bar.IsIndeterminate = false;
                        bar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        itemDetail.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog dlg = new MessageDialog(ex.Message, "Http Exception");
                    dlg.ShowAsync();
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = this.itemsViewSource.View.CurrentItem;
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState["SelectedItem"]
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        OnlinePlayer player;

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        async void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            if (Follow.Children.Count > 2)
            {
                for (int i = 2; i < Follow.Children.Count; i++)
                    Follow.Children.RemoveAt(i);
            }
            stateLabel.Visibility = Visibility.Collapsed;

            OpenXLiveDataItem item;

            if (e.AddedItems.Count <= 0)
                return;
            else
            {
                item = e.AddedItems.FirstOrDefault() as OpenXLiveDataItem;
            }

            player = new OnlinePlayer(item.UniqueId, item.Content, item.Title);

            if (item.Content == null)
            {
                pgr.Visibility = Visibility.Collapsed;
                return;
            }

            if (item.Content != null && !XLiveGameManager.CurrentSession.CurrentPlayer.IsAnonymous)
            {
                stateLabel.Visibility = Visibility.Visible;
                pgr.Visibility = Visibility.Visible;

                if (player.ID != XLiveGameManager.CurrentSession.CurrentPlayer.ID)
                {
                    var result = await XLiveGameManager.CurrentSession.CurrentPlayer.ShowFollowAsync(player);

                    if (result.ReturnValue)
                    {
                        if (player.ID != (result.Tag as OnlinePlayer).ID)
                            return;

                        if (Follow.Children.Count > 2)
                        {
                            for (int i = 2; i < Follow.Children.Count; i++)
                                Follow.Children.RemoveAt(i);
                        }

                        pgr.Visibility = Visibility.Collapsed;
                        var state = player.FollowState;

                        if (state == FollowState.None || state == FollowState.FollowMe)
                        {
                            Button bt = new Button();
                            bt.Content = "Follow";
                            bt.Click += bt_Click;
                            Follow.Children.Add(bt);
                        }
                        else if (state == FollowState.FollowPlayer)
                        {
                            Image img = new Image();
                            img.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            img.Height = 40;
                            img.Width = 90;
                            img.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                            img.Source = new BitmapImage(new Uri("ms-appx:///OpenXLivePages/Assets/Followed.png"));
                            Follow.Children.Add(img);
                        }
                        else
                        {
                            Image img = new Image();
                            img.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            img.Height = 40;
                            img.Width = 90;
                            img.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                            img.Source = new BitmapImage(new Uri("ms-appx:///OpenXLivePages/Assets/Friends.png"));
                            Follow.Children.Add(img);
                        }
                    }
                }
                else
                {
                    if (Follow.Children.Count > 2)
                    {
                        for (int i = 2; i < Follow.Children.Count; i++)
                            Follow.Children.RemoveAt(i);
                    }

                    pgr.Visibility = Visibility.Collapsed;
                    TextBlock tx = new TextBlock();
                    tx.Text = "Me";
                    tx.FontSize = 24;
                    Follow.Children.Add(tx);
                }
            }
        }

        async void bt_Click(object sender, RoutedEventArgs e)
        {
            GameSession session = XLiveGameManager.CurrentSession;
            if (!session.CurrentPlayer.IsAnonymous)
            {
                if (player != null)
                {
                    pgr.Visibility = Visibility.Visible;
                    Follow.Children.RemoveAt(Follow.Children.Count - 1);

                    var result = await session.CurrentPlayer.AddFollowAsync(player);
                    if (result.ReturnValue)
                    {
                        pgr.Visibility = Visibility.Collapsed;
                        if (player.FollowState == FollowState.FollowPlayer)
                        {
                            Image img = new Image();
                            img.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            img.Height = 40;
                            img.Width = 90;
                            img.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                            img.Source = new BitmapImage(new Uri("ms-appx:///OpenXLivePages/Assets/Followed.png"));
                            Follow.Children.Add(img);
                        }
                        else if (player.FollowState == FollowState.Friends)
                        {
                            Image img = new Image();
                            img.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                            img.Height = 40;
                            img.Width = 90;
                            img.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                            img.Source = new BitmapImage(new Uri("ms-appx:///OpenXLivePages/Assets/Friends.png"));
                            Follow.Children.Add(img);
                        }
                        else
                        {
                            Button bt = new Button();
                            bt.Content = "Follow";
                            bt.Click += bt_Click;
                            Follow.Children.Add(bt);
                        }
                    }
                }
            }
            else
            {
                MessageDialog dlg = new MessageDialog("You have not log in yet");
                await dlg.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
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

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion
    }
}
