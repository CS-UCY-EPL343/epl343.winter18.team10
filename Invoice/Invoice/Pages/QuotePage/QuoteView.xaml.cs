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

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    /// Interaction logic for QuoteView.xaml
    /// </summary>
    public partial class QuoteView : Page
    {
        private Quote quote;
        QuoteMain mainPage;

        public QuoteView(QuoteMain mainPage)
        {
            this.mainPage = mainPage;

            InitializeComponent();           
            txtBox_QuoteNumber.Focus();
        }    
       
        private void Btn_LoadQuote_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_QuoteNumber.Text, out int quoteID);
            if (quoteID > 0)
            {
                loadQuote(quoteID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for quote ID.");
            }
        }

        public void loadQuote(int quoteID)
        {
            quote = QuoteViewModel.getQuote(quoteID);
            if (quote != null)
            {
                // Customer details
                textBox_Customer.Text = quote.customer.CustomerName;
                textBox_Contact_Details.Text = quote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = quote.customer.Email;
                textBox_Address.Text = quote.customer.Address + ", " + quote.customer.City + ", " + quote.customer.Country;

                // Quote details
                txtBox_QuoteNumber.Text = quote.idQuote.ToString();
                txtBox_QuoteNumber.IsReadOnly = true;
                txtBox_invoiceDate.Text = quote.createdDate.ToString("d");
                txtBox_issuedBy.Text = quote.issuedBy;

                // Quote products           
                quoteProductsGrid.ItemsSource = quote.products;
            }
            else
            {
                MessageBox.Show("Quote with ID = " + quoteID + ", does not exist");
            }
        }

        private void txtBox_quoteNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Btn_LoadQuote_Click(null, null);
            }
        }

        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.quote = null;
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
            quoteProductsGrid.ItemsSource = null;           
            txtBox_QuoteNumber.IsReadOnly = false;
            txtBox_QuoteNumber.Focus();
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_QuoteNumber.Text, out int quoteID);
            if (txtBox_QuoteNumber.IsReadOnly)
            {
                string msgtext = "You are about to delete the quote with ID = " + quoteID + ". Are you sure?";
                string txt = "Delete Quote";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        QuoteViewModel.deleteQuote(quoteID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Quote with ID = " + quoteID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No quote is loaded");
            }
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (quote != null)
            {
                mainPage.editQuote(quote.idQuote);
            }
        }

        #region PDF
        void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
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
                string filename = "Quote" + txtBox_QuoteNumber.Text + ".pdf"; ;
                pdfRenderer.Save(filename);
                System.Diagnostics.Process.Start(filename);
            }
        }

        void printPdf_click(object sender, RoutedEventArgs e)
        {
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
            }
            else
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
                string filename = "Quote.pdf";
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
            if (File.Exists("Quote_temp.pdf"))
            {
                File.Delete("Quote_temp.pdf");
            }
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
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
                string filename = "Quote_temp.pdf";
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
            Customer customer = quote.customer;
            string[] customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
            customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();

            string[] quoteDetails = new string[3];
            quoteDetails[0] = txtBox_QuoteNumber.Text;
            Console.WriteLine(txtBox_QuoteNumber.Text);
            quoteDetails[1] = txtBox_invoiceDate.Text;
            quoteDetails[2] = txtBox_issuedBy.Text;
            

            List<Product> products = quoteProductsGrid.Items.OfType<Product>().ToList();


            Forms.QuoteForm quote2 = new Forms.QuoteForm("../../Forms/Quote.xml", customerDetails, quoteDetails, products);
            MigraDoc.DocumentObjectModel.Document document = quote2.CreateDocument();
            return document;
        }
        #endregion
    }
}
