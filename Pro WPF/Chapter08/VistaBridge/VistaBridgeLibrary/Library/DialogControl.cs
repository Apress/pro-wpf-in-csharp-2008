using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public abstract class DialogControl
    {
        private static int nextId = TaskDialogDefaults.MinimumDialogControlId;

        protected DialogControl() 
        {
            this.id = nextId;

            // To support wrapping of control IDs - JUST in case you create 2.1 billion
            // custom dialog controls. :)
            if (nextId == Int32.MaxValue)
                nextId = TaskDialogDefaults.MinimumDialogControlId;
            else
                nextId++;
        }

        protected DialogControl(string name) : this()
        {
            this.Name = name;
        }

        private IDialogControlHost hostingDialog;
        internal IDialogControlHost HostingDialog
        {
            get { return hostingDialog; }
            set
            {
                hostingDialog = value;
            }
        }
        
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                // Names for controls need to be quite stable, as we are going to maintain
                // a mapping between the names and the underlying Win32/COM control IDs
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("dialog control name cannot be empty or null");
                if (!String.IsNullOrEmpty(name))
                    throw new InvalidOperationException("Dialog controls cannot be renamed");

                // Note that we don't notify the hosting dialog of the change, as the initial
                // set of name is (must be) always legal, and renames are always illegal
                name = value;
            }
        }

        private int id;
        internal int Id
        {
            get
            {
                return id;
            }
        }

        // Calls hosting dialog, if set yet, to check if property can actually be set while the
        // dialog is in its current state. Host should throw if state doesn't support the change.
        // Note that if the dialog isn't set yet, there are no prop set restrictions yet.
        protected void CheckPropertyChangeAllowed(string propName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propName), "Property changing was not specified");
            if (hostingDialog != null)
                hostingDialog.IsControlPropertyChangeAllowed(propName, this);
        }


        // Calls hosting dialog, if set yet, to indicate that a property has changed, and that 
        // the dialog should do whatever is necessary to propogate the change to the native control.
        // Note that if the dialog isn't set yet, there are no prop set restrictions yet.
        protected void ApplyPropertyChange(string propName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propName), "Property changed was not specified");
            if (hostingDialog != null)
                hostingDialog.ApplyControlPropertyChange(propName, this);

        }

        public override bool Equals(object obj)
        {
            DialogControl control = obj as DialogControl;
            if (control != null)
                return (this.Id == control.Id);
            return false;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
