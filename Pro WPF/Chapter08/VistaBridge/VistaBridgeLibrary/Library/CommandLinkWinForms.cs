using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Microsoft.SDK.Samples.VistaBridge.Interop;
using System.ComponentModel;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class CommandLinkWinForms : Button
    {
        // Add BS_COMMANDLINK style before control creation
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.Style = AddCommandLinkStyle(cp.Style);

                return (cp);
            }
        }

        // Let Windows handle the rendering
        public CommandLinkWinForms()
        {
            FlatStyle = FlatStyle.System;
        }

        // Add Design-Time Support

        // Increase default width
        protected override System.Drawing.Size DefaultSize
        {
            get { return new System.Drawing.Size(180, 60); }
        }

        // Enable note text to be set at design-time
        [Category("Appearance")]
        [Description("Specifies the supporting note text.")]
        [BrowsableAttribute(true)]
        [DefaultValue("(Note Text)")]
        public string NoteText
        {
            get { return (GetNote(this)); }
            set
            {
                SetNote(this, value);
            }
        }

        // Enable shield icon to be set at design-time
        [Category("Appearance")]
        [Description("Indicates whether the button should be decorated with the security shield icon (Windows Vista only).")]
        [BrowsableAttribute(true)]
        [DefaultValue(false)]
        public bool ShieldIcon
        {
            get { return (shieldIconDisplayed); }
            set
            {
                shieldIconDisplayed = value;
                SetShieldIcon(this, this.shieldIconDisplayed);
            }
        }
        private bool shieldIconDisplayed = false;


        #region Interop helpers

        private int AddCommandLinkStyle(int Style)
        {
            int newStyle = Style;

            // Only add BS_COMMANDLINK style on Windows Vista or above.
            // Otherwise, button creation will fail
            if (Environment.OSVersion.Version.Major >= 6)
            {
                newStyle |= NativeMethods.BS_COMMANDLINK;
            }

            return (newStyle);
        }

        private string GetNote(System.Windows.Forms.Button Button)
        {
            int retVal = InteropHelper.SendMessage(Button.Handle, NativeMethods.BCM_GETNOTELENGTH,
                IntPtr.Zero, IntPtr.Zero);

            // Add 1 for null terminator, or you won't get entire string back
            int len = retVal + 1;
            StringBuilder strBld = new StringBuilder(len);

            retVal = InteropHelper.SendMessage(Button.Handle, NativeMethods.BCM_GETNOTE, ref len, strBld);
            return (strBld.ToString());
        }

        private void SetNote(System.Windows.Forms.Button button, string text)
        {
            // This call will be ignored on versions prior to 
            // Windows Vista
            int retVal = InteropHelper.SendMessage(
               button.Handle, NativeMethods.BCM_SETNOTE, 0, text);
        }

        static internal void SetShieldIcon(
         System.Windows.Forms.Button Button, bool Show)
        {
            IntPtr fRequired = new IntPtr(Show ? 1 : 0);
            int retVal = InteropHelper.SendMessage(
               Button.Handle,
                NativeMethods.BCM_SETSHIELD, IntPtr.Zero, fRequired);
        }

        #endregion

    }
}
