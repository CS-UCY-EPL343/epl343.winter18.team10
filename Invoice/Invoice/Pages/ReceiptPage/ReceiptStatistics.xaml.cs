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
using InvoiceX.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ReceiptStatistics : Page
    {
        private ProductViewModel prodViewModel;
        ChartValues<float> totalReceipts = new ChartValues<float>();
        ChartValues<float> totalReceiptsLastYear = new ChartValues<float>();
        ChartValues<float> totalPaidInvoices = new ChartValues<float>();
        ChartValues<float> totalPaidInvoicesLastYear = new ChartValues<float>();
        ChartValues<float> total = new ChartValues<float>();
        ChartValues<float> totalLastYear = new ChartValues<float>();
        ChartValues<float> invoices = new ChartValues<float>();
        ChartValues<float> invoicesLastYear = new ChartValues<float>();

        public ReceiptStatistics()
        {
            InitializeComponent();
            load();
            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public SeriesCollection SeriesCollection3 { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> YFormatter2 { get; set; }

        public void load()
        {
            prodViewModel = new ProductViewModel();
        }


        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            totalReceipts.Clear();
            totalReceiptsLastYear.Clear();
            totalPaidInvoices.Clear();
            totalPaidInvoicesLastYear.Clear();
            total.Clear();
            totalLastYear.Clear();
            invoices.Clear();
            invoicesLastYear.Clear();

            var moment = DateTime.Now;
            if (cmbBoxLast.Text == "Numbers")
            {

                for (var i = 1; i <= 12; i++)
                {
                    var temp1 = ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year);
                    totalReceipts.Add(temp1);
                    var temp2 = ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year - 1);
                    totalReceiptsLastYear.Add(temp2);
                    var temp3 = InvoiceViewModel.getPaidInvoicesbyMonthYear(i, moment.Year);
                    totalPaidInvoices.Add(temp3);
                    var temp4 = InvoiceViewModel.getPaidInvoicesbyMonthYear(i, moment.Year - 1);
                    totalPaidInvoicesLastYear.Add(temp4);
                    total.Add(temp1 + temp3);
                    totalLastYear.Add(temp2 + temp4);
                    invoices.Add(InvoiceViewModel.getTotalSalesMonthYear(i, moment.Year));
                    invoicesLastYear.Add(InvoiceViewModel.getTotalSalesMonthYear(i, moment.Year - 1));
                }


                SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Receipts",
                    Values = totalReceipts
                },
                new LineSeries
                {
                    Title = "Last Year Receipts",
                    Values = totalReceiptsLastYear
                }
            };
                SeriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Paid Invoices",
                    Values = totalPaidInvoices
                },
                new LineSeries
                {
                    Title = "Last Year Paid Invoices",
                    Values = totalPaidInvoicesLastYear
                },

            };
                SeriesCollection3 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Paid Invoices",
                    Values = total
                },
                new LineSeries
                {
                    Title = "Total Last Year Paid Invoices",
                    Values = totalLastYear
                },
                new ColumnSeries
                {
                    Title = "Invoices",
                    Values = invoices
                },
                 new ColumnSeries
                {
                    Title = "Invoices Last Year",
                    Values = invoicesLastYear
                }

            };
                YFormatter = value => value.ToString("c");
            }
            if (cmbBoxLast.Text == "Percentage")
            {
                for (var i = 1; i <= 12; i++)
                {
                    var totalSales = InvoiceViewModel.getTotalSalesMonthYear(i, moment.Year);
                    var totalSalesLY = InvoiceViewModel.getTotalSalesMonthYear(i, moment.Year-1);

                    var temp1 = ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year) / totalSales;
                    totalReceipts.Add(temp1);
                    var temp2 = ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year - 1) / totalSalesLY;
                    totalReceiptsLastYear.Add(temp2);
                    var temp3 = InvoiceViewModel.getPaidInvoicesbyMonthYear(i, moment.Year) / totalSales;
                    totalPaidInvoices.Add(temp3);
                    var temp4 = InvoiceViewModel.getPaidInvoicesbyMonthYear(i, moment.Year - 1) / totalSalesLY;
                    totalPaidInvoicesLastYear.Add(temp4);
                    total.Add(temp1 + temp3);
                    totalLastYear.Add(temp2 + temp4);
                }


                SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Receipts",
                    Values = totalReceipts
                },
                new LineSeries
                {
                    Title = "Last Year Receipts",
                    Values = totalReceiptsLastYear
                }
            };
                SeriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Paid Invoices",
                    Values = totalPaidInvoices
                },
                new LineSeries
                {
                    Title = "Last Year Paid Invoices",
                    Values = totalPaidInvoicesLastYear
                },

            };
                SeriesCollection3 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total Paid Invoices",
                    Values = total
                },
                new LineSeries
                {
                    Title = "Total Last Year Paid Invoices",
                    Values = totalLastYear
                },
               

            };
                YFormatter = value => value.ToString("P");

            }

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