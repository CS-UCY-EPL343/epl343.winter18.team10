using InvoiceX.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;


namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    /// Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        InvoiceViewAll viewAllPage;
        InvoiceView viewPage;
        InvoiceCreate createpage;
        InvoiceEdit editPage;
        InvoiceStatistics invoiceStatistics;

        public InvoiceMain()
        {
            InitializeComponent();
            viewAllPage = new InvoiceViewAll(this);
            viewPage = new InvoiceView(this);
            createpage = new InvoiceCreate(this);
            editPage = new InvoiceEdit(this);
            invoiceStatistics = new InvoiceStatistics();
            btnViewAll_Click(null,null);
        }        

        /// <summary>
        /// Switches to Invoice View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        /// Switches to Invoice Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = invoiceStatistics;
        }

        /// <summary>
        /// Switches to Invoice Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = createpage;
            createpage.load();            
        }

        /// <summary>
        /// Switches to Invoice View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewPage;
        }

        /// <summary>
        /// Switches to Invoice Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = editPage;
            editPage.load();
        }

        /// <summary>
        /// Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style= 
            btnViewAll.Style =  FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        /// Passes the invoice ID to the invoice view page and switches to it
        /// </summary>
        /// <param name="invID"></param>
        public void viewInvoice(int invID)
        {
            viewPage.loadInvoice(invID);
            btnView_Click(null, null);
        }

        /// <summary>
        /// Passes the invoice ID to the invoice edit page and switches to it
        /// </summary>
        /// <param name="invID"></param>
        public void editInvoice(int invID)
        {
            editPage.loadInvoice(invID);
            btnEdit_Click(null, null);
        }

        /// <summary>
        /// Passes the order to the invoice create page and switches to it
        /// </summary>
        /// <param name="order"></param>
        public void issueOrderAsInvoice(Order order)
        {
            createpage.loadOrder(order);
            btnCreate_Click(null, null);
        }
    }
}
