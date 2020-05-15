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
    }
}