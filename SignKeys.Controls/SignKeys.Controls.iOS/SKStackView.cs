using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace SignKeys.Controls.iOS
{
    public class SKStackView : UIStackView
    {
        public EventHandler<EventArgs> SubviewsLayouted;

        public SKStackView()
        {
        }

        public SKStackView(NSCoder coder) : base(coder)
        {
        }

        public SKStackView(CGRect frame) : base(frame)
        {
        }

        public SKStackView(UIView[] views) : base(views)
        {
        }

        protected SKStackView(NSObjectFlag t) : base(t)
        {
        }

        protected internal SKStackView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            SubviewsLayouted?.Invoke(this, EventArgs.Empty);
        }
    }
}
