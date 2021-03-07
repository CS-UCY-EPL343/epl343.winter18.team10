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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    ///     Interaction logic for QuoteCreate.xaml
    /// </summary>
    public partial class QuoteCreate : Page
    {
        private CustomerViewModel customerView;
        private ProductViewModel productView;
        private readonly QuoteMain quoteMain;
        private bool Refresh_DB_data = true;

        public QuoteCreate(QuoteMain quoteMain)
        {
            InitializeComponent();
            this.quoteMain = quoteMain;
        }

        /// <summary>
        ///     Loads the customers and products in their combobox as well as the new quote ID
        /// </summary>
        public void load()
        {
            if (Refresh_DB_data)
            {
                productView = new ProductViewModel();
                customerView = new CustomerViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;
                comboBox_Product.ItemsSource = productView.productList.OrderBy(Product => Product.ProductName);

                textBox_idQuote.Text = (QuoteViewModel.returnLatestQuoteID() + 1).ToString();
                invoiceDate.SelectedDate = DateTime.Today; //set current date 
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
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
                textBox_Product.Text = ((Product) comboBox_Product.SelectedItem).ProductName;
                textBox_ProductDescription.Text = ((Product) comboBox_Product.SelectedItem).ProductDescription;
                textBox_ProductPrice.Text = ((Product) comboBox_Product.SelectedItem).SellPrice.ToString("n2");
            }
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
            if (comboBox_Product.SelectedIndex <= -1 || product_already_selected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }

            if (!float.TryParse(textBox_ProductQuote.Text.Replace('.', ','), out var f) || f < 0 ||
                f > float.Parse(textBox_ProductPrice.Text))
            {
                all_completed = false;
                textBox_ProductQuote.BorderBrush = Brushes.Red;
                MessageBox.Show("Offer price can not be larger than actual price.");
            }
            else
            {
                textBox_ProductQuote.ClearValue(Control.BorderBrushProperty);
            }

            return all_completed;
        }

        /// <summary>
        ///     Adds the product selected to the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (Check_AddProduct_CompletedValues())
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product) comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    OfferPrice = Convert.ToDouble(textBox_ProductQuote.Text.Replace('.', ','))
                });
        }

        /// <summary>
        ///     Removes a product from the product grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_remove_offer_from_grid(object sender, RoutedEventArgs e)
        {
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
            textBox_ProductPrice.Text = "";
            textBox_ProductQuote.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(Control.BorderBrushProperty);
            textBox_ProductQuote.ClearValue(Control.BorderBrushProperty);
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
        ///     Checks if the quote details are completed
        /// </summary>
        /// <returns></returns>
        private bool Check_DetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if the grid has products in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selected any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Quote based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Quote make_object_Quote()
        {
            Quote myquote;
            myquote = new Quote();
            myquote.customer = (Customer) comboBox_customer.SelectedItem;
            myquote.products = ProductDataGrid.Items.OfType<Product>().ToList();
            myquote.idQuote = int.Parse(textBox_idQuote.Text);
            myquote.createdDate = invoiceDate.SelectedDate.Value.Date;
            myquote.issuedBy = issuedBy.Text;
            return myquote;
        }

        /// <summary>
        ///     After validating creates the Quote and switches to viewing it
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
                var quote = make_object_Quote();
                QuoteViewModel.insertQuote(quote);
                MessageBox.Show("Quote with ID " + quote.idQuote + " was created.");
                quoteMain.viewQuote(quote.idQuote);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customer's information
        /// </summary>
        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears the quote details
        /// </summary>
        private void Clear_Details()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(Control.BorderBrushProperty);
            invoiceDate.SelectedDate = DateTime.Today; //set curent date 
        }

        /// <summary>
        ///     Clears all products from the product grid
        /// </summary>
        private void Clear_ProductGrid()
        {
            ProductDataGrid.Items.Clear();
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
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
        ///     The method that handles the event Text Changed on the textbox product quote.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_ProductQuote_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductQuote.ClearValue(Control.BorderBrushProperty);
        }

        private void textBox_Product_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        //load all products of category
        private void Btn_LoadAllCategoryProducts(object sender, RoutedEventArgs e)
        {
            string selectedCategory = categoryComboBox.Text;

            if (selectedCategory == "Καθαριστήρια")
            {
                int[] productsId = { 8, 52, 53, 54, 2, 3, 155, 36, 94, 9, 10, 16, 17, 32, 33, 25, 26, 14, 89 };

                for (int i = 0; i < productsId.Length;i++) {
                    Product temp = ProductViewModel.getProduct(productsId[i]);
                    ProductDataGrid.Items.Add(new Product
                    {
                        idProduct = temp.idProduct,
                        ProductName = temp.ProductName,
                        ProductDescription = temp.ProductDescription,
                        SellPrice = temp.SellPrice,
                        OfferPrice = temp.SellPrice,
                    }) ;
                }
            }
        }
    }
}