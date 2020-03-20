using InvoiceX.Models;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductStatistics : Page
    {
        ProductViewModel prodViewModel;

        public ProductStatistics()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            prodViewModel = new ProductViewModel();
            productComboBox.ItemsSource = prodViewModel.ProductList;
            productComboBox.DisplayMemberPath = "ProductName";
            productComboBox.SelectedValuePath = "ProductName";
        }



        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            Product selectedProduct = (Product)productComboBox.SelectedItem;
            String[] months = (cmbBoxLast.Text).Split(' ');

            int totalProducts = ProductViewModel.getProductCount(selectedProduct.idProduct, int.Parse(months[0]));
            System.Windows.Forms.MessageBox.Show(totalProducts.ToString());


        }
        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cmbBoxBy_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
