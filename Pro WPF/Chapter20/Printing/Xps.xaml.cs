using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Printing;
using System.IO.Packaging;

namespace Printing
{
    /// <summary>
    /// Interaction logic for Xps.xaml
    /// </summary>

    public partial class Xps : System.Windows.Window
    {

        public Xps()
        {
            InitializeComponent();            
        }

         

        private void Window_Loaded(object sender, EventArgs e)
        {            
            XpsDocument doc = new XpsDocument("test.xps", FileAccess.ReadWrite);
            docViewer.Document = doc.GetFixedDocumentSequence();
            
            doc.Close();
        }

        private PrintDialog printDialog = new PrintDialog();
        private void cmdPrintXps_Click(object sender, RoutedEventArgs e)
        {
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(docViewer.Document.DocumentPaginator, "A Fixed Document");
            }
        }

        private void cmdPrintFlow_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "FlowDocument1.xaml");
            if (printDialog.ShowDialog() == true)
            {
                PrintQueue queue = printDialog.PrintQueue;
                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);

                using (FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    FlowDocument flowDocument = (FlowDocument)XamlReader.Load(fs);
                    writer.Write(((IDocumentPaginatorSource)flowDocument).DocumentPaginator);
                }
            }
        }

        private void cmdShowFlow_Click(object sender, RoutedEventArgs e)
        {
            
            if (File.Exists("test2.xps")) File.Delete("test2.xps");

            using (FileStream fs = File.Open("FlowDocument1.xaml", FileMode.Open))
            {
                FlowDocument doc = (FlowDocument)XamlReader.Load(fs);

                XpsDocument xpsDocument = new XpsDocument("test2.xps", FileAccess.ReadWrite);
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                writer.Write(((IDocumentPaginatorSource)doc).DocumentPaginator);

                // Display the new XPS document in a viewer.
                docViewer.Document = xpsDocument.GetFixedDocumentSequence();
                xpsDocument.Close();
            }
        }
    }
}