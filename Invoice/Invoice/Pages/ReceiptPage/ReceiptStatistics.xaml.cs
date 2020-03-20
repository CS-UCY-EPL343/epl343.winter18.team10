using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ReceiptStatistics : Page
    {
        ProductViewModel prodViewModel;

        public ReceiptStatistics()
        {
            InitializeComponent();
            load();
        }

        public void load()
        {
            prodViewModel = new ProductViewModel();
        }



        private void BtnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ChartValues<float> totalReceipts = new ChartValues<float>();
            ChartValues<float> totalReceiptsLastYear = new ChartValues<float>();
            
            System.DateTime moment = DateTime.Now;

            for (int i = 1; i <= 12; i++)
            {
                totalReceipts.Add(ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year));
                totalReceiptsLastYear.Add(ReceiptViewModel.getTotalReceiptsMonthYear(i, moment.Year - 1));

            }


            SeriesCollection = new SeriesCollection
            {
              
                new LiveCharts.Wpf.LineSeries
                {
                    Title = "Total Receipts",
                    Values = totalReceipts
                },
                new LiveCharts.Wpf.LineSeries
                {
                    Title = "Last Year Receipts",
                    Values = totalReceiptsLastYear
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
