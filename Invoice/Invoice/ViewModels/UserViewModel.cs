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

using System.Collections.Generic;
using System.Data;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    internal class UserViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the users
        /// </summary>
        public UserViewModel()
        {
            UsersList = new List<User>();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM User", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var NameDB = dataRow.Field<string>("idUser");
                    var PasswordDB = dataRow.Field<string>("Hash");
                    var AdminDB = dataRow.Field<bool>("AdminPrivileges");

                    UsersList.Add(
                        new User
                        {
                            username = NameDB,
                            hash = PasswordDB,
                            admin = AdminDB
                        });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<User> UsersList { get; set; }

        /// <summary>
        ///     Given the username retrieves the user and returns him
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static User getUserByUsername(string username)
        {
            var user = new User();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM User WHERE idUser = \"" + username + "\"", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count != 0)
                    return new User
                    {
                        username = dt.Rows[0].Field<string>("idUser"),
                        hash = dt.Rows[0].Field<string>("Hash"),
                        salt = dt.Rows[0].Field<string>("Salt"),
                        admin = dt.Rows[0].Field<bool>("AdminPrivileges")
                    };
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return user;
        }

        /// <summary>
        ///     Given the username, hash and salt inserts the user to the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hash"></param>
        /// <param name="salt"></param>
        /// <param name="admin_privileges"></param>
        public static void insertUser(string username, string hash, string salt, bool admin_privileges)
        {
            try
            {
                var query =
                    "INSERT INTO User (idUser, Hash, Salt, AdminPrivileges) Values (@idUser, @Hash, @Salt, @AdminPrivileges)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idUser", username);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@AdminPrivileges", admin_privileges);

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
        ///     Given the username deletes the user from the Database
        /// </summary>
        /// <param name="username"></param>
        public static void deleteUser(string username)
        {
            try
            {
                var query = "DELETE FROM User WHERE idUser = \"" + username + "\"";

                var cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}