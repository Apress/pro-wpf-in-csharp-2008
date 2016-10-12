using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        public TaskDialogHyperlinkClickedEventArgs(string s)
        {
            linkText = s;
        }
        
        private string linkText;
        public string LinkText
        {
            get { return linkText; }
            set { linkText = value; }
        }
    }
}
