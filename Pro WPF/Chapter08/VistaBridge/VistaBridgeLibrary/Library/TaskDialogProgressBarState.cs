using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public enum TaskDialogProgressBarState
    {
        Normal      = NativeMethods.PBST.PBST_NORMAL,
        Error       = NativeMethods.PBST.PBST_ERROR,
        Paused      = NativeMethods.PBST.PBST_PAUSED
    }
}
