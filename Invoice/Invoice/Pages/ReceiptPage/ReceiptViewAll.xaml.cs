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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    /// Interaction logic for ReceiptViewAll.xaml
    /// </summary>
    public partial class ReceiptViewAll : Page
    {
        ReceiptViewModel recViewModel;
        ReceiptMain receiptMain;

        public ReceiptViewAll(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
        }

        public void load()
        {
            recViewModel = new ReceiptViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = recViewModel.receiptList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            receiptDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as Receipt;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.Date.CompareTo(dateTo.Value) <= 0);

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
            receiptDataGrid.ItemsSource = recViewModel.receiptList;
        }

        private void ViewReceipt_Click(object sender, RoutedEventArgs e)
        {
            receiptMain.viewReceipt(((Receipt)receiptDataGrid.SelectedItem).idReceipt);
        }

        private void DeleteReceipt_Click(object sender, RoutedEventArgs e)
        {
            int receiptID = ((Receipt)receiptDataGrid.SelectedItem).idReceipt;
            string msgtext = "You are about to delete the receipt with ID = " + receiptID + ". Are you sure?";
            string txt = "Delete Receipt";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

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
