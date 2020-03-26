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

        public static Invoice getInvoice(int invoiceID)
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

        public static void deleteInvoice(int invoiceID)
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

        public static bool invoiceExists(int id)
        {
            MySqlConnection conn;

            try
            {
                int idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySqlCommand("SELECT idInvoice FROM Invoice where idInvoice=" + id.ToString(), conn);
                conn.Open();

                var queryResult = cmd.ExecuteScalar(); 
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else                    
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

        public static int returnLatestInvoiceID()
        {
            MySqlConnection conn;         
            
            try
            {
                int idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);
                conn.Open();

                var queryResult = cmd.ExecuteScalar();
                if (queryResult != null)
                    idInvoice = Convert.ToInt32(queryResult);
                else
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

        public static void insertInvoice(Invoice invoice)
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }    

        public static void updateInvoice(Invoice invoice, Invoice old_invoice)
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

        public static float getTotalSalesMonthYear(int months, int year)
        {
            MySqlConnection conn;
            float total = 0;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("getSalesByMonthYear", conn);
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

    }


}

