using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;

namespace Microsoft.SDK.Samples.VistaBridge.Interop
{
    internal class DllVersionManager
    {
        private static IntPtr activationContextCookie;

        internal static IntPtr GetNativeFunctionPointer(string dllName, string functionName)
        {
            IntPtr hModule = UnsafeNativeMethods.LoadLibrary(dllName);
            if (hModule == null)
                throw new FileNotFoundException("Couldn't find DLL '" + dllName + "'");
            IntPtr procAddress = UnsafeNativeMethods.GetProcAddress(hModule, functionName);
            if (procAddress == IntPtr.Zero)
                throw new EntryPointNotFoundException(
                    "Can't find function '" + functionName + "' in DLL '" + dllName + "'");
            return procAddress;
        }

        internal static void CreateCorrectActivationContext()
        {
            NativeMethods.ACTCTXW actCtx = new NativeMethods.ACTCTXW();
            actCtx.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.ACTCTXW));
            
            // Get HINSTANCE to the current assembly, in which we've embedded
            // our assembly manifest
            Module m = typeof(DllVersionManager).Module;
            IntPtr hInstance = Marshal.GetHINSTANCE(m);

            actCtx.dwFlags =
                (int)NativeMethods.ACTCTXFlags.ACTCTX_FLAG_RESOURCE_NAME_VALID;
            //actCtx.hModule = hInstance;
            actCtx.lpSource = "c:\\Users\\jeffchri\\Documents\\Visual Studio 2005\\Projects\\NTSDK\\VistaBridge\\VistaBridgeLibrary\\bin\\Debug\\VistaBridgeLibrary.dll";
            actCtx.lpResourceName = 17;

            IntPtr hContext = UnsafeNativeMethods.CreateActCtx(ref actCtx);
            if (hContext == ((IntPtr)(-1)))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            bool success = UnsafeNativeMethods.ActivateActCtx(ref actCtx, ref activationContextCookie);
            if (!success)
                throw new Win32Exception(Marshal.GetLastWin32Error());

        }

        internal static void ResetActivationContext()
        {
            UnsafeNativeMethods.DeactivateActCtx(0, activationContextCookie);
        }
    }
}
