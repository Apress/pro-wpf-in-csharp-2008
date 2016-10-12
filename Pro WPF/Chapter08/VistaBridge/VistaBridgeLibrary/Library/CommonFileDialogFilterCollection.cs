using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class CommonFileDialogFilterCollection : Collection<CommonFileDialogFilter>
    {
        // TODO: Override a few of IList<> members, to block additions if dialog is showing
    }
}
