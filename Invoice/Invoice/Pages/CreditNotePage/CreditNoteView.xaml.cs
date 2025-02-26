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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    ///     Interaction logic for CreditNoteView.xaml
    /// </summary>
    public partial class CreditNoteView : Page
    {
        private CreditNote creditNote;
        private readonly CreditNoteMain mainPage;

        public CreditNoteView(CreditNoteMain creditNoteMain)
        {
            mainPage = creditNoteMain;

            InitializeComponent();
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_creditNoteNumber.Focus();
        }

        /// <summary>
        ///     After validating the credit note ID calls loadCreditNote
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadCreditNote_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_creditNoteNumber.Text, out var creditNoteID);
            if (creditNoteID > 0)
                loadCreditNote(creditNoteID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for credit note ID.");
        }

        /// <summary>
        ///     Loads the credit note information on the page
        /// </summary>
        /// <param name="creditNoteID"></param>
        public void loadCreditNote(int creditNoteID)
        {
            creditNote = CreditNoteViewModel.getCreditNote(creditNoteID);
            if (creditNote != null)
            {
                // Customer details
                textBox_Customer.Text = creditNote.customer.CustomerName;
                textBox_Contact_Details.Text = creditNote.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = creditNote.customer.Email;
                textBox_Address.Text = creditNote.customer.Address + ", " + creditNote.customer.City + ", " +
                                       creditNote.customer.Country;

                // Credit Note details
                txtBox_creditNoteNumber.Text = creditNote.idCreditNote.ToString();
                txtBox_creditNoteNumber.IsReadOnly = true;
                txtBox_createdDate.Text = creditNote.createdDate.ToString("d");
                txtBox_issuedBy.Text = creditNote.issuedBy;
                NetTotal_TextBlock.Text = creditNote.cost.ToString("C");
                Vat_TextBlock.Text = creditNote.VAT.ToString("C");
                TotalAmount_TextBlock.Text = creditNote.totalCost.ToString("C");

                // Credit Note products           
                creditNoteProductsGrid.ItemsSource = creditNote.products;
            }
            else
            {
                MessageBox.Show("Credit Note with ID = " + creditNoteID + ", does not exist");
            }
        }

        /// <summary>
        ///     Method that handles the event Key Down on the textbox creditNoteNumber.
        ///     If the key pressed is Enter then it loads the credit note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_creditNoteNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) Btn_LoadCreditNote_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            creditNote = null;
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Invoice.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            creditNoteProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_creditNoteNumber.IsReadOnly = false;
            txtBox_creditNoteNumber.Focus();
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_creditNoteNumber.Text, out var creditNoteID);
            if (txtBox_creditNoteNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the credit note with ID = " + creditNoteID + ". Are you sure?";
                var txt = "Delete Credit Note";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        CreditNoteViewModel.deleteCreditNote(creditNoteID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Credit Note with ID = " + creditNoteID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No credit note is loaded");
            }
        }

        /// <summary>
        ///     Switches to edit Credit Note page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (creditNote != null) mainPage.editCreditNote(creditNote.idCreditNote);
        }

        #region PDF

        /// <summary>
        ///     Saves the credit note as a pdf file in the predetermined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (creditNote == null)
            {
                MessageBox.Show("Credit Note is not loaded!");
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
                                          "/Documents/InvoiceX/CreditNotes/");

                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") +
                               "/Documents/InvoiceX/CreditNotes/CreditNote" + txtBox_creditNoteNumber.Text + ".pdf";
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
            //Create and save the pdf
            if (creditNote == null)
            {
                MessageBox.Show("Credit Note is not loaded!");
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
                var filename = "CreditNote.pdf";
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
        ///     Preview the credit note as a pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("creditNote_temp.pdf")) File.Delete("creditNote_temp.pdf");
            if (creditNote == null)
            {
                MessageBox.Show("Credit Note is not loaded!");
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
                var filename = "creditNote_temp.pdf";
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
            var customer = creditNote.customer;
            var customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
                                 customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = (CustomerViewModel.calculateCustomerBalance(customer.idCustomer)).ToString("c");
            customerDetails[5] = customer.idCustomer.ToString();

            var creditNoteDetails = new string[6];
            creditNoteDetails[0] = txtBox_creditNoteNumber.Text;
            Console.WriteLine(txtBox_creditNoteNumber.Text);
            creditNoteDetails[1] = txtBox_createdDate.Text;
            creditNoteDetails[2] = txtBox_issuedBy.Text;
            creditNoteDetails[3] = NetTotal_TextBlock.Text;
            creditNoteDetails[4] = Vat_TextBlock.Text;
            creditNoteDetails[5] = TotalAmount_TextBlock.Text;

            var products = creditNoteProductsGrid.Items.OfType<Product>().ToList();


            var creditnote2 = new CreditNoteFormNew("../../Forms/creditNote.xml", customerDetails, creditNoteDetails,
                products);
            var document = creditnote2.CreateDocument();
            return document;
        }

        #endregion
    }
}