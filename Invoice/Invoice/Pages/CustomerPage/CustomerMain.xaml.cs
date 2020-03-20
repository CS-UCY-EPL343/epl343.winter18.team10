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
    /// Interaction logic for CustomerMain.xaml
    /// </summary>
    public partial class CustomerMain : Page
    {
        CustomerView viewPage;
        CustomerCreate createPage;
        CustomerEdit editPage;
        CustomerStatistics statisticsPage;
        public CustomerMain()
        {
            InitializeComponent();
            viewPage = new CustomerView(this);
            createPage = new CustomerCreate();
            editPage = new CustomerEdit();
            statisticsPage = new CustomerStatistics();
            btnView_Click(new object(), new RoutedEventArgs());
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = viewPage;
            viewPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = createPage;
            
        }
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = statisticsPage;

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = editPage;

        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnStatistics.Style = btnCreate.Style = FindResource("ButtonStyle") as Style;
        }

        public void editCustomer(int custID)
        {
            editPage.loadCustomer(custID);
            btnEdit_Click(null, null);
        }
    }
}
