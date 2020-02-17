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
            int.TryParse(txtBox_invoiceNumber.Text, out int invoiceID);
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

            // Customer details
            textBox_Customer.Text = invoice.m_customer.CustomerName;
            textBox_Contact_Details.Text = invoice.m_customer.PhoneNumber.ToString();
            textBox_Email_Address.Text = invoice.m_customer.Email;
            textBox_Address.Text = invoice.m_customer.Address + ", " + invoice.m_customer.City + ", " + invoice.m_customer.Country;

            // Invoice details
            txtBox_invoiceNumber.Text = invoice.m_idInvoice.ToString();
            txtBox_invoiceDate.Text = invoice.m_createdDate.ToString();
            txtBox_dueDate.Text = invoice.m_dueDate.ToString();
            txtBox_issuedBy.Text = invoice.m_issuedBy;
            NetTotal_TextBlock.Text = invoice.m_cost.ToString("N2");
            Vat_TextBlock.Text = invoice.m_VAT.ToString("N2");
            TotalAmount_TextBlock.Text = invoice.m_totalCost.ToString("N2");

            // Invoice products           
            invoiceProductsGrid.ItemsSource = invoice.m_products;
        }

        private void txtBox_invoiceNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Btn_LoadInvoice_Click(new object(), new RoutedEventArgs());
            }
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
