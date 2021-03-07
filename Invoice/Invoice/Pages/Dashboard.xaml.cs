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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;

namespace InvoiceX.Pages
{
    /// <summary>
    ///     Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            Labels = new[] {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

            createChart1();
            createChart2();
            invoicesCount.Text = InvoiceViewModel.get30DaysTotalInvoices();
            totalSales.Text = "€" + InvoiceViewModel.get30DaysTotalSales();
            loadOrderTable();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public string[] Labels { get; set; }
        public string[] Labels2 { get; set; }

        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> Formatter { get; set; }

        public void loadOrderTable()
        {
            var orderViewModel = new OrderViewModel();
            //find the pending orders
            var list = orderViewModel.orderList;
            var list2 = new List<Order>();

            for (var i = 0; i < list.Count; i++)
                if ((int) list.ElementAt(i).status == 2)
                    list2.Add(list.ElementAt(i));

            var _itemSourceList = new CollectionViewSource {Source = list2};
            var Itemlist = _itemSourceList.View;

            orderDataGrid.ItemsSource = Itemlist;
        }

        private void createChart1()
        {
            float totalYearlySales = 0;
            double totalYearlyReceipts = 0;

            var receipts = new double[12];
            for (var i = 0; i < 12; i++) {
                double temp1 = ReceiptViewModel.getTotalReceiptsMonthYear(i + 1, DateTime.Now.Year) + InvoiceViewModel.getPaidInvoicesbyMonthYear(i + 1, DateTime.Now.Year);
                receipts[i] = temp1;
                totalYearlyReceipts += temp1;
            }
            var totalReceipt = new ChartValues<double>();
            totalReceipt.AddRange(receipts);

            var invoices = new double[12];
            for (var i = 0; i < 12; i++)
            {
                invoices[i] = InvoiceViewModel.getTotalSalesMonthYear(i + 1, DateTime.Now.Year);
                totalYearlySales+= InvoiceViewModel.getTotalSalesMonthYear(i + 1, DateTime.Now.Year);
            }
            var total = new ChartValues<double>();
            total.AddRange(invoices);
            SeriesCollection = new SeriesCollection

            {
                new LineSeries
                {
                    Title = "Sales",
                    Values = total
                },
                new LineSeries
                {
                    Title = "Receipts",
                    Values = totalReceipt,
                    PointGeometry = null
                }
            };
            salesCountYearly.Text = totalYearlySales.ToString();
            receiptsCountYearly.Text = totalYearlyReceipts.ToString("C");
            YFormatter = value => value.ToString("C");
            DataContext = this;
        }

        private void createChart2()
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var lastMonth = month.AddMonths(-4);
            var invoices = new double[4];
            var receipts = new double[4];
            var expenses = new double[4];

            Labels2 = new string[4];

            for (var i = 0; i < 4; i++)
            {
                receipts[i] = ReceiptViewModel.getTotalReceiptsMonthYear(lastMonth.Month, lastMonth.Year) + InvoiceViewModel.getPaidInvoicesbyMonthYear(lastMonth.Month, lastMonth.Year); ;
                invoices[i] = InvoiceViewModel.getTotalSalesMonthYear(lastMonth.Month, lastMonth.Year);
                expenses[i] = ExpensesViewModel.getTotalExpensesMonthYear(lastMonth.Month, lastMonth.Year);
                Labels2[i] = lastMonth.Month.ToString();
                lastMonth = lastMonth.AddMonths(1);
            }

            var totalReceipt = new ChartValues<double>();
            totalReceipt.AddRange(receipts);

            var total = new ChartValues<double>();
            total.AddRange(invoices);

            var totalExpenses = new ChartValues<double>();
            totalExpenses.AddRange(expenses);

            SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Sales",
                    Values = total
                },
                new ColumnSeries
                {
                    Title = "Receipts",
                    Values = totalReceipt
                },
                new ColumnSeries
                {
                    Title = "Expenses",
                    Values = totalExpenses
                }
            };

            Formatter = value => value.ToString("N");

            DataContext = this;
        }

        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}