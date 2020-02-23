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
    public class CustomerViewModel
    {
        public List<Customer> CustomersList { get; set; }

        public CustomerViewModel()
        {
            CustomersList = new List<Customer>();
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
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

                    CustomersList.Add(
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

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }


        public static void SendCustomerToDB(Customer customer)
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

                conn.Close();
                MessageBox.Show("Customer addet to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void UpdateCustomerToDB(Customer customer)
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

                conn.Close();
                MessageBox.Show("Customer was Updated");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static int ReturnLatestCustomerID()
        {
            int id_return = 0;
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            try
            {
                string idCustomer;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idCustomer FROM Customer ORDER BY idCustomer DESC LIMIT 1", conn);
                conn.Open();
                id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idCustomer = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idCustomer = "";

                conn.Close();
                return Convert.ToInt32(idCustomer);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return 0;
        }

        public static Customer ReturnCustomerByid(int customerid)
        {

            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            Customer customer = new Customer();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM Customer WHERE idCustomer=" + customerid, conn);
                conn.Open();
                var custome = cmd.ExecuteReader();

                // Now check if any rows returned.
                if (custome.HasRows)
                {
                    custome.Read();// Get first record.                     
                    customer.idCustomer = customerid; //get  values of first row
                    customer.CustomerName = custome.GetString(1);
                    customer.PhoneNumber = custome.GetInt32(2);
                    customer.Email = custome.GetString(3);
                    customer.Country = custome.GetString(4);
                    customer.City = custome.GetString(5);
                    customer.Address = custome.GetString(6);
                    customer.Balance = custome.GetFloat(7);
                    
                }
                custome.Close();// Close reader.


                conn.Close();
                //return Convert.ToInt32(idInvoice);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return customer;
        }

        public static void deleteCustomerByID(int customerID)
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Customer WHERE idCustomer = " + customerID, conn);
                cmd.ExecuteNonQuery();                

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

    }

}

