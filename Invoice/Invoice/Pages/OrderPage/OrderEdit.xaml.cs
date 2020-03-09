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

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    /// Interaction logic for OrderEdit.xaml
    /// </summary>
    public partial class OrderEdit : Page
    {
        ProductViewModel productView;
        public OrderEdit()
        {
            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            load();
        }

        public void load()
        {
            productView = new ProductViewModel();
            comboBox_Product.ItemsSource = productView.ProductList;
        }

        #region PDF
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void printPdf_click(object sender, RoutedEventArgs e)
        {

        }

        private void savePdf_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_AddProduct(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Load_Order(object sender, RoutedEventArgs e)
        {

        }

        public void loadOrder(int orderID)
        {

        }

        private void TextBox_ProductPrice_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_ProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
