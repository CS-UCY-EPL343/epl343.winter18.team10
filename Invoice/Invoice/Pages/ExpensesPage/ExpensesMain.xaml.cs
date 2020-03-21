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

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    /// Interaction logic for ExpensesMain.xaml
    /// </summary>
    public partial class ExpensesMain : Page
    {
        ExpensesViewAll viewAllPage;
        ExpensesView viewPage;
        ExpensesCreate createPage;
        ExpensesEdit editPage;
        // statisticsPage;

        public ExpensesMain()
        {
            InitializeComponent();
            viewAllPage = new ExpensesViewAll(this);
            viewPage = new ExpensesView();
            createPage = new ExpensesCreate();
            editPage = new ExpensesEdit();
            //statisticsPage = new ();
            btnViewAll_Click(null, null);
        }

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = viewAllPage;
            viewAllPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = createPage;
            //createPage.load();
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            //expensesPage.Content = statisticsPage;            
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = editPage;
            //editPage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style =
            btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        public void viewExpense(int expID)
        {
            viewPage.loadExpense(expID);
            btnView_Click(null, null);
        }
    }
}
