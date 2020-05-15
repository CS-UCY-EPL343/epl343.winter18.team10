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
using System.Windows.Input;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    ///     Interaction logic for ExpensesView.xaml
    /// </summary>
    public partial class ExpensesView : Page
    {
        private Expense expense;

        public ExpensesView()
        {
            InitializeComponent();

            txtBox_expenseNumber.Focus();
        }

        /// <summary>
        ///     After validating the expense ID calls loadExpense
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadExpense_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_expenseNumber.Text, out var expenseID);
            if (expenseID > 0)
                loadExpense(expenseID);
            else
                //not a number
                MessageBox.Show("Please insert a valid value for expense ID.");
        }

        /// <summary>
        ///     Loads the expense information on the page
        /// </summary>
        /// <param name="expenseID"></param>
        public void loadExpense(int expenseID)
        {
            expense = ExpensesViewModel.getExpense(expenseID);
            if (expense != null)
            {
                // Supllier details
                textBox_Company.Text = expense.companyName;
                textBox_ContactDetails.Text = expense.contactDetails.ToString();
                textBox_Description.Text = expense.description;
                textBox_Category.Text = expense.category;

                // Receipt details
                txtBox_expenseNumber.Text = expense.idExpense.ToString();
                txtBox_expenseNumber.IsReadOnly = true;
                txtBox_expenseDate.Text = expense.createdDate.ToString("d");
                txtBox_issuedBy.Text = expense.issuedBy;
                txtBox_cost.Text = expense.cost.ToString("C");
                txtBox_vat.Text = expense.VAT.ToString("P");
                txtBox_totalCost.Text = expense.totalCost.ToString("C");
                txtBox_invoiceNumber.Text = expense.invoiceNo.ToString();

                // Receipt payments           
                expensePaymentsGrid.ItemsSource = expense.payments;
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + expenseID + ", does not exist");
            }
        }

        /// <summary>
        ///     Method that handles the event Key Down on the textbox expenseNumber.
        ///     If the key pressed is Enter then it loads the order.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_expenseNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) Btn_LoadExpense_Click(null, null);
        }

        /// <summary>
        ///     Clear all information from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearView_Click(object sender, RoutedEventArgs e)
        {
            expense = null;
            foreach (var ctrl in grid_Supplier.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            foreach (var ctrl in grid_Details.Children)
                if (ctrl.GetType() == typeof(TextBox))
                    ((TextBox) ctrl).Clear();
            expensePaymentsGrid.ItemsSource = null;
            txtBox_expenseNumber.IsReadOnly = false;
            txtBox_expenseNumber.Focus();
        }

        /// <summary>
        ///     Opens the delete dialog prompting the user to confirm deletion or cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtBox_expenseNumber.Text, out var receiptID);
            if (txtBox_expenseNumber.IsReadOnly)
            {
                var msgtext = "You are about to delete the receipt with ID = " + receiptID + ". Are you sure?";
                var txt = "Delete Receipt";
                var button = MessageBoxButton.YesNo;
                var result = MessageBox.Show(msgtext, txt, button);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ReceiptViewModel.deleteReceipt(receiptID);
                        Btn_clearView_Click(null, null);
                        MessageBox.Show("Deleted Receipt with ID = " + receiptID);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            else
            {
                MessageBox.Show("No receipt is loaded");
            }
        }
    }
}