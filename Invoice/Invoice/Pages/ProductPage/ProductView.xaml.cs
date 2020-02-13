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
    public partial class ProductView : Page
    {
        ProductViewModel prodViewModel;

        public ProductView()
        {
            InitializeComponent();
            cmbBoxStatus.SelectionChanged += new SelectionChangedEventHandler(CmbBoxStatus_SelectionChanged);
        }

        public void load()
        {
            prodViewModel = new ProductViewModel();
            filterList();
        }

        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource() { Source = prodViewModel.ProductList };

            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            if (!string.IsNullOrWhiteSpace(txtBoxCategory.Text) || !string.IsNullOrWhiteSpace(txtBoxProduct.Text)
                || cmbBoxStatus.SelectedIndex != 0)
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            productDataGrid.ItemsSource = Itemlist;
        }

        private bool customFilter(Object obj)
        {
            bool logic = true;
            string category = txtBoxCategory.Text;
            string productName = txtBoxProduct.Text;
            int status = cmbBoxStatus.SelectedIndex;
            

            var item = obj as Product;
            if (!string.IsNullOrWhiteSpace(category))
                logic = logic & (item.Category.ToLower().Contains(category.ToLower()));

            if (!string.IsNullOrWhiteSpace(productName))
                logic = logic & (item.ProductName.ToLower().Contains(productName.ToLower()));

            if (status == 1)
                logic = logic & item.LowStock;

            if (status == 2)
                logic = logic & !item.LowStock;

            return logic;
        }

        private void TxtBoxProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void TxtBoxCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        private void CmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtBoxCategory.Clear();
            txtBoxProduct.Clear();
            cmbBoxStatus.SelectedIndex = 0;
            productDataGrid.ItemsSource = prodViewModel.ProductList;
        }
    }
}
