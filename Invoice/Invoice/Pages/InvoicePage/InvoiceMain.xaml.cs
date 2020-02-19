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

        public InvoiceMain()
        {
            InitializeComponent();
            viewAllPage = new InvoiceViewAll(this);
            viewPage = new InvoiceView(this);
            createpage = new InvoiceCreate();
            btnViewAll_Click(new object(), new RoutedEventArgs());
        }        

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewAllPage;
            viewAllPage.load();
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
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style =
            btnViewAll.Style =  FindResource("ButtonStyle") as Style;
        }
        public void loadInvoice(int invID)
        {
            viewPage.loadInvoice(invID);
            btnView_Click(null, null);
        }
    }
}
