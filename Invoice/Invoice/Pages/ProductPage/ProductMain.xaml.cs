using InvoiceX.ViewModels;
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

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    /// Interaction logic for ProductMain.xaml
    /// </summary>
    public partial class ProductMain : Page
    {
        ProductView viewPage;
        ProductCreate createPage;
        ProductEdit editPage;
        ProductStatistics statisticsPage;

        public ProductMain()
        {
            InitializeComponent();
            viewPage = new ProductView(this);
            createPage = new ProductCreate();
            editPage = new ProductEdit();
            statisticsPage = new ProductStatistics();
            btnView_Click(new object(), new RoutedEventArgs());
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = viewPage;
            viewPage.load();
        }
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = statisticsPage;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = createPage;           
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = editPage;
        }

        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style= FindResource("ButtonStyle") as Style;
        }

        public void editProduct(int prodID)
        {
            editPage.loadProduct(prodID);
            btnEdit_Click(null, null);
        }
    }
}
