using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MonoTouch.Dialog.PickerElement;
using MonoTouch.Foundation;

namespace Sample
{

	public partial class ChickenDialogViewController : DialogViewController
	{
		EntryElement chickenName = null;
		PickerElement rating = null;
		
		public ChickenDialogViewController () : base (null)
		{	
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear(animated);
			
			var items = new List<object>();
		
			for(int i = 0;i <= 10;i++)
			{
				items.Add (i);
			}
			chickenName = new EntryElement("Name", null, "");
			rating = new PickerElement("Rating", items.ToArray(), null) {
				Width = 40f
			};
			// set initial rating.
			rating.Value = "5";
			rating.SetSelectedValue(rating.Value);
			
			Root = new RootElement("Rate Chicken") {
				new Section() {
					chickenName,
					rating
				}
			};		
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		
			chickenName.FetchValue();
			//rating.FetchValue();
			
			// TODO: save changes.
			System.Diagnostics.Debug.WriteLine(string.Format("{0} is rated a {1}", chickenName.Value.Trim(), rating.Value.Trim()));
		}
						
		public class PickerLabel : UILabel {
			public PickerLabel(RectangleF rect) : base(rect) {
			}
			
			public override string ToString ()
			{
				return Text;
			}
		}
	}
}

