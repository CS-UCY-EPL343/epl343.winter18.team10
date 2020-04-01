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
    class UserViewModel
    {
        public List<User> UsersList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public UserViewModel()
        {
            UsersList = new List<User>();
            
            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static User getUserByUsername(string username)
        {
            User user = new User();
            try
            {
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
                MessageBox.Show(ex.Message);
            }

            return user;
        }

        public static void insertUser(string username, string hash, string salt, bool admin_privileges)
        {
            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void deleteUser(string username)
        {
            try
            {
                string query = "DELETE FROM User WHERE idUser = \"" + username + "\"";

                MySqlCommand cmd = new MySqlCommand(query, conn);                
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}


