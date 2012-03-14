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
				if (Dvc != null) {
					Dvc.NavigationItem.RightBarButtonItem = oldRightBtn;
				}
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
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			Element root = Parent;
			while (root.Parent != null) {
				root = root.Parent;
			}
			
			// get rid of keyboard if another element triggered it.
			ResignFirstResponders((RootElement)root);
			
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
				foreach(var sect in (root as RootElement)) {
					foreach(var e in sect.Elements) {
						var ee = e as EntryElement;
						if (ee != null) {
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
		
		public void HidePicker() {
			ComboBox.HidePicker();
		}
		
		private void ResignFirstResponders(RootElement root) {
			foreach(var sect in root) {
				foreach(var e in sect.Elements) {
					var ee = e as EntryElement;
					if (ee != null) {
						ee.ResignFirstResponder(false);
					}
					var dte = e as DateTimeElement2;
					if (dte != null) {
						dte.HidePicker();
					}
					var pe = e as PickerElement;
					if (pe != null && pe != this) {
						pe.HidePicker();
					}
				}
			}
		}
	}
	
	
	
	

}

