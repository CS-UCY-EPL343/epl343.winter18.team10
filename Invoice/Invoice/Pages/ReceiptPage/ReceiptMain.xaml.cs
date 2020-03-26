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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    /// Interaction logic for ReceiptMain.xaml
    /// </summary>
    public partial class ReceiptMain : Page
    {
        ReceiptViewAll viewAllPage;
        ReceiptView viewPage;
        ReceiptCreate createPage;
        ReceiptEdit editPage;
        ReceiptStatistics statisticsPage;

        public ReceiptMain()
        {
            InitializeComponent();
            viewAllPage = new ReceiptViewAll(this);
            viewPage = new ReceiptView();
            createPage = new ReceiptCreate(this);
            editPage = new ReceiptEdit(this);
            statisticsPage = new ReceiptStatistics();
            btnViewAll_Click(null, null);
        }

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = viewAllPage;
            viewAllPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = createPage;
            createPage.load();
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = statisticsPage;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = editPage;
            editPage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style =
            btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        public void viewReceipt(int recID)
        {
            viewPage.loadReceipt(recID);
            btnView_Click(null, null);
        }
    }
}
