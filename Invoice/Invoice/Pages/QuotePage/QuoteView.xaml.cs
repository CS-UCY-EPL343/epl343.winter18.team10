// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceX.Forms;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    ///     Interaction logic for QuoteView.xaml
    /// </summary>
    public partial class QuoteView : Page
    {
        private readonly QuoteMain mainPage;
        private Quote quote;

        public QuoteView(QuoteMain mainPage)
        {
            this.mainPage = mainPage;

            InitializeComponent();
            txtBox_QuoteNumber.Focus();
        }

        /// <summary>
        ///     After validating the quote ID calls loadQuote
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadQuote_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_QuoteNumber.Text, out var quoteID);
            if (quoteID > 0)
                loadQuote(quoteID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for quote ID.");
        }

        /// <summary>
        ///     Loads the quote information on the page
        /// </summary>
        /// <param name="quoteID"></param>
        public void loadQuote(int quoteID)
        {
            quote = QuoteViewModel.getQuote(quoteID);
            if (quote != null)
            {
                // Customer details
                textBox_Customer.Text = quote.customer.CustomerName;
                textBox_Contact_Details.Text = quote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = quote.customer.Email;
                textBox_Address.Text =
                    quote.customer.Address + ", " + quote.customer.City + ", " + quote.customer.Country;

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

        /// <summary>
        ///     Method that handles the event Key Down on the textbox quoteNumber.
        ///     If the key pressed is Enter then it loads the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_quoteNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) Btn_LoadQuote_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            quote = null;
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Invoice.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            quoteProductsGrid.ItemsSource = null;
            txtBox_QuoteNumber.IsReadOnly = false;
            txtBox_QuoteNumber.Focus();
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_QuoteNumber.Text, out var quoteID);
            if (txtBox_QuoteNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the quote with ID = " + quoteID + ". Are you sure?";
                var txt = "Delete Quote";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

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

        /// <summary>
        ///     Switches to edit Quote page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (quote != null) mainPage.editQuote(quote.idQuote);
        }

        #region PDF

        /// <summary>
        ///     Saves the quote as a pdf file in the predetermined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
            }
            else
            {
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") +
                                          "/Documents/InvoiceX/Quotes/");
                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") + "/Documents/InvoiceX/Quotes/Quote" +
                               txtBox_QuoteNumber.Text + ".pdf";
                ;

                // Save the PDF document...
                pdfRenderer.Save(filename);
                Process.Start(filename);
            }
        }

        /// <summary>
        ///     Opens the Adobe Acrobat Reader print prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printPdf_click(object sender, RoutedEventArgs e)
        {
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
            }
            else
            {
                //Create and save the pdf
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                var filename = "Quote.pdf";
                pdfRenderer.Save(filename);
                //open adobe acrobat
                var proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                    @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = string.Format(@"/p {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false) proc.WaitForExit(10000);

                proc.EnableRaisingEvents = true;

                proc.Close();
            }
        }

        /// <summary>
        ///     Preview the quote as a pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Quote_temp.pdf")) File.Delete("Quote_temp.pdf");
            if (quote == null)
            {
                MessageBox.Show("Quote is not loaded!");
            }
            else
            {
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                var filename = "Quote_temp.pdf";
                pdfRenderer.Save(filename);

                //open adobe acrobat
                var proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                    @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = string.Format(@" {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false) proc.WaitForExit(10000);

                proc.EnableRaisingEvents = true;

                proc.Close();
            }
        }

        /// <summary>
        ///     Create the pdf based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Document createPdf()
        {
            var customer = quote.customer;
            var customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
                                 customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();

            var quoteDetails = new string[3];
            quoteDetails[0] = txtBox_QuoteNumber.Text;
            Console.WriteLine(txtBox_QuoteNumber.Text);
            quoteDetails[1] = txtBox_invoiceDate.Text;
            quoteDetails[2] = txtBox_issuedBy.Text;


            var products = quoteProductsGrid.Items.OfType<Product>().ToList();


            var quote2 = new QuoteForm("../../Forms/Quote.xml", customerDetails, quoteDetails, products);
            var document = quote2.CreateDocument();
            return document;
        }

        #endregion
    }
}