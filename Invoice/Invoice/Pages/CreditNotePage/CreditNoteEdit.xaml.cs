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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for CreditNoteEdit.xaml
    /// </summary>
    public partial class CreditNoteEdit : Page
    {
       // CreditNoteViewModel creditNoteView;


        bool invoice_loaded = false;
        CreditNote oldCreditNote;

        public CreditNoteEdit()
        {
            InitializeComponent();
          
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            load();
        }

        public void load()
        {
            //creditNoteView = new CreditNoteViewModel();
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
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString("n2");
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();

                
            }
        }

        private void comboBox_invoiceID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_invoiceID.SelectedIndex > -1)
            {
                comboBox_incoiceID_border.BorderThickness = new Thickness(0);                
                comboBox_Product.ItemsSource = InvoiceViewModel.invoice_products_list(int.Parse(comboBox_invoiceID.SelectedItem.ToString()));
            }
        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                 float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductTotal.Text = (price * quantity).ToString("n2");
            }
        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) &&
                float.TryParse(textBox_ProductPrice.Text.Replace('.', ','), out float price) && (comboBox_Product.SelectedIndex > -1))
            {
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
            if ((comboBox_invoiceID.SelectedIndex <= -1) )
            {
                all_completed = false;
                comboBox_incoiceID_border.BorderBrush = Brushes.Red;
                comboBox_incoiceID_border.BorderThickness = new Thickness(1);
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
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat,
                    productInvoiceID = Convert.ToInt32(comboBox_invoiceID.SelectedItem)
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
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }

        private CreditNote createCreditNoteObject()
        {
            CreditNote creditNote = new CreditNote();          
            creditNote.customer = oldCreditNote.customer;
            creditNote.products = ProductDataGrid.Items.OfType<Product>().ToList();
            creditNote.idCreditNote = Int32.Parse(textBox_invoiceNumber.Text);
            creditNote.cost = Double.Parse(NetTotal_TextBlock.Text, NumberStyles.Currency);
            creditNote.VAT = Double.Parse(Vat_TextBlock.Text, NumberStyles.Currency);
            creditNote.totalCost = Double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            creditNote.createdDate = oldCreditNote.createdDate;
            creditNote.issuedBy = issuedBy.Text;
            return creditNote;           
        }

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (Has_Items_Selected())
            {
                if (int.TryParse(textBox_invoiceNumber.Text, out int invoiceId))
                {
                    CreditNoteViewModel.updateCreditNote(createCreditNoteObject(), oldCreditNote);
                   // invoiceMain.viewInvoice(invoiceId);
                    Btn_clearAll_Click(null, null);
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

        private void Btn_Load_CreditNote(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_invoiceNumber.Text, out int creditNoteid);
            if ((CreditNoteViewModel.CreditNoteExists(creditNoteid)))
            {
                Btn_clearAll_Click(null, null);
                loadCreditNote(creditNoteid);
                invoice_loaded = true;

            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for invoice ID.");
            }
        }

        public void loadCreditNote(int creditNoteId)
        {
            oldCreditNote = CreditNoteViewModel.getCreditNote(creditNoteId);
            if (oldCreditNote != null)
            {

                // Customer details
                textBox_Customer.Text = oldCreditNote.customer.CustomerName;
                textBox_Contact_Details.Text = oldCreditNote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = oldCreditNote.customer.Email;
                textBox_Address.Text = oldCreditNote.customer.Address + ", " + oldCreditNote.customer.City + ", " + oldCreditNote.customer.Country;

                // Invoice details
                textBox_invoiceNumber.Text = oldCreditNote.idCreditNote.ToString();
                txtbox_invoiceDate.Text = oldCreditNote.createdDate.ToString("d"); ;
                issuedBy.Text = oldCreditNote.issuedBy;
                NetTotal_TextBlock.Text = oldCreditNote.cost.ToString("C");
                Vat_TextBlock.Text = oldCreditNote.VAT.ToString("C");
                TotalAmount_TextBlock.Text = oldCreditNote.totalCost.ToString("C");
              
                
                foreach (Product p in oldCreditNote.products)
                {

                    ProductDataGrid.Items.Add(p);
                }

                
                comboBox_invoiceID.ItemsSource = InvoiceViewModel.customer_invoices_list(oldCreditNote.customer.idCustomer);
            }
            else
            {
                MessageBox.Show("Credit Note id doesnt't exist");
            }
        }

        
    }
}
