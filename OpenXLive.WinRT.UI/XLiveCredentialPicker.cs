using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;
using Windows.Foundation;
using OpenXLive;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace OpenXLive.WinRT.UI
{
    public enum XLiveCredentialPickerOptions
    {
        CreateAccount,
        NormalLogin,
        SocialLogin
    }

    public sealed class ClosedEventArgs
    {
        public ClosedEventArgs(MyPlayer player)
        {
            this.player = player;
        }
        public MyPlayer player { get; set; }
    }

    /// <summary>
    /// Creates an asynchronous object that displays a dialog box of credentials
    //     to login to OpenXLive System or SNS
    /// </summary>
    public sealed class XLiveCredentialPicker : Control
    {
        public XLiveCredentialPicker()
        {
            this.DefaultStyleKey = typeof(XLiveCredentialPicker);
            ResourceDictionary rc = new ResourceDictionary();
            rc.Source = new Uri("ms-appx:///OpenXLive.WinRT.UI/Themes/ResourceDictionary.xaml");
            Resources.MergedDictionaries.Add(rc);

            var p = ((Window.Current.Content as Frame).Content as Page).Content;
            if (p.GetType() == typeof(Grid) && (p as Grid).RowDefinitions.Count > 0)
            {
                Grid.SetRowSpan(this, (p as Grid).RowDefinitions.Count);
            }
            this.Width = Window.Current.CoreWindow.Bounds.Width;
            this.Height = Window.Current.CoreWindow.Bounds.Height;
            (p as Panel).Children.Add(this);           
        }

        #region EventHandler
        public event EventHandler<ClosedEventArgs> Closed;
        #endregion

        #region fields and methods
        Button _closePicker;
        Grid _firstGrid;
        Grid _secondGrid;
        WebView _webbrowser;
        ProgressRing _pgr;

        Border _border;
        TextBlock _caption;
        WatermarkTextBox _userName;
        WatermarkPasswordBox _password;
        CheckBox _checkBox;
        TextBlock _newAccount;
        StackPanel _stackPanel;
        Button _login;
        Button _cancel;
        Button _create;
        Button _back;
        StackPanel _mainContent;
        StackPanel _newAccountContent;
        StackPanel _secondContent;
        WatermarkTextBox _email;
        WatermarkPasswordBox _newPassword;
        WatermarkPasswordBox _confirmPassword;
        WatermarkTextBox _newUserName;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _closePicker = this.GetTemplateChild("ClosePicker") as Button;
            _closePicker.Click += _closePicker_Click;

            _firstGrid = this.GetTemplateChild("FirstGrid") as Grid;

            _secondGrid = this.GetTemplateChild("SecondGrid") as Grid;

            _webbrowser = this.GetTemplateChild("webbrowser") as WebView;
            _webbrowser.LoadCompleted += _webbrowser_LoadCompleted;

            _pgr = this.GetTemplateChild("pgr") as ProgressRing;

            _border = this.GetTemplateChild("border") as Border;

            _caption = this.GetTemplateChild("Caption") as TextBlock;

            _userName = this.GetTemplateChild("UserName") as WatermarkTextBox;
            _userName.Focus(Windows.UI.Xaml.FocusState.Keyboard);

            _password = this.GetTemplateChild("Password") as WatermarkPasswordBox;
            _password.KeyUp += _password_KeyUp;

            _checkBox = this.GetTemplateChild("RememberMe") as CheckBox;

            _newAccount = this.GetTemplateChild("NewAccount") as TextBlock;
            _newAccount.Tapped += _newAccount_Tapped;

            _login = this.GetTemplateChild("Login") as Button;
            _login.Tapped += _login_Tapped;

            _cancel = this.GetTemplateChild("Cancel") as Button;
            _cancel.Tapped += _cancel_Tapped;

            _stackPanel = this.GetTemplateChild("SNS") as StackPanel;

            _mainContent = this.GetTemplateChild("mainContent") as StackPanel;

            _newAccountContent = this.GetTemplateChild("NewAccountContent") as StackPanel;

            _email = this.GetTemplateChild("email") as WatermarkTextBox;
            
            _newPassword = this.GetTemplateChild("newPassword") as WatermarkPasswordBox;

            _confirmPassword = this.GetTemplateChild("confirmPassword") as WatermarkPasswordBox;

            _newUserName = this.GetTemplateChild("newUserName") as WatermarkTextBox;
            _newUserName.KeyUp += _create_KeyUp;

            _create = this.GetTemplateChild("Create") as Button;
            _create.Tapped += _create_Tapped;

            _back = this.GetTemplateChild("Back") as Button;
            _back.Tapped += _back_Tapped;

            _secondContent = this.GetTemplateChild("secondContent") as StackPanel;
            
            SNSItemSource = XLiveGameManager.CurrentSession.SNSProviders;
            SNSProvidersTemplate = Resources["SNSTemplate"] as DataTemplate;

            if (SNSItemSource != null)
                LoadSource();
        }

        void _closePicker_Click(object sender, RoutedEventArgs e)
        {
            XLiveCredentialPicker_Closed();
        }

        void _back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _newAccountContent.Visibility = Visibility.Collapsed;
            _mainContent.Visibility = Visibility.Visible;
            _userName.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }

        void _newAccount_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_newAccountContent != null)
            {
                _newAccountContent.Visibility = Visibility.Visible;
                _mainContent.Visibility = Visibility.Collapsed;
                _email.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            }
        }

        void _cancel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            UIElement p = null;

            if ((Window.Current.Content as Page) != null)
                p = (Window.Current.Content as Page).Content;
            else if ((Window.Current.Content as Frame) != null)
                p = ((Window.Current.Content as Frame).Content as Page).Content;
            else return;

            (p as Panel).Children.Remove(this);
        }

        async void _create_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var result = await this.PickAsync(XLiveCredentialPickerOptions.CreateAccount);
                //XLiveCredentialPicker_Closed();
            }
        }

        async void _password_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var result = await this.PickAsync(XLiveCredentialPickerOptions.NormalLogin);
                //XLiveCredentialPicker_Closed();
            }
        }

        async void _login_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var myResult = await this.PickAsync(XLiveCredentialPickerOptions.NormalLogin);
            //XLiveCredentialPicker_Closed();
        }

        async void _create_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var myResult = await this.PickAsync(XLiveCredentialPickerOptions.CreateAccount);
            //XLiveCredentialPicker_Closed();
        }

        async void _webbrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _webbrowser.Visibility = Windows.UI.Xaml.Visibility.Visible;
            _pgr.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (e.Uri.AbsoluteUri.ToString() == "http://passport.openxlive.net/Account/SocialClientLogin/"
                || e.Uri.AbsoluteUri.ToString() == "http://passport.openxlive.com/Account/SocialClientLogin/")
            {
                GameSession session = XLiveGameManager.CurrentSession;
                MyPlayer player = session.CurrentPlayer;
                try
                {
                    AsyncProcessResult result = await player.SocialLoginAsync(SocialLoginToken);
                }
                catch (Exception ex)
                {
                    MessageDialog dlg = new MessageDialog(ex.Message, "Login failed");
                    dlg.ShowAsync();
                }

                //if (SNSLoginTapped != null)
                //    SNSLoginTapped(this, new TappedRoutedEventArgs());  
                var myResult = await this.PickAsync(XLiveCredentialPickerOptions.SocialLogin);
                XLiveCredentialPicker_Closed();
            }           
        }

        void XLiveCredentialPicker_Closed()
        {
            UIElement p = null;

            if ((Window.Current.Content as Page) != null)
                p = (Window.Current.Content as Page).Content;
            else if ((Window.Current.Content as Frame) != null)
                p = ((Window.Current.Content as Frame).Content as Page).Content;
            else return;

            (p as Panel).Children.Remove(this);

            if (Closed != null && credentialResults != null)
                Closed(this, new ClosedEventArgs(credentialResults.player));
        }

        XLiveCredentialResults credentialResults = null;

        #endregion
        
        #region DependencyProperty
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(String), typeof(XLiveCredentialPicker), new PropertyMetadata("Log In"));

        public bool IsRemember
        {
            get { return (bool)GetValue(IsRememberProperty); }
            set { SetValue(IsRememberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRemember.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty IsRememberProperty =
            DependencyProperty.Register("IsRemember", typeof(bool), typeof(XLiveCredentialPicker), new PropertyMetadata(true));

        public Brush PickerBackground
        {
            get { return (Brush)GetValue(PickerBackgroundProperty); }
            set { SetValue(PickerBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PickerBackground.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty PickerBackgroundProperty =
            DependencyProperty.Register("PickerBackground", typeof(Brush), typeof(XLiveCredentialPicker), new PropertyMetadata(
                new SolidColorBrush(Windows.UI.Colors.Black)));

        public DataTemplate SNSProvidersTemplate
        {
            get { return (DataTemplate)GetValue(SNSProvidersTemplateProperty); }
            set { SetValue(SNSProvidersTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SNSProvidersTemplate.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SNSProvidersTemplateProperty =
            DependencyProperty.Register("SNSProvidersTemplate", typeof(DataTemplate), typeof(XLiveCredentialResults), new PropertyMetadata(null));

        public IEnumerable SNSItemSource
        {
            get { return (IEnumerable)GetValue(SNSItemSourceProperty); }
            set { SetValue(SNSItemSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SNSItemSource.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SNSItemSourceProperty =
            DependencyProperty.Register("SNSItemSource", typeof(IEnumerable), typeof(XLiveCredentialResults), new PropertyMetadata(null, new PropertyChangedCallback(OnSNSItemSourceChanged)));

        private static void OnSNSItemSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            XLiveCredentialPicker picker = o as XLiveCredentialPicker;
            picker.LoadSource();
        }

        private void LoadSource()
        {
            if (this._stackPanel == null)
                return;
            this._stackPanel.Children.Clear();

            foreach (var item in this.SNSItemSource)
            {
                ContentControl c = new ContentControl();
                c.Content = item;
                c.ContentTemplate = this.SNSProvidersTemplate;
                this._stackPanel.Children.Add(c);
                c.Tapped += c_Tapped;
            }
        }

        string SocialLoginToken = Guid.NewGuid().ToString();

        void c_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _firstGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            _secondGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;

            string loginUrl = ((e.OriginalSource as Image).DataContext as SNSProvider).LoginUrl;
            string url = string.Format("{0}?Token={1}", loginUrl, SocialLoginToken);
            Uri targetUrl = new Uri(url, UriKind.Absolute);

            _webbrowser.Navigate(targetUrl);         
        }

        #endregion

        #region Pick
        /// <summary>
        /// Pick credential and information from the user
        /// </summary>
        /// <param name="credentialOption">The option to login or create new account, "Login" or "Create"</param>
        /// <returns>The credential and information from the user</returns>
        public IAsyncOperation<XLiveCredentialResults> PickAsync(XLiveCredentialPickerOptions credentialOption)
        {     
            if (credentialOption == XLiveCredentialPickerOptions.NormalLogin)
                credentialResults = new XLiveCredentialResults(_userName.Text, _password.Text, _checkBox.IsChecked);
            else if (credentialOption == XLiveCredentialPickerOptions.CreateAccount)
                credentialResults = new XLiveCredentialResults(_email.Text, _newPassword.Text, _confirmPassword.Text, _newUserName.Text);
            else if (credentialOption == XLiveCredentialPickerOptions.SocialLogin)
                credentialResults = new XLiveCredentialResults();

            return Task.Run<XLiveCredentialResults>(async () =>
            {
                GameSession session = XLiveGameManager.CurrentSession;

                if (credentialOption == XLiveCredentialPickerOptions.NormalLogin)
                {
                    if (session != null && session.IsValid)
                    {
                        MyPlayer player = new MyPlayer(session);
                        if (player.IsAnonymous)
                        {
                            try
                            {
                                var result = await player.LogonAsync(
                                    credentialResults.CredentialUserName, credentialResults.CredentialPassword, credentialResults.CredentialSaved);
                                if (result.ReturnValue)
                                {
                                    credentialResults.player = session.CurrentPlayer;
                                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        XLiveCredentialPicker_Closed();
                                    });
                                }
                                else
                                {
                                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                    {
                                        MessageDialog dlg = new MessageDialog(result.ErrorMessage, "Login failed");
                                        await dlg.ShowAsync();
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    MessageDialog dlg = new MessageDialog(ex.Message, "Login failed");
                                    await dlg.ShowAsync();
                                });
                            }

                        }
                    }
                }
                else if (credentialOption == XLiveCredentialPickerOptions.CreateAccount)
                {
                    if (session != null && session.IsValid)
                    {
                        if (credentialResults.NewAccountPassword != credentialResults.NewAccountConfirmPassword)
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                MessageDialog dlg = new MessageDialog("Password is not confirm, please try again");
                                await dlg.ShowAsync();
                            });
                        }
                        else
                        {
                            MyPlayer player = new MyPlayer(session);

                            try
                            {
                                var result = await player.CreateAccountAsync(
                                    credentialResults.NewAccountEmail, credentialResults.NewAccountPassword, credentialResults.NewAccountUserName);
                                if (result.ReturnValue)
                                {
                                    credentialResults.player = session.CurrentPlayer;
                                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                    {
                                        MessageDialog dlg = new MessageDialog("Create account succeed");
                                        await dlg.ShowAsync();
                                        await credentialResults.player.LogonAsync(credentialResults.NewAccountEmail, credentialResults.NewAccountPassword, true);
                                        
                                        XLiveCredentialPicker_Closed();
                                    });
                                }
                                else
                                {
                                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                    {
                                        MessageDialog dlg = new MessageDialog(result.ErrorMessage, "Create account failed");
                                        await dlg.ShowAsync();
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                                {
                                    MessageDialog dlg = new MessageDialog(ex.Message, "Create account failed");
                                    await dlg.ShowAsync();
                                });
                            }
                        }
                    }
                }
                else if (credentialOption == XLiveCredentialPickerOptions.SocialLogin)
                {
                    credentialResults.player = session.CurrentPlayer;
                }
                
                return credentialResults;
            }).AsAsyncOperation();
        }
        #endregion
    }
}
