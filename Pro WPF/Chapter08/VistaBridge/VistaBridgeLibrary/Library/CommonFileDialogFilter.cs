using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    public class CommonFileDialogFilter
    {
        public CommonFileDialogFilter() 
        {
            extensions = new Collection<string>();
        }

        public CommonFileDialogFilter(string displayName, string extensionList) : this()
        {
            if (String.IsNullOrEmpty(extensionList))
                throw new ArgumentNullException("extensionList must be non-null");
            this.DisplayName = displayName;

            // Parse string and create extension strings
            // Format: "bat,cmd"
            // Can support leading "." or "*." - these will be stripped
            string[] rawExtensions = extensionList.Split(',');
            foreach (string extension in rawExtensions)
                extensions.Add(NormalizedExtension(extension));
        }

        private string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set 
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("DisplayName must be non-null");
                displayName = value; 
            }
        }

        private Collection<string> extensions;
        public Collection<string> Extensions
        {
            get { return extensions; }
        }

        private bool showExtensions = true;
        public bool ShowExtensions
        {
            get { return showExtensions; }
            set { showExtensions = value; }
        }

        private string NormalizedExtension(string rawExtension)
        {
            rawExtension = rawExtension.Replace("*", null);
            rawExtension = rawExtension.Replace(".", null);
            return rawExtension;
        }



    }
}
