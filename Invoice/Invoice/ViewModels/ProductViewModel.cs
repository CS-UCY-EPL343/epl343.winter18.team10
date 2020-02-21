using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InvoiceX.Models;
using MySql.Data.MySqlClient;
namespace InvoiceX.ViewModels
{
    class ProductViewModel
    {
        public List<Product> ProductList { get; set; }
        public ProductViewModel()
        {
            ProductList = new List<Product>();

            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Product", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idProductsdb = dataRow.Field<int>("idProduct");
                    var ProductNamedb = dataRow.Field<string>("ProductName");
                    var ProductDescriptiondb = dataRow.Field<string>("Description");
                    var Stockdb = dataRow.Field<int>("Stock");
                    var MinStockdb = dataRow.Field<int>("MinStock");
                    var Costdb = dataRow.Field<float>("Cost");
                    var SellPricedb = dataRow.Field<float>("SellPrice");       
                    var Vatdb = dataRow.Field<float>("VAT");
                    var Categorydb = dataRow.Field<string>("Category");

                    ProductList.Add(
                        new Product()
                        {
                            idProduct = idProductsdb,
                            ProductName = ProductNamedb,
                            ProductDescription = ProductDescriptiondb,
                            Stock = Stockdb,
                            MinStock = MinStockdb,
                            Cost = Costdb,
                            SellPrice = SellPricedb,
                            Vat = Vatdb,
                            Category = Categorydb,
                            Quantity=1
                        });
                }

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void SendProductToDB(Product product)
        {
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert Invoice 
                string query = "INSERT INTO Product (ProductName, Description, Stock, MinStock, Cost, SellPrice, VAT,Category) Values (@ProductName, @Description, @Stock, @MinStock, @Cost, @SellPrice, @VAT,@Category)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                    cmd.Parameters.AddWithValue("@Description", product.ProductDescription);
                    cmd.Parameters.AddWithValue("@Stock", product.Stock);
                    cmd.Parameters.AddWithValue("@MinStock", product.MinStock);
                    cmd.Parameters.AddWithValue("@Cost", product.Cost);
                    cmd.Parameters.AddWithValue("@VAT", product.Vat);
                    cmd.Parameters.AddWithValue("@Category", product.Category);
                    cmd.Parameters.AddWithValue("@SellPrice", product.SellPrice);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Product was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

    }
}


