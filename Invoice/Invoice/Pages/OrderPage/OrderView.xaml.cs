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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InvoiceX.Forms;
using InvoiceX.Models;
using InvoiceX.ViewModels;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    ///     Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : Page
    {
        private Order order;
        private OrderMain orderMain;

        public OrderView(OrderMain orderMain)
        {
            this.orderMain = orderMain;

            InitializeComponent();
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_orderNumber.Focus();
        }

        /// <summary>
        ///     After validating the order ID calls loadOrder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_loadOrder_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out var orderID);
            if (orderID > 0)
                loadOrder(orderID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for order ID.");
        }

        /// <summary>
        ///     Loads the oder information on the page
        /// </summary>
        /// <param name="orderID"></param>
        public void loadOrder(int orderID)
        {
            order = OrderViewModel.getOrder(orderID);
            if (order != null)
            {
                // Customer details
                textBox_Customer.Text = order.customer.CustomerName;
                textBox_Contact_Details.Text = order.customer.PhoneNumber.ToString();
                textBox_Email_Address.Text = order.customer.Email;
                textBox_Address.Text =
                    order.customer.Address + ", " + order.customer.City + ", " + order.customer.Country;

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

        /// <summary>
        ///     Method that handles the event Key Down on the textbox orderNumber.
        ///     If the key pressed is Enter then it loads the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_orderNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) btn_loadOrder_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            order = null;
            foreach (var ctrl in grid_Customer.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Order.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            orderProductsGrid.ItemsSource = null;
            NetTotal_TextBlock.Text = 0.ToString("C");
            Vat_TextBlock.Text = 0.ToString("C");
            TotalAmount_TextBlock.Text = 0.ToString("C");
            txtBox_orderNumber.IsReadOnly = false;
            txtBox_orderNumber.Focus();
        }

        /// <summary>
        ///     Switches to edit Order page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (order != null)
            {
                orderMain.editOrder(order.idOrder);
            }
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_orderNumber.Text, out var orderID);
            if (txtBox_orderNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the order with ID = " + orderID + ". Are you sure?";
                var txt = "Delete Order";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

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

        /// <summary>
        ///     Saves the order as a pdf file in the predetermined location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void savePdf_Click(object sender, RoutedEventArgs e)
        {
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();
                Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") +
                                          "/Documents/InvoiceX/Orders/");

                // Save the PDF document...
                var filename = Environment.GetEnvironmentVariable("USERPROFILE") + "/Documents/InvoiceX/Orders/Order" +
                               txtBox_orderNumber.Text + ".pdf";
                ;

                pdfRenderer.Save(filename);
                Process.Start(filename);
            }
        }

        /// <summary>
        ///     Opens the Adobe Acrobat Reader print prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printPdf_click(object sender, RoutedEventArgs e)
        {
            //Create and save the pdf
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                var filename = "Order.pdf";
                pdfRenderer.Save(filename);
                //open adobe acrobat
                var proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                    @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = string.Format(@"/p {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false) proc.WaitForExit(10000);

                proc.EnableRaisingEvents = true;

                proc.Close();
            }
        }

        /// <summary>
        ///     Preview the order as a pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPdf_click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("Order_temp.pdf")) File.Delete("Order_temp.pdf");
            if (order == null)
            {
                MessageBox.Show("Order is not loaded!");
            }
            else
            {
                var document = createPdf();
                document.UseCmykColor = true;
                // Create a renderer for PDF that uses Unicode font encoding
                var pdfRenderer = new PdfDocumentRenderer(true);

                // Set the MigraDoc document
                pdfRenderer.Document = document;

                // Create the PDF document
                pdfRenderer.RenderDocument();

                // Save the PDF document...
                var filename = "Order_temp.pdf";
                pdfRenderer.Save(filename);

                //open adobe acrobat
                var proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "print";

                //Define location of adobe reader/command line
                //switches to launch adobe in "print" mode
                proc.StartInfo.FileName =
                    @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
                proc.StartInfo.Arguments = string.Format(@" {0}", filename);
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (proc.HasExited == false) proc.WaitForExit(10000);

                proc.EnableRaisingEvents = true;

                proc.Close();
            }
        }

        /// <summary>
        ///     Create the pdf based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Document createPdf()
        {
            var orderDetails = new string[3];
            var customerDetails = new string[2];

            var customer = order.customer;
            customerDetails[1] = customer.CustomerName;
            customerDetails[0] = customer.idCustomer.ToString();

            var products = orderProductsGrid.Items.OfType<Product>().ToList();

            orderDetails[0] = txtBox_orderNumber.Text;
            orderDetails[1] = txtBox_orderDate.Text;
            orderDetails[2] = txtBox_shippingDate.Text;


            var order1 = new OrderForm("../../Forms/Order.xml", customerDetails, orderDetails, products);
            var document = order1.CreateDocument();
            return document;
        }
    }
}