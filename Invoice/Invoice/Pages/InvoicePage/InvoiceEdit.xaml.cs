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


namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceEdit.xaml
    /// </summary>
    public partial class InvoiceEdit : Page
    {
        ProductViewModel productView;
        //UserViewModel userView;
        CustomerViewModel customerView;
      


        public InvoiceEdit()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {            
                productView = new ProductViewModel();              
                customerView = new CustomerViewModel();                     
                comboBox_Product.ItemsSource = productView.ProductList;
                textBox_entermessage.GotFocus += TextBox_GotFocus; //press message box and remove message          
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
                textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString();
                textBox_ProductVat.Text = (((Product)comboBox_Product.SelectedItem).Vat * 100).ToString();
            }

        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {

            int n;
            if (int.TryParse(textBox_ProductQuantity.Text, out n) && float.TryParse(textBox_ProductPrice.Text, out float f) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }


        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (float.TryParse(textBox_ProductPrice.Text, out float f) && int.TryParse(textBox_ProductQuantity.Text, out n) && (comboBox_Product.SelectedIndex > -1))
            {
                textBox_ProductTotal.Text = (Convert.ToDouble(textBox_ProductPrice.Text.Replace('.', ',')) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }
        }

        private bool Check_AddProduct_ComplitedValues()
        {
            bool all_completed = true;
            int n;
            if (comboBox_Product.SelectedIndex <= -1)
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            if (!int.TryParse(textBox_ProductQuantity.Text, out n))
            {
                all_completed = false;
                textBox_ProductQuantity.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
            }
            if (!float.TryParse(textBox_ProductPrice.Text, out float f))
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
            if (Check_AddProduct_ComplitedValues())
            {
                productDataGrit.Items.Add(new Product
                {
                    idProduct = ((Product)comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    Stock = Convert.ToInt32(textBox_ProductQuantity.Text),
                    SellPrice = Convert.ToDouble(textBox_ProductPrice.Text),
                    Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                    Total = Convert.ToDouble(textBox_ProductTotal.Text),
                    Vat = ((Product)comboBox_Product.SelectedItem).Vat
                });

                double NetTotal_TextBlock_var = 0;
                NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
                NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBox_ProductTotal.Text);
                NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("n2");
                double Vat_TextBlock_var = 0;
                Vat_TextBlock_var = Convert.ToDouble(Vat_TextBlock.Text);
                Vat_TextBlock_var = Vat_TextBlock_var + (Convert.ToDouble(textBox_ProductTotal.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                Vat_TextBlock.Text = (Vat_TextBlock_var).ToString("n2");
                TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + Vat_TextBlock_var).ToString("n2");
            }
        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {

            Product CurrentCell_Product = (Product)(productDataGrit.CurrentCell.Item);
            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("n2");
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * (CurrentCell_Product.Vat)).ToString("n2");
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * (CurrentCell_Product.Vat))).ToString("n2");
            productDataGrit.Items.Remove(productDataGrit.CurrentCell.Item);

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

            if (productDataGrit.Items.Count == 0)//vale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxovale enenxo
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
                myinvoice.m_customer = invoice.m_customer;
                myinvoice.m_products = productDataGrit.Items.OfType<Product>().ToList();
                myinvoice.m_idInvoice = Int32.Parse(textBox_invoiceNumber.Text);
                myinvoice.m_cost = double.Parse(NetTotal_TextBlock.Text);
                myinvoice.m_VAT = double.Parse(Vat_TextBlock.Text);
                myinvoice.m_totalCost = double.Parse(TotalAmount_TextBlock.Text);
                myinvoice.m_createdDate = invoiceDate.SelectedDate.Value.Date;
                myinvoice.m_dueDate = invoiceDate.SelectedDate.Value.Date;
                myinvoice.m_issuedBy = issuedBy.Text;
                return myinvoice;
            }
            else
            {
                MessageBox.Show("Invoice id doesnt't exist");
                return myinvoice=null;
            }            
           
        }


        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;
           
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                InvoiceViewModel.edit_Invoice(make_object_Invoice());
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
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Clear_ProductGrid()
        {
            productDataGrit.Items.Clear();
            NetTotal_TextBlock.Text = "0.00";
            Vat_TextBlock.Text = "0.00";
            TotalAmount_TextBlock.Text = "0.00";
            textBox_entermessage.Text = "Write a message here ...";
        }
        

        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {         
            Btn_clearProduct_Click(new object(), new RoutedEventArgs());
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
            int invoiceId = -1;
            if (int.TryParse(textBox_invoiceNumber.Text, out int n))
            {
                invoiceId = int.Parse(textBox_invoiceNumber.Text);
            }
            int latestinvoiceid = InvoiceViewModel.ReturnLatestInvoiceID();
            if ((invoiceId <= latestinvoiceid) && (invoiceId > -1))
            {
                Invoice invoice = InvoiceViewModel.getInvoiceById(invoiceId);

                // Customer details
                textBox_Customer.Text = invoice.m_customer.CustomerName;
                textBox_Contact_Details.Text = invoice.m_customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = invoice.m_customer.Email;
                textBox_Address.Text = invoice.m_customer.Address + ", " + invoice.m_customer.City + ", " + invoice.m_customer.Country;

                // Invoice details
                textBox_invoiceNumber.Text = invoice.m_idInvoice.ToString();
                invoiceDate.SelectedDate = invoice.m_createdDate;
                dueDate.SelectedDate = invoice.m_dueDate;
                issuedBy.Text = invoice.m_issuedBy;
                NetTotal_TextBlock.Text = invoice.m_cost.ToString("n2");
                Vat_TextBlock.Text = invoice.m_VAT.ToString("n2");
                TotalAmount_TextBlock.Text = invoice.m_totalCost.ToString("n2");

                // Invoice products        
                for (int i = 0; i < invoice.m_products.Count; i++)
                {
                    productDataGrit.Items.Add(new Product
                    {
                        idProduct =  invoice.m_products[i].idProduct,
                        ProductName = invoice.m_products[i].ProductName ,
                        ProductDescription = invoice.m_products[i].ProductDescription,
                        Stock = invoice.m_products[i].Stock,
                        SellPrice = invoice.m_products[i].Cost,
                        Quantity = invoice.m_products[i].Quantity,
                        Total = invoice.m_products[i].Total ,
                        Vat = invoice.m_products[i].Vat
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
