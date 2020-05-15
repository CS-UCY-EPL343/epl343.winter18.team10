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

namespace InvoiceX.Pages.CreditNotePage
{
    /// <summary>
    ///     Interaction logic for CreditNoteMain.xaml
    /// </summary>
    public partial class CreditNoteMain : Page
    {
        private readonly CreditNoteCreate createpage;
        private readonly CreditNoteEdit editpage;
        private readonly CreditNoteViewAll viewAllPage;
        private readonly CreditNoteView viewPage;

        public CreditNoteMain()
        {
            InitializeComponent();
            viewAllPage = new CreditNoteViewAll(this);
            viewPage = new CreditNoteView(this);
            createpage = new CreditNoteCreate(this);
            editpage = new CreditNoteEdit(this);
            btnViewAll_Click(null, null);
        }

        /// <summary>
        ///     Switches to the Credit Note View All page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewAll_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnViewAll.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = viewAllPage;
            viewAllPage.load();
        }

        /// <summary>
        ///     Switches to the Credit Note Create page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnCreate.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = createpage;
            createpage.load();
        }

        /// <summary>
        ///     Switches to the Credit Note View page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnView.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = viewPage;
        }

        /// <summary>
        ///     Switches to the Credit Note Edit page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            resetAllBtnStyles();
            btnEdit.Style = FindResource("ButtonStyleSelected") as Style;
            creditNotePage.Content = editpage;
            //editpage.load();
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
        ///     Passes the credit note ID to the credit note view page and switches to it
        /// </summary>
        /// <param name="cdID"></param>
        public void viewCreditNote(int cdID)
        {
            viewPage.loadCreditNote(cdID);
            btnView_Click(null, null);
        }

        /// <summary>
        ///     Passes the credit note ID to the credit note edit page and switches to it
        /// </summary>
        /// <param name="cdID"></param>
        public void editCreditNote(int cdID)
        {
            editpage.loadCreditNote(cdID);
            btnEdit_Click(null, null);
        }
    }
}