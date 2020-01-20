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
        string[] fontFamilies;
        public MyPage()
        {
            InitializeComponent();
            linkTapGesture.Command = new Command<string>(async (url) => await Xamarin.Essentials.Launcher.OpenAsync(url));
            if (Device.RuntimePlatform == Device.iOS)
            {
                foreach (var page in Children)
                {
                    page.On<iOS>().SetUseSafeArea(true);
                }
                fontFamilies = new string[]
                {
                    "LobsterTwo",
                    "Monda-Regular",
                    "Pacifico-Regular",
                    "VarelaRound-Regular"
                };
            }
            else
            {
                fontFamilies = new string[]
                {
                    "LobsterTwo-Regular.ttf#LobsterTwo",
                    "Monda-Regular.ttf#Monda-Regular",
                    "Pacifico-Regular.ttf#Pacifico-Regular",
                    "VarelaRound-Regular.ttf#VarelaRound-Regular"
                };
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

        public void ChangeFont(object sender, EventArgs e)
        {
            var rand = new Random();
            var size = rand.Next(10, 18);
            TabFontSize = size;
            //Font.SystemFontOfSize(size
            TabFontFamily = fontFamilies[rand.Next(0, fontFamilies.Length)];
        }
    }
}
