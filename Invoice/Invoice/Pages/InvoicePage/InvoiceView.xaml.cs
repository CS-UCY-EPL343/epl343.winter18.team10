using InvoiceX.Models;
using InvoiceX.ViewModels;
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
    /// Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : Page
    {
        private Invoice invoice;
        InvoiceMain mainPage;

        public InvoiceView(InvoiceMain mainPage)
        {
            this.mainPage = mainPage;

            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_invoiceNumber.Focus();
        }

        private void Btn_LoadInvoice_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_invoiceNumber.Text, out int invoiceID);
            if (invoiceID > 0)
            {
                loadInvoice(invoiceID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for invoice ID.");
            }
        }

        public void loadInvoice(int invoiceID)
        {
            invoice = InvoiceViewModel.getInvoice(invoiceID);
            if (invoice != null)
            {
                // Customer details
                textBox_Customer.Text = invoice.customer.CustomerName;
                textBox_Contact_Details.Text = invoice.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = invoice.customer.Email;
                textBox_Address.Text = invoice.customer.Address + ", " + invoice.customer.City + ", " + invoice.customer.Country;

                // Invoice details
                txtBox_invoiceNumber.Text = invoice.idInvoice.ToString();
                txtBox_invoiceNumber.IsReadOnly = true;
                txtBox_invoiceDate.Text = invoice.createdDate.ToString("d");
                txtBox_dueDate.Text = invoice.dueDate.ToString("d");
                txtBox_issuedBy.Text = invoice.issuedBy;
                NetTotal_TextBlock.Text = invoice.cost.ToString("C");
                Vat_TextBlock.Text = invoice.VAT.ToString("C");
                TotalAmount_TextBlock.Text = invoice.totalCost.ToString("C");

                // Invoice products           
                invoiceProductsGrid.ItemsSource = invoice.products;
            }
            else
            {
                MessageBox.Show("Invoice with ID = " + invoiceID + ", does not exist");
            }
        }

        private void txtBox_invoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Btn_LoadInvoice_Click(null,null);
            }
        }

        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.invoice = null;
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            foreach (var ctrl in grid_Invoice.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            invoiceProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_invoiceNumber.IsReadOnly = false;
            txtBox_invoiceNumber.Focus();
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_invoiceNumber.Text, out int invoiceID);
            if (txtBox_invoiceNumber.IsReadOnly)
            {                
                string msgtext = "You are about to delete the invoice with ID = " + invoiceID + ". Are you sure?";
                string txt = "Delete Invoice";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        InvoiceViewModel.deleteInvoice(invoiceID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Invoice with ID = " + invoiceID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }                
            }
            else
            {
                MessageBox.Show("No invoice is loaded");
            }
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (invoice != null)
            {
                mainPage.editInvoice(invoice.idInvoice);
            }
        }

        #region PDF
        void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
            }
            else
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
        }

        void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
            }
            else
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

        }

        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Invoice_temp.pdf"))
            {
                File.Delete("Invoice_temp.pdf");
            }
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
            }
            else
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
        }

        private MigraDoc.DocumentObjectModel.Document createPdf()
        {
            Customer customer = invoice.customer;
            string[] customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
            customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();

            string[] invoiceDetails = new string[6];
            invoiceDetails[0] = txtBox_invoiceNumber.Text;
            invoiceDetails[1] = txtBox_invoiceDate.Text;
            invoiceDetails[2] = txtBox_issuedBy.Text;
            invoiceDetails[3] = NetTotal_TextBlock.Text;
            invoiceDetails[4] = Vat_TextBlock.Text;
            invoiceDetails[5] = TotalAmount_TextBlock.Text;

            List<Product> products = invoiceProductsGrid.Items.OfType<Product>().ToList();


            Forms.InvoiceForm invoice2 = new Forms.InvoiceForm("../../Forms/Invoice.xml", customerDetails, invoiceDetails, products);
            MigraDoc.DocumentObjectModel.Document document = invoice2.CreateDocument();
            return document;
        }
        #endregion
    }
}
