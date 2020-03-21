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
        InvoiceEdit editpage;
        InvoiceStatistics invoiceStatistics;

        public InvoiceMain()
        {
            InitializeComponent();
            viewAllPage = new InvoiceViewAll(this);
            viewPage = new InvoiceView(this);
            createpage = new InvoiceCreate(this);
            editpage = new InvoiceEdit();
            invoiceStatistics = new InvoiceStatistics();
            btnViewAll_Click(null,null);
        }        

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewAllPage;
            viewAllPage.load();
        }

        public void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = invoiceStatistics;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = createpage;
            createpage.load();            
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = editpage;
            editpage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style= 
            btnViewAll.Style =  FindResource("ButtonStyle") as Style;
        }

        public void viewInvoice(int invID)
        {
            viewPage.loadInvoice(invID);
            btnView_Click(null, null);
        }

        public void editInvoice(int invID)
        {
            editpage.loadInvoice(invID);
            btnEdit_Click(null, null);
        }

        public void issueOrderAsInvoice(Order order)
        {
            createpage.loadOrder(order);
            btnCreate_Click(null, null);
        }

    }
}
