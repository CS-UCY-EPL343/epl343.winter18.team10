using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductStatistics : Page
    {
        ProductViewModel prodViewModel;

        public ProductStatistics()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            prodViewModel = new ProductViewModel();
            productComboBox.ItemsSource = prodViewModel.productList;
            productComboBox.DisplayMemberPath = "ProductName";
            productComboBox.SelectedValuePath = "ProductName";
        }



        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ChartValues<int> total = new ChartValues<int>();
            ChartValues<float> totalSales = new ChartValues<float>();

            ChartValues<int> totalLastYear = new ChartValues<int>();
            ChartValues<float> totalSalesLastYear = new ChartValues<float>();

            Product selectedProduct = (Product)productComboBox.SelectedItem;
            System.DateTime moment = DateTime.Now;


            for (int i= 1; i <= 12; i++)
            {
                total.Add(ProductViewModel.getProductCount(selectedProduct.idProduct, i,moment.Year));
                totalSales.Add(ProductViewModel.getProductSales(selectedProduct.idProduct, i,moment.Year));
                Console.WriteLine(moment.Year);
                totalLastYear.Add(ProductViewModel.getProductCount(selectedProduct.idProduct, i, moment.Year-1));
                totalSalesLastYear.Add(ProductViewModel.getProductSales(selectedProduct.idProduct, i, moment.Year-1));

            }
            SeriesCollection = new SeriesCollection
            {
              
                new LiveCharts.Wpf.ColumnSeries
                {
                    Title = "Total Sales",
                    Values = totalSales
                },
                new LiveCharts.Wpf.ColumnSeries
                {
                    Title = "Last Year Sales",
                    Values = totalSalesLastYear
                },
            };
            SeriesCollection2 = new SeriesCollection
            {
                 new LiveCharts.Wpf.ColumnSeries
                {
                    Title = "Total Products",
                    Values =total
                },
                new LiveCharts.Wpf.ColumnSeries
                {
                    Title = "Last Year Total Products",
                    Values =totalLastYear
                },


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
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> YFormatter2 { get; set; }

    }
}
