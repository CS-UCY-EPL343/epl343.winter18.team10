﻿// /*****************************************************************************
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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Classes;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    ///     Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class InvoiceCreate : Page
    {
        private CustomerViewModel customerView;
        private readonly InvoiceMain invoiceMain;
        private int orderID = -1;
        private ProductViewModel productView;
        private bool refreshDataDB = true;

        public InvoiceCreate(InvoiceMain invoiceMain)
        {
            InitializeComponent();
            this.invoiceMain = invoiceMain;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Loads the customers in the combobox as well as the new invoice ID
        /// </summary>
        public void load()
        {
            if (refreshDataDB)
            {
                customerView = new CustomerViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;
                textBox_invoiceNumber.Text = (InvoiceViewModel.returnLatestInvoiceID() + 1).ToString();
                invoiceDate.SelectedDate = DateTime.Today; //set current date 
                dueDate.SelectedDate = DateTime.Today.AddDays(30); //set current date +60

            }

            refreshDataDB = false;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the customers.
        ///     Loads the customer's information in the appropriate text boxes, as well as the products in
        ///     combobox_Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                var customer = (Customer) comboBox_customer.SelectedItem;
                textBox_Customer.Text = customer.CustomerName;
                textBox_Address.Text = customer.Address + ", " + customer.City + ", " + customer.Country;
                textBox_Contact_Details.Text = customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = customer.Email;

                productView = new ProductViewModel(customer.idCustomer);
                comboBox_Product.ItemsSource = productView.productList.OrderBy(Product => Product.ProductName);
            }
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the Products.
        ///     Loads the product's information in their appropriate text boxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {
                var product = (Product) comboBox_Product.SelectedItem;
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
                textBox_Product.Text = product.ProductName;
                textBox_ProductQuantity.Text = product.Quantity.ToString();
                textBox_ProductDescription.Text = product.ProductDescription;
                textBox_ProductStock.Text = product.Stock.ToString();
                textBox_ProductPrice.Text = product.SellPrice.ToString("n2");
                textBox_ProductVat.Text = (product.Vat * 100).ToString();
                
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product quantity.
        ///     If the quantity is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out var quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var price) &&
                comboBox_Product.SelectedIndex > -1)
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product price.
        ///     If the price is valid the product Total textbox is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out var quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var price) &&
                comboBox_Product.SelectedIndex > -1)
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
        }

        /// <summary>
        ///     Checks if the product selected in the combobox already exists in the product grid.
        /// </summary>
        /// <returns></returns>
        private bool productAlreadySelected()
        {
            var gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (var p in gridProducts)
                if (p.idProduct == ((Product) comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product '" + p.ProductName + "' already selected");
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Checks if the product details are all completed and valid before its added to the grid.
        /// </summary>
        /// <returns></returns>
        private bool checkAddProductCompletedValues()
        {
            var all_completed = true;
            int n;
            if (comboBox_Product.SelectedIndex <= -1 || productAlreadySelected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }

            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || n < 0)
            {
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
            }

            if (!float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out var f) || f < 0)
            {
                all_completed = false;
                textBox_ProductPrice.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
            }

            return all_completed;
        }

        /// <summary>
        ///     Adds the product selected to the product grid and updates the total and Vat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (checkAddProductCompletedValues())
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product) comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    Stock = Convert.ToInt32(textBox_ProductStock.Text),
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product) comboBox_Product.SelectedItem).Vat
                });

                var netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                netTotal += Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = netTotal.ToString("C");

                var VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                VAT += Convert.ToDouble(textBox_ProductTotal.Text) * ((Product) comboBox_Product.SelectedItem).Vat;
                Vat_TextBlock.Text = VAT.ToString("C");

                TotalAmount_TextBlock.Text = (netTotal + VAT).ToString("C");
            }
        }
        private void Btn_getLatestPrice_Click(object sender, RoutedEventArgs e)
        {
        float ret = 0;
            MySqlConnection conn = DBConnection.Instance.Connection;
            int idProduct = 0;
            int idcustomer = 0;

            if (comboBox_Product.SelectedIndex > 0)
            {
                idProduct = ((Product)comboBox_Product.SelectedItem).idProduct;

            }
            if (comboBox_customer.SelectedIndex > 0)
            {
                idcustomer = ((Customer)comboBox_customer.SelectedItem).idCustomer;

            }

            if (idProduct>0 && idcustomer>0)
            {

            
            try
            {
                var cmd = new MySqlCommand("getLatestProductPrice", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pid", SqlDbType.Int).Value = idProduct;
                cmd.Parameters["@pid"].Direction = ParameterDirection.Input;
                cmd.Parameters.AddWithValue("@cid", SqlDbType.Int).Value = idcustomer;
                cmd.Parameters["@cid"].Direction = ParameterDirection.Input;

                //MySqlDataReader reader = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                    var total2 = cmd.ExecuteScalar();
                    if (total2!=null)
                    {
                        float total3 = 0;

                        if (float.TryParse(total2.ToString(), out total3)) ret = total3;
                        textBox_ProductPrice.Text = total3.ToString();
                        textBox_ProductPrice.BorderBrush = Brushes.Green;
                        textBox_ProductPrice.BorderThickness = new Thickness(2);

                    }


                    //reader.Close();
                }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            }

        }
        /// <summary>
        ///     Removes a product from the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Product) ProductDataGrid.CurrentCell.Item;

            var netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal -= Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");

            var VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            VAT -= CurrentCell_Product.Total * CurrentCell_Product.Vat;
            Vat_TextBlock.Text = VAT.ToString("C");

            TotalAmount_TextBlock.Text = (netTotal + VAT).ToString("C");
            ProductDataGrid.Items.Remove(ProductDataGrid.CurrentCell.Item);
        }

        /// <summary>
        ///     Clears text when textbox gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox) sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        /// <summary>
        ///     Clears the product's information from all the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_Product.SelectedIndex = -1;
            textBox_Product.Text = "";
            textBox_ProductDescription.Text = "";
            textBox_ProductStock.Text = "";
            textBox_ProductQuantity.Text = "";
            textBox_ProductPrice.Text = "";
            textBox_ProductVat.Text = "";
            textBox_ProductTotal.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Checks if a customer is selected
        /// </summary>
        /// <returns></returns>
        private bool checkCustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if the invoice details are completed
        /// </summary>
        /// <returns></returns>
        private bool checkDetailsForm()
        {
            var all_ok = true;
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                all_ok = false;
            }

            if (invoiceDate.SelectedDate.Value > dueDate.SelectedDate.Value)
            {
                dueDate.BorderBrush = Brushes.Red;
                invoiceDate.BorderBrush = Brushes.Red;
                MessageBox.Show("Due date is earlier than created date");
                all_ok = false;
            }

            return all_ok;
        }

        /// <summary>
        ///     Checks if the grid has products in it
        /// </summary>
        /// <returns></returns>
        private bool hasItemsSelected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selected any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Invoice based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Invoice createInvoiceObject()
        {
            return new Invoice
            {
                customer = (Customer)comboBox_customer.SelectedItem,
                products = ProductDataGrid.Items.OfType<Product>().ToList(),
                idInvoice = int.Parse(textBox_invoiceNumber.Text),
                cost = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency),
                VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency),
                totalCost = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency),
                createdDate = invoiceDate.SelectedDate.Value,
                dueDate = dueDate.SelectedDate.Value,
                issuedBy = issuedBy.Text,
                isPaid = (bool)isPaidButton.IsChecked
            };
        }

        /// <summary>
        ///     After validating creates the Invoice and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if (checkCustomerForm() & checkDetailsForm() & hasItemsSelected())
            {
                var inv = createInvoiceObject();
                inv.createdDate += DateTime.Now.TimeOfDay;
                InvoiceViewModel.insertInvoice(inv);
                if (orderID != -1) OrderViewModel.updateOrderStatus(orderID, OrderStatus.Completed);
                MessageBox.Show("Invoice with ID " + inv.idInvoice + " was created.");
                invoiceMain.viewInvoice(inv.idInvoice);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customer's information
        /// </summary>
        private void clearCustomer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears the invoice details
        /// </summary>
        private void clearDetails()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(Control.BorderBrushProperty);
            invoiceDate.ClearValue(Control.BorderBrushProperty);
            dueDate.ClearValue(Control.BorderBrushProperty);
            invoiceDate.SelectedDate = DateTime.Today; //set curent date 

            dueDate.SelectedDate = DateTime.Today.AddDays(60); //set current date +60
        }

        /// <summary>
        ///     Clears all products from the product grid
        /// </summary>
        private void clearProductGrid()
        {
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            orderID = -1;
            comboBox_customer_border.BorderThickness = new Thickness(0);
            Btn_clearProduct_Click(null, null);
            clearCustomer();
            clearDetails();
            clearProductGrid();
            refreshDataDB = true;
            load();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datepicker InvoiceDate.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invoiceDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            invoiceDate.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datepicker dueDate.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dueDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dueDate.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Loads the order information on the page to be issued as an invoice
        /// </summary>
        /// <param name="order"></param>
        public void loadOrder(Order order)
        {
            Btn_clearAll_Click(null, null);
            if (order != null)
            {
                orderID = order.idOrder;

                // Customer details                                
                comboBox_customer.SelectedValue = order.customer.CustomerName;
                textBox_Customer.Text = order.customer.CustomerName;
                textBox_Contact_Details.Text = order.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = order.customer.Email;
                textBox_Address.Text =
                    order.customer.Address + ", " + order.customer.City + ", " + order.customer.Country;

                // Invoice details
                NetTotal_TextBlock.Text = order.cost.ToString("C");
                Vat_TextBlock.Text = order.VAT.ToString("C");
                TotalAmount_TextBlock.Text = order.totalCost.ToString("C");

                // Invoice products        
                for (var i = 0; i < order.products.Count; i++)
                    ProductDataGrid.Items.Add(new Product
                    {
                        idProduct = order.products[i].idProduct,
                        ProductName = order.products[i].ProductName,
                        ProductDescription = order.products[i].ProductDescription,
                        Stock = order.products[i].Stock,
                        SellPrice = order.products[i].SellPrice,
                        Quantity = order.products[i].Quantity,
                        Total = order.products[i].Total,
                        Vat = order.products[i].Vat
                    });
            }
        }
    }
}