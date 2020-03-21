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
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Expense`", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Expense exp = new Expense();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idExpense = dataRow.Field<Int32>("idExpense");
                    var company = dataRow.Field<string>("CompanyName");
                    var category = dataRow.Field<string>("Category");
                    var totalCost = dataRow.Field<float>("TotalCost");
                    var isPaid = dataRow.Field<bool>("IsPaid");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");

                    exp = new Expense()
                    {
                        idExpense = idExpense,
                        companyName = company,
                        category = category,
                        totalCost = totalCost,
                        isPaid = isPaid,
                        createdDate = createdDate
                    };

                    expensesList.Add(exp);
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void deleteExpenseByID(int expenseID)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM ExpensePayment WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Expense WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static Expense getExpenseByID(int expenseID)
        {
            MySqlConnection conn;
            Expense exp = new Expense();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Expense` LEFT JOIN `ExpensePayment` ON `Expense`.`idExpense` = `ExpensePayment`.`idExpense`" +
                    " WHERE `Expense`.`idExpense` = " + expenseID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    exp = null;

                int count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idExpense = dataRow.Field<Int32>("idExpense");
                    var invoiceNo = dataRow.Field<Int32>("InvoiceNo");
                    var company = dataRow.Field<string>("CompanyName");
                    var contactDetails = dataRow.Field<Int32>("PhoneNumber");
                    var category = dataRow.Field<string>("Category");
                    var description = dataRow.Field<string>("Description");
                    var issuedBy = dataRow.Field<string>("IssuedBy");
                    var cost = dataRow.Field<float>("Cost");
                    var vat = dataRow.Field<float>("VAT");
                    var totalCost = dataRow.Field<float>("TotalCost");
                    var isPaid = dataRow.Field<bool>("IsPaid");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");

                    var paymentID = dataRow.Field<Int32>("idPayment");
                    var amount = dataRow.Field<float>("Amount");
                    var method = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), dataRow.Field<string>("PaymentMethod"));
                    var paymentDate = dataRow.Field<DateTime>("PaymentDate");
                    var paymentNumber = dataRow.Field<string>("PaymentNumber");

                    if (count == 0)
                    {
                        count++;
                        exp = new Expense()
                        {
                            idExpense = idExpense,
                            contactDetails = contactDetails,
                            description = description,
                            invoiceNo = invoiceNo,
                            issuedBy = issuedBy,
                            cost = cost,
                            VAT = vat,
                            companyName = company,
                            category = category,
                            totalCost = totalCost,
                            isPaid = isPaid,
                            createdDate = createdDate,
                            payments = new List<Payment>()
                        };
                    }

                    exp.payments.Add(new Payment()
                    {
                        idPayment = paymentID,
                        amount = amount,
                        paymentMethod = method,
                        paymentNumber = paymentNumber,
                        paymentDate = paymentDate
                    });
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return exp;
        }

    }
}
