using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenXLivePages.Common
{
    public sealed class NavigationService
    {
        private NavigationService()
        {
            pageList = new List<UIElement>();
        }

        public static NavigationService GetService()
        {
            if (_service == null)
                _service = new NavigationService();
            return _service;
        }

        public void NavigateTo(Page page)
        {
            pageList.Add(Window.Current.Content);

            Window.Current.Content = page;
            Window.Current.Activate();
        }

        public void GoBack()
        {
            var lastPage = pageList.ElementAt(pageList.Count - 1);

            Window.Current.Content = lastPage;
            Window.Current.Activate();

            pageList.RemoveAt(pageList.Count - 1);
        }

        private static List<UIElement> pageList;

        private static NavigationService _service;

    }
}
