using System;
using MonoTouch.Dialog;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using ClanceysLib;
using MonoTouch.ObjCRuntime;

namespace MonoTouch.Dialog.PickerElement
{
	public class DateTimeElement2 : EntryElement {
		
		static NSString skey = new NSString ("PickerElement");
		static NSString skeyvalue = new NSString ("PickerElementValue");
		public UITextAlignment Alignment = UITextAlignment.Left;
		public UILabel entry;
		public UIColor SelectedBackgroundColor = UIColor.FromRGBA(0.02f, 0.55f, 0.96f, 1f);
		public UIColor SelectedTextColor = UIColor.White;
		public bool ShowDoneButton = false;
		
		private UITableViewCell cell = null;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		private bool doneButtonVisible = false;
		private bool pickerVisible = false;
		
		public event NSAction Tapped;
		
		float modifiedHeightOffset = 0;
		UIColor originalCellBackgroundColor = null;
		UIColor originalEntryBackgroundColor = null;
		UIColor originalCellTextColor = null;
		UIColor originalEntryTextColor = null;
		
		public DateTime DateValue;
		public UIDatePickerMode Mode { 
			get {
				return datePicker.Mode;
			}
			set{
				datePicker.Mode = value;
			}
		}
			
		public UIView ViewForPicker; 
		public DelegateDatePicker datePicker;
		public event Action<DateTimeElement2> DateSelected;
		public event EventHandler PickerClosed;
		public event EventHandler PickerShown;
				
		protected DialogViewController Dvc;
		private UIButton closeBtn;
		private UIBarButtonItem oldRightBtn;
		private UIBarButtonItem doneButton;
		private bool wiredStarted = false;			
		
		
		protected internal NSDateFormatter fmt = new NSDateFormatter () {
			DateStyle = NSDateFormatterStyle.Short
		};
				
		public DateTimeElement2 (string caption, DateTime date, DialogViewController dvc) : base (caption, null, null)
		{			
			this.Dvc = dvc;
			DateValue = date;
			
			// create picker elements
			datePicker = CreatePicker ();
			datePicker.Mode = UIDatePickerMode.DateAndTime; 
			datePicker.ValueChanged += delegate {
				DateValue = datePicker.Date;				
				Value = FormatDate(DateValue);
				RefreshValue();
				
				if (DateSelected != null)
					DateSelected (this);								
			};		
			
			//datePicker.Frame = PickerFrameWithSize (datePicker.SizeThatFits (SizeF.Empty));					
			closeBtn = new UIButton(new RectangleF(0,0,31,32));
			closeBtn.SetImage(UIImage.FromFile("Images/closebox.png"),UIControlState.Normal);
			closeBtn.TouchDown += delegate {
				HidePicker();
			};			
			datePicker.AddSubview(closeBtn);			
						
			Value = FormatDate (date);			
			
			this.Alignment = UITextAlignment.Left;
		}	
		
		public bool ShouldDeselect = true;
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{				
			ViewForPicker = ViewForPicker ?? tableView.Superview;
			
			if (Tapped != null)
				Tapped ();
			if(ShouldDeselect)
				tableView.DeselectRow (path, true);
						
			ShowPicker();			
		}
		
		public void LayoutSubviews ()
		{
			//base.LayoutSubviews ();
			var parentView = ViewForPicker;
			var parentH = parentView.Frame.Height;			
			datePicker.Frame = new RectangleF(0,parentH - datePicker.Frame.Height,parentView.Frame.Size.Width,datePicker.Frame.Height);
			closeBtn.Frame = closeBtn.Frame.SetLocation(new PointF(datePicker.Bounds.Width - 32,datePicker.Bounds.Y));
			datePicker.BringSubviewToFront(closeBtn);
		}
		
		public override void Deselected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			base.Deselected (dvc, tableView, path);
			HidePicker();
		}
		
