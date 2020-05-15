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
using System.Data;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using InvoiceX.Classes;
using InvoiceX.ViewModels;

namespace InvoiceX
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtUsername.Focus();
        }

        /// <summary>
        ///     Checks if the user exists and matches the given password.
        ///     If true then loads the main window of the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            var conn = DBConnection.Instance.Connection;
            if (conn.State == ConnectionState.Open)
            {
                var user = UserViewModel.getUserByUsername(txtUsername.Text);

                if (user.username != null)
                {
                    var isPasswordMatched = VerifyPassword(txtPassword.Password, user.hash, user.salt);

                    if (isPasswordMatched)
                    {
                        //Login Successfull
                        var mainWindow = new MainWindow(user);
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        //Login Failed
                        MessageBox.Show("Password is invalid");
                    }
                }
                else
                {
                    MessageBox.Show("Username is invalid");
                }
            }
            else
            {
                MessageBox.Show("Could not connect to the Database. Check database status or contact administrator.");
            }
        }

        /// <summary>
        ///     Verifies that the password given matches the one on the database
        /// </summary>
        /// <param name="enteredPassword"></param>
        /// <param name="storedHash"></param>
        /// <param name="storedSalt"></param>
        /// <returns></returns>
        private bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        /// <summary>
        ///     Method that handles the event Key Down on the textbox password.
        ///     If the key pressed is Enter then it attempts to login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) btn_login_Click(new object(), new RoutedEventArgs());
        }
    }
}