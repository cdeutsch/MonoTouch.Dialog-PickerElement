// 
//  Copyright 2011  James Clancey
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
	public class PickerElement: ReadOnlyStringElement
	{
		NSString key =new NSString( "UIComboBoxElement");
		protected UIComboBox ComboBox;
		
		public PickerElement (string caption, object[] Items , string DisplayMember) : base (caption) 
		{
			this.ComboBox = new UIComboBox(RectangleF.Empty);
			this.ComboBox.Items = Items;
			this.ComboBox.DisplayMember = DisplayMember;
			this.ComboBox.TextAlignment = UITextAlignment.Right;
			this.ComboBox.BorderStyle = UITextBorderStyle.None;
			this.ComboBox.PickerClosed += delegate {
				Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
			};
			this.ComboBox.ValueChanged += delegate {
				Value = ComboBox.Text;
			};
			Value = ComboBox.Text;
			this.TextAlignment = UITextAlignment.Center;
		}
		
		public void SetSelectedValue(string Value) {
			ComboBox.Text = Value;
			ComboBox.SetSelectedValue(Value);
		}
		public void SetSelectedIndex(int index) {
			ComboBox.SetSelectedIndex(index);
		}
		
		private DialogViewController Dvc;
		private UIBarButtonItem oldRightBtn;
		private UIBarButtonItem doneButton;
		private bool wiredStarted = false;
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			Dvc = dvc;
			base.Selected (dvc, tableView, path);
			ComboBox.ShowPicker();
			if(dvc.NavigationItem.RightBarButtonItem != doneButton)
				oldRightBtn = dvc.NavigationItem.RightBarButtonItem;
			if(doneButton == null)
				doneButton = new UIBarButtonItem("Done",UIBarButtonItemStyle.Bordered, delegate{
					ComboBox.HidePicker();	
					dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
				});
			dvc.NavigationItem.RightBarButtonItem = doneButton;
			if (!wiredStarted) {
				foreach (var e in (Parent as Section).Elements){
					var ee = e as EntryElement;
					if (ee != null) {
						// hide picker when other element is selected. Need to get "Started" is MonoTouch.Dialog core.
						((EntryElement)e).EntryStarted += delegate {
							ComboBox.HidePicker();
							//dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
						};
						ee.ResignFirstResponder(false);
					}
				}
				wiredStarted = true;
			}
			// get rid of keyboard if another element triggered it.
			foreach (var e in (Parent as Section).Elements){
				var ee = e as EntryElement;
				if (ee != null) {
					ee.ResignFirstResponder(false);
				}
			}
		}
		public override void Deselected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			Dvc = dvc;
			base.Deselected (dvc, tableView, path);
			ComboBox.HidePicker();
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			//ComboBox.ViewForPicker = dvc.View.Superview;
			ComboBox.ViewForPicker = tv.Superview;
			return base.GetCell (tv);
		}
		
	}
	
	
	
	

}

