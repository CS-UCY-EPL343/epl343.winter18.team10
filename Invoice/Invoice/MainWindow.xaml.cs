using InvoiceX.Models;
using InvoiceX.Pages;
using InvoiceX.Pages.CreditNotePage;
using InvoiceX.Pages.CustomerPage;
using InvoiceX.Pages.ExpensesPage;
using InvoiceX.Pages.InvoicePage;
using InvoiceX.Pages.OrderPage;
using InvoiceX.Pages.ProductPage;
using InvoiceX.Pages.QuotePage;
using InvoiceX.Pages.ReceiptPage;
using InvoiceX.Pages.SettingsPage;
using InvoiceX.Pages.StatementPage;
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
        private Dashboard m_dashboard;
        private InvoiceMain m_invoice;
        private ReceiptMain m_receipt;
        private CreditNoteMain m_creditNote;
        private StatementMain m_statement;
        private ProductMain m_product;
        private CustomerMain m_customer;
        private OrderMain m_order;
        private QuoteMain m_quote;
        private ExpensesMain m_expenses;
        private SettingsMain m_settings;              

        public MainWindow()
        {
            InitializeComponent();
            //this.user = user;
            //userName.Text = this.user.username;
            //if (this.user.admin)
            //    userPermissions.Text = "Administrator";
            //else
            //    userPermissions.Text = "User";

            m_dashboard = new Dashboard();
            m_invoice = new InvoiceMain();
            m_receipt = new ReceiptMain();
            m_creditNote = new CreditNoteMain();
            m_statement = new StatementMain(this);
            m_product = new ProductMain();
            m_customer = new CustomerMain();
            m_quote = new QuoteMain();
            m_expenses = new ExpensesMain();
            m_settings = new SettingsMain();
            m_order = new OrderMain(this);

            BtnDashboard_Click(null, null);
        }

        private void resetAllBtnStyles()
        {
            btnDashboard.Style = btnInvoice.Style = btnReceipt.Style =
            btnCreditNote.Style = btnStatement.Style = btnProduct.Style =
            btnCustomers.Style = btnOrder.Style = btnQuote.Style =
            btnExpenses.Style = btnSettings.Style = FindResource("sideMenuBtnStyle") as Style;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Dashboard";
            MainPage.Content = m_dashboard;
            m_dashboard.loadOrderTable();
            resetAllBtnStyles();
            btnDashboard.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnInvoice_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Invoice";
            resetAllBtnStyles();
            btnInvoice.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_invoice;
        }

        private void BtnReceipt_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Receipt";
            resetAllBtnStyles();
            btnReceipt.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_receipt;
        }

        private void BtnCreditNote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Credit Note";
            resetAllBtnStyles();
            btnCreditNote.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_creditNote;
        }

        private void BtnStatement_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Statement";
            resetAllBtnStyles();
            btnStatement.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_statement;
        }

        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Product";
            MainPage.Content = m_product;
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
            MainPage.Content = m_order;
        }

        private void BtnQuote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Quote";
            MainPage.Content = m_quote;
            resetAllBtnStyles();
            btnQuote.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Expenses";
            MainPage.Content = m_expenses;
            resetAllBtnStyles();
            btnExpenses.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Settings";
            resetAllBtnStyles();
            btnSettings.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_settings;           
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            string msgtext = "You are about to logout and return to the login screen. Are you sure?";
            string txt = "Logout";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }

        }

        public void issueOrderAsInvoice(Order order)
        {
            m_invoice.issueOrderAsInvoice(order);
            BtnInvoice_Click(null, null);
        }

        public void viewStatementItem(StatementItem item)
        {
            switch (item.itemType)
            {
                case ItemType.Invoice:
                    m_invoice.viewInvoice(item.idItem);
                    BtnInvoice_Click(null, null);
                    break;
                case ItemType.Receipt:
                    m_receipt.viewReceipt(item.idItem);
                    BtnReceipt_Click(null, null);
                    break;
                case ItemType.CreditNote:
                    m_creditNote.viewCreditNote(item.idItem);
                    BtnCreditNote_Click(null, null);
                    break;
            }
        }
    }
}
