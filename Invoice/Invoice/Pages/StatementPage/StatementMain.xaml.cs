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

namespace InvoiceX.Pages.StatementPage
{
    /// <summary>
    ///     Interaction logic for StatementMain.xaml
    /// </summary>
    public partial class StatementMain : Page
    {
        private readonly StatementCreate createPage;
        private readonly MainWindow mainWindow;

        public StatementMain(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            createPage = new StatementCreate(this);
            btnCreate_Click(null, null);
        }

        /// <summary>
        ///     Switches to Statement Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            statementPage.Content = createPage;
        }

        /// <summary>
        ///     Resets all the button styles
        /// </summary>
        private void resetAllBtnStyles()
        {
            btnCreate.Style = FindResource("ButtonStyle") as Style;
        }

        /// <summary>
        ///     Calls the view statement item method in main window
        /// </summary>
        /// <param name="item"></param>
        public void viewItem(StatementItem item)
        {
            mainWindow.viewStatementItem(item);
        }
    }
}