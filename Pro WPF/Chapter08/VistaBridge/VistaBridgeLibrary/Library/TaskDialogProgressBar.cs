using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class TaskDialogProgressBar : TaskDialogBar
    {

        public TaskDialogProgressBar() { }
        public TaskDialogProgressBar(string name) : base(name) { }
        public TaskDialogProgressBar(int minimum, int maximum, int value)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.value = value;
        }

        private int value = TaskDialogDefaults.ProgressBarStartingValue;
        private int minimum = TaskDialogDefaults.ProgressBarMinimumValue;
        private int maximum = TaskDialogDefaults.ProgressBarMaximumValue;

        public int Minimum
        {
            get { return minimum; }
            set 
            {
                CheckPropertyChangeAllowed("Minimum");
                minimum = value;
                ApplyPropertyChange("Minimum");
            }
        }

        public int Maximum
        {
            get { return maximum; }
            set 
            {
                CheckPropertyChangeAllowed("Maximum");
                maximum = value;
                ApplyPropertyChange("Maximum");
            }
        }

        public int Value
        {
            get { return this.value; }
            set 
            {
                CheckPropertyChangeAllowed("Value");
                this.value = value;
                ApplyPropertyChange("Value");
            }
        }

        internal bool HasValidValues
        {
            get { return (minimum <= value && value <= maximum); }
        }

        protected internal override void Reset()
        {
            base.Reset();
            value = minimum;
        }
    }
}
