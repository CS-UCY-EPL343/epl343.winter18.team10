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
        public List<User> UsersList { get; set; }
        private static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                                   "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public UserViewModel()
        {
            UsersList = new List<User>();
            MySqlConnection conn;
            
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
                    var PasswordDB = dataRow.Field<string>("Hash");
                    var AdminDB = dataRow.Field<bool>("AdminPrivileges");

                    UsersList.Add(
                        new User()
                        {
                            username = NameDB,
                            hash = PasswordDB,
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

        public static User getUserByUsername(string username)
        {
            MySqlConnection conn;
            
            User user = new User();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM User WHERE idUser = \"" + username + "\"", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count != 0)
                {
                    return new User()
                    {
                        username = dt.Rows[0].Field<string>("idUser"),
                        hash = dt.Rows[0].Field<string>("Hash"),
                        salt = dt.Rows[0].Field<string>("Salt"),
                        admin = dt.Rows[0].Field<bool>("AdminPrivileges"),
                    };
                }

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return user;
        }

        public static void addUserToDB(string username, string hash, string salt, bool admin_privileges)
        {
            MySqlConnection conn;            

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                
                string query = "INSERT INTO User (idUser, Hash, Salt, AdminPrivileges) Values (@idUser, @Hash, @Salt, @AdminPrivileges)";
                
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {                    
                    cmd.Parameters.AddWithValue("@idUser", username);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@AdminPrivileges", admin_privileges);
                    
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void removeUser(string username)
        {
            MySqlConnection conn;            

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                
                string query = "DELETE FROM User WHERE idUser = \"" + username + "\"";

                MySqlCommand cmd = new MySqlCommand(query, conn);                
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


