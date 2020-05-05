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
    public class ReceiptViewModel
    {
        public List<Receipt> receiptList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public ReceiptViewModel()
        {
            receiptList = new List<Receipt>();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `Receipt`.*, `Customer`.`CustomerName` FROM `Receipt`" +
                    " LEFT JOIN `Customer` ON `Receipt`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Receipt rec = new Receipt();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idReceipt = dataRow.Field<Int32>("idReceipt");
                    var amount = dataRow.Field<float>("Amount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    rec = new Receipt()
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Receipt getReceipt(int receiptID)
        {
            Receipt rec = new Receipt();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewReceipt WHERE ReceiptID = " + receiptID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    rec = null;

                int count = 0;
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

                    var idReceipt = dataRow.Field<Int32>("ReceiptID");
                    var totalAmount = dataRow.Field<float>("TotalAmount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var paymentID = dataRow.Field<Int32>("idPayment");
                    var amount = dataRow.Field<float>("Amount");
                    var method = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), dataRow.Field<string>("PaymentMethod"));
                    var paymentDate = dataRow.Field<DateTime>("PaymentDate");
                    var paymentNumber = dataRow.Field<string>("PaymentNumber");

                    if (count == 0)
                    {
                        count++;
                        rec = new Receipt()
                        {
                            idReceipt = idReceipt,
                            customerName = customerName,
                            totalAmount = totalAmount,
                            createdDate = createdDate,
                            issuedBy = issuedBy,
                            payments = new List<Payment>(),
                            customer = new Customer()
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

                    rec.payments.Add(new Payment()
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

            return rec;
        }

        public static List<StatementItem> getReceiptsForStatement(int customerID, DateTime from, DateTime to)
        {
            List<StatementItem> list = new List<StatementItem>();

            try
            {
                string query = "SELECT `Receipt`.* FROM `Receipt`" +
                    " LEFT JOIN `Customer` ON `Receipt`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                    "`Receipt`.`CreatedDate` >= @from AND `Receipt`.`CreatedDate` <= @to; ";
                DataTable dt = new DataTable();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@customerID", customerID);
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);

                    dt.Load(cmd.ExecuteReader());
                }

                StatementItem inv = new StatementItem();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idReceipt = dataRow.Field<Int32>("idReceipt");
                    var amount = dataRow.Field<float>("Amount");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");

                    inv = new StatementItem()
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
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return list;
        }

        public static void deleteReceipt(int receiptID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Payment WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Receipt WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static float getTotalReceiptsMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                MySqlCommand cmd = new MySqlCommand("getReceiptsByMonthYear", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = months;
                cmd.Parameters["@month"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@year", SqlDbType.Int).Value = year;
                cmd.Parameters["@year"].Direction = ParameterDirection.Input;

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

        public static bool receiptExists(int id)
        {
            try
            {
                int idOrder;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt where idReceipt=" + id.ToString(), conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idOrder = Convert.ToInt32(queryResult);
                else
                    idOrder = 0;

                return ((idOrder == 0) ? false : true);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public static int returnLatestReceiptID()
        {
            try
            {
                int idReceipt;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt ORDER BY idReceipt DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idReceipt = Convert.ToInt32(queryResult);
                else
                    idReceipt = 0;

                return idReceipt;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        public static void insertReceipt(Receipt receipt)
        {
            try
            {
                //insert receipt 
                string query = "INSERT INTO Receipt (idReceipt, idCustomer, Amount, IssuedBy, PreviousBalance, CreatedDate ) Values (@idReceipt, @idCustomer, @Amount, @IssuedBy, @PreviousBalance, @CreatedDate)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
                StringBuilder sCommand = new StringBuilder("INSERT INTO Payment (idReceipt, PaymentMethod, Amount, PaymentNumber, PaymentDate) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Payment p in receipt.payments)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(receipt.idReceipt.ToString()),
                        MySqlHelper.EscapeString(p.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(p.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.paymentNumber.ToString()),
                        MySqlHelper.EscapeString(p.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture))
                       ));
                }

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                //update customer total  
                string query_update_customer_balance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance-@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(query_update_customer_balance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", receipt.totalAmount);
                    cmd3.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateReceipt(Receipt receipt, Receipt oldreceipt)
        {
            try
            {
                //update receipt 
                string query = "UPDATE Receipt SET  Amount=@Amount, IssuedBy=@IssuedBy  WHERE idReceipt=@idReceipt ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idReceipt", receipt.idReceipt);
                    cmd.Parameters.AddWithValue("@Amount", receipt.totalAmount);
                    cmd.Parameters.AddWithValue("@IssuedBy", receipt.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //delete old payments
                string queryDelete = "DELETE from Payment WHERE idReceipt=@idReceipt; ";
                
                using (MySqlCommand cmd = new MySqlCommand(queryDelete, conn))
                {
                    cmd.Parameters.AddWithValue("@idReceipt", oldreceipt.idReceipt);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                StringBuilder sCommand = new StringBuilder("INSERT INTO Payment (idReceipt, PaymentMethod, Amount, PaymentNumber, PaymentDate) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Payment p in receipt.payments)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(receipt.idReceipt.ToString()),
                        MySqlHelper.EscapeString(p.paymentMethod.ToString()),
                        MySqlHelper.EscapeString(p.amount.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.paymentNumber.ToString()),
                        MySqlHelper.EscapeString(p.paymentDate.ToString("yyyy-MM-dd HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture))
                       ));
                }

                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");

                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                //update customer total  
                string queryBalance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance+@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(queryBalance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", oldreceipt.totalAmount - receipt.totalAmount);
                    cmd3.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
