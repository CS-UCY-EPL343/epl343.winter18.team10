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
using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductStatistics : Page
    {
        private ProductViewModel prodViewModel;

        public ProductStatistics()
        {
            InitializeComponent();
            load();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> YFormatter2 { get; set; }

        public void load()
        {
            prodViewModel = new ProductViewModel();
            productComboBox.ItemsSource = prodViewModel.productList;
            productComboBox.DisplayMemberPath = "ProductName";
            productComboBox.SelectedValuePath = "ProductName";
        }


        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            var total = new ChartValues<int>();
            var totalSales = new ChartValues<float>();

            var totalLastYear = new ChartValues<int>();
            var totalSalesLastYear = new ChartValues<float>();

            var selectedProduct = (Product) productComboBox.SelectedItem;
            var moment = DateTime.Now;


            for (var i = 1; i <= 12; i++)
            {
                total.Add(ProductViewModel.getProductCount(selectedProduct.idProduct, i, moment.Year));
                totalSales.Add(ProductViewModel.getProductSales(selectedProduct.idProduct, i, moment.Year));
                Console.WriteLine(moment.Year);
                totalLastYear.Add(ProductViewModel.getProductCount(selectedProduct.idProduct, i, moment.Year - 1));
                totalSalesLastYear.Add(ProductViewModel.getProductSales(selectedProduct.idProduct, i, moment.Year - 1));
            }

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total Sales",
                    Values = totalSales
                },
                new ColumnSeries
                {
                    Title = "Last Year Sales",
                    Values = totalSalesLastYear
                }
            };
            SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total Products",
                    Values = total
                },
                new ColumnSeries
                {
                    Title = "Last Year Total Products",
                    Values = totalLastYear
                }
            };
            YFormatter = value => value.ToString("C");
            YFormatter2 = value => value.ToString();

            DataContext = this;
        }

        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void cmbBoxBy_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}