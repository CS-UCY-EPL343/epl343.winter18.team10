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
    /// Interaction logic for InvoiceView.xaml
    /// </summary>
    public partial class InvoiceView : Page
    {
        private Invoice invoice;

        public InvoiceView()
        {
            InitializeComponent();
        }

        private void Btn_LoadInvoice_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(invoiceNumber.Text, out int invoiceID);
            if (invoiceID != 0)
            {
                loadInvoice(invoiceID);
            }
            else
            {
                //not a number
            }
        }

        public void loadInvoice(int invoiceID)
        {
            invoice = InvoiceViewModel.getInvoiceById(invoiceID);
            invoiceProductsGrid.ItemsSource = invoice.m_products;
        }

        #region PDF
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

    }
}
