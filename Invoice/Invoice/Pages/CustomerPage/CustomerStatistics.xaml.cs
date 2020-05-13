/*****************************************************************************
 * MIT License
 *
 * Copyright (c) 2020 InvoiceX
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class CustomerStatistics : Page
    {
        CustomerViewModel custViewModel;

        public CustomerStatistics()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            custViewModel = new CustomerViewModel();
            customerComboBox.ItemsSource = custViewModel.customersList;
            customerComboBox.DisplayMemberPath = "CustomerName";
            customerComboBox.SelectedValuePath = "CustomerName";
        }

        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ChartValues<float> totalSales = new ChartValues<float>();

            ChartValues<float> totalSalesLastYear = new ChartValues<float>();

            Customer selectedCustomer= (Customer)customerComboBox.SelectedItem;
            System.DateTime moment = DateTime.Now;


            for (int i= 1; i <= 12; i++)
            {
                totalSales.Add(CustomerViewModel.getTotalSalesMonthYear(selectedCustomer.idCustomer, i,moment.Year));
                totalSalesLastYear.Add(CustomerViewModel.getTotalSalesMonthYear(selectedCustomer.idCustomer, i, moment.Year-1));

            }
            SeriesCollection = new SeriesCollection
            {
              
                new LiveCharts.Wpf.LineSeries
                {
                    Title = "Total Sales",
                    Values = totalSales
                },
                new LiveCharts.Wpf.LineSeries
                {
                    Title = "Last Year Sales",
                    Values = totalSalesLastYear
                },
            };

            
            YFormatter = value => value.ToString("C");
            DataContext = this;



        }
        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cmbBoxBy_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> YFormatter2 { get; set; }

    }
}
