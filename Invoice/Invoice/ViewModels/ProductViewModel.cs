using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class ProductViewModel
    {
        public List<Product> productList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public ProductViewModel()
        {
            productList = new List<Product>();

            try
            {
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

                    productList.Add(
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
                            Quantity = 1
                        });
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void insertProduct(Product product)
        {
            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateProduct(Product product)
        {
            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int returnLatestProductID()
        {
            try
            {
                string idProduct;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idProduct FROM Product ORDER BY idProduct DESC LIMIT 1", conn);

                int id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idProduct = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idProduct = "";

                return Convert.ToInt32(idProduct);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }


        public static Product getProduct(int productid)
        {
            Product product = new Product();

            try
            {
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM Product WHERE idProduct=" + productid, conn);
                var produc = cmd.ExecuteReader();

                // Now check if any rows returned.
                if (produc.HasRows)
                {
                    produc.Read();// Get first record.                     
                    product.idProduct = productid; //get  values of first row
                    product.ProductName = produc.GetString(1);
                    product.ProductDescription = produc.GetString(2);
                    product.Stock = produc.GetInt32(3);
                    product.MinStock = produc.GetInt32(4);
                    product.Cost = produc.GetDouble(5);
                    product.SellPrice = produc.GetDouble(6);
                    product.Vat = produc.GetFloat(7);
                    product.Category = produc.GetString(8);
                }

                produc.Close();// Close reader.  
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return product;
        }

        public static void deleteProduct(int productID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Product WHERE idProduct = " + productID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1451)
                {
                    MessageBox.Show("Cannot delete product with ID = " + productID + " as it is used in invoices and/or orders.");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }                
            }
        }
        public static int getProductCount(int productId, int months, int year)
        {
            int total = 0;

            try
            {
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

                if (int.TryParse(total2, out total3))
                {
                    total = total3;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }
        public static float getProductSales(int productId, int months, int year)
        {
            float total = 0;

            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

    }
}




