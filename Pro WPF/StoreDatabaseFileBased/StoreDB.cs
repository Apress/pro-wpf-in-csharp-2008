using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace StoreDatabase
{
	public class StoreDB
	{
        public Product GetProduct(int ID)
        {
            DataSet ds = StoreDB2.ReadDataSet();
            DataRow productRow = ds.Tables["Products"].Select("ProductID = " + ID.ToString())[0];
            Product product = new Product((string)productRow["ModelNumber"],
                    (string)productRow["ModelName"], (decimal)productRow["UnitCost"],
                    (string)productRow["Description"], (string)productRow["CategoryName"],
                    (string)productRow["ProductImage"]);            
            return product;
        }

		public ICollection<Product> GetProducts()
		{
            DataSet ds = StoreDB2.ReadDataSet();

            ObservableCollection<Product> products = new ObservableCollection<Product>();
            foreach (DataRow productRow in ds.Tables["Products"].Rows)
            {
                products.Add(new Product((string)productRow["ModelNumber"],
                    (string)productRow["ModelName"], (decimal)productRow["UnitCost"],
                    (string)productRow["Description"], (string)productRow["CategoryName"],
                    (string)productRow["ProductImage"]));
            }
			return products;
		}

        public ICollection<Product> GetProductsSlow()
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            return GetProducts();
        }

        public ICollection<Category> GetCategoriesAndProducts()
        {
            DataSet ds = StoreDB2.ReadDataSet();
            DataRelation relCategoryProduct = ds.Relations[0];

            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            foreach (DataRow categoryRow in ds.Tables["Categories"].Rows)
            {
                ObservableCollection<Product> products = new ObservableCollection<Product>();
                foreach (DataRow productRow in categoryRow.GetChildRows(relCategoryProduct))
                {
                    products.Add(new Product(productRow["ModelNumber"].ToString(),
                        productRow["ModelName"].ToString(), (decimal)productRow["UnitCost"],
                        productRow["Description"].ToString()));
                }
                categories.Add(new Category(categoryRow["CategoryName"].ToString(), products));
            }
            return categories;
        }
	}

}
