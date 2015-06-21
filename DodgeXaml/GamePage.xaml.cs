using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using MonoGame.Framework;
using OpenXLive;
using Windows.UI.Popups;
using System;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;


namespace DodgeXaml
{
    /// <summary>
    /// The root page used to display the game.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 _game;

        /// <summary>
        /// A static property that returns the current instance of the game page
        /// </summary>
        public static GamePage Current { get; set; }

        public static bool WasLightingTapped { get; set; }

        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            // Set the current instance of the page so that the game can access it
            Current = this;

            //LayoutRoot.Children.Add(new Colorful_FollowMouse());
            
            // Create the game.
            _game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
        }

       

        private void SwapChainBackgroundPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //TouchPanelCapabilities tpc = TouchPanel.GetCapabilities();
            //Debug.WriteLine("Touch panel is available : " + tpc.IsConnected.ToString());
            //Debug.WriteLine("MaximumTouchCount: " + tpc.MaximumTouchCount.ToString() + "\n");

            //TouchPanel.EnabledGestures = GestureType.Hold | GestureType.Tap | GestureType.DoubleTap | GestureType.Flick |
            //    GestureType.FreeDrag | GestureType.HorizontalDrag | GestureType.VerticalDrag;
        }

       
        private void EllipseLighting_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            WasLightingTapped = true;
        }
    }
}
