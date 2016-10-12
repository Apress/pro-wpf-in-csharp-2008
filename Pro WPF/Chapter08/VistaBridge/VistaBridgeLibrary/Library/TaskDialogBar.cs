using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public abstract class TaskDialogBar : TaskDialogControl
    {
        protected TaskDialogBar() {}
        protected TaskDialogBar(string name) : base(name) { }

        private TaskDialogProgressBarState state;
        public TaskDialogProgressBarState State
        {
            get { return state; }
            set 
            {
                CheckPropertyChangeAllowed("State");
                state = value;
                ApplyPropertyChange("State");
            }
        }

        protected internal virtual void Reset()
        {
            state = TaskDialogProgressBarState.Normal;
        }
    }
}
