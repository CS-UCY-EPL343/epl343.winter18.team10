using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class CreditNoteCreate : Page
    {
       // ProductViewModel productView;        
        CustomerViewModel customerView;
        CreditNoteViewModel creditNoteView;
        bool refreshDataDB = true;

        public CreditNoteCreate()
        {
            InitializeComponent();
            txtBlock_NetTotal.Text = (0).ToString("C");
            txtBlock_VAT.Text = (0).ToString("C");
            txtBlock_TotalAmount.Text = (0).ToString("C");
        }

        public void load()
        {
            if (refreshDataDB)
            {                
                customerView = new CustomerViewModel();
                creditNoteView = new CreditNoteViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;                
                textBox_invoiceNumber.Text = (CreditNoteViewModel.returnLatestCreditNoteID()+1).ToString();
                invoiceDate.SelectedDate = DateTime.Today;//set curent date 
            }
            refreshDataDB = false;
        }        

        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                Customer customer = ((Customer)comboBox_customer.SelectedItem);
                textBox_Customer.Text = customer.CustomerName;
                textBox_Address.Text = customer.Address + ", " + customer.City + ", " + customer.Country;
                textBox_Contact_Details.Text = customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = customer.Email;

                comboBox_invoiceID.ItemsSource = InvoiceViewModel.customer_invoices_list(customer.idCustomer);

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_Product.SelectedIndex > -1)
            {
                Product product = ((Product)comboBox_Product.SelectedItem);
                comboBox_Product_border.BorderThickness = new Thickness(0);
                textBox_ProductPrice.ClearValue(TextBox.BorderBrushProperty);
                textBox_ProductQuantity.ClearValue(TextBox.BorderBrushProperty);
                textBox_Product.Text = product.ProductName;
                textBox_ProductQuantity.Text = product.Quantity.ToString();
                textBox_ProductDescription.Text = product.ProductDescription;
                textBox_ProductPrice.Text = product.SellPrice.ToString("n2");
                textBox_ProductVat.Text = (product.Vat * 100).ToString();
               
            }
        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textBox_ProductQuantity.Text, out int quantity) && 
                float.TryParse(textBox_ProductPrice.Text.Replace('.',','), out float price) && (comboBox_Product.SelectedIndex > -1))
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

        private bool productAlreadySelected() 
        {
            List<Product> gridProducts = ProductDataGrid.Items.OfType<Product>().ToList();
            foreach (Product p in gridProducts)
            {               
                if (p.idProduct == ((Product)comboBox_Product.SelectedItem).idProduct)
                {
                    MessageBox.Show("Product '" + p.ProductName + "' already selected");
                    return true;
                }
            }
            return false;
        }

        private bool checkAddProductCompletedValues()
        {
            bool all_completed = true;
            int n;     
            if ((comboBox_Product.SelectedIndex <= -1) || productAlreadySelected())
            {
                all_completed = false;
                comboBox_Product_border.BorderBrush = Brushes.Red;
                comboBox_Product_border.BorderThickness = new Thickness(1);
            }
            if ((comboBox_invoiceID.SelectedIndex <= -1))
            {
                all_completed = false;
                comboBox_incoiceID_border.BorderBrush = Brushes.Red;
                comboBox_incoiceID_border.BorderThickness = new Thickness(1);
            }
            if (!int.TryParse(textBox_ProductQuantity.Text, out n) || (n<0) )
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
            if (checkAddProductCompletedValues())
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
                    productInvoiceID= Convert.ToInt32(comboBox_invoiceID.SelectedItem)
                });
                double netTotal = Double.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency);
                netTotal += Convert.ToDouble(textBox_ProductTotal.Text);
                txtBlock_NetTotal.Text = netTotal.ToString("C");

                double VAT = Double.Parse(txtBlock_VAT.Text, NumberStyles.Currency);
                VAT += (Convert.ToDouble(textBox_ProductTotal.Text) * ((Product)comboBox_Product.SelectedItem).Vat);
                txtBlock_VAT.Text = (VAT).ToString("C");

                txtBlock_TotalAmount.Text = (netTotal + VAT).ToString("C");
            }
        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            Product CurrentCell_Product = (Product)(ProductDataGrid.CurrentCell.Item);

            double netTotal = double.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency);
            netTotal -= Convert.ToDouble(CurrentCell_Product.Total);
            txtBlock_NetTotal.Text = netTotal.ToString("C");

            double VAT = double.Parse(txtBlock_VAT.Text, NumberStyles.Currency);
            VAT -= (CurrentCell_Product.Total * CurrentCell_Product.Vat);
            txtBlock_VAT.Text = VAT.ToString("C");

            txtBlock_TotalAmount.Text = (netTotal + VAT).ToString("C");
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

        private bool checkDetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }
            return true;
        }

        private bool hasItemsSelected()
        {
            if (ProductDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selected any products");
                return false;
            }
            return true;
        }
        
        private CreditNote createCreditNoteObject()
        {
            return new CreditNote
            {
                customer = ((Customer)comboBox_customer.SelectedItem),
                products = ProductDataGrid.Items.OfType<Product>().ToList(),
                idCreditNote = Int32.Parse(textBox_invoiceNumber.Text),
                cost = Double.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency),
                VAT = Double.Parse(txtBlock_VAT.Text, NumberStyles.Currency),
                totalCost = Double.Parse(txtBlock_TotalAmount.Text, NumberStyles.Currency),
                createdDate = invoiceDate.SelectedDate.Value.Date,               
                issuedBy = issuedBy.Text
            };            
        }             

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            
            if (checkCustomerForm() && checkDetailsForm() && hasItemsSelected())
            {
                CreditNote creditNote = createCreditNoteObject();
                CreditNoteViewModel.insertCreditNote(creditNote);
                MessageBox.Show("Credit Note with ID " + creditNote.idCreditNote + " was created.");
               // invoiceMain.viewInvoice(inv.idInvoice);
                Btn_clearAll_Click(null, null);                
            }
        }

        private void clearCustomer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        private void clearDetails()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void clearProductGrid()
        {
            ProductDataGrid.Items.Clear();
            txtBlock_NetTotal.Text = (0).ToString("C");
            txtBlock_VAT.Text = (0).ToString("C");
            txtBlock_TotalAmount.Text = (0).ToString("C");
        }

        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            Btn_clearProduct_Click(null, null);
            clearCustomer();
            clearDetails();
            clearProductGrid();
            refreshDataDB = true;
            load();                       
        }

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }      
   
        public void loadOrder(Order order)
        {
            Btn_clearAll_Click(null, null);
            if (order != null)
            {
                // Customer details                                
                comboBox_customer.SelectedValue = order.customer.CustomerName;
                textBox_Customer.Text = order.customer.CustomerName;
                textBox_Contact_Details.Text = order.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = order.customer.Email;
                textBox_Address.Text = order.customer.Address + ", " + order.customer.City + ", " + order.customer.Country;

                // Invoice details
                txtBlock_NetTotal.Text = order.cost.ToString("C");
                txtBlock_VAT.Text = order.VAT.ToString("C");
                txtBlock_TotalAmount.Text = order.totalCost.ToString("C");

                // Invoice products        
                for (int i = 0; i < order.products.Count; i++)
                {
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
}
