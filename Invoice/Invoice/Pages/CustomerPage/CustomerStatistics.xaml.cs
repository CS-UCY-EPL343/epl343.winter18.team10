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

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class CustomerStatistics : Page
    {
        private CustomerViewModel custViewModel;
        ChartValues<float> totalSales = new ChartValues<float>();

        ChartValues<float> totalSalesLastYear = new ChartValues<float>();

        public CustomerStatistics()
        {
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

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
            custViewModel = new CustomerViewModel();
            customerComboBox.ItemsSource = custViewModel.customersList;
            customerComboBox.DisplayMemberPath = "CustomerName";
            customerComboBox.SelectedValuePath = "CustomerName";
        }

        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            float totalSalesCounter = 0;
            var selectedCustomer = (Customer) customerComboBox.SelectedItem;
            var moment = DateTime.Now;
            totalSales.Clear();
            totalSalesLastYear.Clear();
            for (var i = 1; i <= 12; i++)
            {
                totalSales.Add(CustomerViewModel.getTotalSalesMonthYear(selectedCustomer.idCustomer, i, moment.Year));
                totalSalesCounter += CustomerViewModel.getTotalSalesMonthYear(selectedCustomer.idCustomer, i, moment.Year);
                totalSalesLastYear.Add(
                CustomerViewModel.getTotalSalesMonthYear(selectedCustomer.idCustomer, i, moment.Year - 1));
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Sales",
                    Values = totalSales
                },
                new LineSeries
                {
                    Title = "Last Year Sales",
                    Values = totalSalesLastYear
                }
            };
            salesCount.Text = totalSalesCounter.ToString();
            YFormatter = value => value.ToString("C");
            DataContext = this;

        }


        private void BtnSelectCategory_Click(object sender, RoutedEventArgs e)
        {
            float totalSalesCounter = 0;
            float totalSalesCounterLastYear = 0;

            string selectedCategory = categoryComboBox.Text;
            int comparisonYear = int.Parse(categorycmbBoxLast.Text);
            string categoryBy = categorycmbBoxBy.Text;

            var moment = DateTime.Now;
            totalSales.Clear();
            totalSalesLastYear.Clear();

            if (categoryBy == "Total")
            {
                string[,] tempAmount = CustomerViewModel.getTotalSalesByCategory(moment.Year);
                string[,] tempAmountLastYear = CustomerViewModel.getTotalSalesByCategory(comparisonYear);

                Labels = new string[tempAmount.GetLength(0)];
                for (int i = 0; i < tempAmount.GetLength(0); i++)
                {
                    totalSales.Add(float.Parse(tempAmount[i, 0]));
                    totalSalesLastYear.Add(float.Parse(tempAmountLastYear[i, 0]));

                    Labels[i] = (tempAmount[i, 1]);
                }

                SeriesCollection = new SeriesCollection
            {
               new ColumnSeries
                {
                    Title = "Sales",
                    Values = totalSales
                },
                 new ColumnSeries
                {
                    Title = "Sales Last Year",
                    Values = totalSalesLastYear
                }
            };
                salesCount.Text = totalSalesCounter.ToString();
                YFormatter = value => value.ToString("C");
                DataContext = this;
            }
            else if(categoryBy == "Month")
            {
                for (int j = 1; j <= 12; j++)
                {

                    float salesYearTemp = CustomerViewModel.getTotalSalesByCategoryAndMonth(moment.Year, j, selectedCategory);
                    float salesLastYearTemp = CustomerViewModel.getTotalSalesByCategoryAndMonth(comparisonYear, j, selectedCategory);
                    totalSales.Add(salesYearTemp);
                    totalSalesLastYear.Add(salesLastYearTemp);
                    totalSalesCounter += salesYearTemp;
                    totalSalesCounterLastYear += salesLastYearTemp;
                    SeriesCollection = new SeriesCollection
            {
               new ColumnSeries
                {
                    Title = "Sales",
                    Values = totalSales
                },
                 new ColumnSeries
                {
                    Title = "Sales Last Year",
                    Values = totalSalesLastYear
                }
            };
                }
                salesCount.Text = totalSalesCounter.ToString();
                salesCountLastYear.Text = totalSalesCounterLastYear.ToString();

                
                YFormatter = value => value.ToString("C");
                DataContext = this;
            }

        }
        private void BtnSelectCity_Click(object sender, RoutedEventArgs e)
        {
            float totalSalesCounter = 0;
            float totalSalesCounterLastYear = 0;

            string selectedCity = cityComboBox.Text;
            int comparisonYear = int.Parse(categorycmbBoxLast_Copy.Text);
            string categoryBy = categorycmbBoxBy_Copy.Text;

            var moment = DateTime.Now;
            totalSales.Clear();
            totalSalesLastYear.Clear();

            if (categoryBy == "Total")
            {
                string[,] tempAmount = CustomerViewModel.getTotalSalesByCity(moment.Year);
                string[,] tempAmountLastYear = CustomerViewModel.getTotalSalesByCity(comparisonYear);

                Labels = new string[tempAmount.GetLength(0)];
                for (int i = 0; i < tempAmount.GetLength(0); i++)
                {
                    totalSales.Add(float.Parse(tempAmount[i, 0]));
                    totalSalesLastYear.Add(float.Parse(tempAmountLastYear[i, 0]));

                    Labels[i] = (tempAmount[i, 1]);
                }

                SeriesCollection = new SeriesCollection
            {
               new ColumnSeries
                {
                    Title = "Sales",
                    Values = totalSales
                },
                 new ColumnSeries
                {
                    Title = "Sales Last Year",
                    Values = totalSalesLastYear
                }
            };
                salesCount.Text = totalSalesCounter.ToString();
                YFormatter = value => value.ToString("C");
                DataContext = this;
            }
            else if (categoryBy == "Month")
            {
                for (int j = 1; j <= 12; j++)
                {

                    float salesYearTemp = CustomerViewModel.getTotalSalesByCityAndMonth(moment.Year, j, selectedCity);
                    float salesLastYearTemp = CustomerViewModel.getTotalSalesByCityAndMonth(comparisonYear, j, selectedCity);
                    totalSales.Add(salesYearTemp);
                    totalSalesLastYear.Add(salesLastYearTemp);
                    totalSalesCounter += salesYearTemp;
                    totalSalesCounterLastYear += salesLastYearTemp;
                    SeriesCollection = new SeriesCollection
            {
               new ColumnSeries
                {
                    Title = "Sales",
                    Values = totalSales
                },
                 new ColumnSeries
                {
                    Title = "Sales Last Year",
                    Values = totalSalesLastYear
                }
            };
                }
                salesCount.Text = totalSalesCounter.ToString();
                salesCountLastYear.Text = totalSalesCounterLastYear.ToString();


                YFormatter = value => value.ToString("C");
                DataContext = this;
            }

        }
        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void cmbBoxBy_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
    }