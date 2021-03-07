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
    public class ReceiptViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the receipts
        /// </summary>
        public ReceiptViewModel()
        {
            receiptList = new List<Receipt>();

            try
            {
                var cmd = new MySqlCommand("SELECT `Receipt`.*, `Customer`.`CustomerName` FROM `Receipt`" +
                                           " LEFT JOIN `Customer` ON `Receipt`.`idCustomer` = `Customer`.`idCustomer`; ",
                    conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var rec = new Receipt();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idReceipt = dataRow.Field<int>("idReceipt");
                    var amount = dataRow.Field<float>("Amount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    rec = new Receipt
                    {
                        idReceipt = idReceipt,
                        totalAmount = amount,
                        customerName = customer,
                        createdDate = createdDate,
                        issuedBy = issuedBy
                    };

                    receiptList.Add(rec);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Receipt> receiptList { get; set; }

        /// <summary>
        ///     Given the receipt ID retrieves the receipt and returns it
        /// </summary>
        /// <param name="receiptID"></param>
        /// <returns></returns>
        public static Receipt getReceipt(int receiptID)
        {
            var rec = new Receipt();

            try
            {
                var cmd = new MySqlCommand("SELECT * FROM viewReceipt WHERE ReceiptID = " + receiptID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    rec = null;

                var count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customerName = dataRow.Field<string>("CustomerName");
                    var phoneNumber = dataRow.Field<int>("PhoneNumber");
                    var email = dataRow.Field<string>("Email");
                    var address = dataRow.Field<string>("Address");
                    var country = dataRow.Field<string>("Country");
                    var city = dataRow.Field<string>("City");
                    var customerId = dataRow.Field<int>("idCustomer");
                    var customerBalance = dataRow.Field<float>("Balance");

                    var idReceipt = dataRow.Field<int>("ReceiptID");
                    var totalAmount = dataRow.Field<float>("TotalAmount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var paymentID = dataRow.Field<int>("idPayment");
                    var amount = dataRow.Field<float>("Amount");
                    var method =
                        (PaymentMethod) Enum.Parse(typeof(PaymentMethod), dataRow.Field<string>("PaymentMethod"));
                    var paymentDate = dataRow.Field<DateTime>("PaymentDate");
                    var paymentNumber = dataRow.Field<string>("PaymentNumber");

                    if (count == 0)
                    {
                        count++;
                        rec = new Receipt
                        {
                            idReceipt = idReceipt,
                            customerName = customerName,
                            totalAmount = totalAmount,
                            createdDate = createdDate,
                            issuedBy = issuedBy,
                            payments = new List<Payment>(),
                            customer = new Customer
                            {
                                CustomerName = customerName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Address = address,
                                Country = country,
                                City = city,
                                idCustomer = customerId,
                                Balance = customerBalance
                            }
                        };
                    }

                    rec.payments.Add(new Payment
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

            return rec;
        }

        /// <summary>
        ///     Retrieves and returns the receipts as statement items matching the parameters given
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<StatementItem> getReceiptsForStatement(int customerID, DateTime from, DateTime to)
        {
            var list = new List<StatementItem>();

            try
            {
                var query = "SELECT `Receipt`.* FROM `Receipt`" +
                            " LEFT JOIN `Customer` ON `Receipt`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                            "`Receipt`.`CreatedDate` >= @from AND `Receipt`.`CreatedDate` <= @to; ";
                var dt = new DataTable();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@customerID", customerID);
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);

                    dt.Load(cmd.ExecuteReader());
                }

                var inv = new StatementItem();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idReceipt = dataRow.Field<int>("idReceipt");
                    var amount = dataRow.Field<float>("Amount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");

                    inv = new StatementItem
                    {
                        idItem = idReceipt,
                        credits = amount,
                        createdDate = createdDate,
                        itemType = ItemType.Receipt,
                        balance = balance
                    };

                    list.Add(inv);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     Given the receipt ID deletes the receipt from the Database
        /// </summary>
        /// <param name="receiptID"></param>
        public static void deleteReceipt(int receiptID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM Payment WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Receipt WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns total receipt sales for specific months and year
        /// </summary>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static float getTotalReceiptsMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getReceiptsByMonthYear", conn);
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
        ///     Given the id checks if a receipt exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool receiptExists(int id)
        {
            try
            {
                int idOrder;
                var cmd = new MySqlCommand("SELECT idReceipt FROM Receipt where idReceipt=" + id, conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idOrder = Convert.ToInt32(queryResult);
                else
                    idOrder = 0;

                return idOrder == 0 ? false : true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        /// <summary>
        ///     Returns the latest receipt ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestReceiptID()
        {
            try
            {
                int idReceipt;
                var cmd = new MySqlCommand("SELECT idReceipt FROM Receipt ORDER BY idReceipt DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idReceipt = Convert.ToInt32(queryResult);
                else
                    idReceipt = 0;

                return idReceipt;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the receipt object inserts it in to the database
        /// </summary>
        /// <param name="receipt"></param>
        public static void insertReceipt(Receipt receipt)
        {
            try
            {
                //insert receipt 
                var query =
                    "INSERT INTO Receipt (idReceipt, idCustomer, Amount, IssuedBy, PreviousBalance, CreatedDate ) Values (@idReceipt, @idCustomer, @Amount, @IssuedBy, @PreviousBalance, @CreatedDate)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idReceipt", receipt.idReceipt);
                    cmd.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Amount", receipt.totalAmount);
                    cmd.Parameters.AddWithValue("@IssuedBy", receipt.issuedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", receipt.createdDate);
                    cmd.Parameters.AddWithValue("@PreviousBalance", receipt.customer.Balance);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                var sCommand =
                    new StringBuilder(
                        "INSERT INTO Payment (idReceipt, PaymentMethod, Amount, PaymentNumber, PaymentDate) VALUES ");
                var Rows = new List<string>();

                foreach (var p in receipt.payments)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(receipt.idReceipt.ToString()),
                        MySqlHelper.EscapeString(p.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(p.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.paymentNumber),
                        MySqlHelper.EscapeString(p.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss",
                            CultureInfo.InvariantCulture))
                    ));

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (var myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                //update customer total  

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the old and updated receipt update the receipt
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="oldreceipt"></param>
        public static void updateReceipt(Receipt receipt, Receipt oldreceipt)
        {
            try
            {
                //update receipt 
                var query = "UPDATE Receipt SET  Amount=@Amount, IssuedBy=@IssuedBy  WHERE idReceipt=@idReceipt ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idReceipt", receipt.idReceipt);
                    cmd.Parameters.AddWithValue("@Amount", receipt.totalAmount);
                    cmd.Parameters.AddWithValue("@IssuedBy", receipt.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //delete old payments
                var queryDelete = "DELETE from Payment WHERE idReceipt=@idReceipt; ";

                using (var cmd = new MySqlCommand(queryDelete, conn))
                {
                    cmd.Parameters.AddWithValue("@idReceipt", oldreceipt.idReceipt);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                var sCommand =
                    new StringBuilder(
                        "INSERT INTO Payment (idReceipt, PaymentMethod, Amount, PaymentNumber, PaymentDate) VALUES ");
                var Rows = new List<string>();

                foreach (var p in receipt.payments)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(receipt.idReceipt.ToString()),
                        MySqlHelper.EscapeString(p.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(p.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.paymentNumber),
                        MySqlHelper.EscapeString(p.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss",
                            CultureInfo.InvariantCulture))
                    ));

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
        public static List<int> getCustomerReceipts(int customerID)
        {
            var customer_receipts_list = new List<int>();

            try
            {
                var cmd = new MySqlCommand("SELECT idReceipt FROM Receipt WHERE idCustomer = " + customerID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idReceipt = dataRow.Field<int>("idReceipt");
                    customer_receipts_list.Add(idReceipt);
                }

                return customer_receipts_list;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
        public static float getReceiptAmount(int receiptID)
        {
            float ret = 0;
            try
            {
                var cmd = new MySqlCommand("SELECT Amount FROM receipt WHERE idReceipt = " + receiptID, conn);
                ret = float.Parse((cmd.ExecuteScalar()).ToString());
                return ret;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

    }
}