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

namespace InvoiceX.Pages.ReceiptPage
{
    /// <summary>
    ///     Interaction logic for ReceiptCreate.xaml
    /// </summary>
    public partial class ReceiptCreate : Page
    {
        private CustomerViewModel customerView;
        private readonly ReceiptMain receiptMain;
        private bool Refresh_DB_data = true;

        public ReceiptCreate(ReceiptMain receiptMain)
        {
            InitializeComponent();
            this.receiptMain = receiptMain;
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Loads the customers in the combobox as well as the new receipt ID
        /// </summary>
        public void load()
        {
            if (Refresh_DB_data)
            {
                customerView = new CustomerViewModel();
                comboBox_customer.ItemsSource = customerView.customersList;
                textBox_ReceiptNumber.Text = (ReceiptViewModel.returnLatestReceiptID() + 1).ToString();
                ReceiptDate.SelectedDate = DateTime.Today; //set current date 
                PaymentDate.SelectedDate = DateTime.Today; //set current date 
            }

            Refresh_DB_data = false;
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox containing the customers.
        ///     Loads the customer's information in the appropriate text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_customer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            if (comboBox_customer.SelectedIndex > -1)
            {
                textBox_Customer.Text = ((Customer) comboBox_customer.SelectedItem).CustomerName;
                textBox_Address.Text = ((Customer) comboBox_customer.SelectedItem).Address + ", " +
                                       ((Customer) comboBox_customer.SelectedItem).City + ", " +
                                       ((Customer) comboBox_customer.SelectedItem).Country;
                textBox_Contact_Details.Text = ((Customer) comboBox_customer.SelectedItem).PhoneNumber.ToString();
                textBox_Email_Address.Text = ((Customer) comboBox_customer.SelectedItem).Email;
            }
        }

        /// <summary>
        ///     The method that handles the event Selection Changed on the combobox paymentMethod.
        ///     Clears the red border and adds text based on selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_paymentmethod(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_PaymentMethod.SelectedIndex > -1)
                comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
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

        /// <summary>
        ///     Checks if a payment has already been selected
        /// </summary>
        /// <returns></returns>
        private bool paymenttype_already_selected()
        {
            var gridPayments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
            foreach (var p in gridPayments)
                if (p.paymentMethod == paymentenum)
                {
                    MessageBox.Show("Payment method already selected");
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Checks if the payment details are all completed and valid
        /// </summary>
        /// <returns></returns>
        private bool Check_AddPayment_CompletedValues()
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
            else
            {
                textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
            }

            if (!float.TryParse(textBox_amount.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ||
                f <= 0)
            {
                all_completed = false;
                textBox_amount.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox_amount.ClearValue(Control.BorderBrushProperty);
            }

            if (PaymentDate.SelectedDate == null) PaymentDate.SelectedDate = DateTime.Today; //set curent date 

            return all_completed;
        }

        /// <summary>
        ///     Adds the payment selected to the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddPayment(object sender, RoutedEventArgs e)
        {
            if (Check_AddPayment_CompletedValues())
            {
                Enum.TryParse(comboBox_PaymentMethod.Text, out PaymentMethod paymentenum);
                ReceiptDataGrid.Items.Add(new Payment
                {
                    amount = float.Parse(textBox_amount.Text.Replace(',', '.'),
                        CultureInfo.InvariantCulture.NumberFormat),
                    paymentNumber = comboBox_PaymentMethod.SelectedIndex == 0 ? "" : textBox_paymentNum.Text,
                    paymentDate = PaymentDate.SelectedDate.Value.Date,
                    paymentMethod = paymentenum
                });

                double total_amount = 0;
                total_amount = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
                total_amount = total_amount +
                               double.Parse(textBox_amount.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
                TotalAmount_TextBlock.Text = total_amount.ToString("C");
            }
        }

        /// <summary>
        ///     Removes a payment from the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CreateReceipt_REMOVE(object sender, RoutedEventArgs e)
        {
            var CurrentCell_Product = (Payment) ReceiptDataGrid.CurrentCell.Item;
            var total_amount = double.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            total_amount = total_amount - Convert.ToDouble(CurrentCell_Product.amount);
            TotalAmount_TextBlock.Text = total_amount.ToString("C");
            ReceiptDataGrid.Items.Remove(ReceiptDataGrid.CurrentCell.Item);
        }

        /// <summary>
        ///     Clears the payment's information from all the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_clearProduct_Click(object sender, RoutedEventArgs e)
        {
            comboBox_PaymentMethod.SelectedIndex = -1;
            textBox_paymentNum.Text = "";
            textBox_amount.Text = "";
            comboBox_paymentMethod_border.BorderThickness = new Thickness(0);
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
            textBox_amount.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     Checks that the customer details are completed
        /// </summary>
        /// <returns></returns>
        private bool Check_CustomerForm()
        {
            if (comboBox_customer.SelectedIndex <= -1)
            {
                comboBox_customer_border.BorderBrush = Brushes.Red;
                comboBox_customer_border.BorderThickness = new Thickness(1);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks that the receipt details are completed
        /// </summary>
        /// <returns></returns>
        private bool Check_DetailsForm()
        {
            if (issuedBy.Text.Equals(""))
            {
                issuedBy.BorderBrush = Brushes.Red;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Checks if the grid has payments in it
        /// </summary>
        /// <returns></returns>
        private bool Has_Items_Selected()
        {
            if (ReceiptDataGrid.Items.Count == 0)
            {
                MessageBox.Show("You havent selectet any products");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates and returns the Receipt based on the information on the page
        /// </summary>
        /// <returns></returns>
        private Receipt createReceiptObject()
        {
            Receipt myReceipt;
            myReceipt = new Receipt();
            myReceipt.createdDate = ReceiptDate.SelectedDate.Value.Date;
            //myReceipt.status=
            myReceipt.idReceipt = Convert.ToInt32(textBox_ReceiptNumber.Text);
            myReceipt.customerName = ((Customer) comboBox_customer.SelectedItem).CustomerName;
            myReceipt.customer = (Customer) comboBox_customer.SelectedItem;
            myReceipt.issuedBy = issuedBy.Text;
            myReceipt.totalAmount = float.Parse(TotalAmount_TextBlock.Text, NumberStyles.Currency);
            myReceipt.payments = ReceiptDataGrid.Items.OfType<Payment>().ToList();
            return myReceipt;
        }

        /// <summary>
        ///     After validating creates the receipt and switches to viewing it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Complete_Click(object sender, RoutedEventArgs e)
        {
            var ALL_VALUES_OK = true;
            if (!Check_CustomerForm()) ALL_VALUES_OK = false;
            if (!Check_DetailsForm()) ALL_VALUES_OK = false;
            if (!Has_Items_Selected()) ALL_VALUES_OK = false;
            if (ALL_VALUES_OK)
            {
                var rec = createReceiptObject();
                rec.createdDate += DateTime.Now.TimeOfDay;
                ReceiptViewModel.insertReceipt(rec);
                MessageBox.Show("Receipt with ID " + rec.idReceipt + " was created.");
                receiptMain.viewReceipt(rec.idReceipt);
                Btn_clearAll_Click(null, null);
            }
        }

        /// <summary>
        ///     Clears all the customet information
        /// </summary>
        private void Clear_Customer()
        {
            comboBox_customer.SelectedIndex = -1;
            textBox_Customer.Text = "";
            textBox_Address.Text = "";
            textBox_Contact_Details.Text = "";
            textBox_Email_Address.Text = "";
        }

        /// <summary>
        ///     Clears all the receipt details
        /// </summary>
        private void Clear_Details()
        {
            issuedBy.Text = "";
            issuedBy.ClearValue(Control.BorderBrushProperty);
            ReceiptDate.SelectedDate = DateTime.Today; //set current date 
            PaymentDate.SelectedDate = DateTime.Today; //set current date 
        }

        /// <summary>
        ///     Clears the grid
        /// </summary>
        private void Clear_ProductGrid()
        {
            ReceiptDataGrid.Items.Clear();
            TotalAmount_TextBlock.Text = 0.ToString("C");
        }

        /// <summary>
        ///     Clears all inputs from the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_clearAll_Click(object sender, RoutedEventArgs e)
        {
            comboBox_customer_border.BorderThickness = new Thickness(0);
            Btn_clearProduct_Click(new object(), new RoutedEventArgs());
            Clear_Customer();
            Clear_Details();
            Clear_ProductGrid();
            Refresh_DB_data = true;
            load();
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox IssuedBy.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IssuedBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            issuedBy.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox payment num.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_paymentNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_paymentNum.ClearValue(Control.BorderBrushProperty);
        }

        /// <summary>
        ///     The method that handles the event Text Changed on the textbox amount.
        ///     Clears the red border.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_amount.ClearValue(Control.BorderBrushProperty);
        }
    }
}