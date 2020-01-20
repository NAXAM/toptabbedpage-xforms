using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace SignKeys.Controls
{
    public class TopTabbedPage : Xamarin.Forms.TabbedPage
    {
        /// <summary>
        /// By default, the tabs are layouted with equal spacing.
        /// Enabling FillTabsEqually will make the tabs have the same width (width of the longest tab 
        /// </summary>
        public static readonly BindableProperty FillTabsEquallyProperty = BindableProperty.Create(
            nameof(FillTabsEqually),
            typeof(bool),
            typeof(TopTabbedPage),
            default(bool),
            BindingMode.OneWay);
        public bool FillTabsEqually
        {
            get { return (bool)GetValue(FillTabsEquallyProperty); }
            set { SetValue(FillTabsEquallyProperty, value); }
        }

        public static readonly BindableProperty ShadowedTabbarProperty = BindableProperty.Create(
            nameof(ShadowedTabbar),
            typeof(bool),
            typeof(TopTabbedPage),
            true,
            BindingMode.OneWay);
        public bool ShadowedTabbar
        {
            get { return (bool)GetValue(ShadowedTabbarProperty); }
            set { SetValue(ShadowedTabbarProperty, value); }
        }

        public static readonly BindableProperty IsHighlighterFullWidthProperty = BindableProperty.Create(
            nameof(IsHighlighterFullWidth),
            typeof(bool),
            typeof(TopTabbedPage),
            true,
            BindingMode.OneWay);
        public bool IsHighlighterFullWidth
        {
            get { return (bool)GetValue(IsHighlighterFullWidthProperty); }
            set { SetValue(IsHighlighterFullWidthProperty, value); }
        }

        public static readonly BindableProperty TabFontFamilyProperty = BindableProperty.Create(
            nameof(TabFontFamily),
            typeof(string),
            typeof(TopTabbedPage),
            default(string),
            BindingMode.OneWay);
        public string TabFontFamily
        {
            get { return (string)GetValue(TabFontFamilyProperty); }
            set { SetValue(TabFontFamilyProperty, value); }
        }

        public static readonly BindableProperty TabFontSizeProperty = BindableProperty.Create(
            nameof(TabFontSize),
            typeof(double),
            typeof(TopTabbedPage),
            default(double),
            BindingMode.OneWay);
        public double TabFontSize
        {
            get { return (double)GetValue(TabFontSizeProperty); }
            set { SetValue(TabFontSizeProperty, value); }
        }

        //public static readonly BindableProperty HighlighterAnimationDurationProperty = BindableProperty.Create(
        //    nameof(HighlighterAnimationDuration),
        //    typeof(int),
        //    typeof(TopTabbedPage),
        //    300,
        //    BindingMode.OneWay);
        //public int HighlighterAnimationDuration
        //{
        //    get { return (int)GetValue(HighlighterAnimationDurationProperty); }
        //    set { SetValue(HighlighterAnimationDurationProperty, value); }
        //}

        public TopTabbedPage()
        {
            BarBackgroundColor = Color.White;
            SelectedTabColor = Color.Black;
            UnselectedTabColor = Color.Gray;
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Top);
        }
    }
}