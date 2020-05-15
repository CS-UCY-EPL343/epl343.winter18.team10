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

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    ///     Interaction logic for OrderViewAll.xaml
    /// </summary>
    public partial class OrderViewAll : Page
    {
        private readonly OrderMain orderMain;
        private OrderViewModel orderViewModel;

        public OrderViewAll(OrderMain orderMain)
        {
            InitializeComponent();
            this.orderMain = orderMain;
            cmbBoxStatus.SelectionChanged += cmbBoxStatus_SelectionChanged;
        }

        /// <summary>
        ///     Loads all the orders on to the grid
        /// </summary>
        public void load()
        {
            orderViewModel = new OrderViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource {Source = orderViewModel.orderList};

            var Itemlist = _itemSourceList.View;

            if (dtPickerFrom.SelectedDate.HasValue || dtPickerTo.SelectedDate.HasValue ||
                !string.IsNullOrWhiteSpace(txtBoxCustomer.Text)
                || !string.IsNullOrWhiteSpace(txtBox_City.Text) || cmbBoxStatus.SelectedIndex != 0)
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            orderDataGrid.ItemsSource = Itemlist;
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
            var customerName = txtBoxCustomer.Text;
            var city = txtBox_City.Text;
            var status = cmbBoxStatus.SelectedIndex;

            var item = obj as Order;
            if (dateFrom.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateFrom.Value) >= 0);

            if (dateTo.HasValue)
                logic = logic & (item.createdDate.CompareTo(dateTo.Value) <= 0);

            if (!string.IsNullOrWhiteSpace(customerName))
                logic = logic & item.customerName.ToLower().Contains(customerName.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                logic = logic & item.city.ToLower().Contains(city.ToLower());

            if (cmbBoxStatus.SelectedIndex != 0)
                logic = logic & item.status.Equals((OrderStatus) status);

            return logic;
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
            txtBoxCustomer.Clear();
            cmbBoxStatus.SelectedIndex = 0;
            orderDataGrid.ItemsSource = orderViewModel.orderList;
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
        ///     The method that handles the event Text Changed on the textbox containing the filter City.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_City_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the filter Status.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     Switches to view Order page and loads the specific order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewOrder_Click(object sender, RoutedEventArgs e)
        {
            orderMain.viewOrder(((Order) orderDataGrid.SelectedItem).idOrder);
        }

        /// <summary>
        ///     Switches to edit Order page and loads the specific order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            orderMain.editOrder(((Order) orderDataGrid.SelectedItem).idOrder);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            var orderID = ((Order) orderDataGrid.SelectedItem).idOrder;
            var msgtext = "You are about to delete the order with ID = " + orderID + ". Are you sure?";
            var txt = "Delete Order";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.deleteOrder(orderID);
                    MessageBox.Show("Deleted Order with ID = " + orderID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     The method that handles the event Context Menu Opening on the options of each grid item.
        ///     Based on the status of the order different options are displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOptions_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            var status = ((Order) orderDataGrid.SelectedItem).status;
            var btn = (Button) sender;

            var itemMarkReady = (MenuItem) btn.ContextMenu.Items.GetItemAt(0);
            var itemMarkPending = (MenuItem) btn.ContextMenu.Items.GetItemAt(1);
            var itemIssueInvoice = (MenuItem) btn.ContextMenu.Items.GetItemAt(2);

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

        /// <summary>
        ///     Marks the current order as Ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkOrderReady_Click(object sender, RoutedEventArgs e)
        {
            var orderID = ((Order) orderDataGrid.SelectedItem).idOrder;
            var msgtext = "Mark order with ID = " + orderID + " as ready?";
            var txt = "Order Ready";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.updateOrderStatus(orderID, OrderStatus.Ready);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     Marks the current order as Pending
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkOrderPending_Click(object sender, RoutedEventArgs e)
        {
            var orderID = ((Order) orderDataGrid.SelectedItem).idOrder;
            var msgtext = "Mark order with ID = " + orderID + " as pending?";
            var txt = "Order Pending";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    OrderViewModel.updateOrderStatus(orderID, OrderStatus.Pending);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        ///     Sends the order to be issued as invoice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssueOrderAsInvoice_Click(object sender, RoutedEventArgs e)
        {
            var order = OrderViewModel.getOrder(((Order) orderDataGrid.SelectedItem).idOrder);
            orderMain.issueOrderAsInvoice(order);
        }
    }
}