
namespace OpenXLivePages
{
    public sealed class XLiveParameters
    {
        static XLiveParameters()
        {
            Parameters = new Parameters();
        }

        public XLiveParameters()
        {
        }

        /// <summary>
        /// Used to set properties associated OpenXLive.
        /// </summary>
        public static Parameters Parameters { get; private set; }
    }
}
