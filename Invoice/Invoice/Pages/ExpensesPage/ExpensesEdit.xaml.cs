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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using InvoiceX.Models;
using InvoiceX.ViewModels;

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    ///     Interaction logic for ExpensesEdit.xaml
    /// </summary>
    public partial class ExpensesEdit : Page
    {
        private readonly ExpensesMain expensesMain;

        private bool invoice_loaded;
        private Expense oldExpense;

        public ExpensesEdit(ExpensesMain expensesMain)
        {
            InitializeComponent();
            this.expensesMain = expensesMain;
        }

        /// <summary>
        ///     Checks if the expense details are all completed and valid
        /// </summary>
        /// <returns></returns>
        private bool all_expenses_values_completed()
        {
            var all_completed = true;
            if (comboBox_PaymentMethod.SelectedIndex <= -1)
            {
                all_completed = false;
                comboBox_paymentMethod_border.BorderBrush = Brushes.Red;
                comboBox_paymentMethod_border.BorderThickness = new Thickness(1);
            }

            if (string.IsNullOrWhiteSpace(textBox_paymentNum.Text))
            {
                all_completed = false;
                textBox_paymentNum.BorderBrush = Brushes.Red;
            }

            if (PaymentDate.SelectedDate == null) PaymentDate.SelectedDate = DateTime.Today; //set curent date 
            if (!float.TryParse(textBox_ExpenseAmount.Text.Replace('.', ','), out var fa) || fa < 0)
            {
                all_completed = false;
                textBox_ExpenseAmount.BorderBrush = Brushes.Red;
            }

            if (!float.TryParse(txtBox_VAT.Text.Replace('.', ','), out var f) || f < 0)
            {
                txtBox_VAT.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            return all_completed;
        }

        /// <summary>
        ///     Adds the payment selected to the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddPayment(object sender, RoutedEventArgs e)
        {
            if (all_expenses_values_completed())
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                expensesDataGrid.Items.Add(new Payment
                {
                    amount = float.Parse(textBox_ExpenseAmount.Text.Replace(',', '.'),
                        CultureInfo.InvariantCulture.NumberFormat),
                    Vat = float.Parse(txtBox_VAT.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat),
                    paymentMethod = paymentenum,
                    paymentNumber = comboBox_PaymentMethod.SelectedIndex == 0 ? "" : textBox_paymentNum.Text,
                    paymentDate = PaymentDate.SelectedDate.Value.Date
                });
            }
        }

        /// <summary>
        ///     Removes a payment from the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_remove_expense_from_grid(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Payment) expensesDataGrid.CurrentCell.Item;

            expensesDataGrid.Items.Remove(expensesDataGrid.CurrentCell.Item);
        }

        /// <summary>
        ///     Clears text when textbox gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox) sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        /// <summary>
        ///     Clears the payment's information from all the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_clearPayment_Click(object sender, RoutedEventArgs e)
        {
            comboBox_PaymentMethod.SelectedIndex = -1;
            textBox_paymentNum.Text = "";
            textBox_ExpenseAmount.Text = "";
            comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
            textBox_ExpenseAmount.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Checks if the grid has payments in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (expensesDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Expense based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Expense createExpensesObject()
        {
            return new Expense
            {
                companyName = textBox_Company.Text,
                category = textBox_Category.Text,
                contactDetails = int.Parse(textBox_ContactDetails.Text),
                description = textBox_Description.Text,

                idExpense = int.Parse(textBox_expenseID.Text),
                invoiceNo = string.IsNullOrWhiteSpace(txtBox_invoiceNumber.Text)
                    ? 0
                    : int.Parse(txtBox_invoiceNumber.Text),
                createdDate = expenseDate.SelectedDate.Value,
                issuedBy = issuedBy.Text,
                isPaid = (bool) checkBox_Paid.IsChecked,

                cost = float.Parse(txtBox_cost.Text, NumberStyles.Currency),
                VAT = float.Parse(txtBox_VAT.Text, NumberStyles.Currency),
                totalCost = float.Parse(txtBox_totalCost.Text, NumberStyles.Currency),

                payments = expensesDataGrid.Items.OfType<Payment>().ToList()
            };
        }

        /// <summary>
        ///     Checks that the company details are completed
        /// </summary>
        /// <returns></returns>
        private bool checkCompanyForm()
        {
            var all_completed = true;
            if (string.IsNullOrWhiteSpace(textBox_Company.Text))
            {
                textBox_Company.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_Category.Text))
            {
                textBox_Category.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_ContactDetails.Text) ||
                !int.TryParse(textBox_ContactDetails.Text, out _))
            {
                textBox_ContactDetails.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_Description.Text))
            {
                textBox_Description.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            return all_completed;
        }

        /// <summary>
        ///     Checks that the expense details are completed
        /// </summary>
        /// <returns></returns>
        private bool checkDetailsForm()
        {
            var all_ok = true;
            if (!string.IsNullOrWhiteSpace(txtBox_invoiceNumber.Text))
            {
                if (int.TryParse(txtBox_invoiceNumber.Text, out var id))
                {
                    if (!InvoiceViewModel.invoiceExists(id))
                    {
                        txtBox_invoiceNumber.BorderBrush = Brushes.Red;
                        MessageBox.Show("Invoice ID doesn't exist");
                        all_ok = false;
                    }
                }
                else
                {
                    txtBox_invoiceNumber.BorderBrush = Brushes.Red;
                    all_ok = false;
                }
            }

            if (string.IsNullOrWhiteSpace(issuedBy.Text))
            {
                issuedBy.BorderBrush = Brushes.Red;
                all_ok = false;
            }

            return all_ok;
        }

        /// <summary>
        ///     After validating updates the expense and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            if (checkCompanyForm() & checkDetailsForm() & Has_Items_Selected())
                if (int.TryParse(textBox_expenseID.Text, out var expenseID))
                {
                    ExpensesViewModel.updateExpense(createExpensesObject(), oldExpense);
                    expensesMain.viewExpense(expenseID);
                    Btn_clearAll_Click(null, null);
                }
        }

        /// <summary>
        ///     Clears all the company's information
        /// </summary>
        private void clearCompany()
        {
            textBox_Company.Clear();
            textBox_ContactDetails.Clear();
            textBox_Description.Clear();
            textBox_Category.Clear();
            textBox_Company.ClearValue(Control.BorderBrushProperty);
            textBox_ContactDetails.ClearValue(Control.BorderBrushProperty);
            textBox_Description.ClearValue(Control.BorderBrushProperty);
            textBox_Category.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Clears all the expense details
        /// </summary>
        private void clearDetails()
        {
            textBox_expenseID.Clear();
            txtBox_invoiceNumber.Clear();
            expenseDate.SelectedDate = null;
            issuedBy.Clear();
            textBox_expenseID.ClearValue(Control.BorderBrushProperty);
            issuedBy.ClearValue(Control.BorderBrushProperty);
            txtBox_invoiceNumber.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Clears the grid
        /// </summary>
        private void Clear_expenses_Grid()
        {
            expensesDataGrid.Items.Clear();
            txtBox_cost.Clear();
            txtBox_VAT.Clear();
            txtBox_totalCost.Clear();
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            invoice_loaded = false;
            Btn_clearPayment_Click(null, null);
            clearCompany();
            clearDetails();
            Clear_expenses_Grid();
        }

        /// <summary>
        ///     After validating the expense ID calls loadExpense
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Load_Expense(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_expenseID.Text, out var expenseID);
            if (ExpensesViewModel.expenseExists(expenseID))
            {
                Btn_clearAll_Click(null, null);
                loadExpense(expenseID);
                invoice_loaded = true;
            }
            else
            {
                //not a number
                MessageBox.Show("Please insert a valid value for Expense ID.");
            }
        }

        /// <summary>
        ///     Loads the expense information on the page
        /// </summary>
        /// <param name="expenseID"></param>
        public void loadExpense(int expenseID)
        {
            oldExpense = ExpensesViewModel.getExpense(expenseID);
            if (oldExpense != null)
            {
                // Supllier details
                textBox_Company.Text = oldExpense.companyName;
                textBox_ContactDetails.Text = oldExpense.contactDetails.ToString();
                textBox_Description.Text = oldExpense.description;
                textBox_Category.Text = oldExpense.category;

                // Payment details
                textBox_expenseID.Text = oldExpense.idExpense.ToString();
                expenseDate.Text = oldExpense.createdDate.ToString("d");
                issuedBy.Text = oldExpense.issuedBy;
                checkBox_Paid.IsChecked = oldExpense.isPaid;
                txtBox_cost.Text = oldExpense.cost.ToString("C");
                txtBox_VAT.Text = oldExpense.VAT.ToString();
                txtBox_VAT.Text = (oldExpense.VAT * oldExpense.cost).ToString("C");
                txtBox_totalCost.Text = oldExpense.totalCost.ToString("C");
                txtBox_invoiceNumber.Text = oldExpense.invoiceNo == 0 ? "" : oldExpense.invoiceNo.ToString();

                // Receipt payments 
                foreach (var p in oldExpense.payments) expensesDataGrid.Items.Add(p);
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + expenseID + ", does not exist");
            }
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox paymentMethod.
        ///     Clears the red border and adds text based on selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_paymentmethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            if (comboBox_PaymentMethod.SelectedIndex == 0)
            {
                textBox_paymentNum.IsReadOnly = true;
                textBox_paymentNum.Text = "No number needed ";
            }
            else
            {
                textBox_paymentNum.IsReadOnly = false;
                textBox_paymentNum.Clear();
            }
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox Compant.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Company_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Company.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox Category.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Category_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Category.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox ContactDetails.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_ContactDetails_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ContactDetails.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox Description.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Description.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox InvoiceID.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_invoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_invoiceNumber.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void issuedBy_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox VAT.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_VAT_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_VAT.ClearValue(Control.BorderBrushProperty);
            if (float.TryParse(txtBox_VAT.Text.Replace('.', ','), out var vat) &&
                float.TryParse(txtBox_cost.Text.Replace('.', ','), out var price))
                txtBox_totalCost.Text = (price + vat).ToString("n2");
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox PaymentNum.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_paymentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox ExpenseAmount.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_ExpenseAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ExpenseAmount.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox Cost.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBox_cost_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_cost.ClearValue(Control.BorderBrushProperty);

            if (float.TryParse(txtBox_VAT.Text.Replace('.', ','), out var vat) &&
                float.TryParse(txtBox_cost.Text.Replace('.', ','), out var price))
                txtBox_totalCost.Text = (price + vat).ToString("n2");
        }
    }
}