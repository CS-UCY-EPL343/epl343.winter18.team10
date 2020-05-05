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

    public class CreditNoteViewModel
    {
        public List<CreditNote> creditNoteList { get; set; }
        public List<int> customer_invoices_list { get; set; }
        public List<Product> invoice_products_list { get; set; }

        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public CreditNoteViewModel()
        {
            creditNoteList = new List<CreditNote>();

            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT `CreditNote`.*, `Customer`.`CustomerName` FROM `CreditNote`" +
                    " LEFT JOIN `Customer` ON `CreditNote`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                CreditNote cred = new CreditNote();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idCreditNote = dataRow.Field<Int32>("idCreditNote");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var credTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    cred = new CreditNote()
                    {
                        idCreditNote = idCreditNote,
                        customerName = customer,
                        cost = cost,
                        VAT = VAT,
                        totalCost = credTotalCost,
                        createdDate = createdDate,
                        issuedBy = issuedBy
                    };

                    creditNoteList.Add(cred);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

           
        }

        public static CreditNote getCreditNote(int creditNoteID)
        {
            CreditNote cred = new CreditNote();
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewCreditNote WHERE CreditNoteID = " + creditNoteID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    cred = null;

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

                    var idCreditNote = dataRow.Field<Int32>("CreditNoteID");
                    var cost = dataRow.Field<float>("CreditNoteCost");
                    var VAT = dataRow.Field<float>("CreditNoteVAT");
                    var credTotalCost = dataRow.Field<float>("CreditNoteTotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<Int32>("idProduct");
                    var invoiceID = dataRow.Field<Int32>("idInvoice");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var proVat = dataRow.Field<float>("IPVAT");
                    var quantity = dataRow.Field<Int32>("Quantity");

                    if (count == 0)
                    {
                        count++;
                        cred = new CreditNote()
                        {
                            idCreditNote = idCreditNote,
                            customerName = customerName,
                            cost = cost,
                            VAT = VAT,
                            totalCost = credTotalCost,
                            createdDate = createdDate,
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

                    cred.products.Add(new Product()
                    {
                        idProduct = productID,
                        productInvoiceID = invoiceID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        Stock = stock,
                        Total = proTotalCost* quantity,//@chrisi ekana alagi edo * quantity
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

            return cred;
        }

        public static List<StatementItem> getCreditNotesForStatement(int customerID, DateTime from, DateTime to)
        {
            List<StatementItem> list = new List<StatementItem>();

            try
            {
                string query = "SELECT `CreditNote`.* FROM `CreditNote`" +
                    " LEFT JOIN `Customer` ON `CreditNote`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                    "`CreditNote`.`CreatedDate` >= @from AND `CreditNote`.`CreatedDate` <= @to; ";
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
                    var idCreditNote = dataRow.Field<Int32>("idCreditNote");
                    var credTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");

                    inv = new StatementItem()
                    {
                        idItem = idCreditNote,
                        credits = credTotalCost,
                        createdDate = createdDate,
                        itemType = ItemType.CreditNote,
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

        public static void deleteCreditNote(int creditNoteID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM CreditNoteProduct WHERE idCreditNote = " + creditNoteID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM CreditNote WHERE idCreditNote = " + creditNoteID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

       

      

        public static int returnLatestCreditNoteID()
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idCreditNote FROM CreditNote ORDER BY idCreditNote DESC LIMIT 1", conn);

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

        public static void insertCreditNote(CreditNote creditNote)
        {
            try
            {
                //insert invoice 
                string query = "INSERT INTO CreditNote (idCreditNote, idCustomer, Cost, Vat, TotalCost, CreatedDate, PreviousBalance, IssuedBy) Values (@idCreditNote, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @PreviousBalance, @IssuedBy)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idCreditNote", creditNote.idCreditNote);
                    cmd.Parameters.AddWithValue("@idCustomer", creditNote.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", creditNote.cost);
                    cmd.Parameters.AddWithValue("@Vat", creditNote.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", creditNote.totalCost);
                    cmd.Parameters.AddWithValue("@CreatedDate", creditNote.createdDate);
                    cmd.Parameters.AddWithValue("@IssuedBy", creditNote.issuedBy);
                    cmd.Parameters.AddWithValue("@PreviousBalance", creditNote.customer.Balance);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert product
                StringBuilder sCommand = new StringBuilder("INSERT INTO CreditNoteProduct (idCreditNote, idProduct, idInvoice, Quantity) VALUES ");
                List<string> Rows = new List<string>();


                foreach (Product p in creditNote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}')", MySqlHelper.EscapeString(creditNote.idCreditNote.ToString()),
                        p.idProduct, MySqlHelper.EscapeString(p.productInvoiceID.ToString()), p.Quantity));

                    using (MySqlCommand cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+" +
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
                string queryBalance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance-@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(queryBalance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", creditNote.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", creditNote.customer.idCustomer);
                    cmd3.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static bool CreditNoteExists(int id)
        {
            try
            {
                int idInvoice;
                MySqlCommand cmd = new MySqlCommand("SELECT idCreditNote FROM CreditNote where idCreditNote=" + id.ToString(), conn);

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

        public static void updateCreditNote(CreditNote creditNote, CreditNote old_creditNote)
        {
            try
            {
                //update Credit Note 
                string query = "UPDATE CreditNote SET  Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, IssuedBy=@IssuedBy, PreviousBalance=@PreviousBalance WHERE idCreditNote=@idCreditNote;";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idCreditNote", creditNote.idCreditNote);
                    cmd.Parameters.AddWithValue("@idCustomer", creditNote.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@Cost", creditNote.cost);
                    cmd.Parameters.AddWithValue("@Vat", creditNote.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", creditNote.totalCost);
                    cmd.Parameters.AddWithValue("@IssuedBy", creditNote.issuedBy);
                    cmd.Parameters.AddWithValue("@PreviousBalance", creditNote.customer.Balance);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }
                //update old stock  
                string queryStock = "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-@Quantity) WHERE  idProduct=@idProduct;";
                for (int i = 0; i < old_creditNote.products.Count; i++)
                {
                    using (MySqlCommand cmd3 = new MySqlCommand(queryStock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_creditNote.products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_creditNote.products[i].idProduct);
                        cmd3.ExecuteNonQuery();
                    }
                }

                //delete old Credit Note products
                string queryDelete = "DELETE from CreditNoteProduct WHERE idCreditNote=@idCreditNote; ";

                using (MySqlCommand cmd = new MySqlCommand(queryDelete, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idCreditNote", old_creditNote.idCreditNote);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }


                //insert product
                StringBuilder sCommand = new StringBuilder("INSERT INTO CreditNoteProduct (idCreditNote, idProduct, idInvoice, Quantity) VALUES ");
                List<string> Rows = new List<string>();


                foreach (Product p in creditNote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}')", MySqlHelper.EscapeString(creditNote.idCreditNote.ToString()),
                        p.idProduct, MySqlHelper.EscapeString(p.productInvoiceID.ToString()), p.Quantity));

                    using (MySqlCommand cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+" +
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
                string queryBalance = "UPDATE Customer SET Balance = REPLACE(Balance,Balance,Balance-@amount) WHERE  idCustomer=@idCustomer;";

                using (MySqlCommand cmd3 = new MySqlCommand(queryBalance, conn))
                {
                    cmd3.Parameters.AddWithValue("@amount", creditNote.totalCost - old_creditNote.totalCost);
                    cmd3.Parameters.AddWithValue("@idCustomer", creditNote.customer.idCustomer);
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
