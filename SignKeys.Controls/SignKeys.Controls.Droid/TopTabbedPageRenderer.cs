using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SignKeys.Controls;
using SignKeys.Controls.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly:ExportRenderer(typeof(TopTabbedPage), typeof(TopTabbedPageRenderer))]
namespace SignKeys.Controls.Droid
{
    public class TopTabbedPageRenderer : TabbedPageRenderer
    {
        protected TabLayout TopTabLayout { get; private set; }
        public TopTabbedPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            var currentResource = FormsAppCompatActivity.TabLayoutResource;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.TopTabbar;
            TopTabLayout = null;
            base.OnElementChanged(e);
            FormsAppCompatActivity.TabLayoutResource = currentResource;
            if (e.NewElement is TopTabbedPage page)
            {
                TopTabLayout = ViewGroup.FindChildOfType<TabLayout>();
                if (TopTabLayout == null)
                {
                    System.Diagnostics.Debug.WriteLine("TopTabbedPageRenderer.OnElementChanged(): No TabLayout found. Badge not added");
                    return;
                }
                if (page.ShadowedTabbar)
                {
                    TopTabLayout.Elevation = 8;
                }
                else
                {
                    TopTabLayout.Elevation = 0;
                }
                if (false == page.SelectedTabColor.IsDefault)
                {
                    TopTabLayout.SetSelectedTabIndicatorColor(page.SelectedTabColor.ToAndroid());
                }
                UpdateTabDistribution(page.FillTabsEqually) ;
                TopTabLayout.TabIndicatorFullWidth = page.IsHighlighterFullWidth;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (TopTabLayout == null || !(Element is TopTabbedPage page)) return;
            if (e.PropertyName == TopTabbedPage.ShadowedTabbarProperty.PropertyName)
            {
                if (page.ShadowedTabbar)
                {
                    TopTabLayout.Elevation = 8;
                }
                else
                {
                    TopTabLayout.Elevation = 0;
                }
            }
            else if (e.PropertyName == TopTabbedPage.FillTabsEquallyProperty.PropertyName)
            {
                UpdateTabDistribution(page.FillTabsEqually);
            }
            else if (e.PropertyName == TopTabbedPage.IsHighlighterFullWidthProperty.PropertyName)
            {
                TopTabLayout.TabIndicatorFullWidth = page.IsHighlighterFullWidth;
            }
        }

        void UpdateTabDistribution(bool equalWidth)
        {
            var slidingTabStrip = (ViewGroup)TopTabLayout.GetChildAt(0);
            var count = TopTabLayout.TabCount;
            var i = 0;
            if (equalWidth)
            {
                while (i < count)
                {
                    var tab = slidingTabStrip.GetChildAt(i);
                    i++;
                    var layoutParams = tab.LayoutParameters as LinearLayout.LayoutParams;
                    layoutParams.Weight = 1;
                    layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
                    tab.LayoutParameters = layoutParams;
                }
            }
            else
            {
                while (i < count)
                {
                    var tab = slidingTabStrip.GetChildAt(i);
                    i++;
                    var layoutParams = tab.LayoutParameters as LinearLayout.LayoutParams;
                    layoutParams.Weight = 1;
                    layoutParams.Width = LinearLayout.LayoutParams.WrapContent;
                    tab.LayoutParameters = layoutParams;
                }
            }
        }
    }
}
