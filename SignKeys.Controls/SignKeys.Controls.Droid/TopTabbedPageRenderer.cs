using System;
using System.Collections.Generic;
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
            calculatedWidth = 0;
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
        int calculatedWidth = 0;
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (Device.Idiom == TargetIdiom.Tablet && null != TopTabLayout)
            {
                UpdateTabsWidthOnTablet(r - l);
            }
        }

        void UpdateTabsWidthOnTablet(int containerWidth)
        {
            if (containerWidth == calculatedWidth) return;
            calculatedWidth = containerWidth;
            var slidingTabStrip = (ViewGroup)TopTabLayout.GetChildAt(0);
            var count = TopTabLayout.TabCount;
            var i = 0;
            float totalWidth = 0;
            var widths = new float[count];
            while (i < count)
            {
                var padding = (float)Context.FromPixels(40);

                var tab = slidingTabStrip.GetChildAt(i);
                if (tab is TabLayout.TabView tv)
                {
                    if (tv.ChildCount >= 2 && tv.GetChildAt(1) is TextView textView)
                    {
                        var w = (float)Context.ToPixels(Measure(textView)) + padding;
                        totalWidth += w;
                        widths[i] = w;
                    }
                    else
                    {
                        var w = (float)Context.FromPixels(Measure(tv.Tab.Text)) + padding;
                        totalWidth += w;
                        widths[i] = w;
                    }
                }
                i++;
            }
            i = 0;
            float ratio = 1;
            if (containerWidth > totalWidth)
            {
                ratio = (float)containerWidth / (float)totalWidth;
            }
            while (i < count)
            {
                var tab = slidingTabStrip.GetChildAt(i);
                var layoutParams = tab.LayoutParameters as LinearLayout.LayoutParams;
                layoutParams.Weight = 1;
                layoutParams.Width = (int)Math.Ceiling(widths[i] * ratio);
                tab.LayoutParameters = layoutParams;
                i++;
            }
        }

        void UpdateTabDistribution(bool equalWidth)
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                UpdateTabsWidthOnTablet(Width);
            }
            else
            {
                var slidingTabStrip = (ViewGroup)TopTabLayout.GetChildAt(0);
                var count = TopTabLayout.TabCount;
                var i = 0;
                TopTabLayout.TabMode = TabLayout.ModeScrollable;
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

        public static int Measure(TextView textView)
        {
            var text = textView.Text;
            if (string.IsNullOrEmpty(text)) return 0;
            var bounds = new Android.Graphics.Rect();
            var textPaint = textView.Paint;
            textPaint.GetTextBounds(text, 0, text.Length, bounds);
            int height = bounds.Height();
            int width = bounds.Width();
            return width;
        }

        public int Measure(string text)
        {
            return Measure(new TextView(this.Context)
            {
                Text = text
            });
        }
    }

}
