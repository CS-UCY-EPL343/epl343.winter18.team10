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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    /// Interaction logic for CreditNoteMain.xaml
    /// </summary>
    public partial class CreditNoteMain : Page
    {
        CreditNoteViewAll viewAllPage;
        CreditNoteView viewPage;
        CreditNoteCreate createpage;
        CreditNoteEdit editpage;

        public CreditNoteMain()
        {
            InitializeComponent();
            viewAllPage = new CreditNoteViewAll(this);
            viewPage = new CreditNoteView(this);
            createpage = new CreditNoteCreate(this);
            editpage = new CreditNoteEdit();
            btnViewAll_Click(null, null);
        }

        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = viewAllPage;
            viewAllPage.load();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = createpage;
            createpage.load();
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = viewPage;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = editpage;
            //editpage.load();
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style =
            btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        public void viewCreditNote(int cdID)
        {
            viewPage.loadCreditNote(cdID);
            btnView_Click(null, null);
        }

        public void editCreditNote(int cdID)
        {
            editpage.loadCreditNote(cdID);
            btnEdit_Click(null, null);
        }
    }
}
