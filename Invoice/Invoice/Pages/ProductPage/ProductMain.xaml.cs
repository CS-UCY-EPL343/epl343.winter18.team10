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

namespace InvoiceX.Pages.ProductPage
{
    /// <summary>
    ///     Interaction logic for ProductMain.xaml
    /// </summary>
    public partial class ProductMain : Page
    {
        private readonly ProductCreate createPage;
        private readonly ProductEdit editPage;
        private readonly ProductStatistics statisticsPage;
        private readonly ProductView viewPage;

        public ProductMain()
        {
            InitializeComponent();
            viewPage = new ProductView(this);
            createPage = new ProductCreate();
            editPage = new ProductEdit();
            statisticsPage = new ProductStatistics();
            btnView_Click(new object(), new RoutedEventArgs());
        }

        /// <summary>
        ///     Switches to Product View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = viewPage;
            viewPage.load();
        }

        /// <summary>
        ///     Switches to Product Statistics page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnStatistics.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = statisticsPage;
        }

        /// <summary>
        ///     Switches to Product Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = createPage;
        }

        /// <summary>
        ///     Switches to Product Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            productPage.Content = editPage;
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnEdit.Style = btnView.Style =
                btnCreate.Style = btnStatistics.Style = FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        ///     Passes the product ID to the product edit page and switches to it
        /// </summary>
        /// <param name="prodID"></param>
        public void editProduct(int prodID)
        {
            editPage.loadProduct(prodID);
            btnEdit_Click(null, null);
        }
    }
}