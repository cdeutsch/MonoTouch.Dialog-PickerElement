# MonoTouch.Dialog PickerElement #

This is a UIPicker for MonoTouch.Dialog

It's heavily based on ClanceyLib:
https://github.com/Clancey/ClanceyLib

## Requirements ##

* crdeutsch version of MonoTouch.Dialog:
https://github.com/crdeutsch/MonoTouch.Dialog

* Alternatively find the line in PickerElement under "// MonoTouch.Dialog CUSTOM:" and comment it out and it will build using the built-in MonoTouch.Dialog library. Note that the Picker will not hide if you select a different cell to edit.

## Differences from Clancey version ##

* Updated to hide the keyboard and picker when selecting different cells.
* Supports UIView's as the Items you pass to the Picker for greater customization.
* Actually has a sample of how to use it. ;)