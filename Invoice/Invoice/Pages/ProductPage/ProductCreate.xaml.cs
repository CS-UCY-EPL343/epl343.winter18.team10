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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    ///     Interaction logic for ProductCreate.xaml
    /// </summary>
    public partial class ProductCreate : Page
    {
        public ProductCreate()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Creates and returns the Product based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Product createProductObject()
        {
            var product = new Product();
            product.ProductName = textBox_ProductName.Text;
            product.Category = textBox_ProductCategory.Text;
            product.ProductDescription = textBox_ProductDescription.Text;
            product.Stock = int.Parse(txtBlock_ProductCurrentStock.Text);
            product.MinStock = int.Parse(txtBlock_ProductMinimun_Stock.Text);
            product.SellPrice = double.Parse(txtBlock_ProductSellPrice.Text);
            product.Cost = double.Parse(txtBlock_ProductCost.Text);
            product.SellPrice = double.Parse(txtBlock_ProductSellPrice.Text);
            product.Vat = float.Parse(txtBlock_ProductVat.Text) / 100;
            return product;
        }

        /// <summary>
        ///     After validating creates the product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_createProduct_Click(object sender, RoutedEventArgs e)
        {
            var ProductCreateOK = true;
            if (string.IsNullOrWhiteSpace(textBox_ProductName.Text))
            {
                textBox_ProductName.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_ProductCategory.Text))
            {
                textBox_ProductCategory.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_ProductDescription.Text))
            {
                textBox_ProductDescription.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!int.TryParse(txtBlock_ProductCurrentStock.Text, out var a) || a < 0)
            {
                txtBlock_ProductCurrentStock.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!int.TryParse(txtBlock_ProductMinimun_Stock.Text, out a) || a < 0)
            {
                txtBlock_ProductMinimun_Stock.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!double.TryParse(txtBlock_ProductSellPrice.Text, out var b) || b < 0)
            {
                txtBlock_ProductSellPrice.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!double.TryParse(txtBlock_ProductCost.Text, out b) || b < 0)
            {
                txtBlock_ProductCost.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (!float.TryParse(txtBlock_ProductVat.Text, out var g))
            {
                txtBlock_ProductVat.BorderBrush = Brushes.Red;
                ProductCreateOK = false;
            }

            if (float.TryParse(txtBlock_ProductVat.Text, out var k))
            {
                var vat = float.Parse(txtBlock_ProductVat.Text);
                if (vat < 0 || vat > 100)
                {
                    MessageBox.Show("VAT is not in range 0-100");
                    txtBlock_ProductVat.BorderBrush = Brushes.Red;
                    ProductCreateOK = false;
                }
            }

            if (ProductCreateOK)
            {
                var pro = createProductObject();
                ProductViewModel.insertProduct(pro);
                MessageBox.Show("Product " + pro.ProductName + " was created");
                Btn_clearProduct_Click(null, null);
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Name.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductName.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Category.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductCategory_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductCategory.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Description.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ProductDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ProductDescription.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Stock.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_ProductCurrentStock_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductCurrentStock.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Minimum Stock.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_ProductMinimun_Stock_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductMinimun_Stock.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Sell Price.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_ProductSellPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductSellPrice.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product Cost.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_ProductCost_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductCost.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox containing the Product VAT.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBlock_ProductVat_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBlock_ProductVat.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Clear all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            textBox_ProductName.ClearValue(Control.BorderBrushProperty);
            textBox_ProductCategory.ClearValue(Control.BorderBrushProperty);
            textBox_ProductDescription.ClearValue(Control.BorderBrushProperty);
            txtBlock_ProductCurrentStock.ClearValue(Control.BorderBrushProperty);
            txtBlock_ProductMinimun_Stock.ClearValue(Control.BorderBrushProperty);
            txtBlock_ProductSellPrice.ClearValue(Control.BorderBrushProperty);
            txtBlock_ProductCost.ClearValue(Control.BorderBrushProperty);
            txtBlock_ProductVat.ClearValue(Control.BorderBrushProperty);

            textBox_ProductName.Clear();
            textBox_ProductCategory.Clear();
            textBox_ProductDescription.Clear();
            txtBlock_ProductCurrentStock.Clear();
            txtBlock_ProductMinimun_Stock.Clear();
            txtBlock_ProductSellPrice.Clear();
            txtBlock_ProductCost.Clear();
            txtBlock_ProductVat.Clear();
        }
    }
}