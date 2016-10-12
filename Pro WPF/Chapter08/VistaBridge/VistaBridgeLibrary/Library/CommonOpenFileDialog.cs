using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Library;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Provides access to a Vista Common File Dialog, which allows the user
    /// to select one or more files.
    /// </summary>
    public sealed class CommonOpenFileDialog : CommonFileDialog
    {
        private NativeFileOpenDialog openDialogCoClass;

        public CommonOpenFileDialog() : base() { }
        public CommonOpenFileDialog(string name) : base(name) { }

        #region Public API specific to Open

        public Collection<string> FileNames
        {
            get 
            {
                CheckFileNamesAvailable();
                return fileNames; 
            }
        }

        private bool multiselect;
        public bool Multiselect
        {
            get { return multiselect; }
            set { multiselect = value; }
        }

        #endregion

        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(openDialogCoClass != null,
                "Must call Initialize() before fetching dialog interface");
            return (IFileDialog)openDialogCoClass;
        }

        internal override void InitializeNativeFileDialog()
        {
            openDialogCoClass = new NativeFileOpenDialog();
        }

        internal override void CleanUpNativeFileDialog()
        {
            if (openDialogCoClass != null)
                Marshal.ReleaseComObject(openDialogCoClass);
        }

        internal override void PopulateWithFileNames(Collection<string> names)
        {
            IShellItemArray resultsArray;
            uint count;
            
            openDialogCoClass.GetResults(out resultsArray);
            resultsArray.GetCount(out count);
            for (int i = 0; i < count; i++)
                names.Add(GetFileNameFromShellItem(GetShellItemAt(resultsArray, i)));
        }

        internal override NativeMethods.FOS GetDerivedOptionFlags(NativeMethods.FOS flags)
        {
            if (multiselect)
                flags |= NativeMethods.FOS.FOS_ALLOWMULTISELECT;
            // TODO: other flags

            return flags;
        }
    }
}
