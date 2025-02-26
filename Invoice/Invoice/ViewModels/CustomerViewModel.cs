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
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using LiveCharts;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class CustomerViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the customers
        /// </summary>
        public CustomerViewModel()
        {
            customersList = new List<Customer>();
            lastInvoiceOfCustomer = new List<Invoice>();
            lastInvoice2OfCustomer = new List<Invoice>();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM Customer", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idCustomerDB = dataRow.Field<int>("idCustomer");
                    var CustomerNameDB = dataRow.Field<string>("CustomerName");
                    var PhoneNumberDB = dataRow.Field<int>("PhoneNumber");
                    var EmailDB = dataRow.Field<string>("Email");
                    var CountryDB = dataRow.Field<string>("Country");
                    var CityDB = dataRow.Field<string>("City");
                    var AddressDB = dataRow.Field<string>("Address");
                    var BalanceDB = dataRow.Field<float>("Balance");

                    customersList.Add(
                        new Customer
                        {
                            idCustomer = idCustomerDB,
                            CustomerName = CustomerNameDB,
                            PhoneNumber = PhoneNumberDB,
                            Email = EmailDB,
                            Country = CountryDB,
                            City = CityDB,
                            Address = AddressDB,
                            Balance = BalanceDB
                        });
                    Invoice[] invoices = getLast2InvoicesOfCustomer(idCustomerDB);
                    lastInvoice2OfCustomer.Add(invoices[0]);
                    if (invoices[1] != null)
                    {
                        lastInvoiceOfCustomer.Add(invoices[1]);

                    }
                    else
                    {
                        lastInvoiceOfCustomer.Add(invoices[0]);

                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Customer> customersList { get; set; }

        /// <summary>
        ///     Given the customer object inserts it in to the database
        /// </summary>
        /// <param name="customer"></param>
        public static void insertCustomer(Customer customer)
        {
            try
            {
                //insert Invoice 
                var query =
                    "INSERT INTO Customer (CustomerName,PhoneNumber,Email,Country,City,Address,Balance) Values (@CustomerName,@PhoneNumber,@Email,@Country,@City,@Address,@Balance)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Country", customer.Country);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Balance", customer.Balance);
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
        ///     Given the customer updates the customer
        /// </summary>
        /// <param name="customer"></param>
        public static void updateCustomer(Customer customer)
        {
            try
            {
                //insert Invoice 
                var query =
                    "UPDATE Customer SET CustomerName=@CustomerName,PhoneNumber=@PhoneNumber,Email=@Email,Country=@Country,City=@City,Address=@Address,Balance=@Balance  WHERE idCustomer=@idCustomer";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Country", customer.Country);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@Balance", customer.Balance);
                    cmd.Parameters.AddWithValue("@idCustomer", customer.idCustomer);
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
        ///     Returns the latest customer ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestCustomerID()
        {
            try
            {
                string idCustomer;
                var cmd = new MySqlCommand("SELECT idCustomer FROM Customer ORDER BY idCustomer DESC LIMIT 1", conn);

                var id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idCustomer = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idCustomer = "";

                return Convert.ToInt32(idCustomer);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the customer ID retrieves the customer and returns it
        /// </summary>
        /// <param name="customerid"></param>
        /// <returns></returns>
        public static Customer getCustomer(int customerid)
        {
            var customer = new Customer();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM Customer WHERE idCustomer=" + customerid, conn);
                var queryResult = cmd.ExecuteReader();

                // Now check if any rows returned.
                if (queryResult.HasRows)
                {
                    queryResult.Read(); // Get first record.                     
                    customer.idCustomer = customerid; //get  values of first row
                    customer.CustomerName = queryResult.GetString(1);
                    customer.PhoneNumber = queryResult.GetInt32(2);
                    customer.Email = queryResult.GetString(3);
                    customer.Country = queryResult.GetString(4);
                    customer.City = queryResult.GetString(5);
                    customer.Address = queryResult.GetString(6);
                    customer.Balance = queryResult.GetFloat(7);
                }

                queryResult.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return customer;
        }

        /// <summary>
        ///     Given the customer ID deletes the customer from the Database
        /// </summary>
        /// <param name="customerID"></param>
        public static void deleteCustomer(int customerID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM Customer WHERE idCustomer = " + customerID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                    MessageBox.Show("Cannot delete customer with ID = " + customerID +
                                    " as he is referenced in other documents.");
                else
                    MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns total customer sales for specific months and year
        /// </summary>
        /// <param name="id"></param>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static float getTotalSalesMonthYear(int id, int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getCustomerSalesByMonthYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerId", SqlDbType.Int).Value = id;
                cmd.Parameters["@customerId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = months;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
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
        public static float calculateCustomerBalance(int customerId)
        {
            float ret = getCustomerBalance(customerId);
            List<int> invoices=InvoiceViewModel.getCustomerInvoices(customerId);
            for (int i=0; i < invoices.Count; i++)
            {
                if (InvoiceViewModel.isInvoicePaid(invoices[i])==false)
                {
                    ret += InvoiceViewModel.getInvoiceCost(invoices[i]);
                }
            }
            List<int> receipts = ReceiptViewModel.getCustomerReceipts(customerId);
            for (int i = 0; i < receipts.Count; i++)
            {
                ret -= ReceiptViewModel.getReceiptAmount(receipts[i]);
            }
            List<int> creditNotes = CreditNoteViewModel.getCustomerCreditNotes(customerId);
            for (int i = 0; i < creditNotes.Count; i++)
            {
                ret -= CreditNoteViewModel.getCreditNoteCost(creditNotes[i]);

            }

            return ret;
        }
        public static float getCustomerBalance(int cid)
        {
            float ret;
            try
            {
                var cmd = new MySqlCommand("SELECT Balance FROM customer WHERE idCustomer = " + cid, conn);
                ret = float.Parse((cmd.ExecuteScalar()).ToString());
                return ret;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }
        public static string calculateCustomerBalanceDates(int customerId, DateTime from, DateTime to)
        {
            float ret = getCustomerBalance(customerId);
            List<StatementItem> invoices = InvoiceViewModel.getInvoicesForStatement(customerId,from,to);
            for (int i = 0; i < invoices.Count; i++)
            {
                if (InvoiceViewModel.isInvoicePaid(invoices[i].idItem) == false)
                {
                    ret += invoices[i].charges;
                }
            }
            List<StatementItem> receipts = ReceiptViewModel.getReceiptsForStatement(customerId, from, to);
            for (int i = 0; i < receipts.Count; i++)
            {
                ret -= receipts[i].credits;

            }
            List<StatementItem> creditNotes = CreditNoteViewModel.getCreditNotesForStatement(customerId, from, to);
            for (int i = 0; i < creditNotes.Count; i++)
            {
                ret -= creditNotes[i].charges;
            }

            return ret.ToString();
        }
        public static string[,] getTotalSalesByCity(int year)
        {
            ChartValues<float> sum = new ChartValues<float>();
            string[,] amountCity = new string[5, 2];
            try
            {
                var cmd = new MySqlCommand("getTotalSalesPerCity", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    amountCity[i, 0] = (reader["SUM(Invoice.Cost)"].ToString());
                    amountCity[i, 1] = (reader["City"].ToString());
                    i++;
                }

                reader.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return amountCity;
        }
        public static float getTotalSalesByCityAndMonth(int year, int month, string city)
        {

            float total = 0;
            try
            {
                var cmd = new MySqlCommand("getTotalSalesPerCityMonth", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = month;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters["@city"].Direction = ParameterDirection.Input;

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
        public static string[,] getTotalSalesByCategory(int year)
        {
            ChartValues<float> sum = new ChartValues<float>();
            string[,] amountCategory = new string[5,2];
            try
            {
                var cmd = new MySqlCommand("getTotalSalesPerCategory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;
                MySqlDataReader reader = cmd.ExecuteReader();
                int i = 0;
                while (reader.Read())
                {
                    amountCategory[i,0]= (reader["SUM(Invoice.Cost)"].ToString());
                    amountCategory[i,1] = (reader["category"].ToString());
                    i++;
                }

                reader.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return amountCategory;
        }
        public static float getTotalSalesByCategoryAndMonth(int year,int month,string category)
        {

            float total = 0;
            try
            {
                var cmd = new MySqlCommand("getTotalSalesPerCategoryMonth", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = month;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters["@category"].Direction = ParameterDirection.Input;
                
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
        public List<Invoice> lastInvoiceOfCustomer { get; set; }
        public List<Invoice> lastInvoice2OfCustomer { get; set; }
        
        public Invoice[] getLast2InvoicesOfCustomer(int idCustomer)
        {
            try
            {
                var cmd = new MySqlCommand("SELECT idInvoice,CreatedDate FROM invoice WHERE idCustomer = " + idCustomer+ "  ORDER BY CreatedDate DESC LIMIT 2", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                Invoice[] inv = new Invoice[2];

                int i = 0;
                    foreach (DataRow dataRow in dt.Rows)
                {
                    if (i == 0) { 

                        var idInvoice = dataRow.Field<int>("idInvoice");
                        var createdDate = dataRow.Field<DateTime>("CreatedDate");
                        inv[0] = new Invoice
                        {
                            idInvoice = idInvoice,
                            createdDate = createdDate,
                        };
                    }
                    if (i == 1) {

                        var idInvoice = dataRow.Field<int>("idInvoice");
                        var createdDate = dataRow.Field<DateTime>("CreatedDate");
                        inv[1] = new Invoice
                        {
                            idInvoice = idInvoice,
                            createdDate = createdDate,
                        };
                    }
                    i++;
                }
                return inv;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}