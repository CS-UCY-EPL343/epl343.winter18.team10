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

namespace InvoiceX.Pages.QuotePage
{
    /// <summary>
    /// Interaction logic for QuoteMain.xaml
    /// </summary>
    public partial class QuoteMain : Page
    {
        QuoteViewAll viewAllPage;
        QuoteCreate createpage;
        QuoteView viewPage;
        QuoteEdit editpage;

        public QuoteMain()
        {
            InitializeComponent();
            viewAllPage = new QuoteViewAll(this);
            createpage = new QuoteCreate();
            editpage = new QuoteEdit();
            viewPage = new QuoteView();
            btnViewAll_Click(null, null);
        }

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            quotePage.Content = viewAllPage;
            viewAllPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            quotePage.Content = createpage;
            //createpage.load();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            quotePage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            quotePage.Content = editpage;
            //editpage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style =
            btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        public void viewQuote(int invID)
        {
            //viewPage.loadInvoice(invID);
            btnView_Click(null, null);
        }

        public void editQuote(int invID)
        {
            //editpage.loadInvoice(invID);
            btnEdit_Click(null, null);
        }
    }
}
