using OpenXLive.UI;
using OpenXLivePages.Common;
using OpenXLivePages.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace OpenXLivePages
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class AchievementsPage : OpenXLivePages.Common.LayoutAwarePage
    {
        private Dictionary<string, List<OpenXLiveDataItem>> _results = new Dictionary<string, List<OpenXLiveDataItem>>();

        public AchievementsPage()
        {
            this.InitializeComponent();
            LoadData();
        }

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
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]            
        }

        private void LoadData()
        {
            var filterList = new List<AchievementFilter>();
            filterList.Add(new AchievementFilter("All", 0, true));

            var group = OpenXLiveDataSource.GetGroup("Achievement");
            var all = new List<OpenXLiveDataItem>();
            _results.Add("All", all);


            if (group != null)
            {
                var Adwarded = new List<OpenXLiveDataItem>();
                var Locked = new List<OpenXLiveDataItem>();

                _results.Add("Adwarded", Adwarded);
                _results.Add("Locked", Locked);

                foreach (var item in group.Items)
                {
                    all.Add(item);
                    if (item.Subtitle.ToLower() == "adwarded")
                        Adwarded.Add(item);
                    else if (item.Subtitle.ToLower() == "locked")
                    {
                        Locked.Add(item);
                    }
                }

                filterList.Add(new AchievementFilter("Adwarded", Adwarded.Count, false));
                filterList.Add(new AchievementFilter("Locked", Locked.Count, false));
            }


            filterList[0].Count = all.Count;

            this.DefaultViewModel["Filters"] = filterList;
        }


        private void resultsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            AchievementDetailPage.Id = ((OpenXLiveDataItem)e.ClickedItem).UniqueId;
            AchievementDetailPage.fromPage = this;
            if ((Window.Current.Content as Page) != null || (Window.Current.Content as SwapChainBackgroundPanel) != null)
            {
                AchievementDetailPage page = new AchievementDetailPage();

                var service = NavigationService.GetService();
                service.NavigateTo(page);
            }
            else if ((Window.Current.Content as Frame) != null)
            {
                this.Frame.Navigate(typeof(AchievementDetailPage));
            }
        }

        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as AchievementFilter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                this.DefaultViewModel["Results"] = _results[selectedFilter.Name];

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    return;
                }
            }
        }

        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
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

        private sealed class AchievementFilter : OpenXLivePages.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public AchievementFilter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
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

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }
    }
}
