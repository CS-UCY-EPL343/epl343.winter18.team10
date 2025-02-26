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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MigraDoc.DocumentObjectModel;
using InvoiceX.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MigraDoc.Rendering;

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    ///     Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : Page
    {
        private readonly CustomerMain customerMain;
        private CustomerViewModel custViewModel = new CustomerViewModel();

        public CustomerView(CustomerMain customerMain)
        {
            InitializeComponent();
            this.customerMain = customerMain;
        }

        /// <summary>
        ///     Loads all the customers on to the grid
        /// </summary>
        public void load()
        {
            custViewModel = new CustomerViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource { Source = custViewModel.customersList };

            var Itemlist = _itemSourceList.View;

            if (!string.IsNullOrWhiteSpace(txtBoxFrom.Text) || !string.IsNullOrWhiteSpace(txtBoxTo.Text)
                                                            || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text) ||
                                                            !string.IsNullOrWhiteSpace(txtBoxCity.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            customerDataGrid.ItemsSource = Itemlist;


            List<MyClass> lst = new List<MyClass>();
            for (int i = 0; i < custViewModel.customersList.Count; i++)
            {
                if (custViewModel.customersList[i] != null && custViewModel.lastInvoiceOfCustomer[i] != null) {
                    lst.Add(new MyClass() { CName = custViewModel.customersList[i].CustomerName, Invoice1Date = custViewModel.lastInvoiceOfCustomer[i].createdDate, Invoice2Date = custViewModel.lastInvoice2OfCustomer[i].createdDate, DayDifference = (custViewModel.lastInvoice2OfCustomer[i].createdDate - custViewModel.lastInvoiceOfCustomer[i].createdDate).Days.ToString(), dateProjection = (custViewModel.lastInvoice2OfCustomer[i].createdDate.AddDays((custViewModel.lastInvoice2OfCustomer[i].createdDate - custViewModel.lastInvoiceOfCustomer[i].createdDate).Days))});
        }
    }
            var _itemSourceList2 = new CollectionViewSource { Source = lst };

            var Itemlist2 = _itemSourceList2.View;

            customerDataGridInvoices.ItemsSource = Itemlist2;

        }
        class MyClass
        {
            public string CName { get; set; }
            public DateTime Invoice1Date { get; set; }
            public DateTime Invoice2Date { get; set; }
            public string DayDifference { get; set; }
            public DateTime dateProjection { get; set; }

        }

        /// <summary>
        ///     The custom filter used to filter the grid's items
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool customFilter(object obj)
        {
            var logic = true;
            var balanceFrom = txtBoxFrom.Text;
            var balanceTo = txtBoxTo.Text;
            var customerName = txtBoxCustomer.Text;
            var city = txtBoxCity.Text;

            var item = obj as Customer;
            if (double.TryParse(balanceFrom, out var f))
                logic = logic & (item.Balance >= Convert.ToDouble(balanceFrom));

            if (double.TryParse(balanceTo, out var t))
                logic = logic & (item.Balance <= Convert.ToDouble(balanceTo));

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & item.CustomerName.ToLower().Contains(customerName.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                logic = logic & item.City.ToLower().Contains(city.ToLower());

            return logic;
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter Customer.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter City.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     Clears the filters on the page and reloads the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtBoxFrom.Clear();
            txtBoxTo.Clear();
            txtBoxCustomer.Clear();
            txtBoxCity.Clear();
            customerDataGrid.ItemsSource = custViewModel.customersList;
        }
        private void btnCreateCustomersBalanceSheet_Click(object sender, RoutedEventArgs e)
        {
            var document = createPdfBalanceSheet();

            document.UseCmykColor = true;

            // Create a renderer for PDF that uses Unicode font encoding
            var pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();
            Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") +
                                      "/Documents/InvoiceX/Customers Balance Report/");
            DateTime date = DateTime.Now;

            // Save the PDF document...
            var filename = Environment.GetEnvironmentVariable("USERPROFILE") +
                           "/Documents/InvoiceX/Customers Balance Report/Report" + date.Day+"-"+date.Month+"-"+date.Year + ".pdf";
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
        private void btnCreateCustomersNextOrderInfo_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.Visibility == Visibility.Visible)
            {
                customerDataGrid.Visibility = Visibility.Collapsed;
                customerDataGridInvoices.Visibility = Visibility.Visible;
            }
            else
            {
                customerDataGrid.Visibility = Visibility.Visible;
                customerDataGridInvoices.Visibility = Visibility.Collapsed;

            }

        }
        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter From.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter To.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     Switches to edit Customer page and loads the specific customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            customerMain.editCustomer(((Customer) customerDataGrid.SelectedItem).idCustomer);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerID = ((Customer) customerDataGrid.SelectedItem).idCustomer;
            var msgtext = "You are about to delete the customer with ID = " + customerID + ". Are you sure?";
            var txt = "Delete Customer";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    CustomerViewModel.deleteCustomer(customerID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     The method that handles the event Context Menu Opening on the options of each grid item.
        ///     If the user is not admin the option to delete a customer is hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptions_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            if (MainWindow.user.admin == false)
            {
                var btn = (Button) sender;
                var separator = (Separator) btn.ContextMenu.Items.GetItemAt(1);
                var itemDelete = (MenuItem) btn.ContextMenu.Items.GetItemAt(2);
                separator.Visibility = Visibility.Collapsed;
                itemDelete.Visibility = Visibility.Collapsed;
            }
        }

        private Document createPdfBalanceSheet()
        {

            List<Customer> customers = custViewModel.customersList;

            try
            {
                float total = 0;
                var report = new CustomersBalanceReport("../../Forms/Invoice.xml", customers, total);
                var document = report.CreateDocument();
                return document;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return null;
        }
        
    }
}