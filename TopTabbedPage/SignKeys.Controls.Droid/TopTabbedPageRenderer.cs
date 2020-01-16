using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using SignKeys.Controls;
using SignKeys.Controls.Platform.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TopTabbedPage), typeof(TopTabbedPageRenderer))]
namespace SignKeys.Controls.Platform.Droid
{
    public class TopTabbedPageRenderer : TabbedPageRenderer
    {
        protected TabLayout TopTabLayout { get; private set; }
        float[] tabWidths;
        float maxTabWidth;

        public static void Preserve()
        {
            var now = DateTimeOffset.Now;
        }

        public TopTabbedPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            var currentResource = FormsAppCompatActivity.TabLayoutResource;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.TopTabbar;
            TopTabLayout = null;
            tabWidths = null;
            maxTabWidth = 0;
            base.OnElementChanged(e);
            FormsAppCompatActivity.TabLayoutResource = currentResource;
            if (e.OldElement is TabbedPage oldPage)
            {
                oldPage.ChildAdded -= OnChildAdded;
                oldPage.ChildRemoved -= OnChildRemoved;
            }
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
                page.ChildAdded += OnChildAdded;
                page.ChildRemoved += OnChildRemoved;
                TopTabLayout.TabIndicatorFullWidth = page.IsHighlighterFullWidth;
                ResetTabDistribution();
            }
        }

        private void OnChildRemoved(object sender, ElementEventArgs e)
        {
            ResetTabDistribution();
        }

        private void OnChildAdded(object sender, ElementEventArgs e)
        {
            ResetTabDistribution();
        }

        void ResetTabDistribution()
        {
            CalculateTabsWidth();
            UpdateTabDistribution(((TopTabbedPage)Element).FillTabsEqually, Width) ;
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
                UpdateTabDistribution(page.FillTabsEqually, Width);
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
            if (r - l != calculatedWidth && Element is TopTabbedPage page)
            {
                UpdateTabDistribution(page.FillTabsEqually, r - l);
            }
        }

        void UpdateTabDistribution(bool equalWidth, int containerWidth)
        {
            var count = TopTabLayout.TabCount;
            if (null == tabWidths || tabWidths.Length != count)
            {
                CalculateTabsWidth();
            }
            calculatedWidth = containerWidth;
            var totalWidth = equalWidth ? maxTabWidth * count : tabWidths.Sum();
            float ratio = 1;
            if (containerWidth > totalWidth)
            {
                ratio = (float)containerWidth / (float)totalWidth;
            }
            var slidingTabStrip = (ViewGroup)TopTabLayout.GetChildAt(0);
            var i = 0;
            while (i < count)
            {
                var tab = slidingTabStrip.GetChildAt(i);
                var layoutParams = tab.LayoutParameters as LinearLayout.LayoutParams;
                layoutParams.Weight = 1;
                layoutParams.Width = (int)Math.Ceiling(equalWidth ? maxTabWidth : tabWidths[i] * ratio);
                tab.LayoutParameters = layoutParams;
                i++;
            }
        }

        void CalculateTabsWidth()
        {
            var slidingTabStrip = (ViewGroup)TopTabLayout.GetChildAt(0);
            var count = TopTabLayout.TabCount;
            var i = 0;
            tabWidths = new float[count];
            maxTabWidth = 0;
            var padding = (float)Context.FromPixels(48);
            while (i < count)
            {
                var tab = slidingTabStrip.GetChildAt(i);
                if (tab is TabLayout.TabView tv)
                {
                    if (tv.ChildCount >= 2 && tv.GetChildAt(1) is TextView textView)
                    {
                        var w = (float)Context.ToPixels(Measure(textView)) + padding;
                        tabWidths[i] = w;
                        if (w > maxTabWidth)
                        {
                            maxTabWidth = w;
                        }
                    }
                    else
                    {
                        var w = (float)Context.FromPixels(Measure(tv.Tab.Text)) + padding;
                        tabWidths[i] = w;
                        if (w > maxTabWidth)
                        {
                            maxTabWidth = w;
                        }
                    }
                }
                i++;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Element != null)
                {
                    Element.ChildAdded -= OnChildAdded;
                    Element.ChildRemoved -= OnChildRemoved;
                }
            }
            base.Dispose(disposing);
        }
    }

}
