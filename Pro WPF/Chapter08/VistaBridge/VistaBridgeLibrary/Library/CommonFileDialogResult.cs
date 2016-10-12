using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class CommonFileDialogResult
    {
        public CommonFileDialogResult(bool canceled) 
        {
            this.canceled = canceled;
        }

        private bool canceled;
        public bool Canceled
        {
            get { return canceled; }
        }

    }
}
