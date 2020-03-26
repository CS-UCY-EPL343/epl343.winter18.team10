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

        public void load()
        {
            credVModel = new CreditNoteViewModel();
            filterList();
        }

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

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as CreditNote;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.customerName.ToLower().Contains(customerName.ToLower()));

            return logic;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            creditNoteDataGrid.ItemsSource = credVModel.creditNoteList;
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

        private void ViewCreditNote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.viewCreditNote(((CreditNote)creditNoteDataGrid.SelectedItem).idCreditNote);
        }

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

        private void EditCreditNote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.editCreditNote(((CreditNote)creditNoteDataGrid.SelectedItem).idCreditNote);
        }
    }
}
