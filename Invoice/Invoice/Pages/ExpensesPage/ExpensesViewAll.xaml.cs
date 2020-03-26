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
            cmbBoxStatus.SelectionChanged += new SelectionChangedEventHandler(CmbBoxStatus_SelectionChanged);
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

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue 
                || !string.IsNullOrWhiteSpace(txtBoxCompanyName.Text) || !string.IsNullOrWhiteSpace(txtBoxCategory.Text)
                || cmbBoxStatus.SelectedIndex != 0)
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
            string companyName = txtBoxCompanyName.Text;
            string category = txtBoxCategory.Text;
            int status = cmbBoxStatus.SelectedIndex;

            var item = obj as Expense;
            if (dateFrom.HasValue)
                logic &= (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic &= (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(companyName))
                logic &= (item.companyName.ToLower().Contains(companyName.ToLower()));

            if (!string.IsNullOrWhiteSpace(category))
                logic &= (item.category.ToLower().Contains(category.ToLower()));

            if (status == 1)
                logic &= item.isPaid;

            if (status == 2)
                logic &= !item.isPaid;

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

        private void txtBoxCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void CmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCompanyName.Clear();
            txtBoxCategory.Clear();
            cmbBoxStatus.SelectedIndex = 0;
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
                    ExpensesViewModel.deleteExpense(expenseID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

       
    }
}
