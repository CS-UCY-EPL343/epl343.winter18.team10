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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using InvoiceX.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class InvoiceStatistics : Page
    {
        private ProductViewModel prodViewModel;
        ChartValues<float> totalSales = new ChartValues<float>();
        ChartValues<float> totalSalesLastYear = new ChartValues<float>();
        ChartValues<float> totalSalesAverage = new ChartValues<float>();
        ChartValues<float> totalSalesAverageLY = new ChartValues<float>();

        public InvoiceStatistics()
        {

            InitializeComponent();
            load();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public SeriesCollection SeriesCollectionAverage { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> YFormatter2 { get; set; }

        public void load()
        {
            prodViewModel = new ProductViewModel();
        }


        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {

            totalSales.Clear();
            totalSalesLastYear.Clear();
            totalSalesAverage.Clear();
            totalSalesAverageLY.Clear();

            int comparisonYear = int.Parse(cmbBoxLast.Text);
            float totalSalesAmount = 0;
            float totalSalesAmountLastYear = 0;

            if (cmbBoxBy.Text == "Month")
            {
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

                var moment = DateTime.Now;

                for (var i = 0; i < 12; i++)
                {
                    var temp1 = InvoiceViewModel.getTotalSalesMonthYear(i + 1, moment.Year);
                    totalSales.Add(temp1);
                    totalSalesAmount += temp1;

                    totalSalesAverage.Add(totalSalesAmount / (i+1));


                    temp1 = InvoiceViewModel.getTotalSalesMonthYear(i + 1, comparisonYear);
                    totalSalesLastYear.Add(temp1);
                    totalSalesAmountLastYear += temp1;

                    totalSalesAverageLY.Add(totalSalesAmountLastYear / (i + 1));


                }
            } else if (cmbBoxBy.Text == "Week")
            {
                Labels = new String[52];
                for (int i = 0; i < 52; i++)
                {
                    string dateLabel= FirstDateOfWeekISO8601(DateTime.Now.Year, (i+1)).ToString()+"-"+ FirstDateOfWeekISO8601(DateTime.Now.Year, (i + 2)).ToString();
                    Labels[i] = dateLabel;
                }

                var moment = DateTime.Now;

                for (var i = 0; i < 52; i++)
                {
                    var temp1 = InvoiceViewModel.getTotalSalesWeekYear(i + 1, moment.Year);
                    totalSales.Add(temp1);
                    totalSalesAmount += temp1;

                    temp1 = InvoiceViewModel.getTotalSalesWeekYear(i + 1, comparisonYear);
                    totalSalesLastYear.Add(temp1);
                    totalSalesAmountLastYear += temp1;
                }


              
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
                },
                new LineSeries
                {
                    Title = "Yearly Average",
                    Values = totalSalesAverage
                },
                new LineSeries
                {
                    Title = "Last Year Average",
                    Values = totalSalesAverageLY
                }
            };

            YFormatter = value => value.ToString("C");
            salesCount.Text = totalSalesAmount.ToString();
            productSalesCount.Text = totalSalesAmountLastYear.ToString();
            comparisonYearText.Text = cmbBoxLast.Text;

            DataContext = this;

        }
        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void cmbBoxBy_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

    }
}