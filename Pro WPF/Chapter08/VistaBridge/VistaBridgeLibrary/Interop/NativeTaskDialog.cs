using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Library;


namespace Microsoft.SDK.Samples.VistaBridge.Interop
{
    /// <summary>
    /// Encapsulates the native logic required to create, configure, and show a TaskDialog, 
    /// via the TaskDialogIndirect() Win32 function.
    /// </summary>
    /// <remarks>A new instance of this class should be created for each messagebox show, as
    /// the HWNDs for TaskDialogs do not remain constant across calls to TaskDialogIndirect.
    /// </remarks>
    internal class NativeTaskDialog : IDisposable
    {
        private NativeMethods.TASKDIALOGCONFIG nativeDialogConfig;
        private NativeTaskDialogSettings settings;
        private IntPtr hWndDialog;
        private TaskDialog outerDialog;

        private IntPtr[] updatedStrings = new IntPtr[Enum.GetNames(typeof(NativeMethods.TASKDIALOG_ELEMENTS)).Length];
        private IntPtr buttonArray, radioButtonArray;

        // Flag tracks whether our first radio button click event has come through
        private bool firstRadioButtonClicked = true;

        #region Constructors

        // Configuration is applied at dialog creation time
        internal NativeTaskDialog( 
            NativeTaskDialogSettings settings,
            TaskDialog outerDialog)
        {
            nativeDialogConfig = settings.NativeConfiguration;
            this.settings = settings;

            // Wireup dialog proc message loop for this instance
            nativeDialogConfig.pfCallback = new NativeMethods.PFTASKDIALOGCALLBACK(DialogProc);

            // Keep a reference to the outer shell, so we can notify
            this.outerDialog = outerDialog;
        }

        #endregion

        #region Public Properties

        private NativeDialogShowState showState = NativeDialogShowState.PreShow;
        public NativeDialogShowState ShowState
        {
            get { return showState; }
            set { showState = value; }
        }

        private int selectedButtonID;
        internal int SelectedButtonID
        {
            get { return selectedButtonID; }
        }

        private int selectedRadioButtonID;
        internal int SelectedRadioButtonID
        {
            get { return selectedRadioButtonID; }
        }

        private bool checkBoxChecked;
        internal bool CheckBoxChecked
        {
            get { return checkBoxChecked; }
        }

        #endregion

        internal void NativeShow()
        {
            // Applies config struct and other settings, then
            // calls main Win32 function
            if (settings == null)
                throw new InvalidOperationException("An error has occurred in dialog configuration.");

            // Do a last-minute parse of the various dialog control lists, only 
            // allocate the memory at the last minute
            MarshalDialogControlStructs(settings);

            // Make the call and show the dialog.
            // NOTE: this call is BLOCKING, though the thread WILL re-enter via the DialogProc
            try
            {
                showState = NativeDialogShowState.Showing;

                // Here is the way we would use "vanilla" PInvoke to call TaskDialogIndirect;
                // but if we did so here, we'd be at the mercy of the CLR's native DLL cache -
                // see below for more details.
                // HRESULT hresult = UnsafeNativeMethods.TaskDialogIndirect(
                //    nativeDialogConfig,
                //    out selectedButtonID,
                //    out selectedRadioButtonID,
                //    out checkBoxChecked);

                // Instead, we load comctl and bind to TaskDialogIndirect
                // We do it this way so we can rigidly control which version of comctl32
                // we get loaded, as the CLR's PInvoke mechanism maintains a DLL cache that is 
                // NOT invalidated when the OS activation context is changed such that different
                // DLL versions are needed
                // TEST: Try to get the activation context goo working
                //DllVersionManager.CreateCorrectActivationContext();

                // TEST: Currently no worky, as the activation context isn't getting pushed properly
                IntPtr tdi = DllVersionManager.GetNativeFunctionPointer(
                    ExternDll.ComCtl32, "TaskDialogIndirect");
                
                // TEST: temporarily using the full path to v6 comctl32 
                //IntPtr tdi = DllVersionManager.GetFunctionPointer(
                //    ExternDll.FullPathComCtl32, "TaskDialogIndirect");
                //DllVersionManager.ResetActivationContext();
                SafeNativeMethods.TDIDelegate taskDialogFunctionPointer = (SafeNativeMethods.TDIDelegate)
                    Marshal.GetDelegateForFunctionPointer(tdi, typeof(SafeNativeMethods.TDIDelegate));

                // Invoke TaskDialogIndirect!
                HRESULT hresult = taskDialogFunctionPointer(
                    nativeDialogConfig,
                    out selectedButtonID,
                    out selectedRadioButtonID,
                    out checkBoxChecked);
                if (ErrorHelper.Failed(hresult))
                {
                    string msg;
                    switch (hresult)
                    {
                        case HRESULT.E_INVALIDARG:
                            msg = "Invalid arguments to Win32 call";
                            break;
                        case HRESULT.E_OUTOFMEMORY:
                            msg = "Dialog contents too complex";
                            break;
                        default:
                            msg = "An unexpected internal error occurred in the Win32 call: " + hresult;
                            break;
                    }
                    throw new Win32Exception(msg);
                }
            }
            finally
            {
                showState = NativeDialogShowState.Closed;
            }
        }

