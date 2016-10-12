using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Markup;
using Microsoft.SDK.Samples.VistaBridge.Library;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    // ContentProperty allows us to specify the text of the button as the child text of
    // a button element in XAML, as well as explicitly set with 'Text="<text>"'
    // Note that this attribute is inherited, so it applies to command-links and radio buttons as well
    [ContentProperty("Text")]
    public abstract class TaskDialogButtonBase : TaskDialogControl
    {
        
        protected TaskDialogButtonBase() { }
        protected TaskDialogButtonBase(string name, string text) : base(name)
        {
            this.text = text;
        }

        // Note that we don't need to explicitly implement the add/remove delegate for the Click event;
        // the hosting dialog only needs the delegate information when the Click event is 
        // raised (indirectly) by NativeTaskDialog, so the latest delegate is always available
        public event EventHandler Click;
        internal void RaiseClickEvent()
        {
            EventHandler handler = Click;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private string text;
        public string Text
        {
            get { return text; }
            set 
            {
                CheckPropertyChangeAllowed("Text");
                text = value;
                ApplyPropertyChange("Text");
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set 
            {
                throw new NotImplementedException("This operation is not yet implemented.");
                // TODO: Implement Enabled
                //CheckPropertyChangeAllowed("Enabled");
                //enabled = value;
                //ApplyPropertyChange("Enabled");
            }
        }

        private bool defaultControl;
        public bool Default
        {
            get { return defaultControl; }
            set
            {
                CheckPropertyChangeAllowed("Default");
                defaultControl = value;
                ApplyPropertyChange("Default");
            }
        }

        public override string ToString()
        {
            return text;
        }
    }
}
