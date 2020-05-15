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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    ///     Interaction logic for OrderCreate.xaml
    /// </summary>
    public partial class OrderCreate : Page
    {
        private CustomerViewModel customerView;
        private readonly OrderMain orderMain;
        private ProductViewModel productView;

        private bool Refresh_DB_data = true;

        public OrderCreate(OrderMain orderMain)
        {
            InitializeComponent();
            this.orderMain = orderMain;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Loads the customers in the combobox as well as the new invoice ID
        /// </summary>
        public void load()
        {
            if (Refresh_DB_data)
            {
                productView = new ProductViewModel();
                customerView = new CustomerViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;
                comboBox_Product.ItemsSource = productView.productList;
                textBox_orderNumber.Text = (OrderViewModel.returnLatestOrderID() + 1).ToString();
                OrderDate.SelectedDate = DateTime.Today; //set curent date 
                dueDate.SelectedDate = DateTime.Today.AddDays(60);
                ; //set curent date +60
            }

            Refresh_DB_data = false;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the customers.
        ///     Loads the customer's information in the appropriate text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                textBox_Customer.Text = ((Customer) comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customer) comboBox_customer.SelectedItem).Address + ", " +
                                       ((Customer) comboBox_customer.SelectedItem).City + ", " +
                                       ((Customer) comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customer) comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customer) comboBox_customer.SelectedItem).Email;
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
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(Control.BorderBrushProperty);
                textBox_Product.Text = ((Product) comboBox_Product.SelectedItem).ProductName;
                textBox_ProductQuantity.Text = ((Product) comboBox_Product.SelectedItem).Quantity.ToString();
                textBox_ProductDescription.Text = ((Product) comboBox_Product.SelectedItem).ProductDescription;
                textBox_ProductStock.Text = ((Product) comboBox_Product.SelectedItem).Stock.ToString();
                textBox_ProductPrice.Text = ((Product) comboBox_Product.SelectedItem).SellPrice.ToString("n2");
                textBox_ProductVat.Text = (((Product) comboBox_Product.SelectedItem).Vat * 100).ToString();
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
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
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
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
        }

        /// <summary>
        ///     Checks if the product selected in the combobox already exists in the product grid.
        /// </summary>
        /// <returns></returns>
        private bool product_already_selected()
        {
            var gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (var p in gridProducts)
                if (p.idProduct == ((Product) comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product already selected");
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Checks if the product details are all completed and valid before its added to the grid.
        /// </summary>
        /// <returns></returns>
        private bool Check_AddProduct_CompletedValues()
        {
            var all_completed = true;
            int n;
            if (comboBox_Product.SelectedIndex <= -1 || product_already_selected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            else
            {
                comboBox_Product_border.BorderThickness = new Thickness(0);
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
            if (Check_AddProduct_CompletedValues())
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

                double NetTotal_TextBlock_var = 0;
                NetTotal_TextBlock_var = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("C");
                double Vat_TextBlock_var = 0;
                Vat_TextBlock_var = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                Vat_TextBlock_var = Vat_TextBlock_var + Convert.ToDouble(textBox_ProductTotal.Text) *
                    ((Product) comboBox_Product.SelectedItem).Vat;
                Vat_TextBlock.Text = Vat_TextBlock_var.ToString("C");
                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("C");
            }
        }

        /// <summary>
        ///     Removes a product from the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateOrder_REMOVE(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Product) ProductDataGrid.CurrentCell.Item;
            var netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal = netTotal - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");
            var vat = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            vat = vat - CurrentCell_Product.Total * CurrentCell_Product.Vat;
            Vat_TextBlock.Text = vat.ToString("C");
            TotalAmount_TextBlock.Text = (netTotal + vat).ToString("C");
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
        private bool Check_CustomerForm()
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
        ///     Checks if the order details are completed
        /// </summary>
        /// <returns></returns>
        private bool Check_DetailsForm()
        {
            var all_ok = true;
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                all_ok = false;
            }

            if (OrderDate.SelectedDate.Value > dueDate.SelectedDate.Value)
            {
                dueDate.BorderBrush = Brushes.Red;
                OrderDate.BorderBrush = Brushes.Red;
                MessageBox.Show("Due date is earlier than created date");
                all_ok = false;
            }

            return all_ok;
        }

        /// <summary>
        ///     Checks if the grid has products in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Order based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Order createOrderObject()
        {
            Order my_order;
            my_order = new Order();
            my_order.customer = (Customer) comboBox_customer.SelectedItem;
            my_order.products = ProductDataGrid.Items.OfType<Product>().ToList();
            my_order.idOrder = int.Parse(textBox_orderNumber.Text);
            my_order.cost = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            my_order.VAT = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            my_order.totalCost = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            my_order.createdDate = OrderDate.SelectedDate.Value.Date;
            my_order.shippingDate = dueDate.SelectedDate.Value.Date;
            my_order.issuedBy = issuedBy.Text;
            Enum.TryParse("Pending", out OrderStatus stautsenum);
            my_order.status = stautsenum;
            return my_order;
        }

        /// <summary>
        ///     After validating creates the Order and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            var ALL_VALUES_OK = true;
            if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                var order = createOrderObject();
                OrderViewModel.insertOrder(order);
                MessageBox.Show("Order with ID " + order.idOrder + " was created.");
                orderMain.viewOrder(order.idOrder);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customer's information
        /// </summary>
        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
            comboBox_customer_border.BorderThickness = new Thickness(0);
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears the invoice details
        /// </summary>
        private void Clear_Details()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(Control.BorderBrushProperty);
            OrderDate.ClearValue(Control.BorderBrushProperty);
            dueDate.ClearValue(Control.BorderBrushProperty);
            OrderDate.SelectedDate = DateTime.Today; //set current date 
            dueDate.SelectedDate = DateTime.Today.AddDays(60); //set curent date +60
        }

        /// <summary>
        ///     Clears all products from the product grid
        /// </summary>
        private void Clear_ProductGrid()
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
            Btn_clearProduct_Click(new object(), new RoutedEventArgs());
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            Refresh_DB_data = true;
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
        ///     The method that handles the event Selected Date Changed on the datepicker OrderDate.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void orderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            OrderDate.ClearValue(Control.BorderBrushProperty);
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
    }
}