        // The new task dialog does not support the existing Win32 functions for closing (e.g. EndDialog()); instead,
        // a "click button" message is sent. In this case, we're abstracting out to say that the TaskDialog consumer can
        // simply call "Close" and we'll "click" the cancel button. Note that the cancel button doesn't actually
        // have to exist for this to work.
        internal void NativeClose()
        {
            showState = NativeDialogShowState.Closing;
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_CLICK_BUTTON,
                (int)NativeMethods.TASKDIALOG_COMMON_BUTTON_RETURN_ID.IDCANCEL, 0);
        }

        #region Main Dialog Proc

        private int DialogProc(
            IntPtr hwnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr lpRefData)
        {
            // Fetch the HWND - it may be the first we're getting it
            hWndDialog = hwnd;

            // Big switch on the various notifications the dialog proc is allowed to get
            switch ((NativeMethods.TASKDIALOG_NOTIFICATIONS)msg)
            {
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_CREATED:
                    int result = PerformDialogInitialization();
                    outerDialog.RaiseOpenedEvent();
                    return result;
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_BUTTON_CLICKED:
                    return HandleButtonClick((int)wParam);
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_RADIO_BUTTON_CLICKED:
                    return HandleRadioButtonClick((int)wParam);
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_HYPERLINK_CLICKED:
                    return HandleHyperlinkClick(lParam);
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_HELP:
                    return HandleHelpInvocation();
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_TIMER:
                    return HandleTick((int)wParam);
                case NativeMethods.TASKDIALOG_NOTIFICATIONS.TDN_DESTROYED:
                    return PerformDialogCleanup();
                default:
                    break;
            }
            return (int)HRESULT.S_OK;
        }


        // Once the task dialog HWND is open, we need to send 
        // additional messages to configure it
        private int PerformDialogInitialization()
        {
            // Initialize Progress or Marquee Bar
            if (IsOptionSet(NativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_PROGRESS_BAR))
            {
                UpdateProgressBarRange(settings.ProgressBarMinimum, settings.ProgressBarMaximum);
                
                // The order of the following is important - state is more important than value, 
                // and non-normal states turn off the bar value change animation, which is likely the intended
                // and preferable behavior
                UpdateProgressBarState(settings.ProgressBarState);
                UpdateProgressBarValue(settings.ProgressBarValue);
                
                // Due to a bug that wasn't fixed in time for RTM of Vista,
                // second SendMessage is required if the state is non-Normal
                UpdateProgressBarValue(settings.ProgressBarValue);
            }
            else if (IsOptionSet(NativeMethods.TASKDIALOG_FLAGS.TDF_SHOW_MARQUEE_PROGRESS_BAR))
            {
                // TDM_SET_PROGRESS_BAR_MARQUEE is necessary to cause the marquee to start animating
                // Note that this internal task dialog setting is round-tripped when the marquee is
                // is set to different states, so it never has to be touched/sent again.
                SendMessageHelper(NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_MARQUEE, 1, 0);
                UpdateProgressBarState(settings.ProgressBarState);
            }

            if (settings.ElevatedButtons != null && settings.ElevatedButtons.Count > 0)
            {
                foreach (int id in settings.ElevatedButtons)
                {
                    UpdateElevationIcon(id, true);
                }
            }
            
            return ErrorHelper.IGNORED;
        }

        private int HandleButtonClick(int id)
        {
            // First we raise a Click event, if there is a custom button
            // However, we implement Close() by sending a cancel button, so we
            // don't want to raise a click event in response to that
            if (showState != NativeDialogShowState.Closing)
                outerDialog.RaiseButtonClickEvent(id);

            // Once that returns, we raise a Closing event for the dialog
            // The Win32 API handles button clicking-and-closing as an atomic action,
            // but it is more .NET friendly to split them up.
            // Unfortunately, we do NOT have the return values at this stage ...
            return outerDialog.RaiseClosingEvent(id);
        }

        private int HandleRadioButtonClick(int id)
        {
            // When the dialog sets the radio button to default, it (somewhat confusingly)
            // issues a radio button clicked event - we mask that out - though ONLY if
            // we do have a default radio button
            if (firstRadioButtonClicked && !IsOptionSet(NativeMethods.TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON))
                firstRadioButtonClicked = false;
            else
            {
                outerDialog.RaiseButtonClickEvent(id);
            }

            // Note: we don't raise Closing, as radio buttons are non-committing buttons
            return ErrorHelper.IGNORED;
        }

