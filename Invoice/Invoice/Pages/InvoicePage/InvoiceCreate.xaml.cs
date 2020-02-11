using InvoiceX.Models;
using System;
using System.Collections.Generic;
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

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceCreate.xaml
    /// </summary>
    public partial class InvoiceCreate : Page
    {
        public InvoiceCreate()
        {
            InitializeComponent();
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

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (int.TryParse(textBox_ProductQuantity.Text, out n))
            {
                textBlock_ProductAmount.Text = (((Product)comboBox_Product.SelectedItem).SellPrice * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }


        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            int n;
            if (int.TryParse(textBox_ProductPrice.Text, out n) && int.TryParse(textBox_ProductQuantity.Text, out n))
            {
                textBlock_ProductAmount.Text = (Convert.ToInt32(textBox_ProductPrice.Text) * Convert.ToInt32(textBox_ProductQuantity.Text)).ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            invoiceDataGrid2.Items.Add(new Product
            {
                ProductName = ((Product)comboBox_Product.SelectedItem).ProductName,
                ProductDescription = textBox_ProductDescription.Text,
                Stock = Convert.ToInt32(textBox_ProductQuantity.Text),
                SellPrice = Convert.ToDouble(textBox_ProductPrice.Text),
                Quantity = Convert.ToInt32(textBox_ProductQuantity.Text),
                Total = Convert.ToDouble(textBlock_ProductAmount.Text),
                Vat = Convert.ToDouble(textBlock_ProductAmount.Text) + (Convert.ToDouble(textBlock_ProductAmount.Text) * 0.19)
            });

            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var + Convert.ToDouble(textBlock_ProductAmount.Text);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString();
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * 0.19).ToString();
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * 0.19)).ToString();

        }

        private void Button_Click_CreateInvoice_REMOVE(object sender, RoutedEventArgs e)
        {

            Product CurrentCell_Product = (Product)(invoiceDataGrid2.CurrentCell.Item);
            double NetTotal_TextBlock_var = 0;
            NetTotal_TextBlock_var = Convert.ToDouble(NetTotal_TextBlock.Text);
            NetTotal_TextBlock_var = NetTotal_TextBlock_var - Convert.ToDouble(CurrentCell_Product.Total);
            NetTotal_TextBlock.Text = NetTotal_TextBlock_var.ToString();
            Vat_TextBlock.Text = (NetTotal_TextBlock_var * 0.19).ToString();
            TotalAmount_TextBlock.Text = (NetTotal_TextBlock_var + (NetTotal_TextBlock_var * 0.19)).ToString();
            invoiceDataGrid2.Items.Remove(invoiceDataGrid2.CurrentCell.Item);

        }
    }
}
