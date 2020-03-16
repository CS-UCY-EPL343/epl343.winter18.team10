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
    /// Interaction logic for OrderMain.xaml
    /// </summary>
    public partial class OrderMain : Page
    {
        OrderViewAll viewAllPage;
        OrderView viewPage;
        OrderCreate createpage;
        OrderEdit editpage; 

        public OrderMain()
        {
            InitializeComponent();
            viewAllPage = new OrderViewAll(this);
            viewPage = new OrderView(this);
            createpage = new OrderCreate();
            editpage = new OrderEdit();
            btnViewAll_Click(null,null);
        }

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = viewAllPage;
            viewAllPage.load();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = editpage;
            editpage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = createpage;
            createpage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style =
            btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        public void viewOrder(int orderID)
        {
            viewPage.loadOrder(orderID);
            btnView_Click(null, null);
        }

        public void editOrder(int orderID)
        {
            //editpage.loadOrder(orderID);
            btnEdit_Click(null, null);
        }
    }
}
