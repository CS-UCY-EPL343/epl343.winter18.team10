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

using System.Data;
using System.IO;
using System.Text;
using InvoiceX.Classes;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public static class DatabaseViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Exports the database as SQL script to the filename specified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool exportDatabase(string filename)
        {
            var success = true;

            using (var cmd = new MySqlCommand())
            {
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;
                    mb.ExportToFile(filename);
                }
            }

            return success;
        }

        /// <summary>
        ///     Import database from SQL script from filename specified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool importDatabase(string filename)
        {
            var success = true;

            using (var cmd = new MySqlCommand())
            {
                using (var mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;
                    mb.ImportFromFile(filename);
                }
            }

            return success;
        }

        /// <summary>
        ///     Exports the database as CSV file to the filename specified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool exportDatabaseAsCSV(string filename)
        {
            var success = true;

            using (var file = new StreamWriter(filename))
            {
                file.WriteLine(ConvertDataTableToString(getTable("Invoice")));
                file.WriteLine(ConvertDataTableToString(getTable("InvoiceProduct")));
                file.WriteLine(ConvertDataTableToString(getTable("CreditNote")));
                file.WriteLine(ConvertDataTableToString(getTable("CreditNoteProduct")));
                file.WriteLine(ConvertDataTableToString(getTable("Order")));
                file.WriteLine(ConvertDataTableToString(getTable("OrderProduct")));
                file.WriteLine(ConvertDataTableToString(getTable("Quote")));
                file.WriteLine(ConvertDataTableToString(getTable("QuoteProduct")));
                file.WriteLine(ConvertDataTableToString(getTable("Customer")));
                file.WriteLine(ConvertDataTableToString(getTable("Product")));
                file.WriteLine(ConvertDataTableToString(getTable("Receipt")));
                file.WriteLine(ConvertDataTableToString(getTable("Payment")));
                file.WriteLine(ConvertDataTableToString(getTable("Expense")));
                file.WriteLine(ConvertDataTableToString(getTable("ExpensePayment")));
                file.WriteLine(ConvertDataTableToString(getTable("User")));
            }

            return success;
        }

        /// <summary>
        ///     Given a tableName returns the table from the database
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static DataTable getTable(string tableName)
        {
            var dt = new DataTable();

            using (var cmd = new MySqlCommand("SELECT * FROM `" + tableName + "`", conn))
            {
                dt.Load(cmd.ExecuteReader());
            }

            return dt;
        }

        /// <summary>
        ///     Converts a datatable given to string
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDataTableToString(DataTable dt)
        {
            var sb = new StringBuilder();
            for (var k = 0; k < dt.Columns.Count; k++) sb.Append(dt.Columns[k].ColumnName + ',');
            sb.Append("\r\n");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                for (var k = 0; k < dt.Columns.Count; k++) sb.Append(dt.Rows[i][k].ToString().Replace(",", ".") + ',');
                sb.Append("\r\n");
            }

            return sb.ToString();
        }
    }
}