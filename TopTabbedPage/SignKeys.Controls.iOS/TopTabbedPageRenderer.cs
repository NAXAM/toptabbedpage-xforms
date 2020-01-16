using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using SignKeys.Controls;
using SignKeys.Controls.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TopTabbedPage), typeof(TopTabbedPageRenderer))]
namespace SignKeys.Controls.Platform.iOS
{
    public class TopTabbedPageRenderer : TabbedRenderer
    {
        bool isDisposed;
        TabsView tabsView;
        UIView tabsViewContainer;
        NSLayoutConstraint tabsViewContainerHeighContraint;

        public static void Preserve()
        {
            var now = DateTimeOffset.Now;
        }

        public TopTabbedPageRenderer()
        {
           
        }

        public override nint SelectedIndex {
            get => base.SelectedIndex;
            set
            {
                base.SelectedIndex = value;
                if (false == isDisposed)
                {
                    tabsView.SetSelectedIndex(value, true);
                }
            }
        }

        public override UIViewController SelectedViewController
        {
            get => base.SelectedViewController;
            set
            {
                base.SelectedViewController = value;
                if (null != value && false == isDisposed)
                {
                    tabsView.SelectedIndex = ViewControllers.IndexOf(value);
                }
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TabBar.ShadowImage = new UIImage();
            TabBar.BackgroundImage = new UIImage();
            tabsViewContainer = new UIView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            tabsView = new TabsView()
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            tabsView.TabSelected += OnTabSelected;
            View.AddSubview(tabsViewContainer);
            tabsViewContainer.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor).Active = true;
            tabsViewContainer.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor).Active = true;
            tabsViewContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            tabsViewContainerHeighContraint = tabsViewContainer.HeightAnchor.ConstraintEqualTo(44);
            tabsViewContainerHeighContraint.Active = true;

            tabsViewContainer.AddSubview(tabsView);
            tabsView.LeadingAnchor.ConstraintEqualTo(tabsViewContainer.LeadingAnchor).Active = true;
            tabsView.TrailingAnchor.ConstraintEqualTo(tabsViewContainer.TrailingAnchor).Active = true;
            tabsView.BottomAnchor.ConstraintEqualTo(tabsViewContainer.BottomAnchor).Active = true;
            tabsView.HeightAnchor.ConstraintEqualTo(tabsView.TabHeight + tabsView.HighlighterHeight).Active = true;

        }



        public override void ViewDidLayoutSubviews()
        {
            tabsViewContainerHeighContraint.Constant = View.SafeAreaInsets.Top + tabsView.TabHeight + tabsView.HighlighterHeight;
            var tabFrame = TabBar.Frame;
            //tabFrame.Height = tabSize;
            //tabFrame.Location = CGPoint.Empty;
            tabFrame.Location = new CGPoint(0, View.Frame.Y);
            tabFrame.Height = 0;
            TabBar.Frame = tabFrame;
         
            base.ViewDidLayoutSubviews();
            foreach (var vc in ViewControllers)
            {
                vc.AdditionalSafeAreaInsets = new UIEdgeInsets(tabsView.TabHeight, 0, -tabFrame.Height, 0);
            }
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement is TopTabbedPage oldPage)
            {
                oldPage.PropertyChanged -= OnElementPropertyChanged;
                oldPage.PagesChanged -= OnPagesChanged;
            }
            base.OnElementChanged(e);
            if (e.NewElement is TopTabbedPage newPage)
            {
                newPage.PropertyChanged += OnElementPropertyChanged;
                newPage.PagesChanged += OnPagesChanged;
                tabsView.Titles = newPage.Children.Select((p) => p.Title ?? "").ToArray();
                tabsViewContainer.BackgroundColor = tabsView.BackgroundColor = newPage.BarBackgroundColor.ToUIColor();
                if (false == newPage.SelectedTabColor.IsDefault)
                {
                    tabsView.SelectedTabColor = newPage.SelectedTabColor.ToUIColor();
                }
                if (false == newPage.UnselectedTabColor.IsDefault)
                {
                    tabsView.UnSelectedTabColor = newPage.UnselectedTabColor.ToUIColor();
                }
                tabsView.SetDistribution(newPage.FillTabsEqually ? UIKit.UIStackViewDistribution.FillEqually : UIKit.UIStackViewDistribution.EqualSpacing);
                if (newPage.ShadowedTabbar)
                {
                    tabsViewContainer.Layer.ShadowColor = UIColor.Gray.CGColor;
                    tabsViewContainer.Layer.ShadowOffset = new CGSize(0, 2);
                    tabsViewContainer.Layer.ShadowOpacity = 0.6f;
                }
                else
                {
                    tabsViewContainer.Layer.ShadowOpacity = 0.0f;
                }
                tabsView.IsHighlighterFullWidth = newPage.IsHighlighterFullWidth;
            }
        }

        protected virtual void OnPagesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!(Element is TopTabbedPage page) || isDisposed) return;
            tabsView.Titles = page.Children.Select((p) => p.Title ?? "").ToArray();
        }

        protected virtual void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(Element is TopTabbedPage page) || isDisposed) return;
            if (e.PropertyName == TopTabbedPage.FillTabsEquallyProperty.PropertyName)
            {
                tabsView.SetDistribution(page.FillTabsEqually ? UIKit.UIStackViewDistribution.FillEqually : UIKit.UIStackViewDistribution.EqualSpacing);
            }
            else if (e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
            {
                tabsViewContainer.BackgroundColor = tabsView.BackgroundColor = page.BarBackgroundColor.ToUIColor();
            }
            else if (e.PropertyName == TabbedPage.SelectedTabColorProperty.PropertyName)
            {
                tabsView.SelectedTabColor = page.SelectedTabColor.ToUIColor();
            }
            else if (e.PropertyName == TabbedPage.UnselectedTabColorProperty.PropertyName)
            {
                tabsView.UnSelectedTabColor = page.UnselectedTabColor.ToUIColor();
            }
            else if (e.PropertyName == TopTabbedPage.ShadowedTabbarProperty.PropertyName)
            {
                var pageRef = new WeakReference<TopTabbedPage>(page);
                var containerRef = new WeakReference<UIView>(tabsViewContainer);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (pageRef.TryGetTarget(out TopTabbedPage aPage)
                    && containerRef.TryGetTarget(out UIView container))
                    {
                        if (aPage.ShadowedTabbar)
                        {
                            container.Layer.ShadowColor = UIColor.Gray.CGColor;
                            container.Layer.ShadowOffset = new CGSize(0, 2);
                            container.Layer.ShadowOpacity = 0.6f;
                        }
                        else
                        {
                            container.Layer.ShadowOpacity = 0.0f;
                        }
                    }
                });
            }
            else if (e.PropertyName == TopTabbedPage.IsHighlighterFullWidthProperty.PropertyName)
            {
                tabsView.IsHighlighterFullWidth = page.IsHighlighterFullWidth;
            }
        }

        private void OnTabSelected(object sender, EventArgs e)
        {
            SelectedIndex = tabsView.SelectedIndex;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true;
                tabsView.RemoveFromSuperview();
                tabsView.TabSelected -= OnTabSelected;
                tabsView.Dispose();
                tabsView = null;
                tabsViewContainer.RemoveFromSuperview();
                tabsViewContainer.RemoveConstraint(tabsViewContainerHeighContraint);
                tabsViewContainerHeighContraint.Dispose();
                tabsViewContainerHeighContraint = null;
                tabsViewContainer.Dispose();
                tabsViewContainer = null;
            }
            base.Dispose(disposing);
        }
    }
}
