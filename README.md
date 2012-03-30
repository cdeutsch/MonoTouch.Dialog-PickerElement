# MonoTouch.Dialog PickerElement #

This is a generic UIPicker and a DateTime Picker for MonoTouch.Dialog. 

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

* Added a DateTime Picker that functions consitantly with the generic Picker. 
* Highlights the row you are showing the picker for and scrolls it into focus to avoid the picker hiding the row.
* Updated it to hide the keyboard or picker when selecting different cells to edit.
* Actually has a sample of how to use it. ;)
* Independent DisplayMember and ValueMember


## History ##

### 3/29/2012 ###

* Added support for setting label TextColor for PickerElement

### 3/26/2012 ###

* Added support for hiding PickerElement without animation.

### 3/25/2012 ###

* Added support for hiding DatePicker without animation. Useful when view is disappearing.

### 3/23/2012 ###

* Added ValueChanged Event to PickerElement.

### 3/21/2012 ###

* Improved the ability to control the size of the Value in the table cell versus the caption.

### 3/18/2012 ###

* Added flag to control whether done button is show or not.

### 3/16/2012 ###

* Added feature that highlights the row the picker is being shown for and also scrolls the row into view rather then potentially covering it up.

### 3/14/2012 ###

* Created DateTimeElement2 that creates a UIDatePicker on the same view just like the PickerElement, instead of pushing a new view like the regular MonoTouch.Dialog DateTimeElement.

* Refactored PickerElement and DateTimeElement2 to inherit from EntryElement to fix issue with labels lining up, and deleted ReadOnlyStringElement.cs

* Worked on adding support for when "Next" key is pressed on regular EntryElement. Unfortunately this will require another MonoTouch.Dialog change to work. 

* Added SelectedItem property to PickerElement so you can retrieve the selected object instead of just the value.

### 3/6/2012 ###

* Removed support for using a UIView for the rows since the picker often loses the rows when there are multiple pickers.
* Added support for setting DisplayMember and ValueMember independently.
* Added support for setting the Width of the Picker.
