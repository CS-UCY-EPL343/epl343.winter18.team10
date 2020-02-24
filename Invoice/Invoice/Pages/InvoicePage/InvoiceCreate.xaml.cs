using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class InvoiceCreate : Page
    {
        ProductViewModel productView;        
        CustomerViewModel customerView;
        bool Refresh_DB_data = true;

        public InvoiceCreate()
        {
            InitializeComponent();
        }

        public void load()
        {
            if (Refresh_DB_data)
            {                
                productView = new ProductViewModel();               
                customerView = new CustomerViewModel();               
                comboBox_customer.ItemsSource = customerView.CustomersList;
                comboBox_Product.ItemsSource = productView.ProductList;
                textBox_invoiceNumber.Text = (InvoiceViewModel.ReturnLatestInvoiceID()+1).ToString();
                invoiceDate.SelectedDate = DateTime.Today;//set curent date 
                dueDate.SelectedDate = DateTime.Today.AddDays(60); ;//set curent date +60
                textBox_entermessage.GotFocus += TextBox_GotFocus; //press message box and remove message

            }
            Refresh_DB_data = false;
        }        


        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                textBox_Customer.Text = ((Customer)comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customer)comboBox_customer.SelectedItem).Address + ", " +
                 ((Customer)comboBox_customer.SelectedItem).City + ", " + ((Customer)comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customer)comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customer)comboBox_customer.SelectedItem).Email;
            }
        }

        #region PDF
        private string filenamePath = null;
        void savePdf_Click(object sender, RoutedEventArgs e)
        {
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Invoice.pdf";
            filename = "Invoice.pdf";
            pdfRenderer.Save(filename);
            System.Diagnostics.Process.Start(filename);

        }
        void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Invoice.pdf";
            pdfRenderer.Save(filename);
            //open adobe acrobat
            Process proc = new Process();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "print";

            //Define location of adobe reader/command line
            //switches to launch adobe in "print" mode
            proc.StartInfo.FileName =
              @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            proc.StartInfo.Arguments = String.Format(@"/p {0}", filename);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (proc.HasExited == false)
            {
                proc.WaitForExit(10000);
            }

            proc.EnableRaisingEvents = true;

            proc.Close();

        }
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Invoice_temp.pdf"))
            {
                File.Delete("Invoice_temp.pdf");
            }
            MigraDoc.DocumentObjectModel.Document document = createPdf();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Invoice_temp.pdf";
            pdfRenderer.Save(filename);

            //open adobe acrobat
            Process proc = new Process();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "print";

            //Define location of adobe reader/command line
            //switches to launch adobe in "print" mode
            proc.StartInfo.FileName =
              @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            proc.StartInfo.Arguments = String.Format(@" {0}", filename);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.Start();
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (proc.HasExited == false)
            {
                proc.WaitForExit(10000);
            }

            proc.EnableRaisingEvents = true;

            proc.Close();


        }

        MigraDoc.DocumentObjectModel.Document createPdf()
        {

            string[] customerDetails = new string[6];
            customerDetails[0] = ((Customer)comboBox_customer.SelectedItem).CustomerName;
            customerDetails[1] = ((Customer)comboBox_customer.SelectedItem).Address + ", " +
            ((Customer)comboBox_customer.SelectedItem).City + ", " + ((Customer)comboBox_customer.SelectedItem).Country;
            customerDetails[2] = ((Customer)comboBox_customer.SelectedItem).PhoneNumber.ToString();
            customerDetails[3] = ((Customer)comboBox_customer.SelectedItem).Email;
            customerDetails[4] = ((Customer)comboBox_customer.SelectedItem).Balance.ToString();
            customerDetails[5] = ((Customer)comboBox_customer.SelectedItem).idCustomer.ToString();

            string[] invoiceDetails = new string[6];
            invoiceDetails[0] = textBox_invoiceNumber.Text;
            Console.WriteLine(textBox_invoiceNumber.Text);
            invoiceDetails[1] = invoiceDate.SelectedDate.Value.ToString("dd/MM/yyyy");
            invoiceDetails[2] = issuedBy.Text;
            invoiceDetails[3] = NetTotal_TextBlock.Text;
            invoiceDetails[4] = Vat_TextBlock.Text;
            invoiceDetails[5] = TotalAmount_TextBlock.Text;

            List<Product> products = ProductDataGrid.Items.OfType<Product>().ToList();


            Forms.InvoiceForm invoice = new Forms.InvoiceForm("../../Forms/Invoice.xml", customerDetails, invoiceDetails, products);
            MigraDoc.DocumentObjectModel.Document document = invoice.CreateDocument();
            return document;


        }
        #endregion

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

        private bool Check_AddProduct_CompletedValues()
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
            if (Check_AddProduct_CompletedValues())
            {
                ProductDataGrid.Items.Add(new Product
                {
                    idProduct = ((Product)comboBox_Product.SelectedItem).idProduct,
                    ProductName = textBox_Product.Text,
                    ProductDescription = textBox_ProductDescription.Text,
                    Stock = Convert.ToInt32(textBox_ProductStock.Text),
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

            Product CurrentCell_Product = (Product)(ProductDataGrid.CurrentCell.Item);
            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString("n2");
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * (CurrentCell_Product.Vat)).ToString("n2");
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * (CurrentCell_Product.Vat))).ToString("n2");
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

        private bool Check_DetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }
            return true;
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
            myinvoice = new Invoice();
            myinvoice.customer = ((Customer)comboBox_customer.SelectedItem);           
            myinvoice.products = ProductDataGrid.Items.OfType<Product>().ToList();
            myinvoice.idInvoice = Int32.Parse(textBox_invoiceNumber.Text);
            myinvoice.cost = double.Parse(NetTotal_TextBlock.Text);
            myinvoice.VAT = double.Parse(Vat_TextBlock.Text);
            myinvoice.totalCost = double.Parse(TotalAmount_TextBlock.Text);
            myinvoice.createdDate = invoiceDate.SelectedDate.Value.Date;
            myinvoice.dueDate = invoiceDate.SelectedDate.Value.Date;
            myinvoice.issuedBy = issuedBy.Text;
            return myinvoice;
        }             

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            bool ALL_VALUES_OK = true;
            if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK) InvoiceViewModel.Send_Ivoice_and_Products_to_DB(make_object_Invoice());
        }

        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
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
            ProductDataGrid.Items.Clear();
            NetTotal_TextBlock.Text = "0.00";
            Vat_TextBlock.Text = "0.00";
            TotalAmount_TextBlock.Text = "0.00";
            textBox_entermessage.Text = "Write a message here ...";
        }

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

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }       
    }
}
