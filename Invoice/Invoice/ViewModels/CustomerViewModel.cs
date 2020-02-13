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
        public List<Customers> CustomersList { get; set; }
        public CustomerViewModel()
        {
            CustomersList = new List<Customers>();



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
                        new Customers()
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






    }

}

