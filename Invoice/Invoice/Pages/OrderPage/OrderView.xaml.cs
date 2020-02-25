using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        private Order order;
        OrderMain orderMain;

        public OrderView(OrderMain orderMain)
        {
            this.orderMain = orderMain;

            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_orderNumber.Focus();
        }

        private void btn_loadOrder_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out int orderID);
            if (orderID > 0)
            {
                loadOrder(orderID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for order ID.");
            }
        }

        public void loadOrder(int orderID)
        {
            order = OrderViewModel.getOrderById(orderID);
            if (order != null)
            {
                // Customer details
                textBox_Customer.Text = order.customer.CustomerName;
                textBox_Contact_Details.Text = order.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = order.customer.Email;
                textBox_Address.Text = order.customer.Address + ", " + order.customer.City + ", " + order.customer.Country;

                // Invoice details
                txtBox_orderNumber.Text = order.idOrder.ToString();
                txtBox_orderNumber.IsReadOnly = true;
                txtBox_orderDate.Text = order.createdDate.ToString("dd/mm/yyyy");
                txtBox_shippingDate.Text = order.shippingDate.ToString("dd/mm/yyyy");
                txtBox_issuedBy.Text = order.issuedBy;
                txtBox_status.Text = order.status.ToString();
                NetTotal_TextBlock.Text = order.cost.ToString("C");
                Vat_TextBlock.Text = order.VAT.ToString("C");
                TotalAmount_TextBlock.Text = order.totalCost.ToString("C");

                // Invoice products           
                orderProductsGrid.ItemsSource = order.products;
            }
            else
            {
                MessageBox.Show("Invoice with ID = " + orderID + ", does not exist");
            }
        }

        private void txtBox_orderNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_loadOrder_Click(null, null);
            }
        }

        private void btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.order = null;
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            foreach (var ctrl in grid_Order.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            orderProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_orderNumber.IsReadOnly = false;
            txtBox_orderNumber.Focus();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (order != null)
            {
                //orderMain.editOrder(order.idOrder);
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out int orderID);
            if (txtBox_orderNumber.IsReadOnly)
            {
                string msgtext = "You are about to delete the order with ID = " + orderID + ". Are you sure?";
                string txt = "Delete Order";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        OrderViewModel.deleteOrderByID(orderID);
                        btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Order with ID = " + orderID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No order is loaded");
            }
        }

        private void previewPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
