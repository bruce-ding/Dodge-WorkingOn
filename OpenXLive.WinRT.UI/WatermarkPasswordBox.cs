using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenXLive.WinRT.UI
{
    internal class WatermarkPasswordBox : TextBox
    {
        TextBlock WatermarkContent;
        private PasswordBox PasswordContent;

        public object WatermarkText
        {
            get { return base.GetValue(WatermarkProperty) as string; }
            set { base.SetValue(WatermarkProperty, value); }
        }
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(WatermarkPasswordBox), new PropertyMetadata("Type something...", OnWatermarkPropertyChanged));

        private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            WatermarkPasswordBox watermarkTextBox = sender as WatermarkPasswordBox;
            if (watermarkTextBox != null && watermarkTextBox.WatermarkContent != null)
            {
                watermarkTextBox.DetermineWatermarkContentVisibility();
            }
        }

        public Style WatermarkStyle
        {
            get { return base.GetValue(WatermarkStyleProperty) as Style; }
            set { base.SetValue(WatermarkStyleProperty, value); }
        }
        public static readonly DependencyProperty WatermarkStyleProperty =
            DependencyProperty.Register("WatermarkStyle", typeof(Style), typeof(WatermarkPasswordBox), null);

        public WatermarkPasswordBox()
        {
            DefaultStyleKey = typeof(WatermarkPasswordBox);
            this.TextChanged += WatermarkPasswordBox_TextChanged;
        }

        void WatermarkPasswordBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WatermarkPasswordBox watermarkTextBox = sender as WatermarkPasswordBox;
            if (watermarkTextBox != null && watermarkTextBox.WatermarkContent != null)
            {
                watermarkTextBox.DetermineWatermarkContentVisibility();
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.WatermarkContent = this.GetTemplateChild("watermarkContent") as TextBlock;
            this.PasswordContent = this.GetTemplateChild("ContentElement") as PasswordBox;
            if (WatermarkContent != null && WatermarkContent != null)
            {
                PasswordContent.PasswordChanged += new RoutedEventHandler(PasswordContent_PasswordChanged);
                DetermineWatermarkContentVisibility();
            }
        }

        void PasswordContent_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwdBx = sender as PasswordBox;
            this.Text = passwdBx.Password;
        }

        private void DetermineWatermarkContentVisibility()
        {
            if (string.IsNullOrEmpty(this.PasswordContent.Password))
            {
                this.WatermarkContent.Visibility = Visibility.Visible;
            }
            else
            {
                this.WatermarkContent.Visibility = Visibility.Collapsed;
            }
        }


    }
}
