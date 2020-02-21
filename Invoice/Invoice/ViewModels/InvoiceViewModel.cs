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

        public InvoiceViewModel()
        {
            invoiceList = new List<Invoice>();
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

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
                        m_idInvoice = idInvoice,
                        m_customerName = customer,
                        m_cost = cost,
                        m_VAT = VAT,
                        m_totalCost = invTotalCost,
                        m_createdDate = createdDate,
                        m_dueDate = dueDate,
                        m_issuedBy = issuedBy
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
            string myConnectionString;
            
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            Invoice inv = new Invoice();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewInvoice WHERE InvoiceID = " + invoiceID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                int count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customerName = dataRow.Field<string>("CustomerName");
                    var phoneNumber = dataRow.Field<int>("PhoneNumber");
                    var email = dataRow.Field<string>("Email");
                    var address = dataRow.Field<string>("Address");
                    var country = dataRow.Field<string>("Country");
                    var city = dataRow.Field<string>("City");

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
                            m_idInvoice = idInvoice,
                            m_customerName = customerName,
                            m_cost = cost,
                            m_VAT = VAT,
                            m_totalCost = invTotalCost,
                            m_createdDate = createdDate,
                            m_dueDate = dueDate,
                            m_issuedBy = issuedBy,
                            m_products = new List<Product>(),
                            m_customer = new Customers()
                            {
                                CustomerName = customerName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Address = address,
                                Country = country,
                                City = city
                            }
                        };
                    }

                    inv.m_products.Add(new Product()
                    {
                        idProduct = productID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        Stock = stock,
                        Total = proTotalCost,
                        Quantity = quantity,
                        Cost = proTotalCost / quantity,
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
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
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

        //logika tha ta metakinisoume ta pio kato ----------------------------------------------
        public static int ReturnLatestInvoiceID()
        {
            //int id_return = 0;
            MySqlConnection conn;         
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";
            try
            {
                string idInvoice;
                conn = new MySqlConnection(myConnectionString);
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);
                conn.Open();
               // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idInvoice = Convert.ToString(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idInvoice = "";

                conn.Close();
                return Convert.ToInt32(idInvoice) ;

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
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                //insert Invoice 
                string query = "INSERT INTO Invoice (idInvoice, idCustomer, Cost, Vat, TotalCost, CreatedDate, DueDate, IssuedBy) Values (@idInvoice, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @DueDate, @IssuedBy)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.m_idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.m_customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.m_cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.m_VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.m_totalCost);
                    cmd.Parameters.AddWithValue("@CreatedDate", invoice.m_createdDate);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.m_dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.m_issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
                List<string> Rows = new List<string>();

                // List<Product> list = invoiceDataGrid2.Items.OfType<Product>().ToList();

                foreach (Product p in invoice.m_products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", MySqlHelper.EscapeString(invoice.m_idInvoice.ToString()),
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

                conn.Close();
                MessageBox.Show("Invoice was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

        public static void edit_Invoice(Invoice invoice, Invoice old_invoice)
        {
            MySqlConnection conn;
            string myConnectionString;
            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

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

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.m_idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.m_customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.m_cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.m_VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.m_totalCost);
                    cmd.Parameters.AddWithValue("@CreatedDate", invoice.m_createdDate);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.m_dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.m_issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
                
                //update old stock  
                string query_update_old_stock = "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+@Quantity) WHERE  idProduct=@idProduct;";
                for (int i = 0; i < old_invoice.m_products.Count; i++)
                {                    
                    using (MySqlCommand cmd3 = new MySqlCommand(query_update_old_stock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_invoice.m_products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_invoice.m_products[i].idProduct);                        
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
                    cmd.Parameters.AddWithValue("@idInvoice", old_invoice.m_idInvoice);                   
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
              

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
              List<string> Rows = new List<string>();

              foreach (Product p in invoice.m_products)
              {
                  Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')", MySqlHelper.EscapeString(invoice.m_idInvoice.ToString()),
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
              

                conn.Close();
                MessageBox.Show("Invoice was send to Data Base");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }

    }

}
