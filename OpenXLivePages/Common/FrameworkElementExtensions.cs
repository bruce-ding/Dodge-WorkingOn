using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace OpenXLivePages.Common
{
    public static class FrameworkElementExtensions
    {
        public static void FindByName(DependencyObject obj, int level, string name)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
            {
                if (obj.GetValue(FrameworkElement.NameProperty).ToString() == name)
                {
                    targetElement = obj as FrameworkElement;
                }

                FindByName(VisualTreeHelper.GetChild(obj, i), ++level, name);
            }
        }

        public static FrameworkElement targetElement { get; internal set; }
    }
}
