using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class TaskDialogClosingEventArgs : CancelEventArgs
    {
        private TaskDialogStandardButton standardButton;
        public TaskDialogStandardButton StandardButton
        {
            get { return standardButton; }
            set { standardButton = value; }
        }

        private string customButton;
        public string CustomButton
        {
            get { return customButton; }
            set { customButton = value; }
        }

        
    }
}
