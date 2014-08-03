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
            get { return _application; }
            set
            {
                _application = value;
                SetNewWindowForHook();
            }
        }

        private static Window Window
        {
            get { return _window; }
            set
            {
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

        private static Application _application;
        private static Window _window;

        public static void Initialize(Application application)
        {
            Application = application;
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
                break;
            }
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
        }

        private static Color GetAeroColorFromNumeric(uint color = 0)
        {
            try
            {
                if (color == 0)
                {
                    bool alphaBlend;
                    uint newColor;
                    NativeMethods.DwmGetColorizationColor(out newColor, out alphaBlend);

                    color = newColor;
                }

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
