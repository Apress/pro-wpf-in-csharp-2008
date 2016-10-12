using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SDK.Samples.VistaBridge.Library
{
    /// <summary>
    /// Represents a command-link 
    /// </summary>
    public class TaskDialogCommandLink : TaskDialogButton
    {
        public TaskDialogCommandLink() { }
        public TaskDialogCommandLink(string name, string text) : base(name, text) { }
        public TaskDialogCommandLink(string name, string text, string instruction)
            : base(name, text)
        {
            this.instruction = instruction;
        }

        private string instruction;
        public string Instruction
        {
            get { return instruction; }
            set { instruction = value; }
        }

        public override string ToString()
        {
            string instructionString = (instruction == null ? "" : instruction + "\n");
            return instructionString + Text;
        }
    }
}
