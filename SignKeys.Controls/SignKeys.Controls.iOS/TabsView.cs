using System;
using System.Linq;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SignKeys.Controls.iOS
{
    public class TabsView: UIScrollView
    {
        public EventHandler<EventArgs> TabSelected;

        private string[] _Titles;
        public string[] Titles
        {
            get => _Titles;
            set
            {
                _Titles = value;
                UpdateTabs();
            }
        }

        private UIFont _TitleFont = UIFont.SystemFontOfSize(15);
        public UIFont TitleFont
        {
            get => _TitleFont;
            set
            {
                _TitleFont = value;
                var iter = buttonStack.ArrangedSubviews.GetEnumerator();
                using (iter as IDisposable)
                {
                    while (iter.MoveNext())
                    {
                        if (iter.Current is UIButton button)
                        {
                            button.TitleLabel.Font = value;
                        }
                    }
                }
            }
        }

        private UIColor _SelectedTabColor;
        public UIColor SelectedTabColor
        {
            get => _SelectedTabColor;
            set
            {
                _SelectedTabColor = value;
                var iter = buttonStack.ArrangedSubviews.GetEnumerator();
                using (iter as IDisposable)
                {
                    while (iter.MoveNext())
                    {
                        if (iter.Current is UIButton button)
                        {
                            button.SetTitleColor(_SelectedTabColor, UIControlState.Disabled);
                        }
                    }
                }
                highlighter.BackgroundColor = _SelectedTabColor.CGColor;
            }
        }

        private UIColor _UnselectedTabColor = UIColor.Gray;
        public UIColor UnSelectedTabColor
        {
            get => _UnselectedTabColor;
            set
            {
                _UnselectedTabColor = value;
                var iter = buttonStack.ArrangedSubviews.GetEnumerator();
                using (iter as IDisposable)
                {
                    while (iter.MoveNext())
                    {
                        if (iter.Current is UIButton button)
                        {
                            button.SetTitleColor(_UnselectedTabColor, UIControlState.Normal);
                        }
                    }
                }
                //TODO;
            }
        }

        private nfloat _TabHeight = 40;
        public nfloat TabHeight
        {
            get => _TabHeight;
            set
            {
                _TabHeight = value;
                tabHeightConstraint.Constant = value;
                //TODO
            }
        }

        private nfloat _HighlighterHeight = 2;
        public nfloat HighlighterHeight
        {
            get => _HighlighterHeight;
            set
            {
                var newValue = (nfloat)Math.Max((nfloat)0, value);
                if (Math.Abs(newValue - value) > nfloat.Epsilon)
                {
                    _HighlighterHeight = newValue;
                    tabBottomConstraint.Constant = newValue;
                    //TODO
                    UpdateHighlighterPosition(false);
                }
            }
        }

        private nint _SelectedIndex;
        public nint SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (value != _SelectedIndex)
                {
                    var views = buttonStack.ArrangedSubviews;
                    if (_SelectedIndex + 1 < views.Length)
                    {
                        ((UIButton)views[_SelectedIndex + 1]).Enabled = true;
                    }
                    _SelectedIndex = value;
                    if (_SelectedIndex + 1 < views.Length)
                    {
                        ((UIButton)views[_SelectedIndex + 1]).Enabled = false;
                    }
                }
            }
        }

        private bool _IsHighlighterFullWidth = true;
        public bool IsHighlighterFullWidth
        {
            get => _IsHighlighterFullWidth;
            set
            {
                if (value != _IsHighlighterFullWidth)
                {
                    _IsHighlighterFullWidth = value;
                    UpdateHighlighterPosition(false);
                }
            }
        }

        private NSLayoutConstraint tabHeightConstraint;
        private NSLayoutConstraint tabBottomConstraint;
        private bool isDisposed = false;
        private CALayer highlighter = new CALayer();
        private bool delayHighlighterPosition = false;

        private SKStackView buttonStack = new SKStackView()
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Alignment = UIStackViewAlignment.Fill,
            Axis = UILayoutConstraintAxis.Horizontal,
            Spacing = 0,
            Distribution = UIStackViewDistribution.FillEqually
        };
        
        public TabsView()
        {
            SetupButtonStack();
        }

        public TabsView(CoreGraphics.CGRect frame): base(frame)
        {
            SetupButtonStack();
        }

        public TabsView(NSCoder coder) : base(coder)
        {
            SetupButtonStack();
        }

        protected TabsView(NSObjectFlag t) : base(t)
        {
            SetupButtonStack();
        }

        protected internal TabsView(IntPtr handle) : base(handle)
        {
            SetupButtonStack();
        }

        public override void LayoutSubviews()
        {
            delayHighlighterPosition = true;
            base.LayoutSubviews();
        }

        public void SetSelectedIndex(nint index, bool animated)
        {
            SelectedIndex = index;
            UpdateHighlighterPosition(animated);
        }

        public void SetDistribution(UIStackViewDistribution distribution)
        {
            if (buttonStack.Distribution == distribution) return;
            delayHighlighterPosition = true;
            buttonStack.Distribution = distribution;
            if (distribution == UIStackViewDistribution.FillProportionally)
            {
                var iter = buttonStack.ArrangedSubviews.GetEnumerator();
                using (iter as IDisposable)
                {
                    while (iter.MoveNext())
                    {
                        if (iter.Current is UIButton button)
                        {
                            var constraint = button.Constraints.FirstOrDefault((c) => string.Equals(c.GetIdentifier(), $"button_width_constraint_{button.Tag - 1000}"));
                            if (null == constraint)
                            {
                                constraint = button.WidthAnchor.ConstraintGreaterThanOrEqualTo(button.TitleLabel.TextRectForBounds(new CGRect(0, 0, double.MaxValue, _TabHeight), 1).Width
                                + button.ContentEdgeInsets.Left
                                + button.ContentEdgeInsets.Right);
                                constraint.SetIdentifier($"button_width_constraint_{button.Tag - 1000}");
                            }
                            else
                            {
                                constraint.Constant = button.TitleLabel.TextRectForBounds(new CGRect(0, 0, double.MaxValue, _TabHeight), 1).Width
                                + button.ContentEdgeInsets.Left
                                + button.ContentEdgeInsets.Right;
                            }
                            constraint.Active = true;
                            button.SetNeedsUpdateConstraints();
                        }
                        else
                        {
                            ((UIView)iter.Current).Hidden = false;
                        }
                    }
                }
            }
            else
            {
                var iter = buttonStack.ArrangedSubviews.GetEnumerator();
                using (iter as IDisposable)
                {
                    while (iter.MoveNext())
                    {
                        if (iter.Current is UIButton button)
                        {
                            if (button.Constraints.FirstOrDefault((c) => string.Equals(c.GetIdentifier(), $"button_width_constraint_{button.Tag - 1000}")) is NSLayoutConstraint constraint)
                            {
                                constraint.Active = false;
                                button.SetNeedsUpdateConstraints();
                            }
                        }
                        else
                        {
                            ((UIView)iter.Current).Hidden = distribution != UIStackViewDistribution.EqualSpacing;
                        }
                    }
                }
            }
            //TODO
        }

        private void SetupButtonStack()
        {
            Bounces = false;
            ShowsVerticalScrollIndicator = false;
            ShowsHorizontalScrollIndicator = false;
            ClipsToBounds = false;

            _SelectedTabColor = TintColor;
            highlighter.BackgroundColor = _SelectedTabColor.CGColor;
            Layer.AddSublayer(highlighter);

            Add(buttonStack);
            buttonStack.WidthAnchor.ConstraintGreaterThanOrEqualTo(WidthAnchor).Active = true;
            buttonStack.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            buttonStack.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 0).Active = true;
            buttonStack.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, 0).Active = true;
            tabHeightConstraint = buttonStack.HeightAnchor.ConstraintEqualTo(TabHeight);
            tabHeightConstraint.Active = true;
            tabBottomConstraint = buttonStack.BottomAnchor.ConstraintEqualTo(BottomAnchor);
            tabBottomConstraint.Active = true;

            //Add dummy left and right view for equal spacing
            buttonStack.AddArrangedSubview(CreateDummyEdgeView());
            buttonStack.AddArrangedSubview(CreateDummyEdgeView());

            buttonStack.SubviewsLayouted += OnTabsLayouted;
        }

        private async void OnTabsLayouted(object sender, EventArgs e)
        {
            if (delayHighlighterPosition)
            {
                delayHighlighterPosition = false;
                await Task.Delay(200);
            }
            UpdateHighlighterPosition(false);
        }

        UIView CreateDummyEdgeView()
        {
            var view = new UIView()
            {
                BackgroundColor = UIColor.Clear,
                TranslatesAutoresizingMaskIntoConstraints = false,
                Hidden = true
            };
            view.WidthAnchor.ConstraintEqualTo(0).Active = true;
            return view;
        }

        private void UpdateTabs()
        {
            ClearButtonStack();
            if (null == _Titles || _Titles.Length == 0) return;
            for (var i = 0; i < _Titles.Length; i++)
            {
                var button = new UIButton()
                {
                    Tag = i + 1000,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                button.AddTarget(OnTabButtonTapped, UIControlEvent.TouchUpInside);
                button.SetTitleColor(_SelectedTabColor, UIControlState.Disabled);
                button.SetTitleColor(_UnselectedTabColor, UIControlState.Normal);
                button.BackgroundColor = UIColor.Clear;
                button.TitleLabel.Lines = 0;
                button.TitleLabel.Font = _TitleFont;
                button.SetTitle(_Titles[i], UIControlState.Normal);
                //button.AccessibilityElementsHidden = false;
                button.ContentEdgeInsets = new UIEdgeInsets(0, 8, 0, 8);
                buttonStack.InsertArrangedSubview(button, (nuint)i + 1);
                if (buttonStack.Distribution == UIStackViewDistribution.FillProportionally)
                {
                    var constraint = button.WidthAnchor.ConstraintGreaterThanOrEqualTo(button.TitleLabel.TextRectForBounds(new CGRect(0, 0, double.MaxValue, _TabHeight), 1).Width
                        + button.ContentEdgeInsets.Left
                        + button.ContentEdgeInsets.Right);
                    constraint.Active = true;
                    constraint.SetIdentifier($"button_width_constraint_{i}");
                }
                button.Enabled = i != SelectedIndex;
            }
        }

        private void OnTabButtonTapped(object sender, EventArgs e)
        {
            if (sender is UIButton button)
            {
                SelectedIndex = button.Tag - 1000;
                TabSelected?.Invoke(this, EventArgs.Empty);
            }
        }

        void ClearButtonStack(bool all = false)
        {
            var views = buttonStack.ArrangedSubviews;
            if (false == all)
            {
                views = views.Where((v) => v is UIButton).ToArray();
            }
            foreach (UIView v in views)
            {
                buttonStack.RemoveArrangedSubview(v);
                if (v is UIButton button)
                {
                    button.RemoveTarget(OnTabButtonTapped, UIControlEvent.TouchUpInside);
                }
            }
        }

        void UpdateHighlighterPosition(bool animated)
        {
            var buttons = buttonStack.ArrangedSubviews;
            var index = _SelectedIndex;
            if (index >= 0 && index + 1 < buttons.Length)
            {
                var buttonFrame = buttons[index + 1].Frame;
                var newFrame = new CGRect(buttonFrame.X, buttonFrame.Height, buttonFrame.Width, _HighlighterHeight);
                if (false == _IsHighlighterFullWidth && ((UIButton)buttons[index + 1]).TitleLabel is UILabel label)
                {
                    var posX = label.Frame.X;
                    var parent = label.Superview;
                    while (parent != this)
                    {
                        posX += parent.Frame.X;
                        parent = parent.Superview;
                    }
                    newFrame.X = posX;
                    newFrame.Size = new CGSize(label.Frame.Size.Width, _HighlighterHeight);
                }
                if (animated)
                {
                    UIView.Animate(0.2, () =>
                    {
                        highlighter.Frame = newFrame;
                    });
                }
                else
                {
                    highlighter.Frame = newFrame;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                isDisposed = true;
                ClearButtonStack(true);
                buttonStack.SubviewsLayouted -= OnTabsLayouted;
                buttonStack.RemoveFromSuperview();
                buttonStack.Dispose();
                buttonStack = null;
            }
            base.Dispose(disposing);
        }
    }
}
