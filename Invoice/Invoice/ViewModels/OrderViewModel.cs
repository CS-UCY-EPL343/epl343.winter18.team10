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
    public class OrderViewModel
    {
        public List<Order> orderList { get; set; }
        private static MySqlConnection conn = DBConnection.Instance.Connection;

        public OrderViewModel()
        {
            orderList = new List<Order>();

            try
            {
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static Order getOrder(int orderID)
        {
            Order order = new Order();
            try
            {
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
                        Vat = proVat,
                        SellPrice = proTotalCost / quantity //***Chrisi to cost to ekana gia to kostos paraogis, ego xrisimopio SellPrice **
                    });
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return order;
        }

        public static void deleteOrder(int orderID)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("DELETE FROM OrderProduct WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM `Order` WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateOrderStatus(int orderID, OrderStatus status)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE `Order` SET Status = '" + status.ToString() + "' WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static bool orderExists(int id)
        {
            try
            {
                int idOrder;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idOrder FROM `Order` where idOrder=" + id.ToString(), conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idOrder = 0;

                return ((idOrder == 0) ? false : true);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }


        public static int returnLatestOrderID()
        {
            try
            {
                int idOrder;
                MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand("SELECT idOrder FROM `Order` ORDER BY idOrder DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar();//Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idOrder = 0;

                return idOrder;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        public static void insertOrder(Order order)
        {
            try
            {
                //insert invoice 
                string query = "INSERT INTO `Order` (idOrder, idCustomer, IssuedDate, ShippingDate, Cost, Vat, TotalCost, IssuedBy, Status) Values (@idOrder, @idCustomer, @IssuedDate, @ShippingDate, @Cost, @Vat, @TotalCost, @IssuedBy, @Status)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idOrder", order.idOrder);
                    cmd.Parameters.AddWithValue("@idCustomer", order.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@IssuedDate", order.createdDate);
                    cmd.Parameters.AddWithValue("@ShippingDate", order.shippingDate);
                    cmd.Parameters.AddWithValue("@Cost", order.cost);
                    cmd.Parameters.AddWithValue("@Vat", order.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", order.totalCost);
                    cmd.Parameters.AddWithValue("@IssuedBy", order.issuedBy);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in order.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                       ));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void updateOrder(Order order, Order old_Order)
        {
            try
            {
                //update Order 
                string query = "UPDATE `Order` SET  idOrder=@idOrder, idCustomer=@idCustomer, IssuedDate=@IssuedDate, ShippingDate=@ShippingDate, Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, IssuedBy=@IssuedBy, Status=@Status WHERE idOrder=@idOrder ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Now we can start using the passed values in our parameters:

                    cmd.Parameters.AddWithValue("@idOrder", order.idOrder);
                    cmd.Parameters.AddWithValue("@idCustomer", order.customer.idCustomer);
                    cmd.Parameters.AddWithValue("@IssuedDate", order.createdDate);
                    cmd.Parameters.AddWithValue("@ShippingDate", order.shippingDate);
                    cmd.Parameters.AddWithValue("@Cost", order.cost);
                    cmd.Parameters.AddWithValue("@Vat", order.VAT);
                    cmd.Parameters.AddWithValue("@TotalCost", order.totalCost);
                    cmd.Parameters.AddWithValue("@IssuedBy", order.issuedBy);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //delete old Order products
                string query_delete_invoiceProducts = "DELETE from OrderProduct WHERE idOrder=@idOrder;";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (MySqlCommand cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idOrder", old_Order.idOrder);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                StringBuilder sCommand = new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                List<string> Rows = new List<string>();

                foreach (Product p in order.products)
                {
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                       ));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), conn))
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
