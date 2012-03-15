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
		public UITextAlignment Alignment = UITextAlignment.Left;
		public UILabel entry;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		
		public event NSAction Tapped;
		
		public PickerElement (string caption, object[] Items , string DisplayMember, DialogViewController dvc) : base (caption, null, null) 
		{
			this.Dvc = dvc;
			this.ComboBox = new UIComboBox(RectangleF.Empty);
			this.ComboBox.Items = Items;
			this.ComboBox.DisplayMember = DisplayMember;
			this.ComboBox.TextAlignment = UITextAlignment.Right;
			this.ComboBox.BorderStyle = UITextBorderStyle.None;
			this.ComboBox.PickerClosed += delegate {
				if (Dvc != null) {
					Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
				}
			};
			this.ComboBox.ValueChanged += delegate {
				Value = ComboBox.Text;
				RefreshValue();
			};
			Value = ComboBox.Text;
			this.Alignment = UITextAlignment.Center;
		}
		
		public void SetSelectedValue(string Value) {
			ComboBox.Text = Value;
			ComboBox.SetSelectedValue(Value);
		}
		public void SetSelectedIndex(int index) {
			ComboBox.SetSelectedIndex(index);
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
							ee.ResignFirstResponder(false);
						}
					}
				}
				wiredStarted = true;
			}
			
			ComboBox.ShowPicker();
			if (Dvc != null) {
				if(Dvc.NavigationItem.RightBarButtonItem != doneButton)
					oldRightBtn = Dvc.NavigationItem.RightBarButtonItem;
				if(doneButton == null)
					doneButton = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate{
						ComboBox.HidePicker();	
						Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
					});
				Dvc.NavigationItem.RightBarButtonItem = doneButton;
			}
			
		}
		
		public void HidePicker() {
			ComboBox.HidePicker();
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
			var cell = tv.DequeueReusableCell (Value == null ? skey : skeyvalue);
			if (cell == null){
				cell = new UITableViewCell (UITableViewCellStyle.Value1, skey);
				cell.SelectionStyle = (Tapped != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
			} else 
				RemoveTag (cell, 1);
			
			//cell.Accessory = UITableViewCellAccessory.None;
			//cell.TextLabel.TextAlignment = Alignment;
			
			if (entry == null){												
				SizeF size = ComputeEntryPosition (tv, cell);
				var _entry = new UILabel (new RectangleF (size.Width-8, (cell.ContentView.Bounds.Height-size.Height)/2-1, 320-size.Width - 27, size.Height)){
					Tag = 1,
					//Placeholder = placeholder ?? "",
					Text = val,
					TextAlignment = Alignment,
				};
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

