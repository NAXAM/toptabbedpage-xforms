using System;
using System.Collections.Generic;
using SignKeys.Controls;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace TopTabbedPageDemo
{
    public partial class MyPage : TopTabbedPage
    {
        public MyPage()
        {
            InitializeComponent();
            foreach (var page in Children)
            {
                page.On<iOS>().SetUseSafeArea(true);
            }
        }

        public void ToggleDistribution(object sender, EventArgs e)
        {
            FillTabsEqually = !FillTabsEqually;
        }

        public void ToggleShadow(object sender, EventArgs e)
        {
            ShadowedTabbar = !ShadowedTabbar;
        }

        public void ToggleHighlighter(object sender, EventArgs e)
        {
            IsHighlighterFullWidth = !IsHighlighterFullWidth;
        }
    }
}
