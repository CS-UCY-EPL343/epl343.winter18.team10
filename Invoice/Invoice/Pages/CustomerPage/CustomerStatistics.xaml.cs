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
