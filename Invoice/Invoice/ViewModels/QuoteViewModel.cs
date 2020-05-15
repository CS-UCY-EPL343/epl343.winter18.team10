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
using System.Text;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class QuoteViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the quotes
        /// </summary>
        public QuoteViewModel()
        {
            quoteList = new List<Quote>();

            try
            {
                var cmd = new MySqlCommand("SELECT `Quote`.*, `Customer`.`CustomerName` FROM `Quote`" +
                                           " LEFT JOIN `Customer` ON `Quote`.`idCustomer` = `Customer`.`idCustomer`; ",
                    conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var quote = new Quote();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idQuote = dataRow.Field<int>("idQuote");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    quote = new Quote
                    {
                        idQuote = idQuote,
                        customerName = customer,
                        createdDate = createdDate,
                        issuedBy = issuedBy
                    };

                    quoteList.Add(quote);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Quote> quoteList { get; set; }

        /// <summary>
        ///     Given the quote ID retrieves the quote and returns it
        /// </summary>
        /// <param name="quoteID"></param>
        /// <returns></returns>
        public static Quote getQuote(int quoteID)
        {
            var quote = new Quote();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM viewQuote WHERE QuoteID = " + quoteID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    quote = null;

                var count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customerName = dataRow.Field<string>("CustomerName");
                    var phoneNumber = dataRow.Field<int>("PhoneNumber");
                    var email = dataRow.Field<string>("Email");
                    var address = dataRow.Field<string>("Address");
                    var country = dataRow.Field<string>("Country");
                    var city = dataRow.Field<string>("City");
                    var customerId = dataRow.Field<int>("idCustomer");
                    var customerBalance = dataRow.Field<float>("Balance");

                    var idQuote = dataRow.Field<int>("QuoteID");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<int>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var sellPrice = dataRow.Field<float>("SellPrice");
                    var offerPrice = dataRow.Field<float>("OfferPrice");

                    if (count == 0)
                    {
                        count++;
                        quote = new Quote
                        {
                            idQuote = idQuote,
                            customerName = customerName,
                            createdDate = createdDate,
                            issuedBy = issuedBy,
                            products = new List<Product>(),
                            customer = new Customer
                            {
                                CustomerName = customerName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Address = address,
                                Country = country,
                                City = city,
                                idCustomer = customerId,
                                Balance = customerBalance
                            }
                        };
                    }

                    quote.products.Add(new Product
                    {
                        idProduct = productID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        SellPrice = sellPrice,
                        OfferPrice = offerPrice
                    });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return quote;
        }

        /// <summary>
        ///     Given the quote ID deletes the quote from the Database
        /// </summary>
        /// <param name="quoteID"></param>
        public static void deleteQuote(int quoteID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM QuoteProduct WHERE idQuote = " + quoteID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Quote WHERE idQuote = " + quoteID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the quote object inserts it in to the database
        /// </summary>
        /// <param name="quote"></param>
        public static void insertQuote(Quote quote)
        {
            try
            {
                //insert invoice 
                var query =
                    "INSERT INTO Quote (idQuote, idCustomer, CreatedDate, IssuedBy) Values (@idQuote, @idCustomer, @CreatedDate, @IssuedBy)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idQuote", quote.idQuote);
                    cmd.Parameters.AddWithValue("@idCustomer", quote.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@CreatedDate", quote.createdDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", quote.issuedBy);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                var sCommand = new StringBuilder("INSERT INTO QuoteProduct (idQuote, idProduct, Price) VALUES ");
                var Rows = new List<string>();

                foreach (var p in quote.products)
                    Rows.Add(string.Format("('{0}','{1}','{2}')",
                        MySqlHelper.EscapeString(quote.idQuote.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.OfferPrice.ToString().Replace(",", "."))
                    ));

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (var myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the old and updated quote update the quote
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="old_quote"></param>
        public static void updateQuote(Quote quote, Quote old_quote)
        {
            try
            {
                //update Order 
                var query =
                    "UPDATE Quote SET idQuote=@idQuote, CreatedDate=@CreatedDate, IssuedBy=@IssuedBy  WHERE idQuote=@idQuote ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idQuote", quote.idQuote);
                    cmd.Parameters.AddWithValue("@idCustomer", quote.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@CreatedDate", quote.createdDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", quote.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //delete old Order products
                var query_delete_invoiceProducts = "DELETE from QuoteProduct WHERE idQuote=@idQuote;";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idQuote", old_quote.idQuote);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                var sCommand = new StringBuilder("INSERT INTO QuoteProduct (idQuote, idProduct, Price) VALUES ");
                var Rows = new List<string>();

                foreach (var p in quote.products)
                    Rows.Add(string.Format("('{0}','{1}','{2}')",
                        MySqlHelper.EscapeString(quote.idQuote.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.OfferPrice.ToString().Replace(",", "."))
                    ));

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (var myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns the latest quote ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestQuoteID()
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idQuote FROM Quote ORDER BY idQuote DESC LIMIT 1", conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                return idInvoice;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the id checks if a quote exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool quoteExists(int id)
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idQuote FROM Quote where idQuote=" + id, conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                return idInvoice == 0 ? false : true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
    }
}