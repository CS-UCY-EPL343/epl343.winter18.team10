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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ReceiptMain.xaml
    /// </summary>
    public partial class ReceiptMain : Page
    {
        private readonly ReceiptCreate createPage;
        private readonly ReceiptEdit editPage;
        private readonly ReceiptStatistics statisticsPage;
        private readonly ReceiptViewAll viewAllPage;
        private readonly ReceiptView viewPage;

        public ReceiptMain()
        {
            InitializeComponent();
            viewAllPage = new ReceiptViewAll(this);
            viewPage = new ReceiptView(this);
            createPage = new ReceiptCreate(this);
            editPage = new ReceiptEdit(this);
            statisticsPage = new ReceiptStatistics();
            btnViewAll_Click(null, null);
        }

        /// <summary>
        ///     Switches to Quote View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        ///     Switches to Quote Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = createPage;
            createPage.load();
        }

        /// <summary>
        ///     Switches to Quote Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = statisticsPage;
        }

        /// <summary>
        ///     Switches to Quote View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = viewPage;
        }

        /// <summary>
        ///     Switches to Quote Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            receiptPage.Content = editPage;
            editPage.load();
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
        ///     Passes the receipt ID to the receipt view page and switches to it
        /// </summary>
        /// <param name="recID"></param>
        public void viewReceipt(int recID)
        {
            viewPage.loadReceipt(recID);
            btnView_Click(null, null);
        }

        /// <summary>
        ///     Passes the receipt ID to the receipt edit page and switches to it
        /// </summary>
        /// <param name="recID"></param>
        public void editReceipt(int recID)
        {
            editPage.loadReceipt(recID);
            btnEdit_Click(null, null);
        }
    }
}