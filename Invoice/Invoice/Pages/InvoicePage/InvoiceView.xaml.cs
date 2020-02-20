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
        InvoiceMain mainPage;

        public InvoiceView(InvoiceMain mainPage)
        {
            this.mainPage = mainPage;

            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_invoiceNumber.Focus();
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
            txtBox_invoiceNumber.IsReadOnly = true;
            txtBox_invoiceDate.Text = invoice.m_createdDate.ToString("dd/mm/yyyy");
            txtBox_dueDate.Text = invoice.m_dueDate.ToString("dd/mm/yyyy");
            txtBox_issuedBy.Text = invoice.m_issuedBy;
            NetTotal_TextBlock.Text = invoice.m_cost.ToString("C");
            Vat_TextBlock.Text = invoice.m_VAT.ToString("C");
            TotalAmount_TextBlock.Text = invoice.m_totalCost.ToString("C");

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

        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            foreach (var ctrl in grid_Invoice.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            invoiceProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_invoiceNumber.IsReadOnly = false;
            txtBox_invoiceNumber.Focus();
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

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_invoiceNumber.Text, out int invoiceID);
            if (txtBox_invoiceNumber.IsReadOnly)
            {                
                string msgtext = "You are about to delete the invoice with ID = " + invoiceID + ". Are you sure?";
                string txt = "Delete Invoice";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        InvoiceViewModel.deleteInvoiceByID(invoiceID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Invoice with ID = " + invoiceID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }                
            }
            else
            {
                MessageBox.Show("No invoice is loaded");
            }
        }
    }
}
