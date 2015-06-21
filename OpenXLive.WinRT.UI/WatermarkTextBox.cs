using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace OpenXLive.WinRT.UI
{
    [TemplatePart(Name = WatermarkTextBlockName, Type = typeof(TextBlock))]
    [TemplateVisualState(GroupName = WatermarkStatesGroupName, Name = WatermarkVisibleStateName)]
    [TemplateVisualState(GroupName = WatermarkStatesGroupName, Name = WatermarkHiddenStateName)]
    internal class WatermarkTextBox : TextBox
    {
        #region Consts
        private const string WatermarkStatesGroupName = "WatermarkStates";
        private const string WatermarkVisibleStateName = "WatermarkVisible";
        private const string WatermarkHiddenStateName = "WatermarkHidden";
        private const string WatermarkTextBlockName = "WatermarkTextBlock";
        #endregion

        private TextBlock _watermarkTextBlock;

        #region WatermarkText
        /// <summary>
        /// WatermarkText Dependency Property
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(
                "WatermarkText",
                typeof(string),
                typeof(WatermarkTextBox),
                new PropertyMetadata("Type something..."));

        /// <summary>
        /// Gets or sets the WatermarkText property. This dependency property 
        /// indicates the watermark text to show to the user.
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }
        #endregion

        #region WatermarkStyle
        /// <summary>
        /// WatermarkStyle Dependency Property
        /// </summary>
        private static readonly DependencyProperty WatermarkStyleProperty =
            DependencyProperty.Register(
                "WatermarkStyle",
                typeof(Style),
                typeof(WatermarkTextBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the WatermarkStyle property. This dependency property 
        /// indicates the style of the watermark (TextBlock).
        /// </summary>
        public Style WatermarkStyle
        {
            get { return (Style)GetValue(WatermarkStyleProperty); }
            set { SetValue(WatermarkStyleProperty, value); }
        }
        #endregion

        public WatermarkTextBox()
        {
            DefaultStyleKey = typeof(WatermarkTextBox);
            this.TextChanged += OnTextChanged;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _watermarkTextBlock = GetTemplateChild(WatermarkTextBlockName) as TextBlock;
            UpdateWatermarkVisualState();
        }

        private void UpdateWatermarkVisualState()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                VisualStateManager.GoToState(this, WatermarkVisibleStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, WatermarkHiddenStateName, true);
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermarkVisualState();
        }

    }
}
