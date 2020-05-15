// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System.ComponentModel;
using System.Windows;
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

namespace InvoiceX
{
    /// <summary>
    ///     Interaction logic for Window2.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static User user;
        private bool logout;
        private readonly CreditNoteMain m_creditNote;
        private readonly CustomerMain m_customer;
        private readonly Dashboard m_dashboard;
        private readonly ExpensesMain m_expenses;
        private readonly InvoiceMain m_invoice;
        private readonly OrderMain m_order;
        private readonly ProductMain m_product;
        private readonly QuoteMain m_quote;
        private readonly ReceiptMain m_receipt;
        private readonly SettingsMain m_settings;
        private readonly StatementMain m_statement;

        public MainWindow(User user)
        {
            InitializeComponent();
            MainWindow.user = user;
            userName.Text = MainWindow.user.username;
            if (MainWindow.user.admin)
                userPermissions.Text = "Administrator";
            else
                userPermissions.Text = "User";

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

        ~MainWindow()
        {
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnDashboard.Style = btnInvoice.Style = btnReceipt.Style =
                btnCreditNote.Style = btnStatement.Style = btnProduct.Style =
                    btnCustomers.Style = btnOrder.Style = btnQuote.Style =
                        btnExpenses.Style = btnSettings.Style = FindResource("sideMenuBtnStyle") as Style;
        }

        /// <summary>
        ///     Switches to the Dashboard Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Dashboard";
            MainPage.Content = m_dashboard;
            m_dashboard.loadOrderTable();
            resetAllBtnStyles();
            btnDashboard.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        /// <summary>
        ///     Switches to the Invoice Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInvoice_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Invoice";
            resetAllBtnStyles();
            btnInvoice.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_invoice;
        }

        /// <summary>
        ///     Switches to the Receipt Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReceipt_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Receipt";
            resetAllBtnStyles();
            btnReceipt.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_receipt;
        }

        /// <summary>
        ///     Switches to the Credit Note Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreditNote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Credit Note";
            resetAllBtnStyles();
            btnCreditNote.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_creditNote;
        }

        /// <summary>
        ///     Switches to the Statement Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatement_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Statement";
            resetAllBtnStyles();
            btnStatement.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_statement;
        }

        /// <summary>
        ///     Switches to the Product Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Product";
            MainPage.Content = m_product;
            resetAllBtnStyles();
            btnProduct.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        /// <summary>
        ///     Switches to the Customers Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Customers";
            MainPage.Content = m_customer;
            resetAllBtnStyles();
            btnCustomers.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        /// <summary>
        ///     Switches to the Order Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOrder_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Order";
            resetAllBtnStyles();
            btnOrder.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_order;
        }

        /// <summary>
        ///     Switches to the Quote Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuote_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Quote";
            MainPage.Content = m_quote;
            resetAllBtnStyles();
            btnQuote.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        /// <summary>
        ///     Switches to the Epenses Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Expenses";
            MainPage.Content = m_expenses;
            resetAllBtnStyles();
            btnExpenses.Style = FindResource("sideMenuBtnStyle_selected") as Style;
        }

        /// <summary>
        ///     Switches to the Settings Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            pageTitle.Text = "Settings";
            resetAllBtnStyles();
            btnSettings.Style = FindResource("sideMenuBtnStyle_selected") as Style;
            MainPage.Content = m_settings;
        }

        /// <summary>
        ///     Attempts to logout of the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            logout = true;
            Close();
        }

        /// <summary>
        ///     Calls the issue order as invoice method of Invoice
        /// </summary>
        /// <param name="order"></param>
        public void issueOrderAsInvoice(Order order)
        {
            m_invoice.issueOrderAsInvoice(order);
            BtnInvoice_Click(null, null);
        }

        /// <summary>
        ///     Based on the type of the statement item calls the appropriate view method
        /// </summary>
        /// <param name="item"></param>
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

        /// <summary>
        ///     The method that handles the event Window Closing.
        ///     Different prompts for logging out and exiting the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            string msgtext, txt;
            MessageBoxButton button;
            MessageBoxResult result;
            if (logout)
            {
                logout = false;
                msgtext = "You are about to logout and return to the login screen. Are you sure?";
                txt = "Logout";
                button = MessageBoxButton.YesNo;
                result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        var loginWindow = new LoginWindow();
                        loginWindow.Show();
                        e.Cancel = false;
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                }
            }
            else
            {
                msgtext = "You are about to exit the application. Are you sure?";
                txt = "Exit";
                button = MessageBoxButton.YesNo;
                result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        e.Cancel = false;
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}