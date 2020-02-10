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
            btnFilter_Click(new object(), new RoutedEventArgs());
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
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
                logic = logic & (item.m_date.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.m_date.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.m_customer.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Text = null;
            invoiceDataGrid.ItemsSource = invVModel.invoiceList;
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dtPickerFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtPickerTo.SelectedDate == null)
            {
                dtPickerTo.SelectedDate = dtPickerFrom.SelectedDate;
            }
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("View, Edit, Delete");
        }
    }
}
