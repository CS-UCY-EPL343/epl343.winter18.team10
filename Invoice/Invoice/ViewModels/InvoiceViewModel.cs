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
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewInvoice", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var previousID = -1;
                Invoice inv = new Invoice();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var idInvoice = dataRow.Field<Int32>("InvoiceID");
                    var customer = dataRow.Field<string>("CustomerName");
                    var cost = dataRow.Field<float>("InvoiceCost");
                    var VAT = dataRow.Field<float>("VAT");
                    var invTotalCost = dataRow.Field<float>("InvoiceTotalCost");
                    var date = dataRow.Field<DateTime>("CreatedDate");

                    var product = dataRow.Field<string>("ProductName");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var quantity = dataRow.Field<Int32>("Quantity");

                    if (previousID != idInvoice)
                    {
                        if (previousID != -1)
                            invoiceList.Add(inv);

                        inv = new Invoice()
                        {
                            m_idInvoice = idInvoice,
                            m_customerName = customer,
                            m_cost = cost,
                            m_VAT = VAT,
                            m_totalCost = invTotalCost,
                            m_createdDate = date,
                            m_products = new List<InvoiceProduct>()
                        };
                        previousID = idInvoice;
                    }
                    inv.m_products.Add(new InvoiceProduct()
                    {
                        m_idInvoice = idInvoice,
                        m_productName = product,
                        m_quantity = quantity,
                        m_totalCost = proTotalCost
                    });
                }
                invoiceList.Add(inv);

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
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewInvoice WHERE idInvoice = " + invoiceID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                int count = 0;
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customerName = dataRow.Field<string>("CustomerName");
                    var phoneNumber = dataRow.Field<int>("PhoneNumber");
                    var email = dataRow.Field<string>("Email");
                    var address = dataRow.Field<string>("Address");

                    var idInvoice = dataRow.Field<Int32>("InvoiceID");
                    var cost = dataRow.Field<float>("InvoiceCost");
                    var VAT = dataRow.Field<float>("VAT");
                    var invTotalCost = dataRow.Field<float>("InvoiceTotalCost");
                    var createdDate = dataRow.Field<DateTime>("CreatedDate");
                    var dueDate = dataRow.Field<DateTime>("DueDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("IPCost");
                    var quantity = dataRow.Field<Int32>("Quantity");

                    if (count == 0)
                    {
                        inv = new Invoice()
                        {
                            m_idInvoice = idInvoice,
                            m_customerName = customerName,
                            m_cost = cost,
                            m_VAT = VAT,
                            m_totalCost = invTotalCost,
                            m_createdDate = createdDate,
                            m_dueDate = dueDate,
                            m_products = new List<InvoiceProduct>(),
                            m_customer = new Customers()
                            {
                                CustomerName = customerName,
                                PhoneNumber = phoneNumber,
                                Email = email,
                                Address = address
                            }
                        };
                    }

                    inv.m_products.Add(new InvoiceProduct()
                    {
                        m_idInvoice = idInvoice,
                        m_productName = product,
                        m_quantity = quantity,
                        m_totalCost = proTotalCost
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
    }
}
