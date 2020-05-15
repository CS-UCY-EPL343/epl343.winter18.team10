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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class ExpensesViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the expenses
        /// </summary>
        public ExpensesViewModel()
        {
            expensesList = new List<Expense>();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM `Expense`", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var exp = new Expense();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idExpense = dataRow.Field<int>("idExpense");
                    var company = dataRow.Field<string>("CompanyName");
                    var category = dataRow.Field<string>("Category");
                    var totalCost = dataRow.Field<float>("TotalCost");
                    var isPaid = dataRow.Field<bool>("IsPaid");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");

                    exp = new Expense
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Expense> expensesList { get; set; }

        /// <summary>
        ///     Given the expense ID deletes the expense from the Database
        /// </summary>
        /// <param name="expenseID"></param>
        public static void deleteExpense(int expenseID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM ExpensePayment WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Expense WHERE idExpense = " + expenseID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the expense ID expense the receipt and returns it
        /// </summary>
        /// <param name="expenseID"></param>
        /// <returns></returns>
        public static Expense getExpense(int expenseID)
        {
            var exp = new Expense();

            try
            {
                var cmd = new MySqlCommand(
                    "SELECT * FROM `Expense` LEFT JOIN `ExpensePayment` ON `Expense`.`idExpense` = `ExpensePayment`.`idExpense`" +
                    " WHERE `Expense`.`idExpense` = " + expenseID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    exp = null;

                var count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idExpense = dataRow.Field<int>("idExpense");
                    var invoiceNo = dataRow.Field<int>("InvoiceNo");
                    var company = dataRow.Field<string>("CompanyName");
                    var contactDetails = dataRow.Field<int>("PhoneNumber");
                    var category = dataRow.Field<string>("Category");
                    var description = dataRow.Field<string>("Description");
                    var issuedBy = dataRow.Field<string>("IssuedBy");
                    var cost = dataRow.Field<float>("Cost");
                    var vat = dataRow.Field<float>("VAT");
                    var totalCost = dataRow.Field<float>("TotalCost");
                    var isPaid = dataRow.Field<bool>("IsPaid");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");

                    var paymentID = dataRow.Field<int>("idPayment");
                    var amount = dataRow.Field<float>("Amount");
                    var method =
                        (PaymentMethod) Enum.Parse(typeof(PaymentMethod), dataRow.Field<string>("PaymentMethod"));
                    var paymentDate = dataRow.Field<DateTime>("PaymentDate");
                    var paymentNumber = dataRow.Field<string>("PaymentNumber");

                    if (count == 0)
                    {
                        count++;
                        exp = new Expense
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

                    exp.payments.Add(new Payment
                    {
                        idPayment = paymentID,
                        amount = amount,
                        paymentMethod = method,
                        paymentNumber = paymentNumber,
                        paymentDate = paymentDate
                    });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return exp;
        }

        /// <summary>
        ///     Returns total expenses for specific months and year
        /// </summary>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static float getTotalExpensesMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getExpensesByMonthYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = months;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
                var total2 = cmd.ExecuteScalar().ToString();
                float total3 = 0;
                if (float.TryParse(total2, out total3)) total = total3;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        /// <summary>
        ///     Given the expense object inserts it in to the database
        /// </summary>
        /// <param name="expence"></param>
        public static void insertExpens(Expense expence)
        {
            try
            {
                //insert invoice 
                var query =
                    "INSERT INTO Expense (idExpense,CompanyName,Category,PhoneNumber,Description,InvoiceNo,CreatedDate,Cost,VAT,TotalCost,IsPaid,IssuedBy)" +
                    " Values (@idExpense,@CompanyName,@Category,@PhoneNumber,@Description,@InvoiceNo,@CreatedDate,@Cost,@VAT,@TotalCost,@IsPaid,@IssuedBy)";

                using (var cmd = new MySqlCommand(query, conn))
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
                    cmd.Parameters.AddWithValue("@VAT", expence.VAT / 100);
                    cmd.Parameters.AddWithValue("@TotalCost", expence.totalCost);
                    cmd.Parameters.AddWithValue("@IsPaid", expence.isPaid);
                    cmd.Parameters.AddWithValue("@IssuedBy", expence.issuedBy);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert product
                var sCommand =
                    new StringBuilder(
                        "INSERT INTO ExpensePayment ( idExpense, PaymentMethod, Amount, PaymentNumber,PaymentDate) VALUES ");
                var Rows = new List<string>();
                foreach (var expe in expence.payments)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(expence.idExpense.ToString()),
                        MySqlHelper.EscapeString(expe.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(expe.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(expe.paymentNumber),
                        MySqlHelper.EscapeString(expe.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss",
                            CultureInfo.InvariantCulture))));
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (var myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns the latest expense ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestExpenseID()
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idExpense FROM Expense ORDER BY idExpense DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    idInvoice = 0;

                return idInvoice;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the id checks if a expense exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool expenseExists(int id)
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idExpense FROM Expense where idExpense=" + id, conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    idInvoice = 0;

                return idInvoice == 0 ? false : true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        /// <summary>
        ///     Given the old and updated expense update the expense
        /// </summary>
        /// <param name="expense"></param>
        /// <param name="old_expense"></param>
        public static void updateExpense(Expense expense, Expense old_expense)
        {
            try
            {
                //update Expense 
                var query =
                    "UPDATE Expense SET CompanyName=@CompanyName,Category=@Category,PhoneNumber=@PhoneNumber,Description=@Description,InvoiceNo=@InvoiceNo,CreatedDate=@CreatedDate,Cost=@Cost,VAT=@VAT,TotalCost=@TotalCost,IsPaid=@IsPaid,IssuedBy=@IssuedBy WHERE idExpense=@idExpense ";

                using (var cmd = new MySqlCommand(query, conn))
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
                var queryDelete = "DELETE from ExpensePayment WHERE idExpense=@idExpense; ";

                using (var cmd = new MySqlCommand(queryDelete, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idExpense", old_expense.idExpense);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert payents
                var sCommand =
                    new StringBuilder(
                        "INSERT INTO ExpensePayment ( idExpense, PaymentMethod, Amount, PaymentNumber,PaymentDate) VALUES ");
                var Rows = new List<string>();
                foreach (var expe in expense.payments)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(expense.idExpense.ToString()),
                        MySqlHelper.EscapeString(expe.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(expe.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(expe.paymentNumber),
                        MySqlHelper.EscapeString(expe.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss",
                            CultureInfo.InvariantCulture))));
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (var myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}