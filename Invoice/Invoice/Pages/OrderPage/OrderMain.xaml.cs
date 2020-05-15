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

using System.Windows;
using System.Windows.Controls;
using InvoiceX.Models;

namespace InvoiceX.Pages.OrderPage
{
    /// <summary>
    ///     Interaction logic for OrderMain.xaml
    /// </summary>
    public partial class OrderMain : Page
    {
        private readonly OrderCreate createpage;
        private readonly OrderEdit editpage;
        private readonly MainWindow mainWindow;
        private readonly OrderViewAll viewAllPage;
        private readonly OrderView viewPage;

        public OrderMain(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            viewAllPage = new OrderViewAll(this);
            viewPage = new OrderView(this);
            createpage = new OrderCreate(this);
            editpage = new OrderEdit(this);
            btnViewAll_Click(null, null);
        }

        /// <summary>
        ///     Switches to Order View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        ///     Switches to Order View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = viewPage;
        }

        /// <summary>
        ///     Switches to Order Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = editpage;
            editpage.load();
        }

        /// <summary>
        ///     Switches to Order Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            orderPage.Content = createpage;
            createpage.load();
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style =
                btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        ///     Passes the order ID to the order view page and switches to it
        /// </summary>
        /// <param name="orderID"></param>
        public void viewOrder(int orderID)
        {
            viewPage.loadOrder(orderID);
            btnView_Click(null, null);
        }

        /// <summary>
        ///     Passes the order ID to the order edit page and switches to it
        /// </summary>
        /// <param name="orderID"></param>
        public void editOrder(int orderID)
        {
            editpage.loadOrder(orderID);
            btnEdit_Click(null, null);
        }

        /// <summary>
        ///     Calls the issue order as invoice method of main window
        /// </summary>
        /// <param name="order"></param>
        public void issueOrderAsInvoice(Order order)
        {
            mainWindow.issueOrderAsInvoice(order);
        }
    }
}