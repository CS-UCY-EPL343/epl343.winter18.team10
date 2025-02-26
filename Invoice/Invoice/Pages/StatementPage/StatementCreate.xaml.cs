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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Forms;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace InvoiceX.Pages.StatementPage
{
    /// <summary>
    ///     Interaction logic for StatementCreate.xaml
    /// </summary>
    public partial class StatementCreate : Page
    {
        private CustomerViewModel customerView;
        private bool isCreated;
        private readonly StatementMain statementMain;

        public StatementCreate(StatementMain statementMain)
        {
            InitializeComponent();
            this.statementMain = statementMain;
        }

        /// <summary>
        ///     The method that handles the event Drop Down Opened on the combobox containing the customers.
        ///     Loads the customers in the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_DropDownOpened(object sender, EventArgs e)
        {
            customerView = new CustomerViewModel();
            comboBox_customer.ItemsSource = customerView.customersList;
        }

        /// <summary>
        ///     After validating loads the statement items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_createStatement_Click(object sender, RoutedEventArgs e)
        {
            if (checkCustomerForm() && dateRangeSelected())
            {
                if (loadStatementItems())
                {
                    isCreated = true;
                    fromDate.IsHitTestVisible = false;
                    toDate.IsHitTestVisible = false;
                    comboBox_customer.IsHitTestVisible = false;
                }
                else
                {
                    isCreated = false;
                    MessageBox.Show("For the specific customer and date range nothing could be found.");
                }
            }
            else
            {
                MessageBox.Show("Please filled in missing values!");
            }
        }

        /// <summary>
        ///     Loads the statement items on the grid based on the filters given
        /// </summary>
        /// <returns></returns>
        private bool loadStatementItems()
        {
            var customerID = ((Customer) comboBox_customer.SelectedItem).idCustomer;
            var from = fromDate.SelectedDate.Value.Date;
            from += new TimeSpan(0, 0, 0); // start from 00:00:00 of from date
            var to = toDate.SelectedDate.Value.Date;
            to += new TimeSpan(23, 59, 59); // end on 23:59:59 of to date

            var statement = new List<StatementItem>();
            statement.AddRange(InvoiceViewModel.getInvoicesForStatement(customerID, from, to));
            statement.AddRange(CreditNoteViewModel.getCreditNotesForStatement(customerID, from, to));
            statement.AddRange(ReceiptViewModel.getReceiptsForStatement(customerID, from, to));


            if (statement.Count > 0)
            {
                statementDataGrid.ItemsSource = statement;
                var firstCol = statementDataGrid.Columns.First();
                firstCol.SortDirection = ListSortDirection.Ascending;
                statementDataGrid.Items.SortDescriptions.Add(new SortDescription(firstCol.SortMemberPath,
                    ListSortDirection.Ascending));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if the dates given are valid
        /// </summary>
        /// <returns></returns>
        private bool dateRangeSelected()
        {
            var response = true;
            if (fromDate.SelectedDate == null)
            {
                fromDate.BorderBrush = Brushes.Red;
                response = false;
            }

            if (toDate.SelectedDate == null)
            {
                toDate.BorderBrush = Brushes.Red;
                response = false;
            }

            return response;
        }

        /// <summary>
        ///     Checks that customer details are completed
        /// </summary>
        /// <returns></returns>
        private bool checkCustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Clears all inputs on the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clear_Click(object sender, RoutedEventArgs e)
        {
            isCreated = false;
            comboBox_customer_border.BorderThickness = new Thickness(0);
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            fromDate.SelectedDate = null;
            toDate.SelectedDate = null;
            comboBox_customer.SelectedIndex = -1;
            fromDate.ClearValue(Control.BorderBrushProperty);
            toDate.ClearValue(Control.BorderBrushProperty);
            statementDataGrid.ItemsSource = null;
            fromDate.IsHitTestVisible = true;
            toDate.IsHitTestVisible = true;
            comboBox_customer.IsHitTestVisible = true;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the customers.
        ///     Loads the customer's information in the appropriate text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                var customer = (Customer) comboBox_customer.SelectedItem;
                textBox_Customer.Text = customer.CustomerName;
                textBox_Address.Text = customer.Address + ", " + customer.City + ", " + customer.Country;
                textBox_Contact_Details.Text = customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = customer.Email;
            }
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datePicker containing the From Date.
        ///     Clears the red border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            fromDate.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datePicker containing the To Date.
        ///     Clears the red border
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            toDate.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Calls the viewItem method of statement main class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewItem_Click(object sender, RoutedEventArgs e)
        {
            statementMain.viewItem((StatementItem) statementDataGrid.SelectedItem);
        }

        #region PDF

        private bool isCompleted()
        {
            if (comboBox_customer.Text == null || textBox_Customer.Text == null || textBox_Address.Text == null ||
                textBox_Contact_Details == null || textBox_Email_Address == null
                || !fromDate.SelectedDate.HasValue || !toDate.SelectedDate.HasValue) return false;
            return true;
        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (!isCreated)
            {
                MessageBox.Show("Statement is not completed!");
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
                var customer = (Customer) comboBox_customer.SelectedItem;
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") +
                                          "/Documents/InvoiceX/Statements/");
                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") +
                               "/Documents/InvoiceX/Statements/Statement" + customer.idCustomer + ".pdf";
                ;

                pdfRenderer.Save(filename);
                Process.Start(filename);
            }
        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {
            if (!isCreated)
            {
                MessageBox.Show("Statement is not completed!");
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
                var filename = "Statement.pdf";
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

        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Statement_temp.pdf")) File.Delete("Statement_temp.pdf");
            if (!isCreated)
            {
                MessageBox.Show("Statement is not completed!");
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
                var filename = "Statement_temp.pdf";
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

        private Document createPdf()
        {
            var customerDetails = new string[7];
            var customer = (Customer) comboBox_customer.SelectedItem;
            customerDetails[0] = textBox_Customer.Text;
            customerDetails[1] = textBox_Address.Text;
            customerDetails[2] = textBox_Contact_Details.Text;
            customerDetails[3] = textBox_Email_Address.Text;
            customerDetails[4] = customer.idCustomer.ToString();
            customerDetails[5] = CustomerViewModel.calculateCustomerBalanceDates(customer.idCustomer, new DateTime(2019, 02, 01), fromDate.SelectedDate.Value.AddDays(-1));
            customerDetails[6] = (CustomerViewModel.calculateCustomerBalance(customer.idCustomer)).ToString("c");

            var statementDetails = new string[4];
            statementDetails[0] = fromDate.Text;
            statementDetails[1] = toDate.Text;

            var items = statementDataGrid.Items.OfType<StatementItem>().ToList();

            var allItems = new List<StatementItem>();
            var fromD=new DateTime(2020,01, 01);
            fromD += new TimeSpan(0, 0, 0); // start from 00:00:00 of from date
            var to = (DateTime.Now);
            to += new TimeSpan(23, 59, 59); // end on 23:59:59 of to date


            allItems.AddRange(InvoiceViewModel.getInvoicesForStatement(int.Parse(customerDetails[4]), fromD, to));
            allItems.AddRange(CreditNoteViewModel.getCreditNotesForStatement(int.Parse(customerDetails[4]), fromD, to));
            allItems.AddRange(ReceiptViewModel.getReceiptsForStatement(int.Parse(customerDetails[4]), fromD, to));


            var statement2 = new StatementFormNew("../../Forms/Receipt.xml", customerDetails, statementDetails, items,allItems);
            var document = statement2.CreateDocument();
            return document;
        }

        #endregion
    }
}