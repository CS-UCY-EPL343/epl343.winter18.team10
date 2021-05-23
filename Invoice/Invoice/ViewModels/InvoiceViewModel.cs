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
using System.Text;
using System.Windows;
using InvoiceX.Classes;
using InvoiceX.Models;
using MySql.Data.MySqlClient;

namespace InvoiceX.ViewModels
{
    public class InvoiceViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the invoices
        /// </summary>
        public InvoiceViewModel()
        {
            invoiceList = new List<Invoice>();

            try
            {
                var cmd = new MySqlCommand("SELECT `Invoice`.*, `Customer`.`CustomerName` FROM `Invoice`" +
                                           " LEFT JOIN `Customer` ON `Invoice`.`idCustomer` = `Customer`.`idCustomer`; ",
                    conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var inv = new Invoice();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idInvoice = dataRow.Field<int>("idInvoice");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var invTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var dueDate = dataRow.Field<DateTime>("DueDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    inv = new Invoice
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Invoice> invoiceList { get; set; }

        /// <summary>
        ///     Given the invoice ID retrieves the invoice and returns it
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public static Invoice getInvoice(int invoiceID)
        {
            var inv = new Invoice();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM viewInvoice WHERE InvoiceID = " + invoiceID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    inv = null;

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

                    var idInvoice = dataRow.Field<int>("InvoiceID");
                    var cost = dataRow.Field<float>("InvoiceCost");
                    var VAT = dataRow.Field<float>("InvoiceVAT");
                    var invTotalCost = dataRow.Field<float>("InvoiceTotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var dueDate = dataRow.Field<DateTime>("DueDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");
                    var isPaid = dataRow.Field<bool>("isPaid");

                    var productID = dataRow.Field<int>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var proVat = dataRow.Field<float>("IPVAT");
                    var quantity = dataRow.Field<int>("Quantity");
                    
                    if (count == 0)
                    {
                        count++;
                        inv = new Invoice
                        {
                            idInvoice = idInvoice,
                            customerName = customerName,
                            cost = cost,
                            VAT = VAT,
                            totalCost = invTotalCost,
                            createdDate = createdDate,
                            dueDate = dueDate,
                            issuedBy = issuedBy,
                            isPaid = isPaid,
                            products = new List<Product>(),
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

                    inv.products.Add(new Product
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return inv;
        }

        /// <summary>
        ///     Retrieves and returns the invoices as statement items matching the parameters given
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<StatementItem> getInvoicesForStatement(int customerID, DateTime from, DateTime to)
        {
            var list = new List<StatementItem>();

            try
            {
                var query = "SELECT `Invoice`.* FROM `Invoice`" +
                            " LEFT JOIN `Customer` ON `Invoice`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                            "`Invoice`.`CreatedDate` >= @from AND `Invoice`.`CreatedDate` <= @to; ";
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
                    var idInvoice = dataRow.Field<int>("idInvoice");
                    Invoice temp = InvoiceViewModel.getInvoice(idInvoice);
                    float credits1 = 0;
                    var invTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");
                    if (temp!=null&&temp.isPaid == true)
                    {
                        credits1 = invTotalCost;
                        balance = balance - credits1;
                    }
                        inv = new StatementItem
                        {
                            idItem = idInvoice,
                            charges = invTotalCost,
                            createdDate = createdDate,
                            itemType = ItemType.Invoice,
                            balance = balance,
                            credits = credits1
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
        ///     Given the invoice ID deletes the invoice from the Database
        /// </summary>
        /// <param name="invoiceID"></param>
        /// <returns></returns>
        public static bool deleteInvoice(int invoiceID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM InvoiceProduct WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM Invoice WHERE idInvoice = " + invoiceID, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451)
                    MessageBox.Show("Cannot delete invoice with ID = " + invoiceID +
                                    " as a credit note has been issued on it.");
                else
                    MessageBox.Show(ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Given the id checks if an invoice exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool invoiceExists(int id)
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idInvoice FROM Invoice where idInvoice=" + id, conn);

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
        ///     Returns the latest invoice ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestInvoiceID()
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idInvoice FROM Invoice ORDER BY idInvoice DESC LIMIT 1", conn);

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
        ///     Given the invoice object inserts it in to the database
        /// </summary>
        /// <param name="invoice"></param>
        public static void insertInvoice(Invoice invoice)
        {
            try
            {
                //insert invoice 
                var query =
                    "INSERT INTO Invoice (idInvoice, idCustomer, Cost, Vat, TotalCost, CreatedDate, DueDate, PreviousBalance, IssuedBy,isPaid) Values (@idInvoice, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @DueDate, @PreviousBalance, @IssuedBy,@isPaid)";

                using (var cmd = new MySqlCommand(query, conn))
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
                    cmd.Parameters.AddWithValue("@isPaid", invoice.isPaid);

                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert product
                var sCommand =
                    new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
                var Rows = new List<string>();

                // List<Product> list = invoiceDataGrid2.Items.OfType<Product>().ToList();

                foreach (var p in invoice.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(invoice.idInvoice.ToString()),
                        p.idProduct, p.Quantity, MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))));

                    using (var cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-" +
                                                       p.Quantity + ") WHERE idProduct=" + p.idProduct + ";", conn))
                    {
                        cmd3.ExecuteNonQuery();
                    }
                }

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
        ///     Given the old and updated invoice update the invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="old_invoice"></param>
        public static void updateInvoice(Invoice invoice, Invoice old_invoice)
        {
            try
            {
                //update Invoice 
                var query =
                    "UPDATE Invoice SET  Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, DueDate=@DueDate,isPaid=@isPaid, IssuedBy=@IssuedBy WHERE idInvoice=@idInvoice ";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idInvoice", invoice.idInvoice);
                    cmd.Parameters.AddWithValue("@idCustomer", invoice.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", invoice.cost);
                    cmd.Parameters.AddWithValue("@Vat", invoice.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", invoice.totalCost);
                    cmd.Parameters.AddWithValue("@DueDate", invoice.dueDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", invoice.issuedBy);
                    cmd.Parameters.AddWithValue("@isPaid", invoice.isPaid);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //update old stock  
                var queryStock =
                    "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+@Quantity) WHERE  idProduct=@idProduct;";
                for (var i = 0; i < old_invoice.products.Count; i++)
                    using (var cmd3 = new MySqlCommand(queryStock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_invoice.products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_invoice.products[i].idProduct);
                        cmd3.ExecuteNonQuery();
                    }

                //delete old invoice products
                var queryDelete = "DELETE from InvoiceProduct WHERE idInvoice=@idInvoice; ";

                using (var cmd = new MySqlCommand(queryDelete, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idInvoice", old_invoice.idInvoice);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }


                //insert products
                var sCommand =
                    new StringBuilder("INSERT INTO InvoiceProduct (idInvoice, idProduct, Quantity, Cost, VAT) VALUES ");
                var Rows = new List<string>();

                foreach (var p in invoice.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(invoice.idInvoice.ToString()),
                        p.idProduct, p.Quantity, MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))));
                    //update stock  
                    using (var cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-" +
                                                       p.Quantity + ") WHERE idProduct=" + p.idProduct + ";", conn))
                    {
                        cmd3.ExecuteNonQuery();
                    }
                }

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
        ///     Returns total invoice sales by month for statistics
        /// </summary>
        /// <returns></returns>
        public static double[] getTotalAmountByMonth()
        {
            var total = new double[12];

            try
            {
                for (var i = 1; i <= 12; i++)
                {
                    var cmd = new MySqlCommand("getTotalAmountMonth_Invoices", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", SqlDbType.Int).Value = i;
                    cmd.Parameters["@month"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                    var sum = cmd.ExecuteScalar().ToString();
                    if (sum == null || sum == "")
                        total[i - 1] = 0;
                    else
                        total[i - 1] = double.Parse(sum);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        /// <summary>
        ///     Returns total invoice count for the last 30 days for statistics
        /// </summary>
        /// <returns></returns>
        public static string get30DaysTotalInvoices()
        {
            var total = "";

            try
            {
                var cmd = new MySqlCommand("get30DaysTotalInvoices", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                total = cmd.ExecuteScalar().ToString();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        /// <summary>
        ///     Returns total invoice sales for the last 30 days for statistics
        /// </summary>
        /// <returns></returns>
        public static string get30DaysTotalSales()
        {
            var total = "";

            try
            {
                var cmd = new MySqlCommand("get30DaysTotalSales", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                total = cmd.ExecuteScalar().ToString();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return total;
        }

        /// <summary>
        ///     Returns total invoice sales for specific months and year
        /// </summary>
        /// <param name="months"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static float getTotalSalesMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getSalesByMonthYear", conn);
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
        public static float getTotalSalesWeekYear(int week, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getSalesByWeekYear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@week", SqlDbType.Int).Value = week;
                cmd.Parameters["@week"].Direction = ParameterDirection.Input;
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
        public static float getPaidInvoicesbyMonthYear(int months, int year)
        {
            float total = 0;

            try
            {
                var cmd = new MySqlCommand("getPaidInvoicesbyMonthYear", conn);
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
        ///     Given customer ID returns a list of invoices for the specific customer
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static List<int> getCustomerInvoices(int customerID)
        {
            var customer_invoices_list = new List<int>();

            try
            {
                var cmd = new MySqlCommand("SELECT idInvoice FROM Invoice WHERE idCustomer = " + customerID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idInvoice = dataRow.Field<int>("idInvoice");
                    customer_invoices_list.Add(idInvoice);
                }

                return customer_invoices_list;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
        public static float getInvoiceCost(int invoiceID)
        {
            float ret = 0;
            try
            {
                var cmd = new MySqlCommand("SELECT TotalCost FROM invoice WHERE idInvoice = " + invoiceID, conn);
                ret = float.Parse((cmd.ExecuteScalar()).ToString());
                return ret;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }
        public static bool isInvoicePaid(int invoiceID)
        {
            bool ret = false;
            try
            {
                var cmd = new MySqlCommand("SELECT isPaid FROM invoice WHERE idInvoice = " + invoiceID, conn);
                ret= (bool)cmd.ExecuteScalar();

                return ret;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return ret;

        }
    }
}