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

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    ///     Interaction logic for ExpensesMain.xaml
    /// </summary>
    public partial class ExpensesMain : Page
    {
        private readonly ExpensesCreate createPage;
        private readonly ExpensesEdit editPage;
        private readonly ExpensesViewAll viewAllPage;

        private readonly ExpensesView viewPage;
        // statisticsPage;

        public ExpensesMain()
        {
            InitializeComponent();
            viewAllPage = new ExpensesViewAll(this);
            viewPage = new ExpensesView();
            createPage = new ExpensesCreate(this);
            editPage = new ExpensesEdit(this);
            //statisticsPage = new ();
            btnViewAll_Click(null, null);
        }

        /// <summary>
        ///     Switches to Expenses View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        ///     Switches to Expenses Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = createPage;
            createPage.load();
        }

        /// <summary>
        ///     Switches to Expenses Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            //expensesPage.Content = statisticsPage;            
        }

        /// <summary>
        ///     Switches to Expenses View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = viewPage;
        }

        /// <summary>
        ///     Switches to Expenses Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            expensesPage.Content = editPage;
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
        ///     Passes the expense ID to the expense view page and switches to it
        /// </summary>
        /// <param name="expID"></param>
        public void viewExpense(int expID)
        {
            viewPage.loadExpense(expID);
            btnView_Click(null, null);
        }
    }
}