		public void ShowPicker() {
			// get rid of keyboard if another element triggered it.
			Element root = Parent;
			while (root.Parent != null) {
				root = root.Parent;
			}	
			ResignFirstResponders((RootElement)root);
			
			// MonoTouch.Dialog CUSTOM: Download custom MonoTouch.Dialog from here to enable hiding picker when other element is selected:
			// https://github.com/crdeutsch/MonoTouch.Dialog
			//if (EntryStarted != null) {
			//	EntryStarted(this, null);
			//}
			
			if (Dvc != null && ShowDoneButton) {
				if(Dvc.NavigationItem.RightBarButtonItem != doneButton)
					oldRightBtn = Dvc.NavigationItem.RightBarButtonItem;
				if(doneButton == null)
					doneButton = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate{
						HidePicker();						
					});
				Dvc.NavigationItem.RightBarButtonItem = doneButton;
				doneButtonVisible = true;
			}
			if (!wiredStarted) {
				foreach(var sect in (root as RootElement)) {
					foreach(var e in sect.Elements) {
						var ee = e as EntryElement;
						if (ee != null && ee != this) {
							// MonoTouch.Dialog CUSTOM: Download custom MonoTouch.Dialog from here to enable hiding picker when other element is selected:
							// https://github.com/crdeutsch/MonoTouch.Dialog
							//((EntryElement)e).EntryStarted += delegate {
							//	HidePicker();
							//};
						}
					}
				}
				wiredStarted = true;
			}
			
			if(PickerShown != null)
				PickerShown(this,null);
			
			LayoutSubviews ();
			datePicker.BringSubviewToFront(closeBtn);
			var parentView = ViewForPicker;
			var parentFrame = parentView.Frame;
			
			datePicker.Frame = datePicker.Frame.SetLocation(new PointF(0,parentFrame.Height));
					
			UIView.BeginAnimations("slidePickerIn");			
			UIView.SetAnimationDuration(0.3);
			UIView.SetAnimationDelegate(datePicker);
			UIView.SetAnimationDidStopSelector (new Selector ("fadeInDidFinish"));
			//parentView.AddSubview(closeView);			
			parentView.AddSubview(datePicker);
						
			datePicker.Frame = datePicker.Frame.SetLocation(new PointF(0,parentFrame.Height - datePicker.Frame.Height));
			UIView.CommitAnimations();						
			
			if (originalCellBackgroundColor == null) {
				originalCellBackgroundColor = cell.BackgroundColor;
				cell.BackgroundColor = SelectedBackgroundColor;
				
				originalEntryBackgroundColor = entry.BackgroundColor;
				entry.BackgroundColor = SelectedBackgroundColor;
				
				originalCellTextColor = cell.TextLabel.TextColor;
				cell.TextLabel.TextColor = SelectedTextColor;
				
				originalEntryTextColor = entry.TextColor;
				entry.TextColor = SelectedTextColor;
			}
			
