using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class TaskDialogButton : TaskDialogButtonBase
    {
        public TaskDialogButton() { }
        public TaskDialogButton(string name, string text) : base(name, text) { }

        private bool showElevationIcon;
        public bool ShowElevationIcon
        {
            get { return showElevationIcon; }
            set 
            {
                CheckPropertyChangeAllowed("ShowElevationIcon");
                showElevationIcon = value;
                ApplyPropertyChange("ShowElevationIcon");
            }
        }
    }
}
