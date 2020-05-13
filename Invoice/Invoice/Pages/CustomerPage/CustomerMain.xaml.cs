/*****************************************************************************
 * MIT License
 *
 * Copyright (c) 2020 InvoiceX
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 *****************************************************************************/

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InvoiceX.Pages.CustomerPage
{
    /// <summary>
    /// Interaction logic for CustomerMain.xaml
    /// </summary>
    public partial class CustomerMain : Page
    {
        CustomerView viewPage;
        CustomerCreate createPage;
        CustomerEdit editPage;
        CustomerStatistics statisticsPage;

        public CustomerMain()
        {
            InitializeComponent();
            viewPage = new CustomerView(this);
            createPage = new CustomerCreate();
            editPage = new CustomerEdit();
            statisticsPage = new CustomerStatistics();
            btnView_Click(null, null);
        }

        /// <summary>
        /// Switches to Customer View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = viewPage;
            viewPage.load();
        }

        /// <summary>
        /// Switches to Customer Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = createPage;
            
        }

        /// <summary>
        /// Switches to Customer Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = statisticsPage;
        }

        /// <summary>
        /// Switches to Customer Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            customerPage.Content = editPage;

        }

        /// <summary>
        /// Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style = btnStatistics.Style = btnCreate.Style = FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        /// Passes the customer ID to the customer edit page and switches to it
        /// </summary>
        /// <param name="custID"></param>
        public void editCustomer(int custID)
        {
            editPage.loadCustomer(custID);
            btnEdit_Click(null, null);
        }
    }
}
