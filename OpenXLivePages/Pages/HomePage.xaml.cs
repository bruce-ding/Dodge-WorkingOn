using OpenXLive;
using OpenXLive.UI;
using OpenXLive.WinRT.UI;
using OpenXLivePages.Common;
using OpenXLivePages.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace OpenXLivePages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HomePage : OpenXLivePages.Common.LayoutAwarePage
    {
        public static Image avatar;
        public static TextBlock userName;
        internal static List<LeaderboardFilter> filterList = new List<LeaderboardFilter>();
        internal static List<OpenXLiveDataItem> it = new List<OpenXLiveDataItem>();
        ProgressBar leaderboardProgressBar = null;

        public HomePage()
        {
            this.InitializeComponent();
            this.InitializeUserInfo();

            groupHeaderTemplateSelector selector = new groupHeaderTemplateSelector();
            itemGridView.GroupStyle.ElementAt(0).HeaderTemplateSelector = selector;
            selector.leaderboardStyle = this.Resources["leaderboardGroupHeader"] as DataTemplate;
            selector.normalStyle = this.Resources["normalGroupHeader"] as DataTemplate;

            temp_gridView.leaderboardStyle = this.Resources["StandardListItemTemplate"] as DataTemplate;
            temp_listView.leaderboardStyle = this.Resources["SnappedStandardListItemTemplate"] as DataTemplate;

            LoadData();
        }

        void InitializeUserInfo()
        {
            avatar = new Image();
            userName = new TextBlock();
            UserInfo.Children.Add(userName);
            userName.FontSize = 24;
            userName.FontWeight = Windows.UI.Text.FontWeights.Bold;
            userName.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            userName.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            userName.Tapped += userName_Tapped;

            UserInfo.Children.Add(avatar);
            avatar.Stretch = Stretch.UniformToFill;
            avatar.Width = 70;
            avatar.Height = 70;
            avatar.Margin = new Thickness(20, 0, 0, 0);

            if (XLiveGameManager.CurrentSession != null && !XLiveGameManager.CurrentSession.CurrentPlayer.IsAnonymous)
            {
                userName.Text = OpenXLive.XLiveGameManager.CurrentSession.CurrentPlayer.Title;
                avatar.Source = new BitmapImage(OpenXLive.XLiveGameManager.CurrentSession.CurrentPlayer.Photo.Url);
            }
            else
            {
                userName.Text = "Click to Log In";
                avatar.Source = new BitmapImage(new Uri("http://picture.openxlive.net/avatars/anonymous.png"));
            }
        }

        #region Page Method

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

        private async void LoadData()
        {
            XLiveUIManager.Version = XLiveVersion.GetVersion();

            OpenXLiveDataSource.AllGroups.Clear();
            filterList.Clear();

            GameSession session = XLiveGameManager.CurrentSession;

            if (session.Profile != null)
            {
                XLiveUIManager.GameName = session.Profile.Name;

                var group1 = new OpenXLiveDataGroup("Game", "Game", "Game Group", "", "Game Group");

                OpenXLiveDataSource.GameGroup = group1;
                OpenXLiveDataSource.GameGroup.UniqueId = session.ID;
                OpenXLiveDataSource.GameGroup.Subtitle = session.Profile.Developer;
                OpenXLiveDataSource.GameGroup.Description = session.Profile.Description;
                OpenXLiveDataSource.GameGroup.Image = new BitmapImage(session.Profile.Image.Url);

                OpenXLiveDataSource.GameGroup.Items.Add(new OpenXLiveDataItem(
                    session.ID,
                    session.Profile.Name,
                    session.Profile.Developer,
                    session.Profile.Image.Url.ToString(),
                    session.Profile.Description,
                    session.Profile.OnlineNumber.ToString(),
                    OpenXLiveDataSource.GameGroup));

                group1.Items.Add(new OpenXLiveDataItem(
                    "game-group-item-1",
                    "Online Player",
                    "Online Player",
                    "ms-appx:///OpenXLivePages/Assets/social.png",
                    "Find those who play the same games and share the same joys and adventures with you.",
                    "",
                    group1));
                group1.Items.Add(new OpenXLiveDataItem(
                    "game-group-item-2",
                    "Featured Games",
                    "Featured Games",
                    "ms-appx:///OpenXLivePages/Assets/game.png",
                    "Featured OpenXLive games and apps.",
                    "",
                    group1));

                OpenXLiveDataSource.AllGroups.Add(group1);
            }


            // Get Leaderboard Indentity
            if (session.LeaderboardProfiles.Count > 0)
            {
                var group2 = new OpenXLiveDataGroup(
                                "Leaderboard",
                                "Leaderboard",
                                "Leaderboard Group",
                                "",
                                "Leaderboard Group");

                foreach (LeaderboardProfile item in session.LeaderboardProfiles)
                {
                    group2.HeaderItems.Add(new OpenXLiveDataItem(
                        item.ID,
                        item.Title,
                        item.DataType.ToString(),
                        "",
                        "One of the top players in a game? Show your rankings among millions around the world!",
                        "",
                        group2));

                    filterList.Add(
                        new LeaderboardFilter(
                            item.ID,
                            item.Title,
                            item.Title == session.LeaderboardProfiles.ElementAt(0).Title ? true : false));
                }

                if (group2.HeaderItems.Count > 0)
                {
                    group2.Items.Add(group2.HeaderItems.ElementAt(0));
                    OpenXLiveDataSource.AllGroups.Add(group2);
                }
            }


            // Get Game Achievement
            GameAchievement list = session.Achievements;
            var result = await list.GetAchievementsAsync();
            if (result.ReturnValue)
            {
                if (list != null)
                {
                    var group3 = new OpenXLiveDataGroup(
                                            "Achievement",
                                            "Achievement",
                                            "Achievement Group",
                                            "",
                                            "Achievement Group");

                    if (group3 != null)
                    {
                        group3.Items.Clear();

                        foreach (var item in list.Achievements)
                        {
                            group3.Items.Add(new OpenXLiveDataItem(
                                item.ID,
                                item.Name,
                                item.IsEarned ? "Adwarded" : "Locked",
                                item.IsEarned ? item.Image.Url.ToString() : "ms-appx:///OpenXLivePages/Assets/Lock.png",
                                item.Description,
                                item.Point.ToString(),
                                group3));
                        }

                        if (list.Achievements.Count > 0)
                            OpenXLiveDataSource.AllGroups.Add(group3);
                    }
                }
            }

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var xliveDataGroups = OpenXLiveDataSource.AllGroups;
            this.DefaultViewModel["Groups"] = xliveDataGroups;
            this.DefaultViewModel["Filters"] = filterList;

            pgb.IsIndeterminate = false;
            pgb.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            if (Window.Current.Content is Page || Window.Current.Content is SwapChainBackgroundPanel)
            {
                if (((OpenXLiveDataGroup)group).UniqueId == "Achievement")
                {
                    AchievementsPage page = new AchievementsPage();

                    var service = NavigationService.GetService();
                    service.NavigateTo(page);
                }
                else if (((OpenXLiveDataGroup)group).UniqueId == "Leaderboard")
                {
                    if (((OpenXLiveDataGroup)group).Items.Count > 0)
                    {
                        var itemId = ((OpenXLiveDataGroup)group).Items.ElementAt(0).UniqueId;
                        LeaderboardPage.Title = ((OpenXLiveDataGroup)group).Items.ElementAt(0).Title;
                        LeaderboardPage.Id = ((OpenXLiveDataGroup)group).Items.ElementAt(0).UniqueId;

                        LeaderboardPage page = new LeaderboardPage();

                        var service = NavigationService.GetService();
                        service.NavigateTo(page);
                    }
                }
            }
            else if (Window.Current.Content is Frame)
            {
                if (((OpenXLiveDataGroup)group).UniqueId == "Achievement")
                {
                    this.Frame.Navigate(typeof(AchievementsPage));
                }
                else if (((OpenXLiveDataGroup)group).UniqueId == "Leaderboard")
                {
                    if (((OpenXLiveDataGroup)group).Items.Count > 0)
                    {
                        var itemId = ((OpenXLiveDataGroup)group).Items.ElementAt(0).UniqueId;
                        LeaderboardPage.Title = ((OpenXLiveDataGroup)group).Items.ElementAt(0).Title;
                        LeaderboardPage.Id = ((OpenXLiveDataGroup)group).Items.ElementAt(0).UniqueId;
                        this.Frame.Navigate(typeof(LeaderboardPage));
                    }
                }
            }
            else return;
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Window.Current.Content is Page || Window.Current.Content is SwapChainBackgroundPanel)
            {
                if (((OpenXLiveDataItem)e.ClickedItem).Group.UniqueId == "Leaderboard")
                {
                    // Do nothing...
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == "Online Player")
                {
                    OnlinePlayerPage page = new OnlinePlayerPage();

                    var service = NavigationService.GetService();
                    service.NavigateTo(page);
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == "Featured Games")
                {
                    GamesListPage page = new GamesListPage();

                    var service = NavigationService.GetService();
                    service.NavigateTo(page);
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == OpenXLiveDataSource.GetGroups("AllGroups").First().Items.First().Title)
                {
                    // Do nothing...
                }
                else
                {
                    AchievementDetailPage.Id = ((OpenXLiveDataItem)e.ClickedItem).UniqueId;
                    AchievementDetailPage.fromPage = this;
                    AchievementDetailPage page = new AchievementDetailPage();

                    var service = NavigationService.GetService();
                    service.NavigateTo(page);
                }
            }
            else if (Window.Current.Content is Frame)
            {
                if (((OpenXLiveDataItem)e.ClickedItem).Group.UniqueId == "Leaderboard")
                {
                    // Do nothing...
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == "Online Player")
                {
                    this.Frame.Navigate(typeof(OnlinePlayerPage));
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == "Featured Games")
                {
                    this.Frame.Navigate(typeof(GamesListPage));
                }
                else if (((OpenXLiveDataItem)e.ClickedItem).Title == OpenXLiveDataSource.GetGroups("AllGroups").First().Items.First().Title)
                {
                    // Do nothing...
                }
                else
                {
                    AchievementDetailPage.Id = ((OpenXLiveDataItem)e.ClickedItem).UniqueId;
                    this.Frame.Navigate(typeof(AchievementDetailPage));
                }
            }
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
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

        #endregion

        #region Login Method

        void picker_Closed(object sender, ClosedEventArgs e)
        {
            if (e.player != null && !e.player.IsAnonymous && e.player.Photo != null)
            {
                pgb.IsIndeterminate = true;
                pgb.Visibility = Visibility.Visible;

                userName.Text = e.player.Title;
                avatar.Source = new BitmapImage(e.player.Photo.Url);

                if (Window.Current.Content is Page || Window.Current.Content is SwapChainBackgroundPanel)
                {
                    HomePage page = new HomePage();

                    var service = NavigationService.GetService();
                    service.GoBack();
                    service.NavigateTo(page);
                }
                else if (Window.Current.Content is Frame)
                {
                    this.Frame.GoBack();
                    this.Frame.Navigate(this.GetType());
                }

            }
        }

        async void userName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GameSession session = XLiveGameManager.CurrentSession;

            if (session != null && session.IsValid)
            {
                if (!session.CurrentPlayer.IsAnonymous)
                {
                    var menu = new PopupMenu();

                    byte[] bytePicture = null;
                    menu.Commands.Add(new UICommand("Upload Photo", async (command) =>
                    {
                        FileOpenPicker openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        openPicker.FileTypeFilter.Add(".jpg");
                        openPicker.FileTypeFilter.Add(".jpeg");
                        openPicker.FileTypeFilter.Add(".png");
                        openPicker.CommitButtonText = "Select this image";
                        StorageFile file = await openPicker.PickSingleFileAsync();
                        if (file != null)
                        {
                            var stream = await file.OpenAsync(FileAccessMode.Read);
                            using (Stream s = stream.AsStream())
                            {
                                bytePicture = new byte[s.Length];
                                s.Read(bytePicture, 0, bytePicture.Length);

                                using (var dataWriter = new DataWriter())
                                {
                                    dataWriter.WriteBytes(bytePicture);

                                    var result = await session.CurrentPlayer.UploadPhotoAsync(dataWriter.DetachBuffer());
                                    if (result.ReturnValue)
                                    {
                                        avatar.Source = new BitmapImage(session.CurrentPlayer.Photo.Url);
                                    }
                                }
                            }
                        }
                    }));
                    menu.Commands.Add(new UICommand("Logoff", async (command) =>
                    {
                        MessageDialog dlg = new MessageDialog("Are you sure to log off?", "Log off");
                        dlg.Commands.Add(new UICommand("Yes", async (action) =>
                        {
                            pgb.IsIndeterminate = true;
                            pgb.Visibility = Visibility.Visible;

                            AsyncProcessResult result = await session.CurrentPlayer.LogoffAsync();
                            if (result.ReturnValue)
                            {
                                userName.Text = "Click to Log In";
                                avatar.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("http://picture.openxlive.net/avatars/anonymous.png"));

                                if ((Window.Current.Content as Page) != null || (Window.Current.Content as SwapChainBackgroundPanel) != null)
                                {
                                    HomePage page = new HomePage();

                                    var service = NavigationService.GetService();
                                    service.GoBack();
                                    service.NavigateTo(page);
                                }
                                else if ((Window.Current.Content as Frame) != null)
                                {
                                    this.Frame.GoBack();
                                    this.Frame.Navigate(this.GetType());
                                }
                            }
                        }));
                        dlg.Commands.Add(new UICommand("No"));
                        await dlg.ShowAsync();

                    }));
                    await menu.ShowAsync(e.GetPosition(null));
                }
                else
                {
                    XLiveCredentialPicker picker = new XLiveCredentialPicker();

                    picker.Closed += picker_Closed;
                }
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Game session is not valid, please try later");
                await dlg.ShowAsync();
            }
        }

        #endregion

        #region Leaderboard Method

        private async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (leaderboardProgressBar != null)
            {
                leaderboardProgressBar.Visibility = Visibility.Visible;
                leaderboardProgressBar.IsIndeterminate = true;
            }

            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as LeaderboardFilter;
            if (selectedFilter != null)
            {
                var it = new List<OpenXLiveDataItem>();

                Leaderboard lb = new Leaderboard(XLiveGameManager.CurrentSession, selectedFilter.Id);
                var rs = await lb.GetHighScoresAsync();
                if (rs.ReturnValue)
                {
                    if (lb != null)
                    {
                        it.Clear();
                        foreach (var score in lb.Scores)
                        {
                            it.Add(new OpenXLiveDataItem(
                                score.AnonymousID,
                                score.Title,
                                score.Value.ToString(),
                                score.Image.Url.ToString(),
                                score.Rank.ToString(),
                                "",
                                OpenXLiveDataSource.GetGroup("Leaderboard")));
                        }
                    }

                    FrameworkElementExtensions.FindByName(itemGridView, 0, "leaderboardProgressBar");
                    leaderboardProgressBar = FrameworkElementExtensions.targetElement as ProgressBar;


                    if (leaderboardProgressBar != null)
                    {
                        leaderboardProgressBar.IsIndeterminate = false;
                        leaderboardProgressBar.Visibility = Visibility.Collapsed;
                    }
                }


                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                if (it.Count > 0)
                {
                    this.DefaultViewModel["Results"] = it;

                    // Ensure results are found
                    if (it.Count > 0)
                    {
                        var group = OpenXLiveDataSource.GetGroup("Leaderboard");
                        if (group != null && group.Items.Count > 0 && group.Items.ElementAt(0).Title != selectedFilter.Name)
                        {
                            group.Items.Clear();
                            foreach (var item in group.HeaderItems)
                            {
                                if (item.Title == selectedFilter.Name)
                                {
                                    group.Items.Add(item);
                                    break;
                                }
                            }
                        }

                        return;
                    }
                }
            }
        }

        #endregion

    }

    internal sealed class LeaderboardFilter : OpenXLivePages.Common.BindableBase
    {
        private String _name;
        private bool _active;
        private String _id;

        public LeaderboardFilter(String id, String name, bool active = false)
        {
            this.Id = id;
            this.Name = name;
            this.Active = active;
        }

        public override String ToString()
        {
            return Description;
        }

        public String Name
        {
            get { return _name; }
            set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
        }

        public bool Active
        {
            get { return _active; }
            set { this.SetProperty(ref _active, value); }
        }

        public String Id
        {
            get { return _id; }
            set { this.SetProperty(ref _id, value); }
        }

        public String Description
        {
            get { return String.Format("{0}", _name); }
        }
    }
}
