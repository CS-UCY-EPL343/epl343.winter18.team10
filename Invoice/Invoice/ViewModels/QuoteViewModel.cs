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
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";
        public QuoteViewModel()
        {
            quoteList = new List<Quote>();
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
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


                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static Quote getQuoteByID(int quoteID)
        {
            MySqlConnection conn;

            Quote quote = new Quote();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
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

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return quote;
        }

        public static void deleteQuoteByID(int quoteID)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Quote WHERE QuoteID = " + quoteID, conn);
                cmd.ExecuteNonQuery();

                //cmd = new MySqlCommand("DELETE FROM Invoice WHERE idInvoice = " + quoteID, conn);
                //cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }
    }
}
