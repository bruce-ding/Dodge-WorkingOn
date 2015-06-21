using OpenXLivePages.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenXLivePages.Common
{
    public sealed class groupHeaderTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var gridViewItem = item as OpenXLiveDataGroup;
            DataTemplate t = null;
            if (gridViewItem != null)
            {
                if (gridViewItem.Title == "Leaderboard")
                {
                    t = leaderboardStyle;
                    
                }
                else
                {
                    t = normalStyle;
                }
            }
            return t;
        }

        public DataTemplate leaderboardStyle { get; set; }
        public DataTemplate normalStyle { get; set; }
    }
}
