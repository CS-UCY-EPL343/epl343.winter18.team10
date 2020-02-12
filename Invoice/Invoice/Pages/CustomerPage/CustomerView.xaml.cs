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

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    /// Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : Page
    {
        CustomerViewModel custViewModel = new CustomerViewModel();
        public CustomerView()
        {
            InitializeComponent();
        }
        public void load()
        {
            custViewModel = new CustomerViewModel();
            filterList();
        }
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = custViewModel.CustomersList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (!string.IsNullOrWhiteSpace(txtBoxFrom.Text) || !string.IsNullOrWhiteSpace(txtBoxTo.Text) 
                || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text) || !string.IsNullOrWhiteSpace(txtBoxCity.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            customerDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            string balanceFrom = txtBoxFrom.Text;
            string balanceTo = txtBoxTo.Text;
            string customerName = txtBoxCustomer.Text;
            string city = txtBoxCity.Text;

            var item = obj as Customers;
            if (!string.IsNullOrWhiteSpace(balanceFrom))
                logic = logic & (item.Balance >= Convert.ToDouble(balanceFrom));

            if (!string.IsNullOrWhiteSpace(balanceTo))
                logic = logic & (item.Balance <= Convert.ToDouble(balanceTo));

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.CustomerName.ToLower().Contains(customerName.ToLower()));

            if (!string.IsNullOrWhiteSpace(city))
                logic = logic & (item.City.ToLower().Contains(city.ToLower()));

            return logic;
        }

        private void txtBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void TxtBoxCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtBoxFrom.Clear();
            txtBoxTo.Clear();
            txtBoxCustomer.Clear();
            txtBoxCity.Clear();
            customerDataGrid.ItemsSource = custViewModel.CustomersList;
        }

        private void TxtBoxFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void TxtBoxTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }
    }
}
