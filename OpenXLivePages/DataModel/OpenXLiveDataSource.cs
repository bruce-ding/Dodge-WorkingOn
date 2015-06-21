using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace OpenXLivePages.Data
{
    /// <summary>
    /// Base class for <see cref="OpenXLiveDataItem"/> and <see cref="OpenXLiveDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class OpenXLiveDataCommon : OpenXLivePages.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public OpenXLiveDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(OpenXLiveDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public Uri ImagePath
        {
            get
            {
                return new Uri(OpenXLiveDataCommon._baseUri, this._imagePath);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class OpenXLiveDataItem : OpenXLiveDataCommon
    {
        public OpenXLiveDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, OpenXLiveDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private OpenXLiveDataGroup _group;
        public OpenXLiveDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class OpenXLiveDataGroup : OpenXLiveDataCommon
    {
        public OpenXLiveDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<OpenXLiveDataItem> _headerItems = new ObservableCollection<OpenXLiveDataItem>();
        public ObservableCollection<OpenXLiveDataItem> HeaderItems = new ObservableCollection<OpenXLiveDataItem>();

        private ObservableCollection<OpenXLiveDataItem> _items = new ObservableCollection<OpenXLiveDataItem>();
        public ObservableCollection<OpenXLiveDataItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<OpenXLiveDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }
    }


    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class OpenXLiveDataSource
    {
        private static OpenXLiveDataSource _oxlDataSource = new OpenXLiveDataSource();

        private static ObservableCollection<OpenXLiveDataGroup> _allGroups = new ObservableCollection<OpenXLiveDataGroup>();
        public static ObservableCollection<OpenXLiveDataGroup> AllGroups
        {
            get { return _allGroups; }
        }

        public static IEnumerable<OpenXLiveDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return AllGroups;
        }

        public static OpenXLiveDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static OpenXLiveDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static OpenXLiveDataGroup GameGroup = null;

        public OpenXLiveDataSource()
        {
        }
    }
}
