using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Library;
using Microsoft.SDK.Samples.VistaBridge.Interop;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Provides access to a Vista Common File Dialog, which allows the user
    /// to select the filename and location for a saved file.
    /// </summary>
    public sealed class CommonSaveFileDialog : CommonFileDialog
    {
        private NativeFileSaveDialog saveDialogCoClass;

        public CommonSaveFileDialog() : base() { }
        public CommonSaveFileDialog(string name) : base(name) { }

        #region Public API specific to Save

        private bool overwritePrompt;
        public bool OverwritePrompt
        {
            get { return overwritePrompt; }
            set 
            {
                ThrowIfDialogShowing("OverwritePrompt" + IllegalPropertyChangeString);
                overwritePrompt = value; 
            }
        }

        private bool createPrompt;
        public bool CreatePrompt
        {
            get { return createPrompt; }
            set 
            {
                ThrowIfDialogShowing("CreatePrompt" + IllegalPropertyChangeString);
                createPrompt = value; 
            }
        }

        private bool enableMiniMode;
        public bool EnableMiniMode
        {
            get { return enableMiniMode; }
            set 
            {
                ThrowIfDialogShowing("EnableMiniMode" + IllegalPropertyChangeString);
                enableMiniMode = value; 
            }
        }

        private bool strictExtensions;
        public bool StrictExtensions
        {
            get { return strictExtensions; }
            set
            {
                ThrowIfDialogShowing("StrictExtensions" + IllegalPropertyChangeString);
                strictExtensions = value;
            }
        }

        #endregion

        internal override void InitializeNativeFileDialog()
        {
            saveDialogCoClass = new NativeFileSaveDialog();
        }

        // TODO: make this a template "method" property instead of actual method 
        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(saveDialogCoClass != null,
                "Must call Initialize() before fetching dialog interface");
            return (IFileDialog)saveDialogCoClass;
        }

        internal override void PopulateWithFileNames(System.Collections.ObjectModel.Collection<string> names)
        {
            IShellItem item;
            saveDialogCoClass.GetResult(out item);

            if (item == null)
                throw new InvalidOperationException("Retrieved a null shell item from dialog");
            names.Add(GetFileNameFromShellItem(item));
        }

        internal override void CleanUpNativeFileDialog()
        {
            if (saveDialogCoClass != null)
                Marshal.ReleaseComObject(saveDialogCoClass);
        }

        internal override NativeMethods.FOS GetDerivedOptionFlags(NativeMethods.FOS flags)
        {
            if (overwritePrompt)
                flags |= NativeMethods.FOS.FOS_OVERWRITEPROMPT;
            if (createPrompt)
                flags |= NativeMethods.FOS.FOS_CREATEPROMPT;
            if (!enableMiniMode)
                flags |= NativeMethods.FOS.FOS_DEFAULTNOMINIMODE;
            if (strictExtensions)
                flags |= NativeMethods.FOS.FOS_STRICTFILETYPES;
            return flags;
        }
    }
}
