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
    class UserViewModel
    {
        public List<Users> UsersList { get; set; }
        public UserViewModel()
        {
            UsersList = new List<Users>();

            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM User", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                
                foreach (DataRow dataRow in dt.Rows)
                {
                    var NameDB = dataRow.Field<string>("idUser");
                    var PasswordDB = dataRow.Field<string>("Password_p");
                    var AdminDB = dataRow.Field<bool>("AdminPrivileges");

                    UsersList.Add(
                        new Users()
                        {
                            UserName = NameDB,
                            UserPassword = PasswordDB,
                            admin = AdminDB
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


