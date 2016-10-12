using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace NonCompiledXaml
{
    public class Program : Application
    {
        [STAThread()]
        static void Main()
        {
            Program mn = new Program();
            mn.MainWindow = new Window1();
            mn.MainWindow.ShowDialog();
        }
    }
}
