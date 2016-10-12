using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Abstract base for all custom dialog controls.
    /// </summary>
    public abstract class TaskDialogControl : DialogControl
    {
        protected TaskDialogControl() {}
        protected TaskDialogControl(string name) : base(name) {}
    }
}