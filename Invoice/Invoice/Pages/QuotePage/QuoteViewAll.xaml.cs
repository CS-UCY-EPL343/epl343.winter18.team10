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

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    /// Interaction logic for QuoteViewAll.xaml
    /// </summary>
    public partial class QuoteViewAll : Page
    {
        QuoteViewModel quoteVModel;
        QuoteMain mainPage;

        public QuoteViewAll(QuoteMain mainPage)
        {
            InitializeComponent();
            this.mainPage = mainPage;
        }

        public void load()
        {
            quoteVModel = new QuoteViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = quoteVModel.quoteList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text))
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            quoteDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;

            var item = obj as Quote;
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
            quoteDataGrid.ItemsSource = quoteVModel.quoteList;
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

        private void ViewQuote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.viewQuote(((Quote)quoteDataGrid.SelectedItem).idQuote);
        }

        private void DeleteQuote_Click(object sender, RoutedEventArgs e)
        {
            int quoteID = ((Quote)quoteDataGrid.SelectedItem).idQuote;
            string msgtext = "You are about to delete the quote with ID = " + quoteID + ". Are you sure?";
            string txt = "Delete Quote";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    QuoteViewModel.deleteQuoteByID(quoteID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void EditQuote_Click(object sender, RoutedEventArgs e)
        {
            mainPage.editQuote(((Quote)quoteDataGrid.SelectedItem).idQuote);
        }
    }
}
