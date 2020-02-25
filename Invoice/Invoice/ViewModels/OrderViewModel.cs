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
    public class OrderViewModel
    {
        public List<Order> orderList { get; set; }
        static string myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                           "pwd=CCfHC5PWLjsSJi8G;database=invoice";

        public OrderViewModel()
        {
            orderList = new List<Order>();
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT `Order`.*, `Customer`.`CustomerName`,`Customer`.`City`  FROM `Order`" +
                    " LEFT JOIN `Customer` ON `Order`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Order order = new Order();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var city = dataRow.Field<string>("City");
                    var idOrder = dataRow.Field<Int32>("idOrder");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var TotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
                    var shippingDate = dataRow.Field<DateTime>("ShippingDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");
                    var status = (OrderStatus)Enum.Parse(typeof(OrderStatus), dataRow.Field<string>("Status"));

                    order = new Order()
                    {
                        idOrder = idOrder,
                        customerName = customer,
                        city = city,
                        cost = cost,
                        VAT = VAT,
                        totalCost = TotalCost,
                        createdDate = createdDate,
                        shippingDate = shippingDate,
                        status = status,
                        issuedBy = issuedBy                        
                    };

                    orderList.Add(order);
                }

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }
    }
}
