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

        public static Order getOrderById(int orderID)
        {
            MySqlConnection conn;

            Order order = new Order();
            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM viewOrder WHERE OrderID = " + orderID, conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    order = null;

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

                    var idOrder = dataRow.Field<Int32>("OrderID");
                    var cost = dataRow.Field<float>("OrderCost");
                    var VAT = dataRow.Field<float>("OrderVAT");
                    var orderTotalCost = dataRow.Field<float>("OrderTotalCost");
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
                    var shippingDate = dataRow.Field<DateTime>("ShippingDate");
                    var status = (OrderStatus)Enum.Parse(typeof(OrderStatus), dataRow.Field<string>("Status"));
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<Int32>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("OPCost");
                    var proVat = dataRow.Field<float>("OPVAT");
                    var quantity = dataRow.Field<Int32>("Quantity");

                    if (count == 0)
                    {
                        count++;
                        order = new Order()
                        {
                            idOrder = idOrder,
                            customerName = customerName,
                            cost = cost,
                            VAT = VAT,
                            totalCost = orderTotalCost,
                            createdDate = createdDate,
                            shippingDate = shippingDate,
                            status = status,
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

                    order.products.Add(new Product()
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

            return order;
        }

        public static void deleteOrderByID(int orderID)
        {
            MySqlConnection conn;

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM OrderProduct WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM `Order` WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }
    }
}
