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

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    /// Interaction logic for ExpensesViewAll.xaml
    /// </summary>
    public partial class ExpensesViewAll : Page
    {
        ExpensesViewModel expViewModel;
        ExpensesMain expensesMain;

        public ExpensesViewAll(ExpensesMain expensesMain)
        {
            InitializeComponent();
            this.expensesMain = expensesMain;
        }

        public void load()
        {
            expViewModel = new ExpensesViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = expViewModel.expensesList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            expensesDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as Expense;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.customerName.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null)
            {
                dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            }
            filterList();
        }

        private void dtPickerTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        private void txtBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            expensesDataGrid.ItemsSource = expViewModel.expensesList;
        }

        private void ViewExpense_Click(object sender, RoutedEventArgs e)
        {
            expensesMain.viewExpense(((Expense)expensesDataGrid.SelectedItem).idExpense);
        }

        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            int expenseID = ((Expense)expensesDataGrid.SelectedItem).idExpense;
            string msgtext = "You are about to delete the expense with ID = " + expenseID + ". Are you sure?";
            string txt = "Delete Expense";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExpensesViewModel.deleteExpenseByID(expenseID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
