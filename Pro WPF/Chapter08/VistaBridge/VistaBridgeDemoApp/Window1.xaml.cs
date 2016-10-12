using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Text;
//using System.Windows.Forms;
using Microsoft.SDK.Samples.VistaBridge.Library;


namespace Microsoft.SDK.Samples.VistaBridge
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        private TaskDialog taskDialog;
        private TaskDialogProgressBar bar;

        public Window1()
        {
            InitializeComponent();
        }

        #region MessageBox/TaskDialog Handlers and Helpers

        private void WFMessageBoxClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Hello from Windows Forms!", "Hello World!");
        }

        private void WPFMessageBoxClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Hello from WPF!", "Hello world!");
        }
                
        private void HelloWorldTDClicked(object sender, RoutedEventArgs e)
        {
            TaskDialog.Show("Hello from Vista!", "Hello World!", "Hello World!");
        }

        private void SimpleWaitTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("simpleWaitTD");
            taskDialog.Show();
        }

        private void ConfirmationTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("confirmationTD");
            taskDialog.Show();
        }

        private void ComplexTDClicked(object sender, RoutedEventArgs e)
        {
            taskDialog = FindTaskDialog("complexTD");
            bar = (TaskDialogProgressBar)taskDialog.Controls["ProgressBar"];
            taskDialog.ExpandedText += " Link: <A HREF=\"Http://www.microsoft.com\">Microsoft</A>";
            
            taskDialog.Show();
        }

        private TaskDialog FindTaskDialog(string name)
        {
            return (TaskDialog)FindResource(name);
        }

        private void OnTick(object sender, TaskDialogTickEventArgs e)
        {
            taskDialog.Content = "Seconds elapsed:   " + e.Ticks / 1000;
        }

        private void OnIncreaseClick(object sender, EventArgs e)
        {
            bar.Value = Math.Min(bar.Maximum, bar.Value + 100);
        }

        private void OnDecreaseClick(object sender, EventArgs e)
        {
            bar.Value = Math.Max(0, bar.Value - 100);
        }

        private void OnDialogClosing(object sender, TaskDialogClosingEventArgs e)
        {
            // Any button clicks are considered a "commit" by the dialog, unless
            // closing is explicitly cancelled
            if (e.CustomButton == "increaseButton" || e.CustomButton == "decreaseButton")
                e.Cancel = true;
        }

        private void OnHyperlinkClick(object sender, TaskDialogHyperlinkClickedEventArgs e)
        {
            MessageBox.Show("Link clicked: " + e.LinkText);
        }

        #endregion

        #region File Dialog Handlers and Helpers

        private void WinFormsDialogClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialogOpenFile = new System.Windows.Forms.OpenFileDialog();
            dialogOpenFile.ShowDialog();
        }

        private void WPFDialogClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialogOpenFile = new Microsoft.Win32.OpenFileDialog();
            dialogOpenFile.ShowDialog();
        }

        private void SaveVistaFileDialogClicked(object sender, RoutedEventArgs e)
        {
            CommonSaveFileDialog saveDialog = new CommonSaveFileDialog();
            saveDialog.Title = "My Save File Dialog";
            saveDialog.EnableMiniMode = true;

            CommonFileDialogResult result = saveDialog.ShowDialog();
            if (!result.Canceled)
                MessageBox.Show("File chosen: " + saveDialog.FileName);
        }

        private void OpenVistaFileDialogClicked(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            openDialog.Title = "My Open File Dialog";
            openDialog.Opening += new EventHandler(openDialog_Opening);
            openDialog.Multiselect = true;

            CommonFileDialogResult result = openDialog.ShowDialog();
            if (!result.Canceled)
            {
                StringBuilder output = new StringBuilder("Files selected: ");
                foreach (string file in openDialog.FileNames)
                {
                    output.Append(file);
                    output.Append(Environment.NewLine);
                }
                TaskDialog.Show(output.ToString(), "Files Chosen", "Files Chosen");
            }
        }

        void openDialog_Opening(object sender, EventArgs e)
        {
            MessageBox.Show("we're opening");
        }

        

        #endregion
    }
}