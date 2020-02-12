using InvoiceX.Pages;
using InvoiceX.Pages.CustomerPage;
using InvoiceX.Pages.InvoicePage;
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
        //private Page m_receipt;
        //private Page m_creditNote;
        //private Page m_statement;
        //private Page m_product;
        private Page m_customer = new CustomerMain();
        //private Page m_order;
        //private Page m_offer;
        //private Page m_expenses;
        //private Page m_settings;

        public MainWindow()
        {
            InitializeComponent();
            BtnDashboard_Click(new object(), new RoutedEventArgs());
        }
           
        private void resetAllBtnStyles()
        {
            btnDashboard.Style = btnInvoice.Style = btnReceipt.Style = 
            btnCreditNote.Style = btnStatement.Style = btnProduct.Style = 
            btnCustomers.Style = btnOrder.Style = btnOffer.Style = 
            btnExpenses.Style = btnSettings.Style = FindResource("sideMenuBtnStyle") as Style;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Dashboard";
            MainPage.Content = m_dashboard;
            resetAllBtnStyles();
            btnDashboard.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnInvoice_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Invoice";
            MainPage.Content = m_invoice;
            resetAllBtnStyles();
            btnInvoice.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnReceipt_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Receipt";
            resetAllBtnStyles();
            btnReceipt.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnCreditNote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Credit Note";
            resetAllBtnStyles();
            btnCreditNote.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnStatement_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Statement";
            resetAllBtnStyles();
            btnStatement.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Product";
            resetAllBtnStyles();
            btnProduct.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Customers";
            MainPage.Content = m_customer;
            resetAllBtnStyles();
            btnCustomers.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Order";
            resetAllBtnStyles();
            btnOrder.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnOffer_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Offer";
            resetAllBtnStyles();
            btnOffer.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Expenses";
            resetAllBtnStyles();
            btnExpenses.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Settings";
            resetAllBtnStyles();
            btnSettings.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        
    }
}
