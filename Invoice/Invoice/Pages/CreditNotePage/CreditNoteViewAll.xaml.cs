/*****************************************************************************
 * MIT License
 *
 * Copyright (c) 2020 InvoiceX
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for CreditNoteViewAll.xaml
    /// </summary>
    public partial class CreditNoteViewAll : Page
    {
        CreditNoteMain mainPage;
        CreditNoteViewModel credVModel;

        public CreditNoteViewAll(CreditNoteMain creditNoteMain)
        {
            InitializeComponent();
            this.mainPage = creditNoteMain;
        }

        /// <summary>
        /// Loads all the credit notes on to the grid
        /// </summary>
        public void load()
        {
            credVModel = new CreditNoteViewModel();
            filterList();
        }

        /// <summary>
        /// Filters all the items on the grid based on some input values on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = credVModel.creditNoteList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            creditNoteDataGrid.ItemsSource = Itemlist;
        }

        /// <summary>
        /// The custom filter used to filter the grid's items
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as CreditNote;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.customerName.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        /// <summary>
        /// Clears the filters on the page and reloads the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            creditNoteDataGrid.ItemsSource = credVModel.creditNoteList;
        }

        /// <summary>
        /// The method that handles the event Selected Date Changed on the datePicker containing the From Date.
        /// Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null)
            {
                dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            }
            filterList();
        }

        /// <summary>
        /// The method that handles the event Selected Date Changed on the datePicker containing the filter To Date.
        /// Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtPickerTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        /// The method that handles the event Text Changed on the textbox containing the filter Customer.
        /// Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        /// Switches to view Credit Note page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewCreditNote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.viewCreditNote(((CreditNote)creditNoteDataGrid.SelectedItem).idCreditNote);
        }

        /// <summary>
        /// Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCreditNote_Click(object sender, RoutedEventArgs e)
        {
            int credID = ((CreditNote)creditNoteDataGrid.SelectedItem).idCreditNote;
            string msgtext = "You are about to delete the credit note with ID = " + credID + ". Are you sure?";
            string txt = "Delete Credit Note";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    CreditNoteViewModel.deleteCreditNote(credID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// Switches to edit Credit Note page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCreditNote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.editCreditNote(((CreditNote)creditNoteDataGrid.SelectedItem).idCreditNote);
        }
    }
}
