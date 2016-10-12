using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace Microsoft.SDK.Samples.VistaBridge.Interop
{
    // TODO: suppress unmgd code perm stack walk
    internal static class SafeNativeMethods
    {
        [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
        internal static extern int FormatMessage(
                int flags,
                IntPtr source,
                int messageId,
                int languageId,
                StringBuilder buffer,
                int size,
                IntPtr args);

        internal delegate HRESULT TDIDelegate(
            [In] NativeMethods.TASKDIALOGCONFIG pTaskConfig,
            [Out] out int pnButton,
            [Out] out int pnRadioButton,
            [Out] out bool pVerificationFlagChecked);
            

    }
}
