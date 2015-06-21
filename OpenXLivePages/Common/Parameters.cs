using System.ComponentModel;

namespace OpenXLivePages
{
    /// <summary>
    /// Contains properties that used to be OpenXLive.
    /// </summary>
    public sealed class Parameters : INotifyPropertyChanged
    {
        internal Parameters()
        {

        }

        private string _Background = null;
        /// <summary>
        /// Gets or sets the background image for OpenXLive.
        /// </summary>
        public string Background
        {
            get { return _Background; }
            set
            {
                if (_Background != value)
                {
                    _Background = value;
                    OnNotifyPropertyChanged("Background");
                }
            }
        }

        private string _GameName = "OPENXLIVE";
        /// <summary>
        /// Gets the name of this game.
        /// </summary>
        public string GameName
        {
            get { return _GameName; }
            internal set
            {
                if (_GameName != value)
                {
                    _GameName = value;
                    OnNotifyPropertyChanged("GameName");
                }
            }
        }

        private string _Version = "0.0.0.0";
        /// <summary>
        /// Gets the version of this OpenXLive.
        /// </summary>
        public string Version
        {
            get { return _Version; }
            internal set
            {
                if (_Version != value)
                {
                    _Version = value;
                    OnNotifyPropertyChanged("Version");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
