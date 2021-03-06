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
		DateTimeElement2 date = null;
		
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
			chickenName = new EntryElement("Name Of", null, "");
			chickenName.Value = "Big Bird";
			
			rating = new PickerElement("Rating", items.ToArray(), null, this) {
				Width = 40f,
				ValueWidth = 202f, // use this to align picker value with other elements, for the life of me I can't find a calculation that automatically does it.
				Alignment = UITextAlignment.Left
			};			
			// set initial rating.
			rating.Value = "5";
			rating.SetSelectedValue(rating.Value);
			
			date = new DateTimeElement2("Date", DateTime.Now, this) {
				Alignment = UITextAlignment.Left,
				Mode = UIDatePickerMode.Date
			};
			
			Root = new RootElement("Rate Chicken") {
				new Section() {
					chickenName,
					rating,
					date
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

