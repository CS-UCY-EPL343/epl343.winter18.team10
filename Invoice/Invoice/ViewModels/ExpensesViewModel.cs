using InvoiceX.Classes;
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
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public ExpensesViewModel()
        {
            expensesList = new List<Expense>();

            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void deleteExpense(int expenseID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM ExpensePayment WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Expense WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Expense getExpense(int expenseID)
        {
            Expense exp = new Expense();

            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return exp;
        }
        public static float getTotalExpensesMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                MySqlCommand cmd = new MySqlCommand("getExpensesByMonthYear", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = months;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year "].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                String total2 = cmd.ExecuteScalar().ToString();
                float total3 = 0;
                if (float.TryParse(total2, out total3))
                {
                    total = total3;
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

    }
}
