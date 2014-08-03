using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using AeroColor.Exceptions;

namespace AeroColor
{
    public static class AeroResourceInitializer
    {
        private static Application Application
        {
            get { return Application.Current; }
        }

        private static Window Window
        {
            get { return _window; }
            set
            {
                if (value == null) throw new ArgumentNullException();

                if (_window != null)
                {
                    var existingHook = (HwndSource)PresentationSource.FromVisual(_window);
                    if (existingHook != null)
                    {
                        existingHook.RemoveHook(OnWindowMessage);
                    }
                }

                _window = value;

                //install message hook on main window.
                var newHook = (HwndSource)PresentationSource.FromVisual(_window);
                if (newHook == null)
                {
                    throw new AeroMessageHookInstallException();
                }

                newHook.AddHook(OnWindowMessage);

                _window.Closed += WindowClosed;
            }
        }

        private static Window _window;

        public static void Initialize()
        {
            SetNewWindowForHook();

            var resources = Application.Resources;
            resources["AeroColor"] = GetCurrentAeroColor();

            UpdateResourceDependencies();
        }

        private static void UpdateResourceDependencies()
        {
            var resources = Application.Resources;

            //first work on colors.
            var aeroColor = (Color) resources["AeroColor"];

            const byte threshold = 50;

            var aeroColorLight = aeroColor;
            if (aeroColorLight.R < threshold && aeroColorLight.G < threshold && aeroColorLight.B < threshold)
            {
                var lowestChannelDifference = threshold - Math.Max(Math.Max(aeroColorLight.R, aeroColorLight.G), aeroColorLight.B);
                aeroColorLight.R += (byte)lowestChannelDifference;
                aeroColorLight.G += (byte)lowestChannelDifference;
                aeroColorLight.B += (byte)lowestChannelDifference;
            }

            var aeroColorDark = aeroColor;
            if (aeroColorDark.R > 255 - threshold && aeroColorDark.G > 255 - threshold && aeroColorDark.B > 255 - threshold)
            {
                var lowestChannelDifference = Math.Max(Math.Max(aeroColorDark.R, aeroColorDark.G), aeroColorDark.B) - (255 - threshold);
                aeroColorDark.R -= (byte)lowestChannelDifference;
                aeroColorDark.G -= (byte)lowestChannelDifference;
                aeroColorDark.B -= (byte)lowestChannelDifference;
            }

            resources["AeroColorLight"] = aeroColorLight;
            resources["AeroColorDark"] = aeroColorDark;

            //set brushes.
            resources["AeroBrushLight"] = new SolidColorBrush(aeroColorLight);
            resources["AeroBrush"] = new SolidColorBrush(aeroColor);
            resources["AeroBrushDark"] = new SolidColorBrush(aeroColorDark);
        }

        static void WindowClosed(object sender, EventArgs e)
        {
            SetNewWindowForHook();
        }

        private static void SetNewWindowForHook()
        {
            foreach (Window window in Application.Windows)
            {
                Window = window;
                return;
            }

            Window = Application.MainWindow;
        }

        private static IntPtr OnWindowMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == NativeMethods.WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                OnAeroColorChanged(wparam.ToInt64());
            }

            return IntPtr.Zero;
        }

        private static void OnAeroColorChanged(long newColor)
        {
            var resources = Application.Resources;
            resources["AeroColor"] = GetAeroColorFromNumeric((uint)newColor);

            UpdateResourceDependencies();
        }

        private static Color GetCurrentAeroColor()
        {
            bool alphaBlend;
            uint newColor;
            NativeMethods.DwmGetColorizationColor(out newColor, out alphaBlend);

            return GetAeroColorFromNumeric(newColor);
        }

        private static Color GetAeroColorFromNumeric(uint color = 0)
        {
            try
            {

                var convertedColor = Color.FromArgb((byte)((color >> 24) & 0xFF),
                                                      (byte)((color >> 16) & 0xFF),
                                                      (byte)((color >> 8) & 0xFF),
                                                      (byte)(color & 0xFF));

                var colorChannels = new[] { convertedColor.R, convertedColor.G, convertedColor.B };
                convertedColor.A = 255;

                if ((convertedColor.R < 30 && convertedColor.B < 30 && convertedColor.G < 30) ||
                    (convertedColor.R > 225 && convertedColor.B > 225 && convertedColor.G > 225))
                {
                    //boring color. set it to default Windows 8 color theme
                    convertedColor.R = 37;
                    convertedColor.G = 97;
                    convertedColor.B = 163;
                }
                else
                {
                    convertedColor.R = colorChannels[0];
                    convertedColor.G = colorChannels[1];
                    convertedColor.B = colorChannels[2];
                }

                return convertedColor;
            }
            catch (Exception ex)
            {
                if (ex is DllNotFoundException || ex is EntryPointNotFoundException || ex is FileNotFoundException)
                {
                    //try the XP method instead - glass is not supported.
                    return new Color { A = 255, R = 37, G = 97, B = 163 };
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
