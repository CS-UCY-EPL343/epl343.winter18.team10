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

namespace InvoiceX.Pages.InvoicePage
{
    /// <summary>
    ///     Interaction logic for InvoiceMain.xaml
    /// </summary>
    public partial class InvoiceMain : Page
    {
        private readonly InvoiceCreate createpage;
        private readonly InvoiceEdit editPage;
        private readonly InvoiceStatistics invoiceStatistics;
        private readonly InvoiceViewAll viewAllPage;
        private readonly InvoiceView viewPage;

        public InvoiceMain()
        {
            InitializeComponent();
            viewAllPage = new InvoiceViewAll(this);
            viewPage = new InvoiceView(this);
            createpage = new InvoiceCreate(this);
            editPage = new InvoiceEdit(this);
            invoiceStatistics = new InvoiceStatistics();
            btnViewAll_Click(null, null);
        }

        /// <summary>
        ///     Switches to Invoice View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        ///     Switches to Invoice Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = invoiceStatistics;
        }

        /// <summary>
        ///     Switches to Invoice Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = createpage;
            createpage.load();
        }

        /// <summary>
        ///     Switches to Invoice View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = viewPage;
        }

        /// <summary>
        ///     Switches to Invoice Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            invoicePage.Content = editPage;
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnCreate.Style = btnStatistics.Style =
                btnViewAll.Style = FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        ///     Passes the invoice ID to the invoice view page and switches to it
        /// </summary>
        /// <param name="invID"></param>
        public void viewInvoice(int invID)
        {
            viewPage.loadInvoice(invID);
            btnView_Click(null, null);
        }

        /// <summary>
        ///     Passes the invoice ID to the invoice edit page and switches to it
        /// </summary>
        /// <param name="invID"></param>
        public void editInvoice(int invID)
        {
            editPage.loadInvoice(invID);
            btnEdit_Click(null, null);
        }

        /// <summary>
        ///     Passes the order to the invoice create page and switches to it
        /// </summary>
        /// <param name="order"></param>
        public void issueOrderAsInvoice(Order order)
        {
            createpage.loadOrder(order);
            btnCreate_Click(null, null);
        }
    }
}