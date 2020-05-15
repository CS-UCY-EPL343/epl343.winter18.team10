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

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    ///     Interaction logic for QuoteViewAll.xaml
    /// </summary>
    public partial class QuoteViewAll : Page
    {
        private readonly QuoteMain mainPage;
        private QuoteViewModel quoteVModel;

        public QuoteViewAll(QuoteMain mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
        }

        /// <summary>
        ///     Loads all the quotes on to the grid
        /// </summary>
        public void load()
        {
            quoteVModel = new QuoteViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource {Source = quoteVModel.quoteList};

            var Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue ||
                !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            quoteDataGrid.ItemsSource = Itemlist;
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

            var item = obj as Quote;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & item.customerName.ToLower().Contains(customerName.ToLower());

            return logic;
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
            quoteDataGrid.ItemsSource = quoteVModel.quoteList;
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
        ///     Switches to view Quote page and loads the specific quote
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.viewQuote(((Quote) quoteDataGrid.SelectedItem).idQuote);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            var quoteID = ((Quote) quoteDataGrid.SelectedItem).idQuote;
            var msgtext = "You are about to delete the quote with ID = " + quoteID + ". Are you sure?";
            var txt = "Delete Quote";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    QuoteViewModel.deleteQuote(quoteID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     Switches to edit Quote page and loads the specific quote
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditQuote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.editQuote(((Quote) quoteDataGrid.SelectedItem).idQuote);
        }
    }
}