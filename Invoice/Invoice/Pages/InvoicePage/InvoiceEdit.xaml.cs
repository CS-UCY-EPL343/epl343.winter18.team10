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

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceEdit.xaml
    /// </summary>
    public partial class InvoiceEdit : Page
    {
        ProductViewModel productView;
        bool invoice_loaded = false;

        public InvoiceEdit()
        {
            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            load();
        }

        public void load()
        {
            productView = new ProductViewModel();
            comboBox_Product.ItemsSource = productView.ProductList;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
                textBox_Product.Text = ((Product)comboBox_Product.SelectedItem).ProductName;
                textBox_ProductQuantity.Text = ((Product)comboBox_Product.SelectedItem).Quantity.ToString();
                textBox_ProductDescription.Text = ((Product)comboBox_Product.SelectedItem).ProductDescription;
                textBox_ProductStock.Text = ((Product)comboBox_Product.SelectedItem).Stock.ToString();
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString("n2");
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();
            }
        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                 float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                //textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
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
            int n;
            if ((comboBox_Product.SelectedIndex <= -1) || product_already_selected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || (n < 0))
            {
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
            }
            if (!float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float f) || (f < 0))
            {
                all_completed = false;
                textBox_ProductPrice.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
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
                    Stock = Convert.ToInt32(textBox_ProductStock.Text),
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat
                });

                double NetTotal_TextBlock_var = 0;
                NetTotal_TextBlock_var = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("C");
                double Vat_TextBlock_var = 0;
                Vat_TextBlock_var = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                Vat_TextBlock_var = Vat_TextBlock_var + (Convert.ToDouble(textBox_ProductTotal.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                Vat_TextBlock.Text = (Vat_TextBlock_var).ToString("C");
                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("C");
            }
        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            Product CurrentCell_Product = (Product)(ProductDataGrid.CurrentCell.Item);
            double netTotal = double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            netTotal = netTotal - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = netTotal.ToString("C");
            double vat = double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            vat = vat - (CurrentCell_Product.Total * CurrentCell_Product.Vat);
            Vat_TextBlock.Text = vat.ToString("C");
            TotalAmount_TextBlock.Text = (netTotal + vat).ToString("C");
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
            textBox_ProductStock.Text = "";
            textBox_ProductQuantity.Text = "";
            textBox_ProductPrice.Text = "";
            textBox_ProductVat.Text = "";
            textBox_ProductTotal.Text = "";
            comboBox_Product_border.BorderThickness = new Thickness(0);
            textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
            textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
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

        private Invoice make_object_Invoice()
        {
            Invoice myinvoice;
            int invoiceId = 0;
            if (int.TryParse(textBox_invoiceNumber.Text, out int n))
            {
                invoiceId = int.Parse(textBox_invoiceNumber.Text);
            }
            if (invoiceId <= InvoiceViewModel.ReturnLatestInvoiceID())
            {
                Invoice invoice = InvoiceViewModel.getInvoiceById(invoiceId);
                myinvoice = new Invoice();
                myinvoice.customer = invoice.customer;
                myinvoice.products = ProductDataGrid.Items.OfType<Product>().ToList();
                myinvoice.idInvoice = Int32.Parse(textBox_invoiceNumber.Text);
                myinvoice.cost = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
                myinvoice.VAT = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
                myinvoice.totalCost = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
                myinvoice.createdDate = invoiceDate.SelectedDate.Value.Date;
                myinvoice.dueDate = dueDate.SelectedDate.Value.Date;
                myinvoice.issuedBy = issuedBy.Text;
                return myinvoice;
            }
            else
            {
                MessageBox.Show("Invoice id doesnt't exist");
                return myinvoice = null;
            }
        }

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;

            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                int invoiceId = -1;
                if (int.TryParse(textBox_invoiceNumber.Text, out int n))
                {
                    invoiceId = int.Parse(textBox_invoiceNumber.Text);
                    InvoiceViewModel.edit_Invoice(make_object_Invoice(), InvoiceViewModel.getInvoiceById(invoiceId));
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
            textBox_invoiceNumber.Clear();
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Clear_ProductGrid()
        {
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
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
            int.TryParse(textBox_invoiceNumber.Text, out int invoiceID);
            if ((InvoiceViewModel.InvoiceID_exist_or_not(invoiceID)))
            {
                Btn_clearAll_Click(null, null);
                loadInvoice(invoiceID);
                invoice_loaded = true;

            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for invoice ID.");
            }
        }

        public void loadInvoice(int invoiceId)
        {
            Invoice invoice = InvoiceViewModel.getInvoiceById(invoiceId);
            if (invoice != null)
            {

                // Customer details
                textBox_Customer.Text = invoice.customer.CustomerName;
                textBox_Contact_Details.Text = invoice.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = invoice.customer.Email;
                textBox_Address.Text = invoice.customer.Address + ", " + invoice.customer.City + ", " + invoice.customer.Country;

                // Invoice details
                textBox_invoiceNumber.Text = invoice.idInvoice.ToString();
                invoiceDate.SelectedDate = invoice.createdDate;
                dueDate.SelectedDate = invoice.dueDate;
                issuedBy.Text = invoice.issuedBy;
                NetTotal_TextBlock.Text = invoice.cost.ToString("C");
                Vat_TextBlock.Text = invoice.VAT.ToString("C");
                TotalAmount_TextBlock.Text = invoice.totalCost.ToString("C");

                // Invoice products        
                for (int i = 0; i < invoice.products.Count; i++)
                {
                    ProductDataGrid.Items.Add(new Product
                    {
                        idProduct = invoice.products[i].idProduct,
                        ProductName = invoice.products[i].ProductName,
                        ProductDescription = invoice.products[i].ProductDescription,
                        Stock = invoice.products[i].Stock,
                        SellPrice = invoice.products[i].SellPrice,
                        Quantity = invoice.products[i].Quantity,
                        Total = invoice.products[i].Total,
                        Vat = invoice.products[i].Vat
                    });
                }
            }
            else
            {
                MessageBox.Show("Invoice id doesnt't exist");
            }
        }

    }
}
