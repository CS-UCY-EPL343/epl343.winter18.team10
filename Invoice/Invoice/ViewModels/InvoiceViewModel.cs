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
    public class InvoiceViewModel
    {
        public List<Invoice> invoiceList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public InvoiceViewModel()
        {
            invoiceList = new List<Invoice>();

            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Invoice getInvoice(int invoiceID)
        {
            Invoice inv = new Invoice();
            try
            {
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
                                Balance = customerBalance
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

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return inv;
        }

        public static List<StatementItem> getInvoicesForStatement(int customerID, DateTime from, DateTime to)
        {
            List<StatementItem> list = new List<StatementItem>();            

            try
            {
                string query = "SELECT `Invoice`.* FROM `Invoice`" +
                    " LEFT JOIN `Customer` ON `Invoice`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                    "`Invoice`.`CreatedDate` >= @from AND `Invoice`.`CreatedDate` <= @to; ";
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
                    var idInvoice = dataRow.Field<Int32>("idInvoice");
                    var invTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");

                    inv = new StatementItem()
                    {
                        idItem = idInvoice,                        
                        charges = invTotalCost,
                        createdDate = createdDate,
                        itemType = ItemType.Invoice,
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

        public static bool deleteInvoice(int invoiceID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM InvoiceProduct WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Invoice WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1451)
                {
                    MessageBox.Show("Cannot delete invoice with ID = " + invoiceID + " as a credit note has been issued on it.");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
                return false;
            }
        }

        public static bool invoiceExists(int id)
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySqlCommand("SELECT idInvoice FROM Invoice where idInvoice=" + id.ToString(), conn);

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

        public static int returnLatestInvoiceID()
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);

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

        public static void insertInvoice(Invoice invoice)
        {
            try
            {
                //insert invoice 
                string query = "INSERT INTO Invoice (idInvoice, idCustomer, Cost, Vat, TotalCost, CreatedDate, DueDate, PreviousBalance, IssuedBy) Values (@idInvoice, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @DueDate, @PreviousBalance, @IssuedBy)";
                
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
                    cmd.Parameters.AddWithValue("@PreviousBalance", invoice.customer.Balance);
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
                string queryBalance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance+@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(queryBalance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", invoice.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateInvoice(Invoice invoice, Invoice old_invoice)
        {
            try
            {
                //update Invoice 
                string query = "UPDATE Invoice SET  Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, DueDate=@DueDate, IssuedBy=@IssuedBy WHERE idInvoice=@idInvoice ";
                
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.totalCost);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.issuedBy);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //update old stock  
                string queryStock = "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+@Quantity) WHERE  idProduct=@idProduct;";
                for (int i = 0; i < old_invoice.products.Count; i++)
                {
                    using (MySqlCommand cmd3 = new MySqlCommand(queryStock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_invoice.products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_invoice.products[i].idProduct);
                        cmd3.ExecuteNonQuery();
                    }
                }
                //delete old invoice products
                string queryDelete = "DELETE from InvoiceProduct WHERE idInvoice=@idInvoice; ";
                
                using (MySqlCommand cmd = new MySqlCommand(queryDelete, conn))
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
                string queryBalance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance+@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(queryBalance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", invoice.totalCost - old_invoice.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static double[] getTotalAmountByMonth()
        {
            double[] total = new double[12];

            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    MySqlCommand cmd = new MySqlCommand("getTotalAmountMonth_Invoices", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = i;
                    cmd.Parameters["@month"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                    String sum = cmd.ExecuteScalar().ToString();
                    if (sum == null || sum == "")
                    {
                        total[i - 1] = 0;
                    }
                    else
                    {
                        total[i - 1] = double.Parse(sum);
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        public static String get30DaysTotalInvoices()
        {
            String total = "";

            try
            {
                MySqlCommand cmd = new MySqlCommand("get30DaysTotalInvoices", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                total = cmd.ExecuteScalar().ToString();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        public static String get30DaysTotalSales()
        {
            String total = "";

            try
            {
                MySqlCommand cmd = new MySqlCommand("get30DaysTotalSales", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                total = cmd.ExecuteScalar().ToString();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        public static float getTotalSalesMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                MySqlCommand cmd = new MySqlCommand("getSalesByMonthYear", conn);
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

        public static List<int> getCustomerInvoices(int customerID)
        {
            List<int> customer_invoices_list = new List<int>();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT idInvoice FROM Invoice WHERE idCustomer = " + customerID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());


                foreach (DataRow dataRow in dt.Rows)
                {
                    var idInvoice = dataRow.Field<Int32>("idInvoice");
                    customer_invoices_list.Add(idInvoice);
                }
                return customer_invoices_list;

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;

        }
    }


}

