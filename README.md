aerocolor-wpf
=============

This library adds a few keys to the primary resources file of your WPF based application to allow using the Aero color, and updating that color when it changes in the settings too.

#Resource keys added
The following resource keys are added and maintained once the Aero Color library has been initialized.

##Colors
The following color resources are added.
* AeroColor - This is the exact Aero color as-is.
* AeroColorDark - This is the exact Aero color, only made darker if too light (good for white backgrounds).
* AeroColorLight - This is the exact Aero color, only made lighter if too dark (good for black bacgrounds).

##Brushes
The following brush resources are added, which are just solid colored brushes with their color counterparts as colors.
* AeroBrush
* AeroBrushDark
* AeroBrushLight
