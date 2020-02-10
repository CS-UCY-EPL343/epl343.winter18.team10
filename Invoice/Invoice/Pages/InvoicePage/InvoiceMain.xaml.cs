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
        InvoiceViewAll viewPage = new InvoiceViewAll();
        InvoiceCreate createpage = new InvoiceCreate();

        public InvoiceMain()
        {
            InitializeComponent();
        }        

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            invoicePage.Content = viewPage;
            viewPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            invoicePage.Content = createpage;
        }       

        


    }
}
