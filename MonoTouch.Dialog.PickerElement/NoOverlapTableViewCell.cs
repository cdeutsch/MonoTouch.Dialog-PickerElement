using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace MonoTouch.Dialog.PickerElement
{
	public class NoOverlapTableViewCell : UITableViewCell {
		
		public float? DetailTextLabelWidth = 0;  //this is here because it seems to be god damn impossible to calculate the size of the other elements when they use AutoresizingMask
		public SizeF MaxEntryPosition;
		public RectangleF ContentViewBounds;
		
		public NoOverlapTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) {
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			float indicatorWidth = 0;
			if (this.Accessory == UITableViewCellAccessory.DetailDisclosureButton
			    || this.Accessory == UITableViewCellAccessory.Checkmark) {
				indicatorWidth = 26f;
			}
			else if (this.Accessory == UITableViewCellAccessory.DisclosureIndicator) {
				indicatorWidth = 16f;
			}
						
			if (DetailTextLabel != null) {
				float yOffset = (ContentViewBounds.Height - MaxEntryPosition.Height) / 2 - 1;
				float adjust = (ContentViewBounds.Width - this.ContentView.Bounds.Width);
				float width = ContentViewBounds.Width - MaxEntryPosition.Width - indicatorWidth - adjust - 10f;
				var x = MaxEntryPosition.Width;
				if (DetailTextLabelWidth.HasValue) {
					width = DetailTextLabelWidth.Value;
					x = ContentViewBounds.Width - 30f - indicatorWidth - DetailTextLabelWidth.Value;
					TextLabel.Frame = new System.Drawing.RectangleF(10f, yOffset, x, MaxEntryPosition.Height);
				}
				DetailTextLabel.Frame = new System.Drawing.RectangleF(x, yOffset, width, MaxEntryPosition.Height);
			}
		
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
		}
	}
}

