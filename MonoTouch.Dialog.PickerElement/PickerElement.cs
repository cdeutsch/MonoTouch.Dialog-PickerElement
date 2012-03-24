// 
//  Copyright 2012  Christopher Deutsch
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using MonoTouch.Dialog;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using ClanceysLib;
namespace MonoTouch.Dialog.PickerElement
{
	public class PickerElement: EntryElement
	{
		protected UIComboBox ComboBox;
				
		static NSString skey = new NSString ("PickerElement");
		static NSString skeyvalue = new NSString ("PickerElementValue");
		public UITextAlignment Alignment = UITextAlignment.Center;
		public UILabel entry;
		public UIColor SelectedBackgroundColor = UIColor.FromRGBA(0.02f, 0.55f, 0.96f, 1f);
		public UIColor SelectedTextColor = UIColor.White;
		public bool ShowDoneButton = false;
		public float ValueWidth = 20f;
		public UIColor ValueTextColor = UIColor.Black;
		
		private UITableViewCell cell = null;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		private bool doneButtonVisible = false;
		private bool pickerVisible = false;
		
		public EventHandler ValueChanged {get;set;}
		public event NSAction Tapped;
		
		float modifiedHeightOffset = 0;
		UIColor originalCellBackgroundColor = null;
		UIColor originalEntryBackgroundColor = null;
		UIColor originalCellTextColor = null;
		UIColor originalEntryTextColor = null;
		
		public PickerElement (string caption, object[] Items , string DisplayMember, DialogViewController dvc) : base (caption, null, null) 
		{
			this.Dvc = dvc;
			this.ComboBox = new UIComboBox(RectangleF.Empty);
			this.ComboBox.Items = Items;
			this.ComboBox.DisplayMember = DisplayMember;
			this.ComboBox.TextAlignment = UITextAlignment.Right;
			this.ComboBox.BorderStyle = UITextBorderStyle.None;
			this.ComboBox.PickerClosed += delegate {
				if (Dvc != null && doneButtonVisible) {
					Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;					
					doneButtonVisible = false;
				}
				RestoreTableView();
			};
			this.ComboBox.ValueChanged += delegate {
				Value = ComboBox.Text;
				RefreshValue();
				if (ValueChanged != null) {
					ValueChanged(this, null);
				}
			};
			this.ComboBox.PickerFadeInDidFinish += delegate {
				if (modifiedHeightOffset == 0f) {
					// adjust size.
					var ff = Dvc.TableView.Frame;
					modifiedHeightOffset = 200f;
					Dvc.TableView.Frame = new RectangleF(ff.X, ff.Y, ff.Width, ff.Height - modifiedHeightOffset);
					Dvc.TableView.ScrollToRow (IndexPath, UITableViewScrollPosition.Middle, true);
				}
			};
			Value = ComboBox.Text;
		}
		
		public void SetSelectedValue(string Value) {
			ComboBox.Text = Value;
			ComboBox.SetSelectedValue(Value);
		}
		public void SetSelectedIndex(int index) {
			ComboBox.SetSelectedIndex(index);
		}
		
		public Object SelectedItem {
			get {
				return ComboBox.SelectedItem;
			}
		}
		
		/// <summary>
		/// Can be a collection of anyting. If you don't set the ValueMember or DisplayMember, it will use ToString() for the value and Title.
		/// </summary>
		public object [] Items {
			get{return ComboBox.Items;}
			set{ComboBox.Items = value;}
		}
		
		public string DisplayMember {
			get{return ComboBox.DisplayMember;}
			set {ComboBox.DisplayMember = value;}
		}
		public string ValueMember {
			get{return ComboBox.ValueMember;}
			set {ComboBox.ValueMember = value;}
		}
		public float Width {
			get{return ComboBox.Width;}
			set {ComboBox.Width = value;}
		}
		
		private DialogViewController Dvc;
		private UIBarButtonItem oldRightBtn;
		private UIBarButtonItem doneButton;
		private bool wiredStarted = false;
		
		public bool ShouldDeselect = true;		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{	
			if (Tapped != null)
				Tapped ();
			if(ShouldDeselect)
				tableView.DeselectRow (path, true);
		
			ShowPicker();					
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
			
			// wire up ability to hide picker when other elements are selected.
			if (!wiredStarted) {
				foreach(var sect in (root as RootElement)) {
					foreach(var e in sect.Elements) {
						var ee = e as EntryElement;
						if (ee != null && ee != this) {
							// MonoTouch.Dialog CUSTOM: Download custom MonoTouch.Dialog from here to enable hiding picker when other element is selected:
							// https://github.com/crdeutsch/MonoTouch.Dialog
							//((EntryElement)e).EntryStarted += delegate {
							//	ComboBox.HidePicker();
							//};
						}
					}
				}
				wiredStarted = true;
			}
			
			ComboBox.ShowPicker();
			if (Dvc != null && ShowDoneButton) {
				if(Dvc.NavigationItem.RightBarButtonItem != doneButton)
					oldRightBtn = Dvc.NavigationItem.RightBarButtonItem;
				if(doneButton == null)
					doneButton = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate{
						ComboBox.HidePicker();	
						Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
					});
				Dvc.NavigationItem.RightBarButtonItem = doneButton;
				doneButtonVisible = true;
			}
			
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
		
		public void HidePicker() {
			ComboBox.HidePicker();
			
			RestoreTableView();
		}
		
		private void RestoreTableView() {
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
			
			if (modifiedHeightOffset > 0) {
				// adjust size.
				var ff = Dvc.TableView.Frame;
				Dvc.TableView.Frame = new RectangleF(ff.X, ff.Y, ff.Width, ff.Height + modifiedHeightOffset);
				modifiedHeightOffset = 0f;
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
			SizeF max = new SizeF (-1, -1);
			foreach (var e in s.Elements){
				var ee = e as EntryElement;
				if (ee != null) {
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
			ComboBox.ViewForPicker = tv.Superview;
			cell = tv.DequeueReusableCell (Value == null ? skey : skeyvalue);
			if (cell == null){
				cell = new NoOverlapTableViewCell (UITableViewCellStyle.Value1, skey, ValueWidth);
				cell.SelectionStyle = (Tapped != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
			} else 
				RemoveTag (cell, 1);
			
			entry = cell.DetailTextLabel;
			entry.Text = Value ?? "";	
			entry.Tag = 1;
			entry.TextAlignment = Alignment;
			entry.BackgroundColor = UIColor.Clear;
			entry.TextColor = ValueTextColor;
			
			if (originalEntryBackgroundColor != null) {
				// modify background color to stay consistant.
				cell.BackgroundColor = SelectedBackgroundColor;
				cell.TextLabel.TextColor = SelectedTextColor;
				entry.BackgroundColor = SelectedBackgroundColor;
				entry.TextColor = SelectedTextColor;
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
		
	}
	
	
	
	

}

