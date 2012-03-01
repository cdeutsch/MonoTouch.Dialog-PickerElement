# MonoTouch.Dialog PickerElement #

This is a UIPicker for MonoTouch.Dialog

It's heavily based on ClanceyLib:
https://github.com/Clancey/ClanceyLib

## Requirements ##

* MonoTouch.Dialog (now part of MonoTouch):
http://docs.xamarin.com/ios/tutorials/MonoTouch.Dialog

## Requirements for dismissing Picker when another cell is selected ##

* crdeutsch version of MonoTouch.Dialog:
https://github.com/crdeutsch/MonoTouch.Dialog

* Find the line in PickerElement under "// MonoTouch.Dialog CUSTOM:" and comment in. Now the Picker will hide if you select a different cell to edit.

## Differences from Clancey version ##

* Updated it to hide the keyboard or picker when selecting different cells to edit.
* Changed so the Items in the Picker list are UIView's for greater customization.
* Actually has a sample of how to use it. ;)