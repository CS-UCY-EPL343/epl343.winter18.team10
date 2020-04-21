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

        public static void insertExpens(Expense expence)
        {
            try
            {
                //insert invoice 
                string query = "INSERT INTO Expense (idExpense,CompanyName,Category,PhoneNumber,Description,InvoiceNo,CreatedDate,Cost,VAT,TotalCost,IsPaid,IssuedBy)" +
                    " Values (@idExpense,@CompanyName,@Category,@PhoneNumber,@Description,@InvoiceNo,@CreatedDate,@Cost,@VAT,@TotalCost,@IsPaid,@IssuedBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idExpense", expence.idExpense);
                    cmd.Parameters.AddWithValue("@CompanyName", expence.companyName);
                    cmd.Parameters.AddWithValue("@Category", expence.category);
                    cmd.Parameters.AddWithValue("@PhoneNumber", expence.contactDetails);
                    cmd.Parameters.AddWithValue("@Description", expence.description);
                    cmd.Parameters.AddWithValue("@InvoiceNo", expence.invoiceNo);
                    cmd.Parameters.AddWithValue("@CreatedDate", expence.createdDate);
                    cmd.Parameters.AddWithValue("@Cost", expence.cost);
                    cmd.Parameters.AddWithValue("@VAT", expence.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", expence.totalCost);
                    cmd.Parameters.AddWithValue("@IsPaid", expence.isPaid);
                    cmd.Parameters.AddWithValue("@IssuedBy", expence.issuedBy);
                   
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert product
                StringBuilder sCommand = new StringBuilder("INSERT INTO ExpensePayment ( idExpense, PaymentMethod, Amount, PaymentNumber,PaymentDate) VALUES ");
                List<string> Rows = new List<string>();    
                foreach (Payment expe in expence.payments)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                       
                        MySqlHelper.EscapeString(expence.idExpense.ToString()),
                        MySqlHelper.EscapeString(expe.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(expe.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(expe.paymentNumber.ToString()),
                        MySqlHelper.EscapeString(expe.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture))));                 
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }             
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        public static int returnLatestExpenseID()
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idExpense FROM Expense ORDER BY idExpense DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    idInvoice = 0;

                return idInvoice;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }

        public static bool expenseExists(int id)
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySqlCommand("SELECT idExpense FROM Expense where idExpense=" + id.ToString(), conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    idInvoice = 0;

                return ((idInvoice == 0) ? false : true);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        public static void updateExpense(Expense expense, Expense old_expense)
        {
            try
            {
                //update Expense 
                string query = "UPDATE Expense SET CompanyName=@CompanyName,Category=@Category,PhoneNumber=@PhoneNumber,Description=@Description,InvoiceNo=@InvoiceNo,CreatedDate=@CreatedDate,Cost=@Cost,VAT=@VAT,TotalCost=@TotalCost,IsPaid=@IsPaid,IssuedBy=@IssuedBy WHERE idExpense=@idExpense ";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idExpense", expense.idExpense);
                    cmd.Parameters.AddWithValue("@CompanyName", expense.companyName);
                    cmd.Parameters.AddWithValue("@Category", expense.category);
                    cmd.Parameters.AddWithValue("@PhoneNumber", expense.contactDetails);
                    cmd.Parameters.AddWithValue("@Description", expense.description);
                    cmd.Parameters.AddWithValue("@InvoiceNo", expense.invoiceNo);
                    cmd.Parameters.AddWithValue("@CreatedDate", expense.createdDate);
                    cmd.Parameters.AddWithValue("@Cost", expense.cost);
                    cmd.Parameters.AddWithValue("@VAT", expense.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", expense.totalCost);
                    cmd.Parameters.AddWithValue("@IsPaid", expense.issuedBy);
                    cmd.Parameters.AddWithValue("@IssuedBy", expense.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                
                //delete old Expense payments
                string queryDelete = "DELETE from ExpensePayments WHERE idExpense=@idExpense; ";

                using (MySqlCommand cmd = new MySqlCommand(queryDelete, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idExpense", old_expense.idExpense);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert payents
                StringBuilder sCommand = new StringBuilder("INSERT INTO ExpensePayment ( idExpense, PaymentMethod, Amount, PaymentNumber,PaymentDate) VALUES ");
                List<string> Rows = new List<string>();
                foreach (Payment expe in expense.payments)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",

                        MySqlHelper.EscapeString(expense.idExpense.ToString()),
                        MySqlHelper.EscapeString(expe.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(expe.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(expe.paymentNumber.ToString()),
                        MySqlHelper.EscapeString(expe.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture))));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
