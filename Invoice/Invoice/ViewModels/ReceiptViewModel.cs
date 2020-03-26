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
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public ReceiptViewModel()
        {
            receiptList = new List<Receipt>();
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
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
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
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
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static Receipt getReceipt(int receiptID)
        {
            MySqlConnection conn;

            Receipt rec = new Receipt();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
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
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
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

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return rec;
        }

        public static void deleteReceipt(int receiptID)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM Payments WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Receipt WHERE idReceipt = " + receiptID, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static float getTotalReceiptsMonthYear(int months, int year)
        {
            MySqlConnection conn;
            float total = 0;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("getReceiptsByMonthYear", conn);
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
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;
        }

        public static bool receiptExists(int id)
        {
            MySqlConnection conn;

            try
            {
                int idOrder;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt where idReceipt=" + id.ToString(), conn);
                conn.Open();

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idOrder = Convert.ToInt32(queryResult);
                else
                    idOrder = 0;

                conn.Close();
                return ((idOrder == 0) ? false : true);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return false;
        }

        public static int returnLatestReceiptID()
        {
            MySqlConnection conn;

            try
            {
                int idReceipt;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt ORDER BY idReceipt DESC LIMIT 1", conn);
                conn.Open();

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idReceipt = Convert.ToInt32(queryResult);
                else
                    idReceipt = 0;

                conn.Close();
                return idReceipt;

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return 0;
        }

        public static void insertReceipt(Receipt receipt)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert receipt 
                string query = "INSERT INTO Receipt (idReceipt, idCustomer, Amount, IssuedBy, IssuedDate ) Values (@idReceipt, @idCustomer, @Amount, @IssuedBy, @IssuedDate)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idReceipt", receipt.idReceipt);
                    cmd.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Amount", receipt.totalAmount);
                    cmd.Parameters.AddWithValue("@IssuedBy", receipt.issuedBy);
                    cmd.Parameters.AddWithValue("@IssuedDate", receipt.createdDate);

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


                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void updateReceipt(Receipt receipt, Receipt oldreceipt)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();


                //update receipt 
                string query = "UPDATE Receipt SET  idCustomer=@idCustomer,Amount=@Amount, IssuedBy=@IssuedBy, IssuedDate=@IssuedDate  WHERE idReceipt=@idReceipt ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idReceipt", receipt.idReceipt);
                    cmd.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Amount", receipt.totalAmount);
                    cmd.Parameters.AddWithValue("@IssuedBy", receipt.issuedBy);
                    cmd.Parameters.AddWithValue("@IssuedDate", receipt.createdDate);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //delete old payments
                string query_delete_invoiceProducts = "DELETE from Payment WHERE idReceipt=@idReceipt; ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idReceipt", oldreceipt.idReceipt);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO Payment (idReceipt, PaymentMethod, Amount, PaymentNumber,PaymentDate) VALUES ");
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
                string query_update_customer_balance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance+@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(query_update_customer_balance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", oldreceipt.totalAmount - receipt.totalAmount);
                    cmd3.Parameters.AddWithValue("@idCustomer", receipt.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }


                conn.Close();
                MessageBox.Show("Receipt was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }
    }
}
