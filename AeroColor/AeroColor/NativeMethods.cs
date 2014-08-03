using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AeroColor
{
    class NativeMethods
    {
        internal const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;

        [DllImport("dwmapi.dll", PreserveSig = false, SetLastError = true)]
        internal static extern void DwmGetColorizationColor(out uint ColorizationColor, [MarshalAs(UnmanagedType.Bool)] out bool ColorizationOpaqueBlend);
    }
}
