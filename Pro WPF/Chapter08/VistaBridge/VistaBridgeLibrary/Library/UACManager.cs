using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.SDK.Samples.VistaBridge.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.SDK.Samples.VistaBridge.Services
{
    public static class UACManager
    {
        public static int ExecutePrivilegedProcess(string executablePath)
        {
            int exitCode = -1;

            if (String.IsNullOrEmpty(executablePath))
                throw new ArgumentNullException("Executable file name must be specified");

            using (Process process = CreateDefaultProcess(executablePath))
            {
                if (!process.Start())
                    throw new InvalidOperationException("Couldn't start process '" + executablePath + "'");

                // Synchronously block until process is complete, then return exit code from process
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            return exitCode;
        }

        // TODO: fix this so we can actually get an exit code or other state back
        public static void ExecutePrivilegedProcessAsync(string executablePath, EventHandler exitedEventHandler)
        {
            if (String.IsNullOrEmpty(executablePath))
                throw new ArgumentNullException("Executable file name must be specified");

            using (Process process = CreateDefaultProcess(executablePath))
            {
                if (exitedEventHandler != null)
                    process.Exited += exitedEventHandler;
                if (!process.Start())
                    throw new InvalidOperationException("Couldn't start process '" + executablePath + "'"); 
            }
            return;
        }

        private static Process CreateDefaultProcess(string executablePath)
        {
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.FileName = executablePath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;

            // Must enable Exited event for both sync and async scenarios
            process.EnableRaisingEvents = true;
            return process;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        static internal object LaunchElevatedCOMObject(Guid Clsid, Guid InterfaceID)
        {
            string CLSID = Clsid.ToString("B"); // B formatting directive: returns {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx} 
            string monikerName = "Elevation:Administrator!new:" + CLSID;

            NativeMethods.BIND_OPTS3 bo = new NativeMethods.BIND_OPTS3();
            bo.cbStruct = (uint)Marshal.SizeOf(bo);
            bo.hwnd = IntPtr.Zero;
            bo.dwClassContext = (int)NativeMethods.CLSCTX.CLSCTX_ALL;

            object retVal = UnsafeNativeMethods.CoGetObject(monikerName, ref bo, InterfaceID);

            return (retVal);
        }

        
    }
}
