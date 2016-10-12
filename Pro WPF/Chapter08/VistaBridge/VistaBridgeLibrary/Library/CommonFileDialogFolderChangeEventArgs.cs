using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class CommonFileDialogFolderChangeEventArgs : CancelEventArgs
    {
        public CommonFileDialogFolderChangeEventArgs(string folder)
        {
            this.folder = folder;
        }

        private string folder;
        public string Folder
        {
            get { return folder; }
            set { folder = value; }
        }

    }
}
