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

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceViewAll.xaml
    /// </summary>
    public partial class InvoiceViewAll : Page
    {
        InvoiceViewModel invVModel;

        public InvoiceViewAll()
        {
            InitializeComponent();
        }

        public void load()
        {
            invVModel = new InvoiceViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = invVModel.invoiceList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            invoiceDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as Invoice;
            if (dateFrom.HasValue)
                logic = logic & (item.m_createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.m_createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.m_customerName.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            invoiceDataGrid.ItemsSource = invVModel.invoiceList;
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

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View, Edit, Delete");
        }

        private void txtBoxCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        
    }
}
