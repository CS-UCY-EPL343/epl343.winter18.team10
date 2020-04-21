using InvoiceX.Models;
using InvoiceX.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace InvoiceX.Pages.ExpensesPage
{
    /// <summary>
    /// Interaction logic for ExpensesEdit.xaml
    /// </summary>
    public partial class ExpensesEdit : Page
    {
        
        ExpensesMain expensesMain;

        bool invoice_loaded = false;
        Expense oldExpense;

        public ExpensesEdit()
        {
            InitializeComponent();
            //this.expensesMain = invoiceMain;
            txtBlock_NetTotal.Text = (0).ToString("C");
            txtBlock_VAT.Text = (0).ToString("C");
            txtBlock_TotalAmount.Text = (0).ToString("C");
            load();
        }

        public void load()
        {
            
        }

        private void ComboBox_paymentmethod(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
            {
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);

            }
            if (comboBox_PaymentMethod.SelectedIndex == 0)
            {
                textBox_paymentNum.IsReadOnly = true;
                textBox_paymentNum.Text = "No number needed";

            }
            else
            {
                textBox_paymentNum.IsReadOnly = false;
                textBox_paymentNum.Clear();
            }

        }

        private bool all_expenses_values_completed()
        {
            bool all_completed = true;
            if ((comboBox_PaymentMethod.SelectedIndex <= -1))
            {
                all_completed = false;
                comboBox_paymentMethod_border.BorderBrush = Brushes.Red;
                comboBox_paymentMethod_border.BorderThickness = new Thickness(1);
            }
            if (string.IsNullOrWhiteSpace(textBox_paymentNum.Text.ToString()))
            {
                all_completed = false;
                textBox_paymentNum.BorderBrush = Brushes.Red;
            }
            if (PaymentDate.SelectedDate == null)
            {
                PaymentDate.SelectedDate = DateTime.Today;//set curent date 
            }
            if (!float.TryParse(textBox_ExpenseAmount.Text.Replace('.', ','), out float fa) || (fa < 0))
            {
                all_completed = false;
                textBox_ExpenseAmount.BorderBrush = Brushes.Red;
            }
            if (!float.TryParse(txtBox_VAT.Text.Replace('.', ','), out float f) || (f < 0))
            {
                txtBox_VAT.BorderBrush = Brushes.Red;
                all_completed = false;
            }
            return all_completed;
        }

        private void Btn_AddPayment(object sender, RoutedEventArgs e)
        {
            if (all_expenses_values_completed())
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                expensesDataGrid.Items.Add(new Payment
                {
                    amount = float.Parse(textBox_ExpenseAmount.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat),
                    Vat = float.Parse(txtBox_VAT.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat),
                    paymentMethod = paymentenum,
                    paymentNumber = (comboBox_PaymentMethod.SelectedIndex == 0 ? "" : textBox_paymentNum.Text),
                    paymentDate = PaymentDate.SelectedDate.Value.Date
                });

                double netTotal = Double.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency);
                netTotal += Convert.ToDouble(textBox_ExpenseAmount.Text);
                txtBlock_NetTotal.Text = netTotal.ToString("C");

                double VAT = Double.Parse(txtBlock_VAT.Text, NumberStyles.Currency);
                VAT += (Convert.ToDouble(textBox_ExpenseAmount.Text) * float.Parse(txtBox_VAT.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat));
                txtBlock_VAT.Text = (VAT).ToString("C");

                txtBlock_TotalAmount.Text = (netTotal + VAT).ToString("C");
            }
        }


        private void Button_remove_expense_from_grid(object sender, RoutedEventArgs e)
        {
            Payment CurrentCell_Product = (Payment)(expensesDataGrid.CurrentCell.Item);
            float Vat = float.Parse(txtBox_VAT.Text.Replace(',', '.'), CultureInfo.InvariantCulture.NumberFormat);

            double netTotal = double.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency);
            netTotal -= Convert.ToDouble(CurrentCell_Product.amount);
            txtBlock_NetTotal.Text = netTotal.ToString("C");

            double VAT = double.Parse(txtBlock_VAT.Text, NumberStyles.Currency);
            VAT -= (CurrentCell_Product.amount * Vat);
            txtBlock_VAT.Text = VAT.ToString("C");

            txtBlock_TotalAmount.Text = (netTotal + VAT).ToString("C");
            expensesDataGrid.Items.Remove(expensesDataGrid.CurrentCell.Item);
        }

        /*remove txt from txtbox when clicked (Put GotFocus="TextBox_GotFocus" in txtBox)*/
        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        public void Btn_clearPayment_Click(object sender, RoutedEventArgs e)
        {

            comboBox_PaymentMethod.SelectedIndex = -1;
            textBox_paymentNum.Text = "";
            textBox_ExpenseAmount.Text = "";
            comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            textBox_paymentNum.ClearValue(TextBox.BorderBrushProperty);
            textBox_ExpenseAmount.ClearValue(TextBox.BorderBrushProperty);
        }

        private bool Has_Items_Selected()
        {
            if (expensesDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }
            return true;
        }

        private Expense createExpensesObject()
        {
            return new Expense
            {

                companyName = textBox_Company.Text,
                category = textBox_Category.Text,
                contactDetails = int.Parse(textBox_ContactDetails.Text),
                description = textBox_Description.Text,

                idExpense = int.Parse(textBox_expenseID.Text),
                invoiceNo = (string.IsNullOrWhiteSpace(txtBox_invoiceNumber.Text) ? 0 : int.Parse(txtBox_invoiceNumber.Text)),
                createdDate = expenseDate.SelectedDate.Value,
                issuedBy = issuedBy.Text,
                isPaid = ((bool)(checkBox_Paid.IsChecked)),

                cost = float.Parse(txtBlock_NetTotal.Text, NumberStyles.Currency),
                VAT = float.Parse(txtBox_VAT.Text, NumberStyles.Currency),
                totalCost = float.Parse(txtBlock_TotalAmount.Text, NumberStyles.Currency),

                payments = expensesDataGrid.Items.OfType<Payment>().ToList(),
            };
        }
        private bool checkCompanyForm()
        {
            bool all_completed = true;
            if (string.IsNullOrWhiteSpace(textBox_Company.Text.ToString()))
            {
                textBox_Company.BorderBrush = Brushes.Red;
                all_completed = false;
            }
            if (string.IsNullOrWhiteSpace(textBox_Category.Text.ToString()))
            {
                textBox_Category.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            if (string.IsNullOrWhiteSpace(textBox_ContactDetails.Text.ToString()) || !int.TryParse(textBox_ContactDetails.Text, out _))
            {
                textBox_ContactDetails.BorderBrush = Brushes.Red;
                all_completed = false;
            }
            if (string.IsNullOrWhiteSpace(textBox_Description.Text.ToString()))
            {
                textBox_Description.BorderBrush = Brushes.Red;
                all_completed = false;
            }

            return all_completed;
        }

        private bool checkDetailsForm()
        {
            bool all_ok = true;
            if (!string.IsNullOrWhiteSpace(txtBox_invoiceNumber.Text))
            {
                if (int.TryParse(txtBox_invoiceNumber.Text, out int id))
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
            if (string.IsNullOrWhiteSpace(issuedBy.Text.ToString()))
            {
                issuedBy.BorderBrush = Brushes.Red;
                all_ok = false;
            }
            if (!float.TryParse(txtBox_VAT.Text.Replace('.', ','), out float f) || (f < 0))
            {
                txtBox_VAT.BorderBrush = Brushes.Red;
                all_ok = false;
            }
           
            return all_ok;
        }
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {            
            if (checkCompanyForm() & checkDetailsForm()&Has_Items_Selected())
            {
                if (int.TryParse(textBox_expenseID.Text, out int invoiceId))
                {
                    ExpensesViewModel.updateExpense(createExpensesObject(), oldExpense);
                    //expensesMain.viewExpense(invoiceId);
                    Btn_clearAll_Click(null, null);
                }
            }
        }

        private void clearCompany()
        {
            textBox_Company.Clear();
            textBox_ContactDetails.Clear();
            textBox_Description.Clear();
            textBox_Category.Clear();
            textBox_Company.ClearValue(TextBox.BorderBrushProperty);
            textBox_ContactDetails.ClearValue(TextBox.BorderBrushProperty);
            textBox_Description.ClearValue(TextBox.BorderBrushProperty);
            textBox_Category.ClearValue(TextBox.BorderBrushProperty);
        }

        private void clearDetails()
        {
            textBox_expenseID.Clear();
            txtBox_invoiceNumber.Clear();
            expenseDate.SelectedDate = null;
            issuedBy.Clear();
            textBox_expenseID.ClearValue(TextBox.BorderBrushProperty);
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
            txtBox_invoiceNumber.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Clear_expenses_Grid()
        {
            expensesDataGrid.Items.Clear();
            txtBlock_NetTotal.Text = (0).ToString("C");
            txtBlock_VAT.Text = (0).ToString("C");
            txtBlock_TotalAmount.Text = (0).ToString("C");
        }


        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            invoice_loaded = false;
            Btn_clearPayment_Click(null, null);
            clearCompany();
            clearDetails();
            Clear_expenses_Grid();
            load();
        }

        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)//mono meta to refresh whritable
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void Btn_Load_Expense(object sender, RoutedEventArgs e)
        {
            int.TryParse(textBox_expenseID.Text, out int expenseID);
            if ((ExpensesViewModel.expenseExists(expenseID)))
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
                txtBlock_NetTotal.Text = oldExpense.cost.ToString("C");
                txtBox_VAT.Text = oldExpense.VAT.ToString();
                txtBlock_VAT.Text= (oldExpense.VAT*oldExpense.cost).ToString("C");
                txtBlock_TotalAmount.Text = oldExpense.totalCost.ToString("C");
                txtBox_invoiceNumber.Text = (oldExpense.invoiceNo==0 ? ""  : oldExpense.invoiceNo.ToString());

                // Receipt payments 
                foreach (Payment p in oldExpense.payments)
                {
                    expensesDataGrid.Items.Add(p);
                }
                
            }
            else
            {
                MessageBox.Show("Receipt with ID = " + expenseID + ", does not exist");
            }
        }

        private void ComboBox_paymentmethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
            {
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);

            }
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
        /*clear red when txt changed START*/
        private void textBox_Company_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Company.ClearValue(TextBox.BorderBrushProperty);
        }

        private void textBox_Category_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Category.ClearValue(TextBox.BorderBrushProperty);
        }

        private void textBox_ContactDetails_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ContactDetails.ClearValue(TextBox.BorderBrushProperty);
        }

        private void textBox_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_Description.ClearValue(TextBox.BorderBrushProperty);
        }

        private void txtBox_invoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_invoiceNumber.ClearValue(TextBox.BorderBrushProperty);
        }

        private void issuedBy_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(TextBox.BorderBrushProperty);
        }

        private void txtBox_VAT_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBox_VAT.ClearValue(TextBox.BorderBrushProperty);
        }

        private void textBox_paymentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_paymentNum.ClearValue(TextBox.BorderBrushProperty);
        }

        private void textBox_ExpenseAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_ExpenseAmount.ClearValue(TextBox.BorderBrushProperty);
        }
        /*clear red when txt changed END*/
    }
}
