using System;
using MonoTouch.UIKit;

namespace MonoTouch.Dialog.PickerElement
{
	public class NoOverlapTableViewCell : UITableViewCell {
		
		public float DetailTextLabelWidth = 0;
		
		public NoOverlapTableViewCell(UITableViewCellStyle style, string reuseIdentifier, float DetailTextLabelWidth) : base(style, reuseIdentifier) {
			this.DetailTextLabelWidth = DetailTextLabelWidth;
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			
			float indicatorWidth = 0;
			if (this.Accessory != UITableViewCellAccessory.None) {
				indicatorWidth = 16f;
			}
			
			var curBounds = TextLabel.Bounds;
			TextLabel.Frame = new System.Drawing.RectangleF(10f, 0, 320f - 40f - indicatorWidth - DetailTextLabelWidth, 43f);
			
			if (DetailTextLabel != null) {
				var curBounds2 = DetailTextLabel.Bounds;
				DetailTextLabel.Frame = new System.Drawing.RectangleF(320f - 30f - indicatorWidth - DetailTextLabelWidth, 10f, DetailTextLabelWidth, 21f);
			}
			
		}
	}
}

