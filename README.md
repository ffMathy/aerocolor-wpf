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

#Setup code
This line of code should be put inside your main window's `Loaded` event, or any other event that arises when the first window of your application has been created. Here's an example.

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AeroResourceInitializer.Initialize();
        }
    }

#NuGet
AeroColor is available on NuGet as well!

`install-package aerocolor-wpf`
