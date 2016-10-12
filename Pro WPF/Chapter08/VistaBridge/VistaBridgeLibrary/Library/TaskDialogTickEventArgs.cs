using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class TaskDialogTickEventArgs : EventArgs
    {
        private int ticks;

        public TaskDialogTickEventArgs(int totalTicks)
        {
            ticks = totalTicks;
        }

        public int Ticks
        {
            get { return ticks; }
        }
    }
}
