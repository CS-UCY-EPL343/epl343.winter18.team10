using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class CustomerViewModel
    {
        public List<Customer> customersList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public CustomerViewModel()
        {
            customersList = new List<Customer>();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Customer", conn);
                DataTable dt = new DataTable();
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
                        new Customer()
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public static void insertCustomer(Customer customer)
        {
            try
            {
                //insert Invoice 
                string query = "INSERT INTO Customer (CustomerName,PhoneNumber,Email,Country,City,Address,Balance) Values (@CustomerName,@PhoneNumber,@Email,@Country,@City,@Address,@Balance)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateCustomer(Customer customer)
        {
            try
            {
                //insert Invoice 
                string query = "UPDATE Customer SET CustomerName=@CustomerName,PhoneNumber=@PhoneNumber,Email=@Email,Country=@Country,City=@City,Address=@Address,Balance=@Balance  WHERE idCustomer=@idCustomer";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static int returnLatestCustomerID()
        {
            try
            {
                string idCustomer;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idCustomer FROM Customer ORDER BY idCustomer DESC LIMIT 1", conn);

                int id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idCustomer = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idCustomer = "";

                return Convert.ToInt32(idCustomer);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        public static Customer getCustomer(int customerid)
        {
            Customer customer = new Customer();
            try
            {
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM Customer WHERE idCustomer=" + customerid, conn);
                var queryResult = cmd.ExecuteReader();

                // Now check if any rows returned.
                if (queryResult.HasRows)
                {
                    queryResult.Read();// Get first record.                     
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return customer;
        }

        public static void deleteCustomer(int customerID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Customer WHERE idCustomer = " + customerID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static float getTotalSalesMonthYear(int id, int months, int year)
        {
            float total = 0;

            try
            {
                MySqlCommand cmd = new MySqlCommand("getCustomerSalesByMonthYear", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@customerId", SqlDbType.Int).Value = id;
                cmd.Parameters["@customerId"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = months;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
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