			pickerVisible = true;
		}
		
		public void HidePicker(bool Animated = true) {
			if(PickerClosed!=null)
				PickerClosed(this,null);
			
			var parentView = ViewForPicker;
			
			if (parentView != null) {
				var parentH = parentView.Frame.Height;
				
				if (Animated) {
					UIView.BeginAnimations("slidePickerOut");			
					UIView.SetAnimationDuration(0.3);
					UIView.SetAnimationDelegate(datePicker);			
					UIView.SetAnimationDidStopSelector (new Selector ("fadeOutDidFinish"));	
					datePicker.Frame = datePicker.Frame.SetLocation(new PointF(0,parentH));					
					UIView.CommitAnimations();
				}
				else {
					parentView.SendSubviewToBack(datePicker);
					datePicker.Frame = datePicker.Frame.SetLocation(new PointF(0,parentH));
				}
				//datePicker.RemoveFromSuperview();
				
				if (Dvc != null && doneButtonVisible) {
					Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;					
					doneButtonVisible = false;
				}
				
				// remove bg color
				if (originalCellBackgroundColor != null) {
					cell.BackgroundColor = originalCellBackgroundColor;
					originalCellBackgroundColor = null;
				
					entry.BackgroundColor = originalEntryBackgroundColor;
					originalEntryBackgroundColor = null;
					
					cell.TextLabel.TextColor = originalCellTextColor;
					originalCellTextColor = null;
				
					entry.TextColor = originalEntryTextColor;
					originalEntryTextColor = null;
				}
				
				if (Dvc != null && modifiedHeightOffset > 0) {
					// adjust size.
					var ff = Dvc.TableView.Frame;
					Dvc.TableView.Frame = new RectangleF(ff.X, ff.Y, ff.Width, ff.Height + modifiedHeightOffset);
					modifiedHeightOffset = 0f;
				}
			}
			
			// MonoTouch.Dialog CUSTOM: Download custom MonoTouch.Dialog from here to enable hiding picker when other element is selected:
			// https://github.com/crdeutsch/MonoTouch.Dialog
			//if (pickerVisible) {
			//	if (EntryEnded != null) {
			//		EntryEnded(this, null);
			//	}
			//}
			pickerVisible = false;
		}
		
		
		// 
		// Computes the X position for the entry by aligning all the entries in the Section
		//
		SizeF ComputeEntryPosition (UITableView tv, UITableViewCell cell)
		{
			Section s = Parent as Section;
			if (s.EntryAlignment.Width != 0)
				return s.EntryAlignment;
			
			// If all EntryElements have a null Caption, align UITextField with the Caption
			// offset of normal cells (at 10px).
			SizeF max = new SizeF (-15, tv.StringSize ("M", font).Height);
			foreach (var e in s.Elements){
				var ee = e as EntryElement;
				if (ee == null)
					continue;
				
				if (ee.Caption != null) {
					var size = tv.StringSize (ee.Caption, font);
					if (size.Width > max.Width)
						max = size;
				}
			}
			s.EntryAlignment = new SizeF (25 + Math.Min (max.Width, 160), max.Height);
			return s.EntryAlignment;
		}

		
		public override UITableViewCell GetCell (UITableView tv)
		{
			Value = FormatDate (DateValue);
			
			cell = tv.DequeueReusableCell (Value == null ? skey : skeyvalue);
			if (cell == null){
				cell = new UITableViewCell (UITableViewCellStyle.Value1, skey);
				cell.SelectionStyle = (Tapped != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
			} else 
				RemoveTag (cell, 1);
			
			if (originalEntryBackgroundColor != null) {
				// modify background color to stay consistant.
				cell.BackgroundColor = SelectedBackgroundColor;
				cell.TextLabel.TextColor = SelectedTextColor;
			}
			
			//cell.Accessory = UITableViewCellAccessory.None;
			//cell.TextLabel.TextAlignment = Alignment;
			
			if (entry == null){
				SizeF size = ComputeEntryPosition (tv, cell);
				var _entry = new UILabel (new RectangleF (size.Width, (cell.ContentView.Bounds.Height-size.Height)/2-1, cell.ContentView.Bounds.Width - size.Width - 10f, size.Height)){
					Tag = 1,
					//Placeholder = placeholder ?? "",
					Text = val,
					TextAlignment = Alignment,
					BackgroundColor = UIColor.Clear
				};
				_entry.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleLeftMargin;
				_entry.Text = Value ?? "";	
				entry = _entry;
				//entry.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
				//	UIViewAutoresizing.FlexibleLeftMargin;
				
			}
			
			cell.TextLabel.Text = Caption;
			cell.ContentView.AddSubview (entry);						
			return cell;
		}
		
		
		public void RefreshValue() {
			if (entry != null) {
				entry.Text = Value;
			}
		}
		
		public override string Summary ()
		{
			return Caption;
		}
		
		public override bool Matches (string text)
		{
			return (Value != null ? Value.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1: false) || base.Matches (text);
		}
		
		private void ResignFirstResponders(RootElement root) {
			foreach(var sect in root) {
				foreach(var e in sect.Elements) {
					var ee = e as EntryElement;
					if (ee != null && ee != this) {
						ee.ResignFirstResponder(false);
					}
				}
			}
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing){
				if (fmt != null){
					fmt.Dispose ();
					fmt = null;
				}
				if (datePicker != null){
					datePicker.Dispose ();
					datePicker = null;
				}
				if (closeBtn != null) {
					closeBtn.Dispose();
					closeBtn = null;
				}
			}
		}
		
		public virtual string FormatDate (DateTime dt)
		{
			switch (datePicker.Mode) {
				case UIDatePickerMode.Date:
					return fmt.ToString (dt);
				
				case UIDatePickerMode.Time:
					return dt.ToLocalTime ().ToShortTimeString ();
				
				default:
					return fmt.ToString (dt) + " " + dt.ToLocalTime ().ToShortTimeString ();
			}			
		}
		
		public virtual DelegateDatePicker CreatePicker ()
		{
			var picker = new DelegateDatePicker (this){
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				Mode = UIDatePickerMode.DateAndTime,
				Date = DateValue
			};
			return picker;
		}
		                                                                                                                                
		static RectangleF PickerFrameWithSize (SizeF size)
		{                                                                                                                                    
			var screenRect = UIScreen.MainScreen.ApplicationFrame;
			float fY = 0, fX = 0;
			
			switch (UIApplication.SharedApplication.StatusBarOrientation){
			case UIInterfaceOrientation.LandscapeLeft:
			case UIInterfaceOrientation.LandscapeRight:
				fX = (screenRect.Height - size.Width) /2;
				fY = (screenRect.Width - size.Height) / 2 -17;
				break;
				
			case UIInterfaceOrientation.Portrait:
			case UIInterfaceOrientation.PortraitUpsideDown:
				fX = (screenRect.Width - size.Width) / 2;
				fY = (screenRect.Height - size.Height) / 2 - 25;
				break;
			}
			
			return new RectangleF (fX, fY, size.Width, size.Height);
		}  
		
		
		[Export("fadeInDidFinish")]
		public void FadeInDidFinish ()
		{
			if (Dvc != null && modifiedHeightOffset == 0f) {
				// adjust size.
				var ff = Dvc.TableView.Frame;
				modifiedHeightOffset = 200f;
				Dvc.TableView.Frame = new RectangleF(ff.X, ff.Y, ff.Width, ff.Height - modifiedHeightOffset);
				Dvc.TableView.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, true);
			}
		}		
		
		[Export("fadeOutDidFinish")]
		public void FadeOutDidFinish ()
		{
			var parentView = ViewForPicker;
			parentView.SendSubviewToBack(datePicker);
		}
		
		// MonoTouch.Dialog CUSTOM: Download custom MonoTouch.Dialog from here to enable support for "next" button being clicked.
		// https://github.com/crdeutsch/MonoTouch.Dialog
		/*
		bool becomeResponder;		
		/// <summary>
		/// Makes this cell the first responder (get the focus)
		/// </summary>
		/// <param name="animated">
		/// Whether scrolling to the location of this cell should be animated
		/// </param>
		public override void BecomeFirstResponder (bool animated)
		{
			becomeResponder = true;
			var tv = GetContainerTableView ();
			if (tv == null)
				return;
			tv.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, animated);
			if (entry != null){
				ShowPicker();
				becomeResponder = false;
			}
		}
		public override void ResignFirstResponder (bool animated)
		{
			becomeResponder = false;
			var tv = GetContainerTableView ();
			if (tv == null)
				return;
			tv.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, animated);
			if (entry != null)
				HidePicker();
		}
		*/	
		
		public class DelegateDatePicker : UIDatePicker {
			
			DateTimeElement2 element;
			
			public DelegateDatePicker(DateTimeElement2 Element): base(RectangleF.Empty) {
				element = Element;
			}
			
			[Export("fadeInDidFinish")]
			public void FadeInDidFinish ()
			{
				if (element.Dvc != null && element.modifiedHeightOffset == 0f) {
					// adjust size.
					var ff = element.Dvc.TableView.Frame;
					element.modifiedHeightOffset = 200f;
					element.Dvc.TableView.Frame = new RectangleF(ff.X, ff.Y, ff.Width, ff.Height - element.modifiedHeightOffset);
					element.Dvc.TableView.ScrollToRow (element.IndexPath, UITableViewScrollPosition.Middle, true);
				}
			}		
			
			[Export("fadeOutDidFinish")]
			public void FadeOutDidFinish ()
			{
				var parentView = element.ViewForPicker;
				parentView.SendSubviewToBack(element.datePicker);
			}
		}

		
	}
	
}

