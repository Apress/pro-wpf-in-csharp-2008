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
using StoreDatabase;

namespace DataBinding
{
    /// <summary>
    /// Interaction logic for EditProductObject.xaml
    /// </summary>

    public partial class EditProductObject : System.Windows.Window
    {
        private Product product;

        public EditProductObject()
        {
            InitializeComponent();
        }

        private void cmdGetProduct_Click(object sender, RoutedEventArgs e)
        {
            int ID;
            if (Int32.TryParse(txtID.Text, out ID))
            {
                try
                {
                    product = App.StoreDb.GetProduct(ID);
                    gridProductDetails.DataContext = product;
                }
                catch
                {
                    MessageBox.Show("Error contacting database.");
                }                
            }
            else
            {
                MessageBox.Show("Invalid ID.");
            }
        }

        private void cmdUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            // Make sure update has taken place.
            FocusManager.SetFocusedElement(this, (Button)sender);

            MessageBox.Show(product.UnitCost.ToString());
        }


        private void cmdIncreasePrice_Click(object sender, RoutedEventArgs e)
        {
            product.UnitCost *= 1.1M;          
        }

        private void validationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {                
                MessageBox.Show(e.Error.ErrorContent.ToString());
            }
        }

        private void cmdGetExceptions_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            GetErrors(sb, gridProductDetails);
            string message = sb.ToString();
            if (message != "") MessageBox.Show(message);
        }
        private void GetErrors(StringBuilder sb, DependencyObject obj)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                // Ignore strings and dependency objects that aren't elements.
                TextBox element = child as TextBox;
                if (element == null) continue;

                if (Validation.GetHasError(element))
                {
                    sb.Append(element.Text + " has errors:\r\n");
                    foreach (ValidationError error in Validation.GetErrors(element))
                    {
                        sb.Append("  " + error.ErrorContent.ToString());

                        sb.Append("\r\n");
                    }
                }

                // Check the children of this object.
                GetErrors(sb, element);
            }
        }
    }
}