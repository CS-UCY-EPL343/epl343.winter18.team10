using InvoiceX.Classes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceX.ViewModels
{
    public static class DatabaseViewModel
    {
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public static bool exportDatabase(string filename)
        {
            bool success = true;

            using (MySqlCommand cmd = new MySqlCommand())
            {
                using (MySqlBackup mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;                    
                    mb.ExportToFile(filename);
                }
            }

            return success;
        }

        public static bool importDatabase(string filename)
        {
            bool success = true;

            using (MySqlCommand cmd = new MySqlCommand())
            {
                using (MySqlBackup mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = conn;
                    mb.ImportFromFile(filename);
                }
            }

            return success;
        }

        public static bool exportDatabaseAsCSV(string filename)
        {
            bool success = true;
           
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename))
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

        private static DataTable getTable(string tableName)
        {
            DataTable dt = new DataTable();            
            
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `" + tableName + "`", conn))
            {
                dt.Load(cmd.ExecuteReader());
            }

            return dt;
        }

        public static string ConvertDataTableToString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                sb.Append(dt.Columns[k].ColumnName + ',');
            }
            sb.Append("\r\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    sb.Append(dt.Rows[i][k].ToString().Replace(",", ".") + ',');
                }
                sb.Append("\r\n");
            }

            return sb.ToString();
        }
    }
}