        private int HandleHyperlinkClick(IntPtr pszHREF)
        {
            string link = Marshal.PtrToStringUni(pszHREF);
            outerDialog.RaiseHyperlinkClickEvent(link);

            return ErrorHelper.IGNORED;
        }


        private int HandleTick(int ticks)
        {
            outerDialog.RaiseTickEvent(ticks);
            return ErrorHelper.IGNORED;
        }

        private int HandleHelpInvocation()
        {
            outerDialog.RaiseHelpInvokedEvent();
            return ErrorHelper.IGNORED;
        }

        // There should be little we need to do here, as the use of the NativeTaskDialog is
        // that it is instantiated for a single show, then disposed of
        private int PerformDialogCleanup()
        {
            firstRadioButtonClicked = true;

            return ErrorHelper.IGNORED;
        }

        #endregion

        #region Update members

        internal void UpdateProgressBarValue(int i)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_POS, i, 0);
        }

        internal void UpdateProgressBarRange(int min, int max)
        {
            AssertCurrentlyShowing();

            // Build range LPARAM - note it is in REVERSE intuitive order ...
            long range = MakeLongLParam(settings.ProgressBarMaximum, settings.ProgressBarMinimum);
            SendMessageHelper(NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_RANGE, 0, range);
        }

        internal void UpdateProgressBarState(TaskDialogProgressBarState state)
        {
            AssertCurrentlyShowing(); 
            SendMessageHelper(NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_PROGRESS_BAR_STATE, (int)state, 0);
        }

        internal void UpdateContent(string content)
        {
            UpdateTextCore(content, NativeMethods.TASKDIALOG_ELEMENTS.TDE_CONTENT);
        }

        internal void UpdateInstruction(string instruction)
        {
            UpdateTextCore(instruction, NativeMethods.TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION);
        }

        internal void UpdateFooterText(string footerText)
        {
            UpdateTextCore(footerText, NativeMethods.TASKDIALOG_ELEMENTS.TDE_FOOTER);
        }

        internal void UpdateExpandedText(string expandedText)
        {
            UpdateTextCore(expandedText, NativeMethods.TASKDIALOG_ELEMENTS.TDE_EXPANDED_INFORMATION);
        }

        private void UpdateTextCore(string s, NativeMethods.TASKDIALOG_ELEMENTS element)
        {
            AssertCurrentlyShowing();

            FreeOldString(element);
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_ELEMENT_TEXT,
                (int)element,
                (long)MakeNewString(s, element));
        }

        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon)
        {
            UpdateIconCore(mainIcon, NativeMethods.TASKDIALOG_ICON_ELEMENT.TDIE_ICON_MAIN);
        }

        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon)
        {
            UpdateIconCore(footerIcon, NativeMethods.TASKDIALOG_ICON_ELEMENT.TDIE_ICON_FOOTER);
        }

        private void UpdateIconCore(TaskDialogStandardIcon icon, NativeMethods.TASKDIALOG_ICON_ELEMENT element)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_UPDATE_ICON,
                (int)element,
                (long)icon);
        }

        internal void UpdateCheckBoxChecked(bool checkBoxChecked)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_CLICK_VERIFICATION,
                (checkBoxChecked ? 1 : 0),
                1);
        }

        internal void UpdateElevationIcon(int buttonId, bool showIcon)
        {
            AssertCurrentlyShowing();
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE,
                buttonId,
                Convert.ToInt32(showIcon));
        }

        internal void UpdateButtonEnabled(int controlId, bool enabled)
        {
            // TODO: Implement Enabled - note that setting the initial enable state suggests
            // another member on the NativeTaskDialogSettings class (similar to 
            // ShowElevatedIcon
        }

        internal void AssertCurrentlyShowing()
        {
            Debug.Assert(showState == NativeDialogShowState.Showing, "Update*() methods should only be called while native dialog is showing");
        }

        #endregion

        #region Helpers

        private int SendMessageHelper(NativeMethods.TASKDIALOG_MESSAGES msg, int wParam, long lParam)
        {
            // Be sure to at least assert here - messages to invalid handles often just disappear silently
            Debug.Assert(hWndDialog != null, "HWND for dialog is null during SendMessage");

            return (int)UnsafeNativeMethods.SendMessage(
                hWndDialog,
                (uint)msg,
                (IntPtr)wParam,
                new IntPtr(lParam));
        }

        private bool IsOptionSet(NativeMethods.TASKDIALOG_FLAGS flag)
        {
            return ((nativeDialogConfig.dwFlags & flag) == flag);
        }

        // Allocates a new string on the unmanaged heap, and stores the pointer
        // so we can free it later
        private IntPtr MakeNewString(string s, NativeMethods.TASKDIALOG_ELEMENTS element)
        {
            IntPtr newStringPtr = Marshal.StringToHGlobalUni(s);
            updatedStrings[(int)element] = newStringPtr;
            return newStringPtr;
        }

        // Checks to see if the given element already has an updated string, and if so, 
        // frees it. This is done in preparation for a call to MakeNewString(), to prevent
        // leaks from multiple updates calls on the same element within a single native dialog lifetime.
        private void FreeOldString(NativeMethods.TASKDIALOG_ELEMENTS element)
        {
            int elementIndex = (int)element;
            if (updatedStrings[elementIndex] != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(updatedStrings[elementIndex]);
                updatedStrings[elementIndex] = IntPtr.Zero;
            }
        }

        // Based on the following defines in WinDef.h and WinUser.h:
        // #define MAKELPARAM(l, h) ((LPARAM)(DWORD)MAKELONG(l, h))
        // #define MAKELONG(a, b) ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff)) | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
        private long MakeLongLParam(int a, int b)
        {
            return (a << 16) + b;
        }

        // Builds the actual configuration that the NativeTaskDialog (and underlying Win32 API)
        // expects, by parsing the various control lists, marshalling to the unmgd heap, etc.
        private void MarshalDialogControlStructs(NativeTaskDialogSettings settings)
        {
            if (settings.Buttons != null && settings.Buttons.Length > 0)
            {
                buttonArray = AllocateAndMarshalButtons(settings.Buttons);
                settings.NativeConfiguration.pButtons = buttonArray;
                settings.NativeConfiguration.cButtons = (uint)settings.Buttons.Length;
            }
            if (settings.RadioButtons != null && settings.RadioButtons.Length > 0)
            {
                radioButtonArray = AllocateAndMarshalButtons(settings.RadioButtons);
                settings.NativeConfiguration.pRadioButtons = radioButtonArray;
                settings.NativeConfiguration.cRadioButtons = (uint)settings.RadioButtons.Length;
            }
        }

        private IntPtr AllocateAndMarshalButtons(NativeMethods.TASKDIALOG_BUTTON[] structs)
        {
            IntPtr initialPtr = Marshal.AllocHGlobal(
                Marshal.SizeOf(typeof(NativeMethods.TASKDIALOG_BUTTON)) * structs.Length);
            IntPtr currentPtr = initialPtr;
            foreach (NativeMethods.TASKDIALOG_BUTTON button in structs)
            {
                // TODO: Need to get MarshalAs(UnmanagedType.LPArray) working here
                Marshal.StructureToPtr(button, currentPtr, false);
                currentPtr = (IntPtr)((int)currentPtr + Marshal.SizeOf(button));
            }
            return initialPtr;
        }

        #endregion

        #region IDispose Pattern

        private bool disposed;

        // Finalizer and IDisposable implementation
        public void Dispose() { Dispose(true); }
        ~NativeTaskDialog() { Dispose(false); }

        // Core disposing logic
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true; 

                // Single biggest resource - make sure the dialog itself has been instructed
                // to close
                if (showState == NativeDialogShowState.Showing)
                    NativeClose();
                
                // Clean up custom allocated strings that were updated
                // while the dialog was showing. Note that the strings
                // passed in the initial TaskDialogIndirect call will
                // be cleaned up automagically by the default marshalling logic.
                if (updatedStrings != null)
                {
                    for (int i = 0; i < updatedStrings.Length; i++)
                    {
                        if (updatedStrings[i] != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(updatedStrings[i]);
                            updatedStrings[i] = IntPtr.Zero;
                        }
                    }
                }

                // Clean up the button and radio button arrays, if any
                if (buttonArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buttonArray);
                    buttonArray = IntPtr.Zero;
                }
                if (radioButtonArray != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(radioButtonArray);
                    radioButtonArray = IntPtr.Zero;
                }

                if (disposing)
                {
                    // Clean up managed resources - currently there are none 
                    // that are interesting.
                }
            }
        }

        #endregion


        // TODO: Proof of concept - as of FebCTP, TDM_NAVIGATE_PAGE is too flicker-ful
        // to expose in the managed wrapper
        bool alternate = true;
        internal void NavigateTest()
        {
            alternate = !alternate;
            string text = (alternate ? "Text1" : "Text2");
            
            //Change buttons:
            //settings.Buttons[0].pszButtonText = text;
            //settings.NativeConfiguration.pButtons =
            //    AllocateAndMarshalButtons(settings.Buttons);

            settings.NativeConfiguration.pszMainInstruction = "New instructions: " + text;

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(settings.NativeConfiguration));
            Marshal.StructureToPtr(settings.NativeConfiguration, ptr, false);
            SendMessageHelper(
                NativeMethods.TASKDIALOG_MESSAGES.TDM_NAVIGATE_PAGE,
                0, (int)ptr);
            
        }
    }
}
