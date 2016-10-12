using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace StoreDatabase
{
    public class StoreDB2
    {       
        public DataTable GetProducts()
        {            
            return StoreDB2.ReadDataSet().Tables[0];
        }

        public DataSet GetCategoriesAndProducts()
        {            
            return StoreDB2.ReadDataSet();
        }

        internal static DataSet ReadDataSet()
        {            
            DataSet ds = new DataSet();
            ds.ReadXmlSchema("store.xsd");
            ds.ReadXml("store.xml");
            return ds;
        }

    }
}
