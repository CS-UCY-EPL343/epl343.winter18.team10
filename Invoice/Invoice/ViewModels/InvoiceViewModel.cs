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
    public class InvoiceViewModel
    {
        public List<Invoice> invoiceList { get; set; }
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public InvoiceViewModel()
        {
            invoiceList = new List<Invoice>();
            MySqlConnection conn;            

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT `Invoice`.*, `Customer`.`CustomerName` FROM `Invoice`" +
                    " LEFT JOIN `Customer` ON `Invoice`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Invoice inv = new Invoice();
                foreach (DataRow dataRow in dt.Rows)
                {                   
                    var customer = dataRow.Field<string>("CustomerName");
                    var idInvoice = dataRow.Field<Int32>("idInvoice");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var invTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var dueDate = dataRow.Field<DateTime>("DueDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    inv = new Invoice()
                    {
                        idInvoice = idInvoice,
                        customerName = customer,
                        cost = cost,
                        VAT = VAT,
                        totalCost = invTotalCost,
                        createdDate = createdDate,
                        dueDate = dueDate,
                        issuedBy = issuedBy
                    };

                    invoiceList.Add(inv);
                }


                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static Invoice getInvoiceById(int invoiceID)
        {
            MySqlConnection conn;
            
            Invoice inv = new Invoice();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewInvoice WHERE InvoiceID = " + invoiceID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    inv = null;

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

                    var idInvoice = dataRow.Field<Int32>("InvoiceID");
                    var cost = dataRow.Field<float>("InvoiceCost");
                    var VAT = dataRow.Field<float>("InvoiceVAT");
                    var invTotalCost = dataRow.Field<float>("InvoiceTotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var dueDate = dataRow.Field<DateTime>("DueDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<Int32>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var proVat = dataRow.Field<float>("IPVAT");
                    var quantity = dataRow.Field<Int32>("Quantity");

                    if (count == 0)
                    {
                        count++;
                        inv = new Invoice()
                        {
                            idInvoice = idInvoice,
                            customerName = customerName,
                            cost = cost,
                            VAT = VAT,
                            totalCost = invTotalCost,
                            createdDate = createdDate,
                            dueDate = dueDate,
                            issuedBy = issuedBy,
                            products = new List<Product>(),
                            customer = new Customer()
                            {
                                CustomerName = customerName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Address = address,
                                Country = country,
                                City = city,
                                idCustomer = customerId,
                                Balance=customerBalance
                            }
                        };
                    }

                    inv.products.Add(new Product()
                    {
                        idProduct = productID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        Stock = stock,
                        Total = proTotalCost,
                        Quantity = quantity,
                        SellPrice = proTotalCost / quantity,
                        Vat = proVat
                    });
                }

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }

            return inv;
        }

        public static void deleteInvoiceByID(int invoiceID)
        {
            MySqlConnection conn;
           
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM InvoiceProduct WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Invoice WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static bool InvoiceID_exist_or_not(int id)
        {
            //int id_return = 0;
            MySqlConnection conn;

            try
            {
                int idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice where idInvoice=" + id.ToString(), conn);
                conn.Open();
                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                conn.Close();
                return ((idInvoice==0)?false:true);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return false;
        }

        public static int ReturnLatestInvoiceID()
        {
            //int id_return = 0;
            MySqlConnection conn;         
            
            try
            {
                int idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);
                conn.Open();
               // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = 0;

                conn.Close();
                return idInvoice ;

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return 0;
        }

        public static bool ReceiptID_exist_or_not(int id)
        {
            //int id_return = 0;
            MySqlConnection conn;

            try
            {
                int idOrder;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt where idReceipt=" + id.ToString(), conn);
                conn.Open();
                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
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

        public static int ReturnLatestReceiptID()
        {
            //int id_return = 0;
            MySqlConnection conn;

            try
            {
                int idReceipt;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idReceipt FROM Receipt ORDER BY idReceipt DESC LIMIT 1", conn);
                conn.Open();
                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idReceipt = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
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


        public static void Send_Ivoice_and_Products_to_DB(Invoice invoice)
        {
            MySqlConnection conn;
            
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert invoice 
                string query = "INSERT INTO Invoice (idInvoice, idCustomer, Cost, Vat, TotalCost, CreatedDate, DueDate, IssuedBy) Values (@idInvoice, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @DueDate, @IssuedBy)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.totalCost);
                    cmd.Parameters.AddWithValue("@CreatedDate", invoice.createdDate);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert product
                StringBuilder sCommand = new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
                List<string> Rows = new List<string>();

                // List<Product> list = invoiceDataGrid2.Items.OfType<Product>().ToList();

                foreach (Product p in invoice.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", MySqlHelper.EscapeString(invoice.idInvoice.ToString()),
                        p.idProduct, p.Quantity, MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")), MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))));

                    using (MySqlCommand cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-" +
                        p.Quantity.ToString() + ") WHERE idProduct=" + p.idProduct.ToString() + ";", conn))
                    {
                        cmd3.ExecuteNonQuery();
                    }
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
                    cmd3.Parameters.AddWithValue("@amount", invoice.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Invoice was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void Send_Receipt_to_DB(Receipt receipt)
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
                        cmd3.Parameters.AddWithValue("@amount",receipt.totalAmount);
                        cmd3.Parameters.AddWithValue("@idCustomer",receipt.customer.idCustomer);
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

        public static void Send_Receipt_to_DB(Receipt receipt,Receipt oldreceipt)
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
                string query_update_customer_balance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance-@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(query_update_customer_balance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", receipt.totalAmount- oldreceipt.totalAmount);                    
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

        public static void edit_Invoice(Invoice invoice, Invoice old_invoice)
        {
            MySqlConnection conn;
           
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                //update Invoice 
                string query = "UPDATE Invoice SET  Cost=@Cost,Vat=@Vat, TotalCost=@TotalCost, CreatedDate=@CreatedDate, DueDate=@DueDate, IssuedBy=@IssuedBy WHERE idInvoice=@idInvoice ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.totalCost);
                    cmd.Parameters.AddWithValue("@CreatedDate", invoice.createdDate);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
                
                //update old stock  
                string query_update_old_stock = "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+@Quantity) WHERE  idProduct=@idProduct;";
                for (int i = 0; i < old_invoice.products.Count; i++)
                {                    
                    using (MySqlCommand cmd3 = new MySqlCommand(query_update_old_stock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_invoice.products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_invoice.products[i].idProduct);                        
                        cmd3.ExecuteNonQuery();
                    }
                }
                //delete old invoice products
                string query_delete_invoiceProducts = "DELETE from InvoiceProduct WHERE idInvoice=@idInvoice; ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idInvoice", old_invoice.idInvoice);                   
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
              
                
                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
              List<string> Rows = new List<string>();

              foreach (Product p in invoice.products)
              {
                  Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", MySqlHelper.EscapeString(invoice.idInvoice.ToString()),
                      p.idProduct, p.Quantity, MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")), MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))));
                  //update stock  
                  using (MySqlCommand cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-" +
                      p.Quantity.ToString() + ") WHERE idProduct=" + p.idProduct.ToString() + ";", conn))
                  {
                      cmd3.ExecuteNonQuery();
                  }
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
                    cmd3.Parameters.AddWithValue("@amount", invoice.totalCost- old_invoice.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
                
                conn.Close();
                MessageBox.Show("Invoice was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static double[] getTotalAmountByMonth()
        {
            MySqlConnection conn;            
            double[] total = new double[12];
            
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                for (int i = 1; i <= 12; i++)
                {
                    MySqlCommand cmd = new MySqlCommand("getTotalAmountMonth_Invoices", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = i;
                    cmd.Parameters["@month"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                    String sum = cmd.ExecuteScalar().ToString();
                    if (sum == null || sum=="")
                    {
                        total[i - 1] = 0;
                    }
                    else
                    {
                        total[i - 1] = double.Parse(sum);
                    }
                }
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;

        }

        public static String get30DaysTotalInvoices()
        {
            MySqlConnection conn;           
            String total="";
            
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                
                    MySqlCommand cmd = new MySqlCommand("get30DaysTotalInvoices", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                    total = cmd.ExecuteScalar().ToString();
                    conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;
        }

        public static String get30DaysTotalSales()
        {
            MySqlConnection conn;           
            String total = "";
            
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("get30DaysTotalSales", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                total = cmd.ExecuteScalar().ToString();
                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return total;
        }
        /*order start-----------------------------------------------------------------------------------------------------*/
        public static bool OrderID_exist_or_not(int id)
        {
            //int id_return = 0;
            MySqlConnection conn;

            try
            {
                int idOrder;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idOrder FROM `Order` where idOrder=" + id.ToString(), conn);
                conn.Open();
                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
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
        

        public static int ReturnLatestOrderID()
        {            
            MySqlConnection conn;
            try
            {
                int idOrder;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idOrder FROM `Order` ORDER BY idOrder DESC LIMIT 1", conn);
                conn.Open();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idOrder = 0;

                conn.Close();
                return idOrder;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
            return 0;
        }

        public static void Send_Order_to_DB(Order order)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert invoice 
                string query = "INSERT INTO `Order` (idOrder, idCustomer, IssuedDate, ShippingDate, Cost, Vat, TotalCost, IssuedBy, Status) Values (@idOrder, @idCustomer, @IssuedDate, @ShippingDate, @Cost, @Vat, @TotalCost, @IssuedBy, @Status)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idOrder", order.idOrder);
                    cmd.Parameters.AddWithValue("@idCustomer", order.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@IssuedDate", order.createdDate);
                    cmd.Parameters.AddWithValue("@ShippingDate", order.shippingDate);
                    cmd.Parameters.AddWithValue("@Cost", order.cost);
                    cmd.Parameters.AddWithValue("@Vat", order.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", order.totalCost);
                    cmd.Parameters.AddWithValue("@IssuedBy", order.issuedBy);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in order.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                       ));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Order was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void update_Order(Order order, Order old_Order)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                //update Order 
                string query = "UPDATE `Order` SET  idOrder=@idOrder, idCustomer=@idCustomer, IssuedDate=@IssuedDate, ShippingDate=@ShippingDate, Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, IssuedBy=@IssuedBy, Status=@Status WHERE idOrder=@idOrder ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idOrder", order.idOrder);
                    cmd.Parameters.AddWithValue("@idCustomer", order.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@IssuedDate", order.createdDate);
                    cmd.Parameters.AddWithValue("@ShippingDate", order.shippingDate);
                    cmd.Parameters.AddWithValue("@Cost", order.cost);
                    cmd.Parameters.AddWithValue("@Vat", order.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", order.totalCost);
                    cmd.Parameters.AddWithValue("@IssuedBy", order.issuedBy);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

               
                //delete old Order products
                string query_delete_invoiceProducts = "DELETE from OrderProduct WHERE idOrder=@idOrder;";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idOrder", old_Order.idOrder);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }


                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in order.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                       ));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }

                conn.Close();
                MessageBox.Show("Order was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        /*Order finish-----------------------------------------------------------------------------------------------------*/

    }

}

