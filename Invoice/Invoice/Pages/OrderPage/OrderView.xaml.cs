using InvoiceX.Models;
using InvoiceX.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        private Order order;
        OrderMain orderMain;

        public OrderView(OrderMain orderMain)
        {
            this.orderMain = orderMain;

            InitializeComponent();
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_orderNumber.Focus();
        }

        private void btn_loadOrder_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out int orderID);
            if (orderID > 0)
            {
                loadOrder(orderID);
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for order ID.");
            }
        }

        public void loadOrder(int orderID)
        {
            order = OrderViewModel.getOrder(orderID);
            if (order != null)
            {
                // Customer details
                textBox_Customer.Text = order.customer.CustomerName;
                textBox_Contact_Details.Text = order.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = order.customer.Email;
                textBox_Address.Text = order.customer.Address + ", " + order.customer.City + ", " + order.customer.Country;

                // Invoice details
                txtBox_orderNumber.Text = order.idOrder.ToString();
                txtBox_orderNumber.IsReadOnly = true;
                txtBox_orderDate.Text = order.createdDate.ToString("dd/mm/yyyy");
                txtBox_shippingDate.Text = order.shippingDate.ToString("dd/mm/yyyy");
                txtBox_issuedBy.Text = order.issuedBy;
                txtBox_status.Text = order.status.ToString();
                NetTotal_TextBlock.Text = order.cost.ToString("C");
                Vat_TextBlock.Text = order.VAT.ToString("C");
                TotalAmount_TextBlock.Text = order.totalCost.ToString("C");

                // Invoice products           
                orderProductsGrid.ItemsSource = order.products;
            }
            else
            {
                MessageBox.Show("Invoice with ID = " + orderID + ", does not exist");
            }
        }

        private void txtBox_orderNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btn_loadOrder_Click(null, null);
            }
        }

        private void btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            this.order = null;
            foreach (var ctrl in grid_Customer.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            foreach (var ctrl in grid_Order.Children)
            {
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox)ctrl).Clear();
            }
            orderProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = (0).ToString("C");
            Vat_TextBlock.Text = (0).ToString("C");
            TotalAmount_TextBlock.Text = (0).ToString("C");
            txtBox_orderNumber.IsReadOnly = false;
            txtBox_orderNumber.Focus();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (order != null)
            {
                //orderMain.editOrder(order.idOrder);
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out int orderID);
            if (txtBox_orderNumber.IsReadOnly)
            {
                string msgtext = "You are about to delete the order with ID = " + orderID + ". Are you sure?";
                string txt = "Delete Order";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        OrderViewModel.deleteOrder(orderID);
                        btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Order with ID = " + orderID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No order is loaded");
            }
        }

        void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {
                MigraDoc.DocumentObjectModel.Document document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();
                System.IO.Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("USERPROFILE") + "/Documents/InvoiceX/Orders/");

                // Save the PDF document...
                string filename = System.Environment.GetEnvironmentVariable("USERPROFILE") + "/Documents/InvoiceX/Orders/Order" + txtBox_orderNumber.Text + ".pdf"; ;

                pdfRenderer.Save(filename);
                System.Diagnostics.Process.Start(filename);
            }
        }
        void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {

                MigraDoc.DocumentObjectModel.Document document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                string filename = "Order.pdf";
                pdfRenderer.Save(filename);
                //open adobe acrobat
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                  @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = String.Format(@"/p {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false)
                {
                    proc.WaitForExit(10000);
                }

                proc.EnableRaisingEvents = true;

                proc.Close();
            }

        }
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Order_temp.pdf"))
            {
                File.Delete("Order_temp.pdf");
            }
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {

                MigraDoc.DocumentObjectModel.Document document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer = new MigraDoc.Rendering.PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                string filename = "Order_temp.pdf";
                pdfRenderer.Save(filename);

                //open adobe acrobat
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                    @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = String.Format(@" {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false)
                {
                    proc.WaitForExit(10000);
                }

                proc.EnableRaisingEvents = true;

                proc.Close();

            }
        }

        MigraDoc.DocumentObjectModel.Document createPdf()
        {
            string[] orderDetails = new string[3];
            string[] customerDetails = new string[2];

            Customer customer = order.customer;
            customerDetails[1]=customer.CustomerName;
            customerDetails[0] = customer.idCustomer.ToString();

            List<Product> products = orderProductsGrid.Items.OfType<Product>().ToList();

            orderDetails[0] = txtBox_orderNumber.Text;
            orderDetails[1] = txtBox_orderDate.Text;
            orderDetails[2] = txtBox_shippingDate.Text;



            Forms.OrderForm order1 = new Forms.OrderForm("../../Forms/Order.xml", customerDetails, orderDetails, products);
            MigraDoc.DocumentObjectModel.Document document = order1.CreateDocument();
            return document;

        }
    }
}
