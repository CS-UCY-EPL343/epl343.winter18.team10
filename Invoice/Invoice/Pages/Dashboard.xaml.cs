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
using LiveCharts;
using LiveCharts.Wpf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace Invoice.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            createpdf_button.Click += createPdf;
            createChart1();
            createChart2();
            
        }
        void createPdf(object sender, RoutedEventArgs e)
        {
            InvoiceForm invoice = new InvoiceForm("../../invoice.xml");
            Document document = invoice.CreateDocument();
            document.UseCmykColor = true;
            // Create a renderer for PDF that uses Unicode font encoding
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Invoice.pdf";
            filename = "Invoice.pdf";
            pdfRenderer.Save(filename);
            System.Diagnostics.Process.Start(filename);
            Environment.Exit(1);

        }

        void createChart1()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Sales",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4, 10, 20, 30, 40, 50, 60,10 }
                },
                new LineSeries
                {
                    Title = "Receipts",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6,10,20,30,40,50,60,10 },
                    PointGeometry = null
                },
            };

            YFormatter = value => value.ToString("C");
            DataContext = this;

        }
        void createChart2()
        {
            SeriesCollection2 = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Sales",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                },
                new ColumnSeries
                {
                    Title = "Receipts",
                    Values = new ChartValues<double> { 10, 20, 22, 44 }
                },
                new ColumnSeries
                {
                    Title = "Expenses",
                    Values = new ChartValues<double> { 10, 2, 6, 10 }
                }

            };

            SeriesCollection2[1].Values.Add(48d);

            Labels2 = new[] { "Jun", "Jul", "Aug", "Sep" };
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

