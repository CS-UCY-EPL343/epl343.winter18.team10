using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    /// Interaction logic for QuoteEdit.xaml
    /// </summary>
    public partial class QuoteEdit : Page
    {
        ProductViewModel productView;
        bool invoice_loaded = false;
        Quote oldQuote;

        public QuoteEdit()
        {
            InitializeComponent();           
            load();
        }

        public void load()
        {
            productView = new ProductViewModel();
            comboBox_Product.ItemsSource = productView.productList;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
                textBox_Product.Text = ((Product)comboBox_Product.SelectedItem).ProductName;
                textBox_ProductDescription.Text = ((Product)comboBox_Product.SelectedItem).ProductDescription;
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString("n2");
            }
        }

       

      

        bool product_already_selected()
        {
            List<Product> gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (Product p in gridProducts)
            {
                if (p.idProduct == ((Product)comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product already selected");
                    return true;
                }
            }
            return false;
        }

        private bool Check_AddProduct_CompletedValues()
        {
            bool all_completed = true;
            if ((comboBox_Product.SelectedIndex <= -1) || product_already_selected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
          
            if (!float.TryParse(textBox_ProductQuote.Text.Replace('.', ','), out float f) || (f < 0))
            {
                all_completed = false;
                textBox_ProductQuote.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuote.ClearValue(TextBox.BorderBrushProperty);
            }

            return all_completed;
        }

        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {
            if (Check_AddProduct_CompletedValues() && invoice_loaded)
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product)comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    OfferPrice = Convert.ToDouble(textBox_ProductQuote.Text.Replace('.', ','))
                });

               
            }
        }

        private void Button_Click_remove_offer_from_grid(object sender, RoutedEventArgs e)
        {
           
            ProductDataGrid.Items.Remove(ProductDataGrid.CurrentCell.Item);
        }

        /*remove txt from txtbox when clicked (Put GotFocus="TextBox_GotFocus" in txtBox)*/
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_Product.SelectedIndex = -1;
            textBox_Product.Text = "";
            textBox_ProductDescription.Text = "";
            textBox_ProductPrice.Text = "";
            textBox_ProductQuote.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductQuote.ClearValue(TextBox.BorderBrushProperty);
        }

        private bool Has_Items_Selected()
        {
            if (ProductDataGrid.Items.Count == 0)//vale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxo
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }

        private Quote make_object_Quote()
        {
            Quote myquote;
            myquote = new Quote();
            myquote.customer = oldQuote.customer;
            myquote.products = ProductDataGrid.Items.OfType<Product>().ToList();
            myquote.idQuote = Int32.Parse(textBox_idQuote.Text);
            myquote.createdDate = invoiceDate.SelectedDate.Value.Date;
            myquote.issuedBy = issuedBy.Text;
            return myquote;
        }


        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;

            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                int invoiceId = -1;
                if (int.TryParse(textBox_idQuote.Text, out int n))
                {
                    invoiceId = int.Parse(textBox_idQuote.Text);
                    QuoteViewModel.updateQuote(make_object_Quote(), oldQuote);
                }
            }
        }

        private void Clear_Customer()
        {
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        private void Clear_Details()
        {
            issuedBy.Text = "";
            textBox_idQuote.Clear();
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Clear_ProductGrid()
        {
            ProductDataGrid.Items.Clear();
            
        }


        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            invoice_loaded = false;
            Btn_clearProduct_Click(null, null);
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            load();
        }

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)//mono meta to refresh whritable
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Btn_Load_Invoice(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_idQuote.Text, out int invoiceID);
            if ((QuoteViewModel.quoteExists(invoiceID)))
            {
                Btn_clearAll_Click(null, null);
                loadInvoice(invoiceID);
                invoice_loaded = true;

            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for Quote ID.");
            }
        }

        public void loadInvoice(int invoiceId)
        {
            oldQuote = QuoteViewModel.getQuote(invoiceId);
            if (oldQuote != null)
            {

                // Customer details
                textBox_Customer.Text = oldQuote.customer.CustomerName;
                textBox_Contact_Details.Text = oldQuote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = oldQuote.customer.Email;
                textBox_Address.Text = oldQuote.customer.Address + ", " + oldQuote.customer.City + ", " + oldQuote.customer.Country;

                // Invoice details
                textBox_idQuote.Text = oldQuote.idQuote.ToString();
                invoiceDate.SelectedDate = oldQuote.createdDate;               
                issuedBy.Text = oldQuote.issuedBy;
             

                                         
                foreach (Product p in oldQuote.products)
                {

                    ProductDataGrid.Items.Add(p);
                }

            }
            else
            {
                MessageBox.Show("Invoice id doesnt't exist");
            }
        }

        private void textBox_ProductQuote_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductQuote.ClearValue(TextBox.BorderBrushProperty);
        }
    }
}
