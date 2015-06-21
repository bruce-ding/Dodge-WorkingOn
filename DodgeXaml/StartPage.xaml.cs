using OpenXLive.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍
using SharpDX.XInput;
using Windows.System;
using Windows.ApplicationModel.Store;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using DodgeXaml.CommonHelper;

namespace DodgeXaml
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StartPage : Page
    {

        /// <summary>
        /// A static property that returns the current instance of the game page
        /// </summary>
        public static StartPage Current { get; set; }
        public bool ToggleSwitchAudioIsOn { get; set; }
        public bool HasToggled { get; set; }
        public bool StartGameButtonClicked { get; set; }
        public bool ToggleSwitchShowFPSIsOn { get; set; }

        public bool HasFPSToggled { get; set; }
       

        public StartPage()
        {
            this.InitializeComponent();
            Current = this;

            ToggleSwitchAudioIsOn = toggleSwitchAudio.IsOn = true;
            ToggleSwitchShowFPSIsOn = toggleSwitchShowFPS.IsOn = true;

        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void toggleSwitchAudio_Toggled(object sender, RoutedEventArgs e)
        {
            //if (!((ToggleSwitch)sender).IsOn)
            //{
            //    this.toggleSwitchHerderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/audioOff.png"));
            //}
            //else
            //{
            //    this.toggleSwitchHerderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/audioOn.png"));
            //}

            ToggleSwitchAudioIsOn = ((ToggleSwitch)sender).IsOn;
            HasToggled = true;
        }

        private void toggleSwitchShowFPS_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitchShowFPSIsOn = ((ToggleSwitch)sender).IsOn;

            HasFPSToggled = true;
        }

        private void OpenXLive_Click(object sender, RoutedEventArgs e)
        {
            XLiveUIManager.Show(XLivePages.HomePage);
        }

        private void imgStartPlay_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            //Game1.Current.currentGameState = Game1.GameState.InGame;
            StartGameButtonClicked = true;
            Game1.Current.NavigateFromStartPage = true;
        }

        private void imgOpenXLive_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            XLiveUIManager.Show(XLivePages.HomePage);
        }

        private void ImgStore_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.Content = new StorePage();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // 跳转到 Windows 商店中指定 app 的主页（PDP - program display page; PFN - package family name）
            await Launcher.LaunchUriAsync(CurrentApp.LinkUri);
        }

        private async void btnReview_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(CurrentApp.LinkUri);

        }

        private void btnToast_Click(object sender, RoutedEventArgs e)
        {
            XamlHelper.MakeToast("测试", "Hello World!", "ms-appx:///Assets/rain.jpg");
        }

     

    }
}
