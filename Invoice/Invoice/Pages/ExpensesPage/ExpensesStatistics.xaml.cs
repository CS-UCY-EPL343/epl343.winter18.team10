﻿// /*****************************************************************************
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

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    ///     Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ExpensesStatistics : Page
    {
        private ExpensesViewModel expViewModel;
        ChartValues<float> totalSales = new ChartValues<float>();

        ChartValues<float> totalSalesLastYear = new ChartValues<float>();

        public ExpensesStatistics()
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
            expViewModel = new ExpensesViewModel();
            companyComboBox.ItemsSource = expViewModel.expensesCompaniesList;

            categoryComboBox.ItemsSource = expViewModel.expensesCategoriesList;


        }

        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            float totalSalesCounter = 0;
            float totalCompanyExpenses = 0;

            var moment = DateTime.Now;
            totalSales.Clear();
            totalSalesLastYear.Clear();
            if (!(companyComboBox.SelectedIndex==-1))
            {

                for (var i = 1; i <= 12; i++)
                {
                    float temp = ExpensesViewModel.getExpensesMonthYearbyCompany(companyComboBox.Text, i, moment.Year);
                    totalSales.Add(temp);
                    totalSalesLastYear.Add(ExpensesViewModel.getExpensesMonthYearbyCompany(companyComboBox.Text, i, moment.Year - 1));
                    totalSalesCounter += ExpensesViewModel.getTotalExpensesMonthYear(i, moment.Year);
                    totalCompanyExpenses += temp;
                }
            }
            else if (!(categoryComboBox.SelectedIndex == -1))
            {
                for (var i = 1; i <= 12; i++)
                {
                    totalSales.Add(ExpensesViewModel.getExpensesMonthYearbyCategory(categoryComboBox.Text, i, moment.Year));
                    totalSalesLastYear.Add(ExpensesViewModel.getExpensesMonthYearbyCategory(categoryComboBox.Text, i, moment.Year - 1));
                    totalSalesCounter += ExpensesViewModel.getTotalExpensesMonthYear(i, moment.Year);
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
                }
            };
            ExpensesCount.Text = totalSalesCounter.ToString();
            ExpensesCount2.Text = totalCompanyExpenses.ToString();
            YFormatter = value => value.ToString("C");
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