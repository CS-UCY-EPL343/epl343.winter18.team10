using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace InvoiceX.Pages
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        private InvoiceViewModel invVModel;

        public InvoiceMain()
        {
            InitializeComponent();
            btnViewAll_Click(new object(), new RoutedEventArgs());
        }        

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            invVModel = new InvoiceViewModel();
            btnFilter_Click(new object(), new RoutedEventArgs());
            //invoiceDataGrid.ItemsSource = invVModel.invoiceList;
            viewAllTab.Visibility = Visibility.Visible;
            createTab.Visibility = Visibility.Hidden;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            viewAllTab.Visibility = Visibility.Hidden;
            createTab.Visibility = Visibility.Visible;
        }
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {            
            var _itemSourceList = new CollectionViewSource() { Source = invVModel.invoiceList };

            ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            invoiceDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as Invoice;
            if (dateFrom.HasValue)            
                logic = logic & (item.m_date.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.m_date.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.m_customer.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Text = null;
            invoiceDataGrid.ItemsSource = invVModel.invoiceList;
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            textBox_Address.Text = ((Customers)comboBox1.SelectedItem).Address + ", " +
            ((Customers)comboBox1.SelectedItem).City + ", " + ((Customers)comboBox1.SelectedItem).Country;
            textBox_Contact_Details.Text = ((Customers)comboBox1.SelectedItem).PhoneNumber.ToString();
            textBox_Email_Address.Text = ((Customers)comboBox1.SelectedItem).Email;


        }
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

            // Create the print dialog object and set options
            PrintDialog pDialog = new PrintDialog();
            pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            pDialog.UserPageRangeEnabled = true;

            // Display the dialog. This returns true if the user presses the Print button.
            Nullable<Boolean> print = pDialog.ShowDialog();
            if (print == true)
            {
                XpsDocument xpsDocument = new XpsDocument(filename, FileAccess.ReadWrite);
                FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            }
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
            System.Diagnostics.Process.Start(filename);
            

        }

        MigraDoc.DocumentObjectModel.Document createPdf()
        {

            string[] customerDetails = new string[6];
            customerDetails[0] = ((Customers)comboBox1.SelectedItem).CustomerName;
            customerDetails[1] = ((Customers)comboBox1.SelectedItem).Address + ", " +
            ((Customers)comboBox1.SelectedItem).City + ", " + ((Customers)comboBox1.SelectedItem).Country;
            customerDetails[2] = ((Customers)comboBox1.SelectedItem).PhoneNumber.ToString();
            customerDetails[3] = ((Customers)comboBox1.SelectedItem).Email;
            customerDetails[4] = ((Customers)comboBox1.SelectedItem).Balance.ToString();
            customerDetails[5] = ((Customers)comboBox1.SelectedItem).idCustomer.ToString();

            string[] invoiceDetails = new string[6];
            invoiceDetails[0] = invoiceNumber.Text;
            Console.WriteLine(invoiceNumber.Text);
            invoiceDetails[1] = invoiceDate.SelectedDate.Value.ToString("dd/MM/yyyy");
            invoiceDetails[2] = issuedBy.Text;
            invoiceDetails[3] = NetTotal_TextBlock.Text;
            invoiceDetails[4] = Vat_TextBlock.Text;
            invoiceDetails[5] = TotalAmount_TextBlock.Text;

            List<Product> products = invoiceDataGrid2.Items.OfType<Product>().ToList();


            Forms.InvoiceForm invoice = new Forms.InvoiceForm("../../Forms/Invoice.xml", customerDetails, invoiceDetails, products);
            MigraDoc.DocumentObjectModel.Document document = invoice.CreateDocument();
            return document;
           
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null)
            {
                dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            }
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View, Edit, Delete");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox_ProductDescription.Text = ((Product)comboBox_Product.SelectedItem).ProductDescription;
            textBlock_ProductStock.Text = ((Product)comboBox_Product.SelectedItem).Stock.ToString();
            textBox_ProductPrice.Text = ((Product)comboBox_Product.SelectedItem).SellPrice.ToString();
            textBlock_ProductVat.Text = "19%";


        }

        private void TextBox_ProductQuanity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (int.TryParse(textBox_ProductQuanity.Text, out n))
            {
                textBlock_ProductAmount.Text = (((Product)comboBox_Product.SelectedItem).SellPrice * Convert.ToInt32(textBox_ProductQuanity.Text)).ToString();
            }


        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (int.TryParse(textBox_ProductPrice.Text, out n) && int.TryParse(textBox_ProductQuanity.Text, out n))
            {
                textBlock_ProductAmount.Text = (Convert.ToInt32(textBox_ProductPrice.Text) * Convert.ToInt32(textBox_ProductQuanity.Text)).ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            invoiceDataGrid2.Items.Add(new Product
            {               
                ProductName = ((Product)comboBox_Product.SelectedItem).ProductName,
                ProductDescription = textBox_ProductDescription.Text,
                Stock = Convert.ToInt32(textBox_ProductQuanity.Text),
                SellPrice = Convert.ToDouble(textBox_ProductPrice.Text),
                Quantity= Convert.ToInt32(textBox_ProductQuanity.Text),
                Total = Convert.ToDouble(textBlock_ProductAmount.Text) ,
                Vat = Convert.ToDouble(textBlock_ProductAmount.Text) + (Convert.ToDouble(textBlock_ProductAmount.Text) * 0.19)
            });

            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBlock_ProductAmount.Text);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString();
            Vat_TextBlock.Text= (NetTotal_TextBlock_var * 0.19).ToString(); 
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * 0.19 )).ToString();

        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {
            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var - Convert.ToDouble(textBlock_ProductAmount.Text);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString();
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * 0.19).ToString();
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * 0.19)).ToString();

            invoiceDataGrid2.Items.Remove(invoiceDataGrid2.CurrentCell.Item);

        }


    }
}
