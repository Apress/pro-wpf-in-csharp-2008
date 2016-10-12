using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace StoreDatabase
{
	public class Product : INotifyPropertyChanged, IDataErrorInfo
	{
		private string modelNumber;
		public string ModelNumber
		{
			get { return modelNumber; }
			set {
                modelNumber = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ModelNumber"));
            }
		}              

		private string modelName;
		public string ModelName
		{
			get { return modelName; }
			set {
                modelName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ModelName"));
            }
		}

		private decimal unitCost;
		public decimal UnitCost
		{
			get { return unitCost; }
			set { unitCost = value;
                OnPropertyChanged(new PropertyChangedEventArgs("UnitCost"));
            }
		}

		private string description;
		public string Description
		{
			get { return description; }
			set { description = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Description"));
            }
		}

        private string categoryName;
        public string CategoryName
        {
            get { return categoryName; }
            set { categoryName = value; }
        }

        private string productImagePath;
        public string ProductImagePath
        {
            get { return productImagePath; }
            set { productImagePath = value; }
        }        

		public Product(string modelNumber, string modelName,
			decimal unitCost, string description)
		{
			ModelNumber = modelNumber;
			ModelName = modelName;
			UnitCost = unitCost;
			Description = description;
		}

        public Product(string modelNumber, string modelName,
           decimal unitCost, string description, 
           string productImagePath)
            :
           this(modelNumber, modelName, unitCost, description)
        {            
            ProductImagePath = productImagePath;
        }

        public Product(string modelNumber, string modelName,
            decimal unitCost, string description, string categoryName,
            string productImagePath) : 
            this(modelNumber, modelName, unitCost, description)
        {
            CategoryName = categoryName;
            ProductImagePath = productImagePath;
        }

		public override string ToString()
		{
			return ModelName + " (" + ModelNumber + ")";
		}

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
                       
        #region IDataErrorInfo Members

        // Error handling takes place here.
        public string this[string propertyName]
        {
            get
            {
                if (propertyName == "ModelNumber")
                {
                    bool valid = true;
                    foreach (char c in ModelNumber)
                    {
                        if (!Char.IsLetterOrDigit(c))
                        {
                            valid = false;
                            break;
                        }                        
                    }
                    if (!valid) return "The ModelNumber can only contain letters and numbers.";
                }
                return null;
            }
        }

        // WPF doesn't use this property.
        public string Error
        {
            get { return null; }
        }

        #endregion
    }
}
