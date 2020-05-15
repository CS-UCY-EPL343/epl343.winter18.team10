// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class ProductViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the products
        /// </summary>
        public ProductViewModel()
        {
            productList = new List<Product>();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM Product", conn);
                var dt = new DataTable();
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
                        new Product
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Constructor that given customer ID loads the quotes and products of that customer
        /// </summary>
        /// <param name="customerID"></param>
        public ProductViewModel(int customerID)
        {
            productList = new List<Product>();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM Product", conn);
                var dt = new DataTable();
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
                        new Product
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            /*quotes*/
            productList_quotes = new List<Product>();

            try
            {
                var cmd = new MySqlCommand("SELECT idProduct,OfferPrice FROM viewQuote WHERE idCustomer=@idCustomer",
                    conn);
                cmd.Parameters.AddWithValue("@idCustomer", customerID);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idProductsdb = dataRow.Field<int>("idProduct");
                    var OfferPrice = dataRow.Field<float>("OfferPrice");


                    productList_quotes.Add(
                        new Product
                        {
                            idProduct = idProductsdb,
                            SellPrice = OfferPrice
                        });
                }

                foreach (var product in productList_quotes)
                {
                    var product_with_quote = productList.FirstOrDefault(r => r.idProduct == product.idProduct);
                    product_with_quote.SellPrice = product.SellPrice;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Product> productList { get; set; }
        public List<Product> productList_quotes { get; set; }

        /// <summary>
        ///     Given the product object inserts it in to the database
        /// </summary>
        /// <param name="product"></param>
        public static void insertProduct(Product product)
        {
            try
            {
                //insert Invoice 
                var query =
                    "INSERT INTO Product (ProductName, Description, Stock, MinStock, Cost, SellPrice, VAT,Category) Values (@ProductName, @Description, @Stock, @MinStock, @Cost, @SellPrice, @VAT,@Category)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the old and updated product updates the product
        /// </summary>
        /// <param name="product"></param>
        public static void updateProduct(Product product)
        {
            try
            {
                //insert Invoice 
                var query =
                    "UPDATE Product SET ProductName=@ProductName, Description=@Description, Stock=@Stock, MinStock=@MinStock, Cost=@Cost, SellPrice=@SellPrice, VAT=@VAT,Category=@Category  WHERE idProduct=@idProduct";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns the latest product ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestProductID()
        {
            try
            {
                string idProduct;
                var cmd = new MySqlCommand("SELECT idProduct FROM Product ORDER BY idProduct DESC LIMIT 1", conn);

                var id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idProduct = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idProduct = "";

                return Convert.ToInt32(idProduct);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the product ID retrieves the product and returns it
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public static Product getProduct(int productid)
        {
            var product = new Product();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM Product WHERE idProduct=" + productid, conn);
                var produc = cmd.ExecuteReader();

                // Now check if any rows returned.
                if (produc.HasRows)
                {
                    produc.Read(); // Get first record.                     
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

                produc.Close(); // Close reader.  
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return product;
        }

        /// <summary>
        ///     Given the product ID deletes the product from the Database
        /// </summary>
        /// <param name="productID"></param>
        public static void deleteProduct(int productID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM Product WHERE idProduct = " + productID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                    MessageBox.Show("Cannot delete product with ID = " + productID +
                                    " as it is used in invoices and/or orders.");
                else
                    MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Gets the product count in a year to be used for statistics
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int getProductCount(int productId, int months, int year)
        {
            var total = 0;

            try
            {
                var cmd = new MySqlCommand("getTotalProducts", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", SqlDbType.Int).Value = productId;
                cmd.Parameters["@productId"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@months", SqlDbType.Int).Value = months;
                cmd.Parameters["@months"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                var total2 = cmd.ExecuteScalar().ToString();
                var total3 = 0;

                if (int.TryParse(total2, out total3)) total = total3;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        /// <summary>
        ///     Gets the product sales in a year to be used for statistics
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static float getProductSales(int productId, int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getTotalProductSales", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@productId", SqlDbType.Int).Value = productId;
                cmd.Parameters["@productId"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@months", SqlDbType.Int).Value = months;
                cmd.Parameters["@months"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                var total2 = cmd.ExecuteScalar().ToString();
                float total3 = 0;

                if (float.TryParse(total2, out total3)) total = total3;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }
    }
}