using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public abstract class CommonFileDialogControl : DialogControl
    {
        protected CommonFileDialogControl() {}
        protected CommonFileDialogControl(string name) : base(name) { }
    }
}
