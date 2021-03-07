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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ReceiptView.xaml
    /// </summary>
    public partial class ReceiptView : Page
    {
        private Receipt receipt;
        private readonly ReceiptMain receiptMain;

        public ReceiptView(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_receiptNumber.Focus();
        }

        /// <summary>
        ///     After validating the receipt ID calls loadReceipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadReceipt_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_receiptNumber.Text, out var receiptID);
            if (receiptID > 0)
                loadReceipt(receiptID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for receipt ID.");
        }

        /// <summary>
        ///     Loads the receipt information on the page
        /// </summary>
        /// <param name="receiptID"></param>
        public void loadReceipt(int receiptID)
        {
            receipt = ReceiptViewModel.getReceipt(receiptID);
            if (receipt != null)
            {
                // Customer details
                textBox_Customer.Text = receipt.customer.CustomerName;
                textBox_Contact_Details.Text = receipt.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = receipt.customer.Email;
                textBox_Address.Text = receipt.customer.Address + ", " + receipt.customer.City + ", " +
                                       receipt.customer.Country;

                // Receipt details
                txtBox_receiptNumber.Text = receipt.idReceipt.ToString();
                txtBox_receiptNumber.IsReadOnly = true;
                txtBox_receiptDate.Text = receipt.createdDate.ToString("d");
                txtBox_issuedBy.Text = receipt.issuedBy;
                TotalAmount_TextBlock.Text = receipt.totalAmount.ToString("C");

                // Receipt payments           
                receiptPaymentsGrid.ItemsSource = receipt.payments;
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + receiptID + ", does not exist");
            }
        }

        /// <summary>
        ///     Method that handles the event Key Down on the textbox receiptNumber.
        ///     If the key pressed is Enter then it loads the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_receiptNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) Btn_LoadReceipt_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            receipt = null;
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Invoice.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            receiptPaymentsGrid.ItemsSource = null;
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_receiptNumber.IsReadOnly = false;
            txtBox_receiptNumber.Focus();
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_receiptNumber.Text, out var receiptID);
            if (txtBox_receiptNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the receipt with ID = " + receiptID + ". Are you sure?";
                var txt = "Delete Receipt";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ReceiptViewModel.deleteReceipt(receiptID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Receipt with ID = " + receiptID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No receipt is loaded");
            }
        }

        /// <summary>
        ///     Switches to edit Receipt page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (receipt != null) receiptMain.editReceipt(receipt.idReceipt);
        }

        #region PDF

        /// <summary>
        ///     Saves the receipt as a pdf file in the predetermined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (receipt == null)
            {
                MessageBox.Show("Receipt is not loaded!");
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
                                          "/Documents/InvoiceX/Receipts/");
                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") +
                               "/Documents/InvoiceX/Receipts/Receipt" + txtBox_receiptNumber.Text + ".pdf";
                ;

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
            if (receipt == null)
            {
                MessageBox.Show("Receipt is not loaded!");
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
                var filename = "Receipt.pdf";
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
        ///     Preview the receipt as a pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Receipt_temp.pdf")) File.Delete("Receipt_temp.pdf");
            if (receipt == null)
            {
                MessageBox.Show("Receipt is not loaded!");
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
                var filename = "Receipt_temp.pdf";
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
            var customer = receipt.customer;
            var customerDetails = new string[6];
            customerDetails[0] = customer.CustomerName;
            customerDetails[1] = customer.Address + ", " +
                                 customer.City + ", " + customer.Country;
            customerDetails[2] = customer.PhoneNumber.ToString();
            customerDetails[3] = customer.Email;
            customerDetails[4] = customer.Balance.ToString();
            customerDetails[5] = customer.idCustomer.ToString();

            var receiptDetails = new string[4];
            receiptDetails[0] = txtBox_receiptNumber.Text;
            receiptDetails[1] = txtBox_receiptDate.Text;
            receiptDetails[2] = txtBox_issuedBy.Text;
            receiptDetails[3] = receipt.totalAmount.ToString();


            var payments = receiptPaymentsGrid.Items.OfType<Payment>().ToList();


            var receipt2 = new ReceiptFormNew("../../Forms/Receipt.xml", customerDetails, receiptDetails, payments);
            var document = receipt2.CreateDocument();
            return document;
        }

        #endregion
    }
}