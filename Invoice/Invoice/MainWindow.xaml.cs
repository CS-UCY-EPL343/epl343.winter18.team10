using InvoiceX.Pages;
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
using System.Windows.Shapes;

namespace InvoiceX
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Page m_dashboard = new Dashboard();
        private Page m_invoice = new InvoiceMain();
        private Page m_receipt;
        private Page m_creditNote;
        private Page m_statement;
        private Page m_product;
        private Page m_customers;
        private Page m_order;
        private Page m_offer;
        private Page m_expenses;
        private Page m_settings;

        public MainWindow()
        {
            InitializeComponent();
            btnDashboard.Focus();
            MainPage.Content = m_dashboard;
        }
           
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Home";
            MainPage.Content = m_dashboard;
        }

        private void BtnInvoice_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Invoice";
            MainPage.Content = m_invoice;
        }

        private void BtnReceipt_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Receipt";
        }

        private void BtnCreditNote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Credit Note";
        }

        private void BtnStatement_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Statement";
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Product";
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Customers";
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Order";
        }

        private void BtnOffer_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Offer";
        }

        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Expenses";
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Settings";
        }

        
    }
}
