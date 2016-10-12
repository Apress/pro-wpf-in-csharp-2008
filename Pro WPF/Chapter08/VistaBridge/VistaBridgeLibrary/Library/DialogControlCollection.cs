using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Library;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public sealed class DialogControlCollection<T> : Collection<T> where T : DialogControl
    {
        private IDialogControlHost hostingDialog;

        internal DialogControlCollection(IDialogControlHost host)
        {
            hostingDialog = host;
        }

        protected override void InsertItem(int index, T control)
        {
            // Check for dups, lack of host, and during-show adds
            if (Items.Contains(control))
                throw new InvalidOperationException("Dialog cannot have more than one control with the same name");
            if (control.HostingDialog != null)
                throw new InvalidOperationException("Dialog control must be removed from current collections first");
            if (!hostingDialog.IsCollectionChangeAllowed())
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported");

            // Reparent, add control
            control.HostingDialog = hostingDialog;
            base.InsertItem(index, control);

            // Notify that we've added a control
            hostingDialog.ApplyCollectionChanged();

        }

        protected override void RemoveItem(int index)
        {
            // Notify that we're about to remove a control - should throw if dialog showing
            if (!hostingDialog.IsCollectionChangeAllowed())
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported");

            DialogControl control = (DialogControl)Items[index];

            // Unparent and remove
            control.HostingDialog = null;
            base.RemoveItem(index);

            hostingDialog.ApplyCollectionChanged();
        }

        // Indexer overload for easily accessing controls by name (esp. useful if
        // overall dialog created in XAML rather than constructed in code)
        public T this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException("control name must be non-empty");

                foreach (T control in base.Items)
                {
                    // NOTE: we don't ToLower() the strings - casing effects 
                    // hash codes, so we are case-sensitive
                    if (control.Name == name)
                        return control;
                }
                return null;
            }
        }

        internal DialogControl GetControlbyId(int id)
        {
            foreach (DialogControl control in Items)
            {
                // Match?
                if (control.Id == id)
                    return control;
            }

            // Control Id not found - likely an error, but the calling function
            // should ultimately decide
            return null;
        }
    }
}
