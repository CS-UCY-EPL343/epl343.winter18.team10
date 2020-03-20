using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using InvoiceX.Models;
using MySql.Data.MySqlClient;
namespace InvoiceX.ViewModels
{
    class ProductViewModel
    {
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                   "pwd=CCfHC5PWLjsSJi8G;database=invoice";

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

        public static void UpdateProductToDB(Product product)
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
                string query = "UPDATE Product SET ProductName=@ProductName, Description=@Description, Stock=@Stock, MinStock=@MinStock, Cost=@Cost, SellPrice=@SellPrice, VAT=@VAT,Category=@Category  WHERE idProduct=@idProduct";
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
                    cmd.Parameters.AddWithValue("@idProduct", product.idProduct);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Product was Updated");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static int ReturnLatestProductID()
        {
            int id_return = 0;
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            try
            {
                string idProduct;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idProduct FROM Product ORDER BY idProduct DESC LIMIT 1", conn);
                conn.Open();
                id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idProduct = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idProduct = "";

                conn.Close();
                return Convert.ToInt32(idProduct);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return 0;
        }

        
        public static Product ReturnProductByid(int productid)
        {
           
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            Product product = new Product();
            try
            {                
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM Product WHERE idProduct="+ productid, conn);
                conn.Open();
                var produc = cmd.ExecuteReader();
               
                // Now check if any rows returned.
                if (produc.HasRows)
                {
                    produc.Read();// Get first record.                     
                    product.idProduct = productid; //get  values of first row
                    product.ProductName= produc.GetString(1);
                    product.ProductDescription= produc.GetString(2);
                    product.Stock = produc.GetInt32(3);
                    product.MinStock = produc.GetInt32(4);
                    product.Cost = produc.GetDouble(5);
                    product.SellPrice = produc.GetDouble(6);
                    product.Vat = produc.GetFloat(7);
                    product.Category = produc.GetString(8);
                }
                produc.Close();// Close reader.
                

                conn.Close();
                //return Convert.ToInt32(idInvoice);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return product;
        }

        public static void deleteProductByID(int productID)
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Product WHERE idProduct = " + productID, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }
        public static int getProductCount(int productId, int months,int year)
        {
            MySqlConnection conn;
            int total=0 ;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("getTotalProducts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", SqlDbType.Int).Value = productId;
                cmd.Parameters["@productId"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@months", SqlDbType.Int).Value = months;
                cmd.Parameters["@months"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                String total2 = cmd.ExecuteScalar().ToString();
                int total3 = 0;
                if (int.TryParse(total2,out total3)) { 
                    total = total3;
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;
        }
        public static float getProductSales(int productId, int months,int year)
        {
            MySqlConnection conn;
            float total = 0;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("getTotalProductSales", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", SqlDbType.Int).Value = productId;
                cmd.Parameters["@productId"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@months", SqlDbType.Int).Value = months;
                cmd.Parameters["@months"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                String total2 = cmd.ExecuteScalar().ToString();
                float total3 = 0;
                if (float.TryParse(total2, out total3))
                {
                    total = total3;
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;
        }

    }
}




