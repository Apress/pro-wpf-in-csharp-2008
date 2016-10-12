using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Helper class that exposes various Win32 functions as managed methods and properties.
    /// </summary>
    public static class InteropHelper
    {
        // Note: since these are public APIs that call immediately into native APIs, we
        // need to make sure we require full-trust, as the unmanaged code check is only a 
        // link demand - and since this code is the caller and it has full-trust, the check
        // would always succeed
        public static int SendMessage(IntPtr hWnd, uint msg, int wParam, bool lParam)
        {
            return UnsafeNativeMethods.SendMessage(hWnd, msg, wParam, lParam);   
        }

        public static int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return UnsafeNativeMethods.SendMessage(hWnd, msg, wParam, lParam);
        }

        public static int SendMessage(IntPtr hWnd, uint msg, int wParam, string lParam)
        {
            return UnsafeNativeMethods.SendMessage(hWnd, msg, wParam, lParam);
        }

        public static int SendMessage(IntPtr hWnd, uint msg, ref int wParam, StringBuilder lParam)
        {
            return UnsafeNativeMethods.SendMessage(hWnd, msg, ref wParam, lParam);
        }

        // TODO: find a better home for this ...
        public static void SetWindowsFormsButtonShield(
          System.Windows.Forms.Button Button,
          bool ShowShield)
        {
            InteropHelper.SendMessage(Button.Handle, NativeMethods.BCM_SETSHIELD, 0, ShowShield);
        }
    }
}
