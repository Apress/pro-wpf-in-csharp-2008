using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Provides standard combinations of standard buttons in the TaskDialog.
    /// </summary>
    public enum TaskDialogStandardButtons
    {
        None        = TaskDialogStandardButton.None,
        Cancel      = TaskDialogStandardButton.Cancel,
        OkCancel    = TaskDialogStandardButton.Ok | TaskDialogStandardButton.Cancel,
        Yes         = TaskDialogStandardButton.Yes,
        YesNo       = TaskDialogStandardButton.Yes | TaskDialogStandardButton.No,
        YesNoCancel = TaskDialogStandardButton.Yes | TaskDialogStandardButton.No | TaskDialogStandardButton.Cancel,
        RetryCancel = TaskDialogStandardButton.Retry | TaskDialogStandardButton.Cancel,
        Close       = TaskDialogStandardButton.Close
    }
}
