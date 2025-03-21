﻿// /*****************************************************************************
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

using MySql.Data.MySqlClient;

namespace InvoiceX.Classes
{
    /// <summary>
    ///     The singleton class that contains the MySqlConnection object so that
    ///     only one instance of it exists in the entire program.
    /// </summary>
    public sealed class DBConnection
    {
        private static DBConnection instance;

        private readonly string myConnectionString = "server=localhost;uid=root;" +
                                                     "pwd=;database=invoice;charset=utf8";

        public MySqlConnection Connection;

        private DBConnection()
        {
            try
            {
                Connection = new MySqlConnection(myConnectionString);
                Connection.Open();
            }
            catch (MySqlException ex)
            {
            }
        }

        public static DBConnection Instance
        {
            get
            {
                if (instance == null) instance = new DBConnection();
                return instance;
            }
        }

        ~DBConnection() // finalizer
        {
            Connection.Close();
        }
    }
}