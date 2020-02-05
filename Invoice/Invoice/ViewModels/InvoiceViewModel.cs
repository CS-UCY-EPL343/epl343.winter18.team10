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
                    var date = dataRow.Field<DateTime>("Date");

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
                            m_customer = customer,
                            m_cost = cost,
                            m_VAT = VAT,
                            m_totalCost = invTotalCost,
                            m_date = date.Date.ToString("dd/MM/yyyy"),
                            m_products = new List<InvoiceProduct>()

                            //m_idInvoice = Convert.ToInt32(dataRow[dt.Columns.IndexOf("idInvoice")]),
                            //m_customer = Convert.ToInt32(dataRow[dt.Columns.IndexOf("idCustomer")]),
                            //m_cost = Convert.ToDouble(dataRow[dt.Columns.IndexOf("Cost")]),
                            //m_VAT = Convert.ToDouble(dataRow[dt.Columns.IndexOf("VAT")]),
                            //m_totalCost = Convert.ToDouble(dataRow[dt.Columns.IndexOf("TotalCost")]),
                            //m_date = Convert.ToDateTime(dataRow[dt.Columns.IndexOf("Date")])
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
    }
}
