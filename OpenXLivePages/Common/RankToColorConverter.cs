using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OpenXLivePages.Common
{
    /// <summary>
    /// Value converter that translates rank number to corresponding color
    /// </summary>
    public sealed class RankToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string && (string)value == "1")
                return "#FFBA081A";
            else if (value is string && (string)value == "2")
                return "#FFEB3F0D";
            else if (value is string && (string)value == "3")
                return "#FFFAD03E";
            else 
                return "#FF007233";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string && (string)value == "#FFBA081A")
                return "1";
            else if (value is string && (string)value == "#FFEB3F0D")
                return "2";
            else if (value is string && (string)value == "#FFFAD03E")
                return "3";
            else
                return "4";
        }
    }
}
