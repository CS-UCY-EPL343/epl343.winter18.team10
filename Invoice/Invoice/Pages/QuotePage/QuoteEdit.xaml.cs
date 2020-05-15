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
    ///     Interaction logic for QuoteEdit.xaml
    /// </summary>
    public partial class QuoteEdit : Page
    {
        private Quote oldQuote;
        private ProductViewModel productView;
        private bool quote_loaded;
        private readonly QuoteMain quoteMain;

        public QuoteEdit(QuoteMain quoteMain)
        {
            InitializeComponent();
            this.quoteMain = quoteMain;
            load();
        }

        /// <summary>
        ///     Loads the products in the combobox
        /// </summary>
        public void load()
        {
            productView = new ProductViewModel();
            comboBox_Product.ItemsSource = productView.productList;
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
            if (Check_AddProduct_CompletedValues() && quote_loaded)
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
            myquote.customer = oldQuote.customer;
            myquote.products = ProductDataGrid.Items.OfType<Product>().ToList();
            myquote.idQuote = int.Parse(textBox_idQuote.Text);
            myquote.createdDate = quoteDate.SelectedDate.Value.Date;
            myquote.issuedBy = issuedBy.Text;
            return myquote;
        }

        /// <summary>
        ///     After validating updates the Quote and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            var ALL_VALUES_OK = true;

            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
                if (int.TryParse(textBox_idQuote.Text, out var quoteID))
                {
                    QuoteViewModel.updateQuote(make_object_Quote(), oldQuote);
                    quoteMain.viewQuote(quoteID);
                    Btn_clearAll_Click(null, null);
                }
        }

        /// <summary>
        ///     Clears all the customer's information
        /// </summary>
        private void Clear_Customer()
        {
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
            textBox_idQuote.Clear();
            issuedBy.ClearValue(Control.BorderBrushProperty);
            quoteDate.SelectedDate = null;
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
            quote_loaded = false;
            Btn_clearProduct_Click(null, null);
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            load();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e) //mono meta to refresh whritable
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     After validating the quote ID calls loadQuote
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Load_Quote(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_idQuote.Text, out var quoteID);
            if (QuoteViewModel.quoteExists(quoteID))
            {
                Btn_clearAll_Click(null, null);
                loadQuote(quoteID);
                quote_loaded = true;
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for Quote ID.");
            }
        }

        /// <summary>
        ///     Loads the quote information on the page
        /// </summary>
        /// <param name="quoteID"></param>
        public void loadQuote(int quoteID)
        {
            oldQuote = QuoteViewModel.getQuote(quoteID);
            if (oldQuote != null)
            {
                // Customer details
                textBox_Customer.Text = oldQuote.customer.CustomerName;
                textBox_Contact_Details.Text = oldQuote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = oldQuote.customer.Email;
                textBox_Address.Text = oldQuote.customer.Address + ", " + oldQuote.customer.City + ", " +
                                       oldQuote.customer.Country;

                // Quote details
                textBox_idQuote.Text = oldQuote.idQuote.ToString();
                quoteDate.SelectedDate = oldQuote.createdDate;
                issuedBy.Text = oldQuote.issuedBy;

                foreach (var p in oldQuote.products) ProductDataGrid.Items.Add(p);
            }
            else
            {
                MessageBox.Show("Quote id doesnt't exist");
            }
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
    }
}