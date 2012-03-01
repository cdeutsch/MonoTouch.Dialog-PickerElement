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
namespace MonoTouch.Dialog.PickerElement
{
	public class ReadOnlyStringElement: Element
	{
		static NSString skey = new NSString ("ReadOnlyStringElement");
		static NSString skeyvalue = new NSString ("ReadOnlyStringElementValue");
		public UITextAlignment Alignment = UITextAlignment.Left;
		public UILabel entry;
		static UIFont font = UIFont.BoldSystemFontOfSize (17);
		
		public string Value { 
			get {
				return val;
			}
			set {
				val = value;
				if (entry != null)
					entry.Text = value;
			}
		}
		string val;
		
		public UITextAlignment TextAlignment {
			get{return alignment;}
			set{
				alignment = value;
				if (entry != null)
					entry.TextAlignment = value;
			}
		}	
		UITextAlignment alignment;
		
		public ReadOnlyStringElement (string caption) : base (caption) {}
		
		public ReadOnlyStringElement (string caption, string value) : base (caption)
		{
			this.Value = value;
		}
		
		public ReadOnlyStringElement (string caption,  NSAction tapped) : base (caption)
		{
			Tapped += tapped;
		}
		
		public ReadOnlyStringElement (string caption,string value,  NSAction tapped) : base (caption)
		{
			this.Value = value;
			Tapped += tapped;
		}
		
		public event NSAction Tapped;
				
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
				var ee2 = e as ReadOnlyStringElement;
				if (ee2 != null) {
					var size = tv.StringSize (ee2.Caption, font);
					if (size.Width > max.Width)
						max = size;				
				}
			}
			s.EntryAlignment = new SizeF (25 + Math.Min (max.Width, 160), max.Height);
			return s.EntryAlignment;
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
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
				var _entry = new UILabel (new RectangleF (size.Width, (cell.ContentView.Bounds.Height-size.Height)/2-1, 320-size.Width - 15, size.Height)){
					Tag = 1,
					//Placeholder = placeholder ?? "",
					Text = val,
					TextAlignment = alignment,
				};
				_entry.Text = Value ?? "";	
				entry = _entry;
				entry.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
					UIViewAutoresizing.FlexibleLeftMargin;
				
			}
			
			cell.TextLabel.Text = Caption;
			cell.ContentView.AddSubview (entry);						
			return cell;
		}

		public override string Summary ()
		{
			return Caption;
		}
		public bool ShouldDeselect = true;
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Tapped != null)
				Tapped ();
			if(ShouldDeselect)
				tableView.DeselectRow (indexPath, true);
		}
		
		public override bool Matches (string text)
		{
			return (Value != null ? Value.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1: false) || base.Matches (text);
		}
	}
	
	
	
	

}

