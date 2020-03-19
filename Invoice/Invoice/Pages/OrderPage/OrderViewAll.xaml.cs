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

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    /// Interaction logic for OrderViewAll.xaml
    /// </summary>
    public partial class OrderViewAll : Page
    {
        OrderViewModel orderViewModel;
        OrderMain orderMain;

        public OrderViewAll(OrderMain orderMain)
        {
            InitializeComponent();
            this.orderMain = orderMain;
            cmbBoxStatus.SelectionChanged += new SelectionChangedEventHandler(cmbBoxStatus_SelectionChanged);
        }

        public void load()
        {
            orderViewModel = new OrderViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = orderViewModel.orderList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue || !string.IsNullOrWhiteSpace(txtBoxCustomer.Text)
                || !string.IsNullOrWhiteSpace(txtBox_City.Text) || cmbBoxStatus.SelectedIndex != 0)
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            orderDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            DateTime? dateFrom = dtPickerFrom.SelectedDate;
            DateTime? dateTo = dtPickerTo.SelectedDate;
            string customerName = txtBoxCustomer.Text;
            string city = txtBox_City.Text;
            int status = cmbBoxStatus.SelectedIndex;

            var item = obj as Order;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & (item.customerName.ToLower().Contains(customerName.ToLower()));

            if (!string.IsNullOrWhiteSpace(city))
                logic = logic & (item.city.ToLower().Contains(city.ToLower()));

            if (cmbBoxStatus.SelectedIndex != 0)
                logic = logic & (item.status.Equals((OrderStatus)status));

            return logic;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            dtPickerFrom.SelectedDate = null;
            dtPickerTo.SelectedDate = null;
            txtBoxCustomer.Clear();
            cmbBoxStatus.SelectedIndex = 0;
            orderDataGrid.ItemsSource = orderViewModel.orderList;
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

        private void txtBox_City_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void cmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            orderMain.viewOrder(((Order)orderDataGrid.SelectedItem).idOrder);
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderID = ((Order)orderDataGrid.SelectedItem).idOrder;
            string msgtext = "You are about to delete the order with ID = " + orderID + ". Are you sure?";
            string txt = "Delete Order";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.deleteOrderByID(orderID);                    
                    MessageBox.Show("Deleted Order with ID = " + orderID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void btnOptions_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            OrderStatus status = ((Order)orderDataGrid.SelectedItem).status;
            var btn = (Button)sender;
            
            var itemMarkReady = (MenuItem)btn.ContextMenu.Items.GetItemAt(0);
            var itemMarkPending = (MenuItem)btn.ContextMenu.Items.GetItemAt(1);
            var itemIssueInvoice = (MenuItem)btn.ContextMenu.Items.GetItemAt(2);

            if (status == OrderStatus.Pending)
            {
                itemMarkReady.Visibility = Visibility.Visible;
                itemMarkPending.Visibility = Visibility.Collapsed;
                itemIssueInvoice.Visibility = Visibility.Collapsed;
            }
            else if (status == OrderStatus.Ready)
            {
                itemMarkReady.Visibility = Visibility.Collapsed;
                itemMarkPending.Visibility = Visibility.Visible;
                itemIssueInvoice.Visibility = Visibility.Visible;
            }
            else
            {
                itemMarkReady.Visibility = Visibility.Collapsed;
                itemMarkPending.Visibility = Visibility.Collapsed;
                itemIssueInvoice.Visibility = Visibility.Collapsed;
            }
        }

        private void MarkOrderReady_Click(object sender, RoutedEventArgs e)
        {
            int orderID = ((Order)orderDataGrid.SelectedItem).idOrder;
            string msgtext = "Mark order with ID = " + orderID + " as ready?";
            string txt = "Order Ready";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.markOrderAsReady(orderID);                    
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void MarkOrderPending_Click(object sender, RoutedEventArgs e)
        {
            int orderID = ((Order)orderDataGrid.SelectedItem).idOrder;
            string msgtext = "Mark order with ID = " + orderID + " as pending?";
            string txt = "Order Pending";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.markOrderAsPending(orderID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void IssueOrderAsInvoice_Click(object sender, RoutedEventArgs e)
        {
            Order order = OrderViewModel.getOrderById(((Order)orderDataGrid.SelectedItem).idOrder); 
            orderMain.issueOrderAsInvoice(order);
        }

        
    }
}
