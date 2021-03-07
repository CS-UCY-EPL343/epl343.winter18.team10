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
    /// <summary>
    ///     The class that communicates with the database in relation with Credit Notes
    /// </summary>
    public class CreditNoteViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the credit notes
        /// </summary>
        public CreditNoteViewModel()
        {
            creditNoteList = new List<CreditNote>();

            try
            {
                var cmd = new MySqlCommand("SELECT `CreditNote`.*, `Customer`.`CustomerName` FROM `CreditNote`" +
                                           " LEFT JOIN `Customer` ON `CreditNote`.`idCustomer` = `Customer`.`idCustomer`; ",
                    conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var cred = new CreditNote();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var idCreditNote = dataRow.Field<int>("idCreditNote");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var credTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    cred = new CreditNote
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<CreditNote> creditNoteList { get; set; }

        /// <summary>
        ///     Given the credit note ID retrieves the credit note and returns it
        /// </summary>
        /// <param name="creditNoteID"></param>
        /// <returns></returns>
        public static CreditNote getCreditNote(int creditNoteID)
        {
            var cred = new CreditNote();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM viewCreditNote WHERE CreditNoteID = " + creditNoteID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    cred = null;

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

                    var idCreditNote = dataRow.Field<int>("CreditNoteID");
                    var cost = dataRow.Field<float>("CreditNoteCost");
                    var VAT = dataRow.Field<float>("CreditNoteVAT");
                    var credTotalCost = dataRow.Field<float>("CreditNoteTotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<int>("idProduct");
                    var invoiceID = dataRow.Field<int>("idInvoice");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var proVat = dataRow.Field<float>("IPVAT");
                    var quantity = dataRow.Field<int>("Quantity");

                    if (count == 0)
                    {
                        count++;
                        cred = new CreditNote
                        {
                            idCreditNote = idCreditNote,
                            customerName = customerName,
                            cost = cost,
                            VAT = VAT,
                            totalCost = credTotalCost,
                            createdDate = createdDate,
                            issuedBy = issuedBy,
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

                    cred.products.Add(new Product
                    {
                        idProduct = productID,
                        productInvoiceID = invoiceID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        Stock = stock,
                        Total = proTotalCost * quantity, //@chrisi ekana alagi edo * quantity
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

            return cred;
        }

        /// <summary>
        ///     Retrieves and returns the credit notes as statement items matching the parameters given
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<StatementItem> getCreditNotesForStatement(int customerID, DateTime from, DateTime to)
        {
            var list = new List<StatementItem>();

            try
            {
                var query = "SELECT `CreditNote`.* FROM `CreditNote`" +
                            " LEFT JOIN `Customer` ON `CreditNote`.`idCustomer` = `Customer`.`idCustomer` WHERE `Customer`.`idCustomer` = @customerID AND " +
                            "`CreditNote`.`CreatedDate` >= @from AND `CreditNote`.`CreatedDate` <= @to; ";
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
                    var idCreditNote = dataRow.Field<int>("idCreditNote");
                    var credTotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var balance = dataRow.Field<float>("PreviousBalance");

                    inv = new StatementItem
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return list;
        }

        /// <summary>
        ///     Given the credit note ID deletes the credit note from the Database
        /// </summary>
        /// <param name="creditNoteID"></param>
        public static void deleteCreditNote(int creditNoteID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM CreditNoteProduct WHERE idCreditNote = " + creditNoteID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM CreditNote WHERE idCreditNote = " + creditNoteID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Returns the latest credit note ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestCreditNoteID()
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idCreditNote FROM CreditNote ORDER BY idCreditNote DESC LIMIT 1",
                    conn);

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
        ///     Given the credit note object inserts it in to the database
        /// </summary>
        /// <param name="creditNote"></param>
        public static void insertCreditNote(CreditNote creditNote)
        {
            try
            {
                //insert invoice 
                var query =
                    "INSERT INTO CreditNote (idCreditNote, idCustomer, Cost, Vat, TotalCost, CreatedDate, PreviousBalance, IssuedBy) Values (@idCreditNote, @idCustomer, @Cost, @Vat, @TotalCost, @CreatedDate, @PreviousBalance, @IssuedBy)";

                using (var cmd = new MySqlCommand(query, conn))
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
                var sCommand =
                    new StringBuilder(
                        "INSERT INTO CreditNoteProduct (idCreditNote, idProduct, idInvoice, Quantity) VALUES ");
                var Rows = new List<string>();


                foreach (var p in creditNote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}')",
                        MySqlHelper.EscapeString(creditNote.idCreditNote.ToString()),
                        p.idProduct, MySqlHelper.EscapeString(p.productInvoiceID.ToString()), p.Quantity));

                    using (var cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+" +
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
        ///     Given the id checks if a credit note exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool CreditNoteExists(int id)
        {
            try
            {
                int idInvoice;
                var cmd = new MySqlCommand("SELECT idCreditNote FROM CreditNote where idCreditNote=" + id, conn);

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
        ///     Given the old and updated credit note update the credit note
        /// </summary>
        /// <param name="creditNote"></param>
        /// <param name="old_creditNote"></param>
        public static void updateCreditNote(CreditNote creditNote, CreditNote old_creditNote)
        {
            try
            {
                //update Credit Note 
                var query =
                    "UPDATE CreditNote SET  Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, IssuedBy=@IssuedBy, PreviousBalance=@PreviousBalance WHERE idCreditNote=@idCreditNote;";

                using (var cmd = new MySqlCommand(query, conn))
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
                var queryStock =
                    "UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock-@Quantity) WHERE  idProduct=@idProduct;";
                for (var i = 0; i < old_creditNote.products.Count; i++)
                    using (var cmd3 = new MySqlCommand(queryStock, conn))
                    {
                        cmd3.Parameters.AddWithValue("@Quantity", old_creditNote.products[i].Quantity);
                        cmd3.Parameters.AddWithValue("@idProduct", old_creditNote.products[i].idProduct);
                        cmd3.ExecuteNonQuery();
                    }

                //delete old Credit Note products
                var queryDelete = "DELETE from CreditNoteProduct WHERE idCreditNote=@idCreditNote; ";

                using (var cmd = new MySqlCommand(queryDelete, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idCreditNote", old_creditNote.idCreditNote);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }


                //insert product
                var sCommand =
                    new StringBuilder(
                        "INSERT INTO CreditNoteProduct (idCreditNote, idProduct, idInvoice, Quantity) VALUES ");
                var Rows = new List<string>();


                foreach (var p in creditNote.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}')",
                        MySqlHelper.EscapeString(creditNote.idCreditNote.ToString()),
                        p.idProduct, MySqlHelper.EscapeString(p.productInvoiceID.ToString()), p.Quantity));

                    using (var cmd3 = new MySqlCommand("UPDATE Product SET Stock = REPLACE(Stock,Stock,Stock+" +
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
        public static List<int> getCustomerCreditNotes(int customerID)
        {
            var customer_creditNotes_list = new List<int>();

            try
            {
                var cmd = new MySqlCommand("SELECT idCreditNote FROM creditnote WHERE idCustomer = " + customerID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow dataRow in dt.Rows)
                {
                    var idCreditNote = dataRow.Field<int>("idCreditNote");
                    customer_creditNotes_list.Add(idCreditNote);
                }

                return customer_creditNotes_list;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }
        public static float getCreditNoteCost(int creditNoteId)
        {
            float ret = 0;
            try
            {
                var cmd = new MySqlCommand("SELECT TotalCost FROM creditnote WHERE idCreditNote = " + creditNoteId, conn);
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