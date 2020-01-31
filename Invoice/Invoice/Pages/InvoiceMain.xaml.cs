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

namespace Invoice.Pages
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        public InvoiceMain()
        {
            InitializeComponent();
            tempData();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Visible;
            createTab.Visibility = Visibility.Hidden;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            viewTab.Visibility = Visibility.Hidden;
            createTab.Visibility = Visibility.Visible;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tempData()
        {
            List<Invoice.Classes.Invoice> invoices = new List<Invoice.Classes.Invoice>();

            for (int i = 0; i < 50; i++)
            {
                List<Invoice.Classes.InvoiceProduct> products = new List<Classes.InvoiceProduct>();
                products.Add(new Classes.InvoiceProduct()
                {
                    m_idInvoice = i,
                    m_idProduct = 3,
                    m_quantity = 15,
                    m_totalCost = 535.25 + 25.37
                });
                invoices.Add(new Classes.Invoice()
                {
                    m_date = "10/10/2020",
                    m_idInvoice = i,
                    m_customer = "Panikos",
                    m_cost = 535.25,
                    m_VAT = 25.37,
                    m_totalCost = 535.25 + 25.37,
                    m_products = products
                });
            }
            invoiceDataGrid.ItemsSource = invoices;
        }
    }
}
