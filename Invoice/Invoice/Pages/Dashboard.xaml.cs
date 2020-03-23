using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InvoiceX.Forms;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace InvoiceX.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            createChart1();
            createChart2();
            invoicesCount.Text = ViewModels.InvoiceViewModel.get30DaysTotalInvoices();
            totalSales.Text = "€"+ ViewModels.InvoiceViewModel.get30DaysTotalSales();
            loadOrderTable();

        }

        public void loadOrderTable() {
            OrderViewModel orderViewModel = new OrderViewModel();
            //find the pending orders
            List<Order> list = orderViewModel.orderList;
            List<Order> list2 = new List<Order>();

            for (int i=0; i < list.Count; i++)
            {
                if ((int)(list.ElementAt(i).status)==2)
                {
                    list2.Add(list.ElementAt(i));
                }
            }

            var _itemSourceList = new CollectionViewSource() { Source = list2 };
            System.ComponentModel.ICollectionView Itemlist = _itemSourceList.View;

            orderDataGrid.ItemsSource = Itemlist;

        }

        void createChart1()
        {
            double[] receipts = new double[12];
            for (int i=0; i<12; i++)
            {
                receipts[i] = ViewModels.ReceiptViewModel.getTotalReceiptsMonthYear(i, DateTime.Now.Year);
            }
            ChartValues<double> totalReceipt = new ChartValues<double>();
            totalReceipt.AddRange(receipts);

            double[] invoices = new double[12];
            for (int i = 0; i < 12; i++)
            {
                invoices[i] = ViewModels.InvoiceViewModel.getTotalSalesMonthYear(i, DateTime.Now.Year);
            }

            ChartValues<double> total = new ChartValues<double>();
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
                },
            };

            YFormatter = value => value.ToString("C");
            DataContext = this;

        }
        void createChart2()
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var lastMonth = month.AddMonths(-4);
            double[] invoices = new double[4];
            double[] receipts = new double[4];
            Labels2 = new string[4];

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(lastMonth);
                receipts[i] = ViewModels.ReceiptViewModel.getTotalReceiptsMonthYear(lastMonth.Month, lastMonth.Year);
                invoices[i] = ViewModels.InvoiceViewModel.getTotalSalesMonthYear(lastMonth.Month, lastMonth.Year);
                Labels2[i] = lastMonth.Month.ToString();
                lastMonth = lastMonth.AddMonths(1);
            }
            ChartValues<double> totalReceipt = new ChartValues<double>();
            totalReceipt.AddRange(receipts);

            ChartValues<double> total = new ChartValues<double>();
            total.AddRange(invoices);

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
                    Values = new ChartValues<double> { 10, 2, 6, 10 }
                }

            };

            Formatter = value => value.ToString("N");

            DataContext = this;
        }
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }

        public string[] Labels { get; set; }
        public string[] Labels2 { get; set; }

        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> Formatter { get; set; }

        public void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

