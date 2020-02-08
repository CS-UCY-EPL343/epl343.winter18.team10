using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
namespace InvoiceX.Pages
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        public InvoiceMain()
        {
            InitializeComponent();
            load();
        }

        private void load()
        {
            InvoiceViewModel invVModel = new InvoiceViewModel();
            invoiceDataGrid.ItemsSource = invVModel.invoiceList;
            //printPdf_button.Click += printPdf;
            //createpdf_button.Click += createPdf;

        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Visible;
            createTab.Visibility = Visibility.Hidden;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Hidden;
            createTab.Visibility = Visibility.Visible;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            textBox_Address.Text = ((Customers)comboBox1.SelectedItem).Address + ", " +
                ((Customers)comboBox1.SelectedItem).City + ", " + ((Customers)comboBox1.SelectedItem).Country;
            textBox_Contact_Details.Text = ((Customers)comboBox1.SelectedItem).PhoneNumber.ToString();
            textBox_Email_Address.Text = ((Customers)comboBox1.SelectedItem).Email;


        }
        void createPdf(object sender, RoutedEventArgs e)
        {
            Forms.InvoiceForm invoice = new Forms.InvoiceForm("../../Forms/Invoice.xml");
            MigraDoc.DocumentObjectModel.Document document = invoice.CreateDocument();
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
            Forms.PDFViewer viewer = new Forms.PDFViewer(filename);
            viewer.Show();

        }
        void printPdf(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.UserPageRangeEnabled = true;
            bool? doPrint = printDialog.ShowDialog();
            if (doPrint != true)
            {
                return;
            }

        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            load();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Text = null;
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
            invoiceDataGrid2.Items.Add(new
            {
                bITEMS = ((Product)comboBox_Product.SelectedItem).ProductName,
                bDESCRIPTION = textBox_ProductDescription.Text,
                bQUANITY = textBox_ProductQuanity.Text,
                bPRICE = textBox_ProductPrice.Text,
                bVAT = textBlock_ProductVat.Text,
                bAMOUNT = textBlock_ProductAmount.Text
            });

            var dataGridCellInfo = new DataGridCellInfo(invoiceDataGrid2.Items[0], invoiceDataGrid2.Columns[0]);

            MessageBox.Show(dataGridCellInfo.Item.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            invoiceDataGrid2.Items.Remove(invoiceDataGrid2.CurrentCell.Item);

        }


    }
}
