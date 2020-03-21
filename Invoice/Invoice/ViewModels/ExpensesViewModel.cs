using InvoiceX.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InvoiceX.ViewModels
{
    public class ExpensesViewModel
    {
        public List<Expense> expensesList { get; set; }
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public ExpensesViewModel()
        {
            expensesList = new List<Expense>();
            //MySqlConnection conn;

            //try
            //{
            //    conn = new MySqlConnection(myConnectionString);
            //    conn.Open();
            //    MySqlCommand cmd = new MySqlCommand("SELECT `Receipt`.*, `Customer`.`CustomerName` FROM `Receipt`" +
            //        " LEFT JOIN `Customer` ON `Receipt`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
            //    DataTable dt = new DataTable();
            //    dt.Load(cmd.ExecuteReader());

            //    Receipt rec = new Receipt();
            //    foreach (DataRow dataRow in dt.Rows)
            //    {
            //        var customer = dataRow.Field<string>("CustomerName");
            //        var idReceipt = dataRow.Field<Int32>("idReceipt");
            //        var amount = dataRow.Field<float>("Amount");
            //        var createdDate = dataRow.Field<DateTime>("IssuedDate");
            //        var issuedBy = dataRow.Field<string>("IssuedBy");

            //        rec = new Expense()
            //        {
            //            idReceipt = idReceipt,
            //            totalAmount = amount,
            //            customerName = customer,
            //            createdDate = createdDate,
            //            issuedBy = issuedBy
            //        };

            //        expensesList.Add(rec);
            //    }
            //    conn.Close();
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            //}
        }

        public static void deleteExpenseByID(int expenseID)
        {
            //MySqlConnection conn;

            //try
            //{
            //    conn = new MySqlConnection(myConnectionString);
            //    conn.Open();
            //    MySqlCommand cmd = new MySqlCommand("DELETE FROM Payments WHERE idReceipt = " + expenseID, conn);
            //    cmd.ExecuteNonQuery();

            //    cmd = new MySqlCommand("DELETE FROM Receipt WHERE idReceipt = " + expenseID, conn);
            //    cmd.ExecuteNonQuery();

            //    conn.Close();
            //}
            //catch (MySql.Data.MySqlClient.MySqlException ex)
            //{
            //    MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            //}
        }

    }
}
