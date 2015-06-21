using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenXLivePages.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace OpenXLivePages.Common
{
    public sealed class GridViewItemTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var gridViewItem = item as OpenXLiveDataItem;

            DataTemplate t = null;

            if (gridViewItem.Group.Title == "Leaderboard")
            {
                t = leaderboardStyle;

            }
            else if (gridViewItem.Group.Title == "Achievement")
            {
                t = Application.Current.Resources["Standard250x150ItemTemplate"] as DataTemplate;
            }
            else
            {
                t = Application.Current.Resources["Standard173x173ItemTemplate"] as DataTemplate;
            }

            return t;
        }

        public DataTemplate leaderboardStyle { get; set; }
    }

    public sealed class ListViewItemTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var listViewItem = item as OpenXLiveDataItem;

            DataTemplate t;

            if (listViewItem.Group.Title == "Leaderboard")
            {
                t = leaderboardStyle;
            }
            else if (listViewItem.Group.Title == "Achievement")
            {
                t = Application.Current.Resources["Standard80ItemTemplate"] as DataTemplate;
            }
            else
            {
                t = Application.Current.Resources["Standard80ItemTemplate"] as DataTemplate;
            }
            return t;
        }

        public DataTemplate leaderboardStyle { get; set; }
    }
}
