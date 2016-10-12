using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StoreDatabase
{
    public class Category : INotifyPropertyChanged
    {
        private string categoryName;
        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CategoryName"));
            }
        }

        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Products"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public Category(string categoryName, ObservableCollection<Product> products)
        {
            CategoryName = categoryName;
            Products = products;
        }
    }

}
