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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceX.Forms;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    ///     Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : Page
    {
        private Invoice invoice;
        private readonly InvoiceMain mainPage;

        public InvoiceView(InvoiceMain mainPage)
        {
            this.mainPage = mainPage;

            InitializeComponent();
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_invoiceNumber.Focus();
        }

        /// <summary>
        ///     After validating the invoice ID calls loadInvoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadInvoice_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_invoiceNumber.Text, out var invoiceID);
            if (invoiceID > 0)
                loadInvoice(invoiceID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for invoice ID.");
        }

        /// <summary>
        ///     Loads the invoice information on the page
        /// </summary>
        /// <param name="invoiceID"></param>
        public void loadInvoice(int invoiceID)
        {
            invoice = InvoiceViewModel.getInvoice(invoiceID);
            if (invoice != null)
            {
                // Customer details
                textBox_Customer.Text = invoice.customer.CustomerName;
                textBox_Contact_Details.Text = invoice.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = invoice.customer.Email;
                textBox_Address.Text = invoice.customer.Address + ", " + invoice.customer.City + ", " +
                                       invoice.customer.Country;

                // Invoice details
                txtBox_invoiceNumber.Text = invoice.idInvoice.ToString();
                txtBox_invoiceNumber.IsReadOnly = true;
                txtBox_invoiceDate.Text = invoice.createdDate.ToString("d");
                txtBox_dueDate.Text = invoice.dueDate.ToString("d");
                txtBox_issuedBy.Text = invoice.issuedBy;
                NetTotal_TextBlock.Text = invoice.cost.ToString("C");
                Vat_TextBlock.Text = invoice.VAT.ToString("C");
                TotalAmount_TextBlock.Text = invoice.totalCost.ToString("C");
                if (invoice.isPaid == true)
                {
                    isPaidButton.IsChecked = true;
                }
                // Invoice products           
                invoiceProductsGrid.ItemsSource = invoice.products;
            }
            else
            {
                MessageBox.Show("Invoice with ID = " + invoiceID + ", does not exist");
            }
        }

        /// <summary>
        ///     Method that handles the event Key Down on the textbox invoiceNumber.
        ///     If the key pressed is Enter then it loads the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_invoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) Btn_LoadInvoice_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            invoice = null;
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Invoice.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            invoiceProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_invoiceNumber.IsReadOnly = false;
            txtBox_invoiceNumber.Focus();
            isPaidButton.IsChecked = false;

        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_invoiceNumber.Text, out var invoiceID);
            if (txtBox_invoiceNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the invoice with ID = " + invoiceID + ". Are you sure?";
                var txt = "Delete Invoice";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        if (InvoiceViewModel.deleteInvoice(invoiceID))
                        {
                            Btn_clearView_Click(null, null);
                            MessageBox.Show("Deleted Invoice with ID = " + invoiceID);
                        }

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

        /// <summary>
        ///     Switches to edit Invoice page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (invoice != null) mainPage.editInvoice(invoice.idInvoice);
        }

        #region PDF

        /// <summary>
        ///     Saves the invoice as a pdf file in the predetermined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
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
                                          "/Documents/InvoiceX/Invoices/");

                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") +
                               "/Documents/InvoiceX/Invoices/Invoice" + txtBox_invoiceNumber.Text + ".pdf";
                ;
                try
                {
                    pdfRenderer.Save(filename);
                    Process.Start(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        ///     Opens the Adobe Acrobat Reader print prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
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
                var filename = "Invoice.pdf";

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
        ///     Preview the order as a pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Invoice_temp.pdf")) File.Delete("Invoice_temp.pdf");
            if (invoice == null)
            {
                MessageBox.Show("Invoice is not loaded!");
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
                var filename = "Invoice_temp.pdf";
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

            var customer = invoice.customer;
            var customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
                                 customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = (CustomerViewModel.calculateCustomerBalance(customer.idCustomer)).ToString("c");
            customerDetails[5] = customer.idCustomer.ToString();

            var invoiceDetails = new string[6];
            invoiceDetails[0] = txtBox_invoiceNumber.Text;
            invoiceDetails[1] = txtBox_invoiceDate.Text;
            invoiceDetails[2] = txtBox_issuedBy.Text;
            invoiceDetails[3] = NetTotal_TextBlock.Text;
            invoiceDetails[4] = Vat_TextBlock.Text;
            invoiceDetails[5] = TotalAmount_TextBlock.Text;

            var products = invoiceProductsGrid.Items.OfType<Product>().ToList();

            try
            {
                var invoice2 = new InvoiceFormNew("../../Forms/Invoice.xml", customerDetails, invoiceDetails, products);
                var document = invoice2.CreateDocument();
                return document;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }

        #endregion
    }

}