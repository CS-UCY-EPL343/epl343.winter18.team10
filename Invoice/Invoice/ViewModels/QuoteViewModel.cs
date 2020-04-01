using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceX.ViewModels
{
    public class QuoteViewModel
    {
        public List<Quote> quoteList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public QuoteViewModel()
        {
            quoteList = new List<Quote>();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `Quote`.*, `Customer`.`CustomerName` FROM `Quote`" +
                    " LEFT JOIN `Customer` ON `Quote`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Quote quote = new Quote();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idQuote = dataRow.Field<Int32>("idQuote");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    quote = new Quote()
                    {
                        idQuote = idQuote,
                        customerName = customer,
                        createdDate = createdDate,
                        issuedBy = issuedBy
                    };

                    quoteList.Add(quote);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Quote getQuote(int quoteID)
        {
            Quote quote = new Quote();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewQuote WHERE QuoteID = " + quoteID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    quote = null;

                int count = 0;
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

                    var idQuote = dataRow.Field<Int32>("QuoteID");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<Int32>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var sellPrice = dataRow.Field<float>("SellPrice");
                    var offerPrice = dataRow.Field<float>("OfferPrice");

                    if (count == 0)
                    {
                        count++;
                        quote = new Quote()
                        {
                            idQuote = idQuote,
                            customerName = customerName,
                            createdDate = createdDate,
                            issuedBy = issuedBy,
                            products = new List<Product>(),
                            customer = new Customer()
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

                    quote.products.Add(new Product()
                    {
                        idProduct = productID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        SellPrice = sellPrice,
                        OfferPrice = offerPrice
                    });
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return quote;
        }

        public static void deleteQuote(int quoteID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM QuoteProduct WHERE idQuote = " + quoteID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Quote WHERE idQuote = " + quoteID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void insertQuote(Quote quote)
        {
            try
            {
                //insert invoice 
                string query = "INSERT INTO Quote (idQuote, idCustomer, CreatedDate, IssuedBy) Values (@idQuote, @idCustomer, @CreatedDate, @IssuedBy)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
                StringBuilder sCommand = new StringBuilder("INSERT INTO QuoteProduct (idQuote, idProduct, Price) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in quote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}')",
                        MySqlHelper.EscapeString(quote.idQuote.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.OfferPrice.ToString().Replace(",", "."))
                       ));
                }

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void updateQuote(Quote quote, Quote old_quote)
        {
            try
            {
                //update Order 
                string query = "UPDATE Quote SET idQuote=@idQuote, CreatedDate=@CreatedDate, IssuedBy=@IssuedBy  WHERE idQuote=@idQuote ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
                string query_delete_invoiceProducts = "DELETE from QuoteProduct WHERE idQuote=@idQuote;";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idQuote", old_quote.idQuote);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO QuoteProduct (idQuote, idProduct, Price) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in quote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}')",
                        MySqlHelper.EscapeString(quote.idQuote.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.OfferPrice.ToString().Replace(",", "."))
                       ));
                }

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int returnLatestQuoteID()
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idQuote FROM Quote ORDER BY idQuote DESC LIMIT 1", conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                return idInvoice;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        public static bool quoteExists(int id)
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idQuote FROM Quote where idQuote=" + id.ToString(), conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                return ((idInvoice == 0) ? false : true);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }
    }
}
