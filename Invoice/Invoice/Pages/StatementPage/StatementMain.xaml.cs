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

namespace InvoiceX.Pages.StatementPage
{
    /// <summary>
    /// Interaction logic for StatementMain.xaml
    /// </summary>
    public partial class StatementMain : Page
    {
        StatementCreate createPage;

        public StatementMain()
        {
            InitializeComponent();
            createPage = new StatementCreate();
            btnCreate_Click(null, null);
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            statementPage.Content = createPage;
            createPage.load();
        }

        private void resetAllBtnStyles()
        {
            btnCreate.Style = FindResource("ButtonStyle") as Style;
        }
    }
}
