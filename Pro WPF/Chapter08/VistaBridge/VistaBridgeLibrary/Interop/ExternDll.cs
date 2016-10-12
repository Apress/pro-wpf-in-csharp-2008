using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Interop
{
    internal class ExternDll
    {
        public const string ComCtl32 = "comctl32.dll";
        public const string Kernel32 = "kernel32.dll";
        public const string ComDlg32 = "comdlg32.dll";
        public const string User32   = "user32.dll";

        // TEST: making sure our explicit LoadLibrary/GetProcAddr approach is working ... obviously we can't keep this
        public const string FullPathComCtl32 = 
            "c:\\Windows\\winsxs\\amd64_microsoft.windows.common-controls_6595b64144ccf1df_6.0.5382.0_none_6103e4885ffbfb32\\comctl32.dll";
    }
}
