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
    public class OrderViewModel
    {
        private static readonly MySqlConnection conn = DBConnection.Instance.Connection;

        /// <summary>
        ///     Constructor that fills the list with all the orders
        /// </summary>
        public OrderViewModel()
        {
            orderList = new List<Order>();

            try
            {
                var cmd = new MySqlCommand(
                    "SELECT `Order`.*, `Customer`.`CustomerName`,`Customer`.`City`  FROM `Order`" +
                    " LEFT JOIN `Customer` ON `Order`.`idCustomer` = `Customer`.`idCustomer`; ", conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                var order = new Order();
                foreach (DataRow dataRow in dt.Rows)
                {
                    var customer = dataRow.Field<string>("CustomerName");
                    var city = dataRow.Field<string>("City");
                    var idOrder = dataRow.Field<int>("idOrder");
                    var cost = dataRow.Field<float>("Cost");
                    var VAT = dataRow.Field<float>("VAT");
                    var TotalCost = dataRow.Field<float>("TotalCost");
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
                    var shippingDate = dataRow.Field<DateTime>("ShippingDate");
                    var issuedBy = dataRow.Field<string>("IssuedBy");
                    var status = (OrderStatus) Enum.Parse(typeof(OrderStatus), dataRow.Field<string>("Status"));

                    order = new Order
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
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public List<Order> orderList { get; set; }

        /// <summary>
        ///     Given the order ID retrieves the order and returns it
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static Order getOrder(int orderID)
        {
            var order = new Order();
            try
            {
                var cmd = new MySqlCommand("SELECT * FROM viewOrder WHERE OrderID = " + orderID, conn);
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count == 0)
                    order = null;

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

                    var idOrder = dataRow.Field<int>("OrderID");
                    var cost = dataRow.Field<float>("OrderCost");
                    var VAT = dataRow.Field<float>("OrderVAT");
                    var orderTotalCost = dataRow.Field<float>("OrderTotalCost");
                    var createdDate = dataRow.Field<DateTime>("IssuedDate");
                    var shippingDate = dataRow.Field<DateTime>("ShippingDate");
                    var status = (OrderStatus) Enum.Parse(typeof(OrderStatus), dataRow.Field<string>("Status"));
                    var issuedBy = dataRow.Field<string>("IssuedBy");

                    var productID = dataRow.Field<int>("idProduct");
                    var product = dataRow.Field<string>("ProductName");
                    var prodDescription = dataRow.Field<string>("Description");
                    var stock = dataRow.Field<int>("Stock");
                    var proTotalCost = dataRow.Field<float>("OPCost");
                    var proVat = dataRow.Field<float>("OPVAT");
                    var quantity = dataRow.Field<int>("Quantity");

                    if (count == 0)
                    {
                        count++;
                        order = new Order
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

                    order.products.Add(new Product
                    {
                        idProduct = productID,
                        ProductName = product,
                        ProductDescription = prodDescription,
                        Stock = stock,
                        Total = proTotalCost,
                        Quantity = quantity,
                        Cost = proTotalCost / quantity,
                        Vat = proVat,
                        SellPrice = proTotalCost /
                                    quantity //***Chrisi to cost to ekana gia to kostos paraogis, ego xrisimopio SellPrice **
                    });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return order;
        }

        /// <summary>
        ///     Given the order ID deletes the order from the Database
        /// </summary>
        /// <param name="orderID"></param>
        public static void deleteOrder(int orderID)
        {
            try
            {
                var cmd = new MySqlCommand("DELETE FROM OrderProduct WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM `Order` WHERE idOrder = " + orderID, conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the order ID and the order status updates the status
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="status"></param>
        public static void updateOrderStatus(int orderID, OrderStatus status)
        {
            try
            {
                var cmd = new MySqlCommand("UPDATE `Order` SET Status = '" + status + "' WHERE idOrder = " + orderID,
                    conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        ///     Given the id checks if an order exists with that id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool orderExists(int id)
        {
            try
            {
                int idOrder;
                var cmd = new MySqlCommand("SELECT idOrder FROM `Order` where idOrder=" + id, conn);

                // id_return = cmd.ExecuteNonQuery();
                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idOrder = 0;

                return idOrder == 0 ? false : true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        /// <summary>
        ///     Returns the latest order ID from the database
        /// </summary>
        /// <returns></returns>
        public static int returnLatestOrderID()
        {
            try
            {
                int idOrder;
                var cmd = new MySqlCommand("SELECT idOrder FROM `Order` ORDER BY idOrder DESC LIMIT 1", conn);

                var queryResult = cmd.ExecuteScalar(); //Return an object so first check for null
                if (queryResult != null)
                    // If we have result, then convert it from object to string.
                    idOrder = Convert.ToInt32(queryResult);
                else
                    // Else make id = "" so you can later check it.
                    idOrder = 0;

                return idOrder;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return 0;
        }

        /// <summary>
        ///     Given the order object inserts it in to the database
        /// </summary>
        /// <param name="order"></param>
        public static void insertOrder(Order order)
        {
            try
            {
                //insert invoice 
                var query =
                    "INSERT INTO `Order` (idOrder, idCustomer, IssuedDate, ShippingDate, Cost, Vat, TotalCost, IssuedBy, Status) Values (@idOrder, @idCustomer, @IssuedDate, @ShippingDate, @Cost, @Vat, @TotalCost, @IssuedBy, @Status)";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
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
                var sCommand =
                    new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                var Rows = new List<string>();

                foreach (var p in order.products)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                    ));
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
        ///     Given the old and updated order update the order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="old_Order"></param>
        public static void updateOrder(Order order, Order old_Order)
        {
            try
            {
                //update Order 
                var query =
                    "UPDATE `Order` SET  idOrder=@idOrder, idCustomer=@idCustomer, IssuedDate=@IssuedDate, ShippingDate=@ShippingDate, Cost=@Cost, Vat=@Vat, TotalCost=@TotalCost, IssuedBy=@IssuedBy, Status=@Status WHERE idOrder=@idOrder ";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query, conn))
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
                var query_delete_invoiceProducts = "DELETE from OrderProduct WHERE idOrder=@idOrder;";
                // Yet again, we are creating a new object that implements the IDisposable
                // interface. So we create a new using statement.
                using (var cmd = new MySqlCommand(query_delete_invoiceProducts, conn))
                {
                    // Now we can start using the passed values in our parameters:
                    cmd.Parameters.AddWithValue("@idOrder", old_Order.idOrder);
                    // Execute the query
                    cmd.ExecuteNonQuery();
                }

                //insert products
                var sCommand =
                    new StringBuilder("INSERT INTO OrderProduct (idOrder, idProduct, Quantity, Cost, Vat) VALUES ");
                var Rows = new List<string>();

                foreach (var p in order.products)
                    Rows.Add(string.Format("('{0}','{1}','{2}','{3}','{4}')",
                        MySqlHelper.EscapeString(order.idOrder.ToString()),
                        MySqlHelper.EscapeString(p.idProduct.ToString()),
                        MySqlHelper.EscapeString(p.Quantity.ToString()),
                        MySqlHelper.EscapeString(p.Total.ToString().Replace(",", ".")),
                        MySqlHelper.EscapeString(p.Vat.ToString().Replace(",", "."))
                    ));
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
    }
}