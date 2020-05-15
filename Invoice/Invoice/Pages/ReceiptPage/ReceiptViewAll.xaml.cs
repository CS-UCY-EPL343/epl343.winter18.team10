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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ReceiptViewAll.xaml
    /// </summary>
    public partial class ReceiptViewAll : Page
    {
        private readonly ReceiptMain receiptMain;
        private ReceiptViewModel recViewModel;

        public ReceiptViewAll(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
        }

        /// <summary>
        ///     Loads all the receipts on to the grid
        /// </summary>
        public void load()
        {
            recViewModel = new ReceiptViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource {Source = recViewModel.receiptList};

            var Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue ||
                !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            receiptDataGrid.ItemsSource = Itemlist;
        }

        /// <summary>
        ///     The custom filter used to filter the grid's items
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool customFilter(object obj)
        {
            var logic = true;
            var dateFrom = dtPickerFrom.SelectedDate;
            var dateTo = dtPickerTo.SelectedDate;
            var customerName = txtBoxCustomer.Text;

            var item = obj as Receipt;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & item.customerName.ToLower().Contains(customerName.ToLower());

            return logic;
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datePicker containing the From Date.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null) dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Selected Date Changed on the datePicker containing the filter To Date.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtPickerTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
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
        ///     Clears the filters on the page and reloads the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            receiptDataGrid.ItemsSource = recViewModel.receiptList;
        }

        /// <summary>
        ///     Switches to view Receipt page and loads the specific receipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewReceipt_Click(object sender, RoutedEventArgs e)
        {
            receiptMain.viewReceipt(((Receipt) receiptDataGrid.SelectedItem).idReceipt);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteReceipt_Click(object sender, RoutedEventArgs e)
        {
            var receiptID = ((Receipt) receiptDataGrid.SelectedItem).idReceipt;
            var msgtext = "You are about to delete the receipt with ID = " + receiptID + ". Are you sure?";
            var txt = "Delete Receipt";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ReceiptViewModel.deleteReceipt(receiptID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}