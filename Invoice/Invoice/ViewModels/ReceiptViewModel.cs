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

        public static Receipt getReceiptByID(int receiptID)
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

        public static void deleteReceiptByID(int receiptID)
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

    }
}
