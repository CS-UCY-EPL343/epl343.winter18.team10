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

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    ///     Interaction logic for ExpensesViewAll.xaml
    /// </summary>
    public partial class ExpensesViewAll : Page
    {
        private readonly ExpensesMain expensesMain;
        private ExpensesViewModel expViewModel;

        public ExpensesViewAll(ExpensesMain expensesMain)
        {
            InitializeComponent();
            this.expensesMain = expensesMain;
            cmbBoxStatus.SelectionChanged += CmbBoxStatus_SelectionChanged;
        }

        /// <summary>
        ///     Loads all the expenses on to the grid
        /// </summary>
        public void load()
        {
            expViewModel = new ExpensesViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource {Source = expViewModel.expensesList};

            var Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue
                                                   || !string.IsNullOrWhiteSpace(txtBoxCompanyName.Text) ||
                                                   !string.IsNullOrWhiteSpace(txtBoxCategory.Text)
                                                   || cmbBoxStatus.SelectedIndex != 0)
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            expensesDataGrid.ItemsSource = Itemlist;
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
            var companyName = txtBoxCompanyName.Text;
            var category = txtBoxCategory.Text;
            var status = cmbBoxStatus.SelectedIndex;

            var item = obj as Expense;
            if (dateFrom.HasValue)
                logic &= item.createdDate.CompareTo(dateFrom.Value) >= 0;

            if (dateTo.HasValue)
                logic &= item.createdDate.CompareTo(dateTo.Value) <= 0;

            if (!string.IsNullOrWhiteSpace(companyName))
                logic &= item.companyName.ToLower().Contains(companyName.ToLower());

            if (!string.IsNullOrWhiteSpace(category))
                logic &= item.category.ToLower().Contains(category.ToLower());

            if (status == 1)
                logic &= item.isPaid;

            if (status == 2)
                logic &= !item.isPaid;

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
        ///     The method that handles the event Text Changed on the textbox containing the filter Category.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the filter Status.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            txtBoxCompanyName.Clear();
            txtBoxCategory.Clear();
            cmbBoxStatus.SelectedIndex = 0;
            expensesDataGrid.ItemsSource = expViewModel.expensesList;
        }

        /// <summary>
        ///     Switches to view Expense page and loads the specific expense
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewExpense_Click(object sender, RoutedEventArgs e)
        {
            expensesMain.viewExpense(((Expense) expensesDataGrid.SelectedItem).idExpense);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            var expenseID = ((Expense) expensesDataGrid.SelectedItem).idExpense;
            var msgtext = "You are about to delete the expense with ID = " + expenseID + ". Are you sure?";
            var txt = "Delete Expense";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExpensesViewModel.deleteExpense(expenseID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}