// /*****************************************************************************
//  * MIT License
//  *
//  * Copyright (c) 2020 InvoiceX
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.
//  *
//  *****************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : Page
    {
        private readonly ProductMain productMain;
        private ProductViewModel prodViewModel;

        public ProductView(ProductMain productMain)
        {
            InitializeComponent();
            this.productMain = productMain;
            cmbBoxStatus.SelectionChanged += CmbBoxStatus_SelectionChanged;
        }

        /// <summary>
        ///     Loads all the products on to the grid
        /// </summary>
        public void load()
        {
            prodViewModel = new ProductViewModel();
            filterList();
        }

        /// <summary>
        ///     Filters all the items on the grid based on some filters given on the page
        /// </summary>
        private void filterList()
        {
            var _itemSourceList = new CollectionViewSource {Source = prodViewModel.productList};

            var Itemlist = _itemSourceList.View;

            if (!string.IsNullOrWhiteSpace(txtBoxCategory.Text) || !string.IsNullOrWhiteSpace(txtBoxProduct.Text)
                                                                || cmbBoxStatus.SelectedIndex != 0)
            {
                var filter = new Predicate<object>(customFilter);
                Itemlist.Filter = filter;
            }

            productDataGrid.ItemsSource = Itemlist;
        }

        /// <summary>
        ///     The custom filter used to filter the grid's items
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool customFilter(object obj)
        {
            var logic = true;
            var category = txtBoxCategory.Text;
            var productName = txtBoxProduct.Text;
            var status = cmbBoxStatus.SelectedIndex;

            var item = obj as Product;
            if (!string.IsNullOrWhiteSpace(category))
                logic = logic & item.Category.ToLower().Contains(category.ToLower());

            if (!string.IsNullOrWhiteSpace(productName))
                logic = logic & item.ProductName.ToLower().Contains(productName.ToLower());

            if (status == 1)
                logic = logic & item.LowStock;

            if (status == 2)
                logic = logic & !item.LowStock;

            return logic;
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter Product.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the filter Category.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBoxCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the filter Status.
        ///     Calls the filterList method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filterList();
        }

        /// <summary>
        ///     Clears the filters on the page and reloads the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtBoxCategory.Clear();
            txtBoxProduct.Clear();
            cmbBoxStatus.SelectedIndex = 0;
            productDataGrid.ItemsSource = prodViewModel.productList;
        }

        /// <summary>
        ///     Switches to edit Product page and loads the specific product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            productMain.editProduct(((Product) productDataGrid.SelectedItem).idProduct);
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var productID = ((Product) productDataGrid.SelectedItem).idProduct;
            var msgtext = "You are about to delete the product with ID = " + productID + ". Are you sure?";
            var txt = "Delete Product";
            var button = MessageBoxButton.YesNo;
            var result = MessageBox.Show(msgtext, txt, button);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    ProductViewModel.deleteProduct(productID);
                    load();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}